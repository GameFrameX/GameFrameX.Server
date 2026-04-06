// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.NetWork.RemoteMessaging.Contracts;

/// <summary>
/// 统一远程消息调用客户端。屏蔽底层连接、编解码与请求匹配细节。
/// </summary>
public interface IRemoteMessageClient
{
    /// <summary>
    /// 发送请求并等待响应（简化版本）。
    /// </summary>
    /// <typeparam name="TResponse">响应消息类型</typeparam>
    /// <param name="serviceName">目标服务名</param>
    /// <param name="requestMessage">请求消息对象</param>
    /// <param name="timeoutMs">超时毫秒数</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应消息对象；超时或失败时返回 null</returns>
    Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : MessageObject;

    /// <summary>
    /// 发送请求并返回结构化结果（包含状态码、耗时、重试信息）。
    /// </summary>
    /// <typeparam name="TResponse">响应消息类型</typeparam>
    /// <param name="context">调用上下文（含超时、重试策略、追踪信息）</param>
    /// <param name="requestMessage">请求消息对象</param>
    /// <returns>结构化调用结果</returns>
    Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        RemoteCallContext context,
        MessageObject requestMessage)
        where TResponse : class;
}
