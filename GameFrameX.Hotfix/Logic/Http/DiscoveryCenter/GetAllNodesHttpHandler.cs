// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.DiscoveryCenterManager;
using GameFrameX.DiscoveryCenterManager.Server;

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