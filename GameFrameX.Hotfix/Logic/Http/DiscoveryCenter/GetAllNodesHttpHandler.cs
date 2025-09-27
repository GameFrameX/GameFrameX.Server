// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.ServerManager;

namespace GameFrameX.Hotfix.Logic.Http.DiscoveryCenter;

/// <summary>
/// 获取所有节点列表HTTP处理器
/// 提供获取服务发现中心管理的所有服务节点列表的HTTP接口
/// http://localhost:20001/game/api/getAllNodes
/// </summary>
[HttpMessageMapping(typeof(GetAllNodesHttpHandler))]
[HttpMessageResponse(typeof(GetAllNodesResponse))]
[HttpMessageRequest(typeof(GetAllNodesRequest))]
[Description("获取所有节点列表接口，返回服务发现中心管理的所有服务节点信息")]
public sealed class GetAllNodesHttpHandler : BaseHttpHandler
{

    /// <summary>
    /// 处理获取所有节点列表的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含所有节点列表的JSON响应</returns>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var allNodes = NamingServiceManager.Instance.GetAllNodes();
        var response = new GetAllNodesResponse
        {
            Message = allNodes,
            Count = allNodes.Count
        };
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 获取所有节点列表的请求对象
/// </summary>
public sealed class GetAllNodesRequest : HttpMessageRequestBase
{
    // 获取所有节点不需要任何参数
}

/// <summary>
/// 获取所有节点列表的HTTP响应类
/// </summary>
public sealed class GetAllNodesResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 所有节点列表，按服务器ID分组
    /// </summary>
    /// <value>返回一个包含所有服务节点信息的列表集合，每个内部列表代表同一服务器ID下的节点</value>
    [Description("所有节点列表，按服务器ID分组")]
    public List<List<IServiceInfo>> Message { get; set; }

    /// <summary>
    /// 节点组数量
    /// </summary>
    /// <value>返回节点组的总数量</value>
    [Description("节点组数量")]
    public int Count { get; set; }
}