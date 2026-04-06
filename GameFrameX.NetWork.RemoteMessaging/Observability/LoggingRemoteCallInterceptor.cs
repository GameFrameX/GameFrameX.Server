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

namespace GameFrameX.NetWork.RemoteMessaging.Observability;

/// <summary>
/// 日志拦截器。记录远程调用的请求前、响应后和异常信息。
/// </summary>
/// <remarks>
/// Logging interceptor. Records remote call information before requests, after responses, and on exceptions.
/// </remarks>
internal sealed class LoggingRemoteCallInterceptor : IRemoteCallInterceptor
{
    /// <summary>
    /// 记录远程调用开始日志。
    /// </summary>
    /// <remarks>
    /// Logs the start of a remote call.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnBeforeCallAsync(RemoteCallContext context, MessageObject request)
    {
        LogHelper.Debug("RemoteCall 开始, Service: {serviceName}, Message: {messageType}, Timeout: {timeoutMs}ms", context.ServiceName, request.GetType().Name, context.TimeoutMs);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 记录远程调用完成日志。
    /// </summary>
    /// <remarks>
    /// Logs the completion of a remote call.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <param name="response">响应消息（可能为 null） / The response message (may be null)</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnAfterCallAsync(RemoteCallContext context, MessageObject request, MessageObject response, long elapsedMs)
    {
        LogHelper.Debug("RemoteCall 完成, Service: {serviceName}, Message: {messageType}, Elapsed: {elapsedMs}ms", context.ServiceName, request.GetType().Name, elapsedMs);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 记录远程调用异常日志。
    /// </summary>
    /// <remarks>
    /// Logs a remote call exception.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <param name="exception">异常对象 / The exception that occurred</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnExceptionAsync(RemoteCallContext context, MessageObject request, Exception exception, long elapsedMs)
    {
        LogHelper.Error(exception, "RemoteCall 异常, Service: {serviceName}, Message: {messageType}, Elapsed: {elapsedMs}ms", context.ServiceName, request.GetType().Name, elapsedMs);
        return Task.CompletedTask;
    }
}