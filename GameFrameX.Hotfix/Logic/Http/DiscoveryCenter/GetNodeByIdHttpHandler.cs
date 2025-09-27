using GameFrameX.NetWork.HTTP;
using GameFrameX.ServerManager;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 根据服务器ID获取节点信息的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(GetNodeByIdHttpHandler))]
[HttpMessageResponse(typeof(GetNodeByIdResponse))]
[HttpMessageRequest(typeof(GetNodeByIdRequest))]
public class GetNodeByIdHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理根据服务器ID获取节点信息的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含节点信息的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法通过 <see cref="NamingServiceManager.TryGet(long)"/> 方法查找指定ID的服务节点。
    /// 如果找到匹配的节点，返回节点详细信息；否则返回失败状态。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var idRequest = (GetNodeByIdRequest)request;
        // 从请求对象中获取ServerId
        long serverId = idRequest.ServerId;
        
        var namingServiceManager = NamingServiceManager.Instance;
        List<IServiceInfo> serviceInfoList = null;
        
        if (serverId > 0)
        {
            serviceInfoList = namingServiceManager.TryGet(serverId);
        }

        var response = new GetNodeByIdResponse
        {
            Success = serviceInfoList != null && serviceInfoList.Count > 0,
            Message = serviceInfoList != null && serviceInfoList.Count > 0 
                ? "成功获取节点信息" 
                : $"未找到ServerId为 {serverId} 的节点",
            Node = serviceInfoList?.FirstOrDefault(),
            ServerId = serverId
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 根据服务器ID获取节点信息的请求对象
/// </summary>
public class GetNodeByIdRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 服务器ID
    /// </summary>
    /// <value>要查询的服务器唯一标识符</value>
    public long ServerId { get; set; }
}

/// <summary>
/// 根据服务器ID获取节点信息的响应对象
/// </summary>
public class GetNodeByIdResponse : HttpMessageResponseBase
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
    /// 查询的服务器ID
    /// </summary>
    /// <value>请求中指定的服务器ID</value>
    public long ServerId { get; set; }
    
    /// <summary>
    /// 节点信息
    /// </summary>
    /// <value>指定ID对应的服务节点信息，如果未找到则为null</value>
    public IServiceInfo Node { get; set; }
}