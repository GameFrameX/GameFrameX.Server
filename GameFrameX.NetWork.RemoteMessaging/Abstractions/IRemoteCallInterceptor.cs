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

namespace GameFrameX.NetWork.RemoteMessaging.Abstractions;

/// <summary>
/// 远程调用拦截器。支持请求前后和异常时的统一处理。
/// </summary>
public interface IRemoteCallInterceptor
{
    /// <summary>
    /// 请求发送前调用。
    /// </summary>
    /// <param name="context">调用上下文</param>
    /// <param name="request">请求消息</param>
    Task OnBeforeCallAsync(RemoteCallContext context, MessageObject request);

    /// <summary>
    /// 请求成功后调用。
    /// </summary>
    /// <param name="context">调用上下文</param>
    /// <param name="request">请求消息</param>
    /// <param name="response">响应消息（可能为 null）</param>
    /// <param name="elapsedMs">耗时毫秒数</param>
    Task OnAfterCallAsync(RemoteCallContext context, MessageObject request, MessageObject response, long elapsedMs);

    /// <summary>
    /// 请求异常时调用。
    /// </summary>
    /// <param name="context">调用上下文</param>
    /// <param name="request">请求消息</param>
    /// <param name="exception">异常对象</param>
    /// <param name="elapsedMs">耗时毫秒数</param>
    Task OnExceptionAsync(RemoteCallContext context, MessageObject request, Exception exception, long elapsedMs);
}
