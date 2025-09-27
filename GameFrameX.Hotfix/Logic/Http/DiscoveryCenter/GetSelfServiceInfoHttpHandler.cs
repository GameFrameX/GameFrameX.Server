using GameFrameX.NetWork.HTTP;
using GameFrameX.ServerManager;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 获取自身服务信息的HTTP处理器
/// </summary>
/// <remarks>
/// 该处理器用于获取当前服务器的自身服务信息，包括服务器类型、名称、ID、IP地址、端口等详细信息。
/// 这对于服务发现、健康检查和监控系统非常有用。
/// </remarks>
[HttpMessageMapping(typeof(GetSelfServiceInfoHttpHandler))]
[HttpMessageResponse(typeof(GetSelfServiceInfoResponse))]
[HttpMessageRequest(typeof(GetSelfServiceInfoRequest))]
public class GetSelfServiceInfoHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理获取自身服务信息的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含自身服务信息的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法通过 <see cref="NamingServiceManager.SelfServiceInfo"/> 属性获取当前服务器的详细信息。
    /// 如果服务器尚未初始化或注册，可能返回空的服务信息。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var namingServiceManager = NamingServiceManager.Instance;
        var selfServiceInfo = namingServiceManager.SelfServiceInfo;

        var response = new GetSelfServiceInfoResponse
        {
            Success = true,
            Message = selfServiceInfo != null ? "成功获取自身服务信息" : "自身服务信息尚未初始化",
            ServiceInfo = selfServiceInfo
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 获取自身服务信息的请求对象
/// </summary>
/// <remarks>
/// 该请求不需要任何参数，因为是获取当前服务器自身的信息
/// </remarks>
public class GetSelfServiceInfoRequest : HttpMessageRequestBase
{
    // 此请求类不需要任何属性，因为获取自身服务信息不需要参数
}

/// <summary>
/// 获取自身服务信息的响应对象
/// </summary>
/// <remarks>
/// 包含当前服务器的详细信息，如果服务器尚未初始化，ServiceInfo 可能为 null。
/// </remarks>
public class GetSelfServiceInfoResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 获取或设置操作是否成功
    /// </summary>
    /// <value>
    /// 如果成功获取自身服务信息则为 <c>true</c>；否则为 <c>false</c>
    /// </value>
    public bool Success { get; set; }

    /// <summary>
    /// 获取或设置响应消息
    /// </summary>
    /// <value>
    /// 描述操作结果的消息字符串
    /// </value>
    /// <remarks>
    /// 通常包含操作成功或失败的详细描述信息
    /// </remarks>
    public string Message { get; set; }

    /// <summary>
    /// 获取或设置自身服务信息
    /// </summary>
    /// <value>
    /// 当前服务器的 <see cref="IServiceInfo"/> 实例，如果服务器尚未初始化则可能为 <c>null</c>
    /// </value>
    /// <remarks>
    /// 包含服务器类型、名称、ID、实例ID、内外网IP地址、端口等完整的服务信息。
    /// 该信息对于服务发现、负载均衡和监控系统非常重要。
    /// </remarks>
    /// <seealso cref="IServiceInfo"/>
    public IServiceInfo ServiceInfo { get; set; }
}