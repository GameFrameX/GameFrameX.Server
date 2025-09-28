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
/// 根据SessionId获取节点信息的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(GetNodeBySessionIdHttpHandler))]
[HttpMessageResponse(typeof(GetNodeBySessionIdResponse))]
[HttpMessageRequest(typeof(GetNodeBySessionIdRequest))]
public sealed class GetNodeBySessionIdHttpHandler : BaseHttpHandler
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