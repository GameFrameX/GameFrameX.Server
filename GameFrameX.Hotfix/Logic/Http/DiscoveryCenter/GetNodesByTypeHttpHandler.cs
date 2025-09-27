using GameFrameX.NetWork.HTTP;
using GameFrameX.ServerManager;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 根据服务器类型获取节点列表的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(GetNodesByTypeHttpHandler))]
[HttpMessageResponse(typeof(GetNodesByTypeResponse))]
[HttpMessageRequest(typeof(GetNodesByTypeRequest))]
public class GetNodesByTypeHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理根据服务器类型获取节点列表的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含节点列表的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法通过 <see cref="NamingServiceManager.GetNodesByType(ServerType)"/> 方法获取指定类型的所有服务节点。
    /// 返回的节点列表包含该类型下所有活跃的服务实例信息。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var typeRequest = (GetNodesByTypeRequest)request;
        var serverType = typeRequest.ServerType;
        
        // 验证ServerType参数
        if (serverType == ServerType.None)
        {
            var errorResponse = new GetNodesByTypeResponse
            {
                Success = false,
                Message = "无效的ServerType参数",
                ServerType = ServerType.None,
                Nodes = new List<IServiceInfo>(),
                Count = 0
            };
            return Task.FromResult(HttpJsonResult.SuccessString(errorResponse));
        }
        
        var namingServiceManager = NamingServiceManager.Instance;
        var nodes = namingServiceManager.GetNodesByType(serverType);

        var response = new GetNodesByTypeResponse
        {
            Success = true,
            Message = $"成功获取服务器类型 {serverType} 的节点列表",
            ServerType = serverType,
            Nodes = nodes,
            Count = nodes.Count
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 根据服务器类型获取节点列表的请求对象
/// </summary>
public class GetNodesByTypeRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    /// <value>要查询的服务器类型枚举值</value>
    public ServerType ServerType { get; set; }
}

/// <summary>
/// 根据服务器类型获取节点列表的响应对象
/// </summary>
public class GetNodesByTypeResponse : HttpMessageResponseBase
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
    public string Message { get; set; }
    
    /// <summary>
    /// 查询的服务器类型
    /// </summary>
    /// <value>请求中指定的服务器类型</value>
    public ServerType ServerType { get; set; }
    
    /// <summary>
    /// 指定类型的节点列表
    /// </summary>
    /// <value>包含所有指定类型服务节点信息的 <see cref="List{T}"/> 其中 T 为 <see cref="IServiceInfo"/></value>
    public List<IServiceInfo> Nodes { get; set; } = new();
    
    /// <summary>
    /// 节点总数
    /// </summary>
    /// <value>指定类型节点的总数量</value>
    public int Count { get; set; }
}