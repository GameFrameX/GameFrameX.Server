using GameFrameX.NetWork.HTTP;
using GameFrameX.ServerManager;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 根据SessionId获取节点信息的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(GetNodeBySessionIdHttpHandler))]
[HttpMessageResponse(typeof(GetNodeBySessionIdResponse))]
[HttpMessageRequest(typeof(GetNodeBySessionIdRequest))]
public class GetNodeBySessionIdHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理根据SessionId获取节点信息的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含节点信息的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法通过 <see cref="NamingServiceManager.GetNodeBySessionId(string)"/> 方法查找指定SessionId的服务节点。
    /// 如果找到匹配的节点，返回节点详细信息；否则返回失败状态。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var sessionRequest = (GetNodeBySessionIdRequest)request;
        // 从请求对象中获取SessionId
        string sessionId = sessionRequest.SessionId ?? string.Empty;
        
        var namingServiceManager = NamingServiceManager.Instance;
        var serviceInfo = namingServiceManager.GetNodeBySessionId(sessionId);

        var response = new GetNodeBySessionIdResponse
        {
            Success = serviceInfo != null,
            Message = serviceInfo != null ? "成功获取节点信息" : $"未找到SessionId为 {sessionId} 的节点",
            Node = serviceInfo,
            SessionId = sessionId
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 根据SessionId获取节点信息的请求对象
/// </summary>
public class GetNodeBySessionIdRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 会话ID
    /// </summary>
    /// <value>要查询的会话唯一标识符</value>
    public string SessionId { get; set; } = string.Empty;
}

/// <summary>
/// 根据SessionId获取节点信息的响应对象
/// </summary>
public class GetNodeBySessionIdResponse : HttpMessageResponseBase
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
    /// 查询的SessionId
    /// </summary>
    /// <value>请求中指定的会话ID</value>
    public string SessionId { get; set; } = string.Empty;
    
    /// <summary>
    /// 节点信息
    /// </summary>
    /// <value>指定SessionId对应的服务节点信息，如果未找到则为null</value>
    public IServiceInfo Node { get; set; }
}