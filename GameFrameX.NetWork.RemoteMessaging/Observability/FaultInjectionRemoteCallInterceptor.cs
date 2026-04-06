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
/// 故障注入拦截器。用于演练和测试，模拟超时、断连、慢响应等故障。
/// 通过环境变量 RemoteMessaging__FaultInjection__Type 和 RemoteMessaging__FaultInjection__DelayMs 控制。
/// 仅在开发环境生效。
/// </summary>
/// <remarks>
/// Fault injection interceptor. Used for drills and testing to simulate timeout, connection drop,
/// slow response, and other faults. Controlled via environment variables
/// RemoteMessaging__FaultInjection__Type and RemoteMessaging__FaultInjection__DelayMs.
/// Only effective in development environments.
/// </remarks>
internal sealed class FaultInjectionRemoteCallInterceptor : IRemoteCallInterceptor
{
    private readonly int _delayMs;
    private readonly FaultInjectionType _injectionType;

    /// <summary>
    /// 初始化故障注入拦截器。从环境变量读取注入类型和延迟配置。
    /// </summary>
    /// <remarks>
    /// Initializes the fault injection interceptor. Reads injection type and delay configuration from environment variables.
    /// </remarks>
    public FaultInjectionRemoteCallInterceptor()
    {
        var typeStr = Environment.GetEnvironmentVariable("RemoteMessaging__FaultInjection__Type");
        var delayStr = Environment.GetEnvironmentVariable("RemoteMessaging__FaultInjection__DelayMs");

        _injectionType = Enum.TryParse<FaultInjectionType>(typeStr, true, out var type) ? type : FaultInjectionType.None;
        _delayMs = int.TryParse(delayStr, out var delay) ? delay : 0;
    }

    /// <summary>
    /// 故障注入预处理。根据注入类型模拟超时、断连或慢响应。
    /// </summary>
    /// <remarks>
    /// Fault injection pre-processing. Simulates timeout, connection drop, or slow response based on injection type.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public async Task OnBeforeCallAsync(RemoteCallContext context, MessageObject request)
    {
        switch (_injectionType)
        {
            case FaultInjectionType.Timeout:
                LogHelper.Info("FaultInjection: 模拟超时, Service: {serviceName}, Delay: {delayMs}ms", context.ServiceName, context.TimeoutMs + 1000);
                await Task.Delay(context.TimeoutMs + 1000);
                throw new TimeoutException("Fault injection: simulated timeout");

            case FaultInjectionType.ConnectionDrop:
                LogHelper.Info("FaultInjection: 模拟连接断开, Service: {serviceName}", context.ServiceName);
                throw new IOException("Fault injection: simulated connection drop");

            case FaultInjectionType.SlowResponse:
                var slowDelay = _delayMs > 0 ? _delayMs : 2000;
                LogHelper.Info("FaultInjection: 模拟慢响应, Service: {serviceName}, Delay: {delayMs}ms", context.ServiceName, slowDelay);
                await Task.Delay(slowDelay);
                break;
        }
    }

    /// <summary>
    /// 故障注入后处理（空操作）。
    /// </summary>
    /// <remarks>
    /// Fault injection post-processing (no-op).
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <param name="response">响应消息（可能为 null） / The response message (may be null)</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnAfterCallAsync(RemoteCallContext context, MessageObject request, MessageObject response, long elapsedMs)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 故障注入异常处理（空操作）。
    /// </summary>
    /// <remarks>
    /// Fault injection exception handling (no-op).
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <param name="exception">异常对象 / The exception that occurred</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnExceptionAsync(RemoteCallContext context, MessageObject request, Exception exception, long elapsedMs)
    {
        return Task.CompletedTask;
    }
}