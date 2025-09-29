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
/// 移除节点的HTTP处理器
/// </summary>
[HttpMessageMapping(typeof(RemoveNodeHttpHandler))]
[HttpMessageResponse(typeof(RemoveNodeResponse))]
[HttpMessageRequest(typeof(RemoveNodeRequest))]
public sealed class RemoveNodeHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 处理移除节点的HTTP请求
    /// </summary>
    /// <param name="ip">客户端IP地址</param>
    /// <param name="url">请求的URL地址</param>
    /// <param name="request">HTTP请求对象</param>
    /// <returns>包含移除操作结果的JSON响应字符串</returns>
    /// <remarks>
    /// 该方法支持通过 <see cref="IServiceInfo.ServerId"/> 或 <see cref="IServiceInfo.SessionId"/> 来移除服务节点。
    /// 优先使用ServerId进行移除，如果ServerId为空则使用SessionId。
    /// 移除操作通过 <see cref="NamingServiceManager.TryRemoveByInstanceId"/> 或 <see cref="NamingServiceManager.TrySessionRemove(string)"/> 方法执行。
    /// </remarks>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        var removeRequest = (RemoveNodeRequest)request;
        // 从请求对象中获取ServerId和SessionId
        string serverId = removeRequest.ServerId?.ToString() ?? string.Empty;
        string sessionId = removeRequest.SessionId ?? string.Empty;
        
        var namingServiceManager = NamingServiceManager.Instance;
        bool success = false;
        string message;

        if (!string.IsNullOrEmpty(serverId) && long.TryParse(serverId, out long serverIdLong))
        {
            // 通过ServerId移除节点
            success = namingServiceManager.TryRemoveByInstanceId(serverIdLong);
            message = success 
                ? $"成功移除ServerId为 {serverId} 的节点" 
                : $"移除ServerId为 {serverId} 的节点失败，节点可能不存在";
        }
        else if (!string.IsNullOrEmpty(sessionId))
        {
            // 通过SessionId移除节点
            success = namingServiceManager.TryRemoveBySessionId(sessionId);
            message = success 
                ? $"成功移除SessionId为 {sessionId} 的节点" 
                : $"移除SessionId为 {sessionId} 的节点失败，节点可能不存在";
        }
        else
        {
            message = "请求参数无效，必须提供有效的ServerId或SessionId";
        }

        var response = new RemoveNodeResponse
        {
            Success = success,
            Message = message,
            ServerId = !string.IsNullOrEmpty(serverId) && long.TryParse(serverId, out long serverIdValue) ? serverIdValue : 0,
            SessionId = sessionId ?? string.Empty
        };
        
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

/// <summary>
/// 移除节点的请求对象
/// </summary>
public class RemoveNodeRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 服务器ID（可选）
    /// </summary>
    /// <value>要移除的服务器唯一标识符，与SessionId二选一</value>
    public long? ServerId { get; set; }
    
    /// <summary>
    /// 会话ID（可选）
    /// </summary>
    /// <value>要移除的会话唯一标识符，与ServerId二选一</value>
    public string SessionId { get; set; }
}

/// <summary>
/// 移除节点的响应对象
/// </summary>
public class RemoveNodeResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 操作是否成功
    /// </summary>
    /// <value>表示移除操作是否成功执行的布尔值</value>
    public bool Success { get; set; }
    
    /// <summary>
    /// 响应消息
    /// </summary>
    /// <value>描述移除操作结果的消息字符串</value>
    public string Message { get; set; } = string.Empty;
    
    /// <summary>
    /// 请求中的服务器ID
    /// </summary>
    /// <value>请求中指定的服务器ID，如果未指定则为null</value>
    public long ServerId { get; set; }
    
    /// <summary>
    /// 请求中的SessionId
    /// </summary>
    /// <value>请求中指定的会话ID，如果未指定则为null</value>
    public string SessionId { get; set; }
}