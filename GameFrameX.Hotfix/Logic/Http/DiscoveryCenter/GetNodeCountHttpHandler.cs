using GameFrameX.NetWork.HTTP;
using GameFrameX.ServerManager;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 获取节点总数的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(GetNodeCountHttpHandler))]
[HttpMessageResponse(typeof(GetNodeCountResponse))]
[HttpMessageRequest(typeof(GetNodeCountRequest))]
public class GetNodeCountHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理获取节点总数的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含节点总数信息的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法通过 <see cref="NamingServiceManager.GetAllNodes()"/> 方法获取所有节点，
    /// 并根据请求中的 <see cref="ServerType"/> 进行过滤统计。
    /// 如果未指定服务器类型，则返回所有类型的节点统计信息。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var countRequest = (GetNodeCountRequest)request;
        // 从请求对象中获取ServerType（可选）
        ServerType? serverType = countRequest.ServerType;
        
        var namingServiceManager = NamingServiceManager.Instance;
        var allNodeGroups = namingServiceManager.GetAllNodes();
        
        // 按服务器类型分组统计
        var countByType = new Dictionary<ServerType, int>();
        foreach (ServerType type in Enum.GetValues<ServerType>())
        {
            countByType[type] = 0;
        }
        
        // 遍历所有节点组，然后遍历每个组中的节点
        foreach (var nodeGroup in allNodeGroups)
        {
            foreach (var node in nodeGroup)
            {
                if (countByType.ContainsKey(node.Type))
                {
                    countByType[node.Type]++;
                }
            }
        }
        
        // 如果指定了服务器类型，只返回该类型的统计
        int totalCount;
        if (serverType.HasValue)
        {
            totalCount = countByType.GetValueOrDefault(serverType.Value, 0);
        }
        else
        {
            // 计算所有节点的总数
            int totalNodeCount = 0;
            foreach (var nodeGroup in allNodeGroups)
            {
                totalNodeCount += nodeGroup.Count;
            }
            totalCount = totalNodeCount;
        }

        var response = new GetNodeCountResponse
        {
            TotalCount = totalCount,
            CountByType = countByType,
            QueryServerType = serverType
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 获取节点总数的请求对象
/// </summary>
public class GetNodeCountRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 服务器类型（可选）
    /// </summary>
    /// <value>要统计的特定服务器类型，如果为null则统计所有类型</value>
    public ServerType? ServerType { get; set; }
}

/// <summary>
/// 获取节点总数的响应对象
/// </summary>
public class GetNodeCountResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    /// <value>表示操作是否成功执行的布尔值</value>
    public bool Success { get; set; }
    
    /// <summary>
    /// 响应消息
    /// </summary>
    /// <value>描述操作结果的消息字符串</value>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// 节点总数
    /// </summary>
    /// <value>查询范围内的节点总数量</value>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// 按服务器类型分组的节点数量统计
    /// </summary>
    /// <value>以服务器类型为键，对应节点数量为值的 <see cref="Dictionary{TKey, TValue}"/> 其中 TKey 为 <see cref="ServerType"/>，TValue 为 <see cref="int"/></value>
    public Dictionary<ServerType, int> CountByType { get; set; } = new();
    
    /// <summary>
    /// 查询的服务器类型
    /// </summary>
    /// <value>请求中指定的服务器类型，如果为null表示查询所有类型</value>
    public ServerType? QueryServerType { get; set; }
}