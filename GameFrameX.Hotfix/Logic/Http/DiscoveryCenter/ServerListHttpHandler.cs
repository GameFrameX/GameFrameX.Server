// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.ServerManager;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 获取外部服务器列表的HTTP处理器
/// </summary>
/// <remarks>
/// 该处理器用于获取所有外部可访问的服务器节点信息，主要用于服务发现和负载均衡。
/// 外部节点是指具有有效SessionId的服务器节点，通常是可以接受外部连接的服务器。
/// 访问地址示例：http://localhost:20001/api/discovery/servers
/// </remarks>
[HttpMessageMapping(typeof(ServerListHttpHandler))]
[HttpMessageResponse(typeof(ServerListResponse))]
[HttpMessageRequest(typeof(ServerListRequest))]
[Description("获取外部服务器列表接口，用于服务发现和负载均衡")]
public sealed class ServerListHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理获取外部服务器列表的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含外部服务器列表的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法通过 <see cref="NamingServiceManager.GetOuterNodes"/> 方法获取所有外部可访问的服务器节点。
    /// 外部节点通常包含游戏服务器、网关服务器等可以接受客户端连接的服务器。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var serviceInfos = NamingServiceManager.Instance.GetOuterNodes();
        var response = new ServerListResponse
        {
            ServerList = serviceInfos,
            Count = serviceInfos.Count,
            Success = true,
            Message = "成功获取外部服务器列表"
        };
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 获取外部服务器列表的请求对象
/// </summary>
/// <remarks>
/// 该请求不需要任何参数，因为是获取所有外部服务器的列表
/// </remarks>
public sealed class ServerListRequest : HttpMessageRequestBase
{
    // 此请求类不需要任何属性，因为获取外部服务器列表不需要参数
}

/// <summary>
/// 获取外部服务器列表的响应对象
/// </summary>
/// <remarks>
/// 包含所有外部可访问的服务器节点信息，用于客户端进行服务发现和连接选择。
/// </remarks>
public sealed class ServerListResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 获取或设置外部服务器列表
    /// </summary>
    /// <value>
    /// 包含所有外部可访问服务器信息的 <see cref="IServiceInfo"/> 列表
    /// </value>
    /// <remarks>
    /// 外部服务器是指具有有效SessionId的服务器节点，通常可以接受客户端连接。
    /// 列表中的每个服务器包含服务器类型、名称、ID、IP地址、端口等详细信息。
    /// </remarks>
    /// <seealso cref="IServiceInfo"/>
    [Description("外部服务器信息列表")]
    public List<IServiceInfo> ServerList { get; set; }

    /// <summary>
    /// 获取或设置服务器数量
    /// </summary>
    /// <value>
    /// 外部服务器的总数量
    /// </value>
    /// <remarks>
    /// 该值等于 <see cref="ServerList"/> 的元素个数，便于客户端快速了解可用服务器数量。
    /// </remarks>
    [Description("外部服务器总数量")]
    public int Count { get; set; }

    /// <summary>
    /// 获取或设置操作是否成功
    /// </summary>
    /// <value>
    /// 如果成功获取服务器列表则为 <c>true</c>；否则为 <c>false</c>
    /// </value>
    [Description("操作是否成功")]
    public bool Success { get; set; }

    /// <summary>
    /// 获取或设置响应消息
    /// </summary>
    /// <value>
    /// 描述操作结果的消息字符串
    /// </value>
    /// <remarks>
    /// 通常包含操作成功或失败的详细描述信息，便于调试和日志记录。
    /// </remarks>
    [Description("响应消息")]
    public string Message { get; set; }
}