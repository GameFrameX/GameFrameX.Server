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
/// <remarks>
/// Unified remote message call client. Abstracts away underlying connection management, encoding/decoding, and request-response matching details.
/// </remarks>
public interface IRemoteMessageClient
{
    // ─────────────────────────────────────────────────────────────
    //  CallAsync — 简化调用（失败返回 null）
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 发送请求并等待响应（使用默认超时）。
    /// </summary>
    /// <remarks>
    /// Sends a request and waits for a response (using default timeout).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <returns>响应消息对象；超时或失败时返回 null / The response message object; returns null on timeout or failure</returns>
    Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage)
        where TResponse : class, IResponseMessage;

    /// <summary>
    /// 发送请求并等待响应（使用默认超时，支持取消）。
    /// </summary>
    /// <remarks>
    /// Sends a request and waits for a response (using default timeout, with cancellation support).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>响应消息对象；超时或失败时返回 null / The response message object; returns null on timeout or failure</returns>
    Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        CancellationToken cancellationToken)
        where TResponse : class, IResponseMessage;

    /// <summary>
    /// 发送请求并等待响应（指定超时）。
    /// </summary>
    /// <remarks>
    /// Sends a request and waits for a response (with specified timeout).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>响应消息对象；超时或失败时返回 null / The response message object; returns null on timeout or failure</returns>
    Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage;

    // ─────────────────────────────────────────────────────────────
    //  CallWithResultAsync — 结构化结果调用
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 发送请求并返回结构化结果（使用默认超时）。
    /// </summary>
    /// <remarks>
    /// Sends a request and returns a structured result (using default timeout).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage)
        where TResponse : class, IResponseMessage;

    /// <summary>
    /// 发送请求并返回结构化结果（指定超时）。
    /// </summary>
    /// <remarks>
    /// Sends a request and returns a structured result (with specified timeout).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs)
        where TResponse : class, IResponseMessage;

    /// <summary>
    /// 发送请求并返回结构化结果（指定超时和取消令牌）。
    /// </summary>
    /// <remarks>
    /// Sends a request and returns a structured result (with specified timeout and cancellation token).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs,
        CancellationToken cancellationToken)
        where TResponse : class, IResponseMessage;

    /// <summary>
    /// 发送请求并返回结构化结果（完整上下文版本）。
    /// </summary>
    /// <remarks>
    /// Sends a request and returns a structured result (full context version).
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="context">调用上下文（含超时、重试策略、追踪信息、环境参数） / The call context (including timeout, retry strategy, tracing info, and environment parameters)</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        RemoteCallContext context,
        MessageObject requestMessage)
        where TResponse : class, IResponseMessage;

    // ─────────────────────────────────────────────────────────────
    //  CallWithRetryAsync / CallWithoutRetryAsync — 重试语义便捷方法
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 发送幂等请求并返回结构化结果（自动启用重试）。适用于读操作和查询接口。
    /// </summary>
    /// <remarks>
    /// Sends an idempotent request and returns a structured result (retry automatically enabled). Suitable for read operations and query interfaces.
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <param name="maxRetryCount">最大重试次数 / Maximum retry count</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithRetryAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        int maxRetryCount = RemoteCallContext.DefaultMaxRetryCount,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage;

    /// <summary>
    /// 发送非幂等请求并返回结构化结果（禁用重试）。适用于写操作和状态变更接口。
    /// </summary>
    /// <remarks>
    /// Sends a non-idempotent request and returns a structured result (retry disabled). Suitable for write operations and state-changing interfaces.
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithoutRetryAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage;

    // ─────────────────────────────────────────────────────────────
    //  CallWithMetadataAsync — 携带环境参数的调用
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 发送请求并返回结构化结果（携带环境参数）。环境参数可通过拦截器读取，用于日志记录、链路追踪等。
    /// </summary>
    /// <remarks>
    /// Sends a request and returns a structured result (with environment parameters). Environment parameters can be read by interceptors for logging, tracing, etc.
    /// </remarks>
    /// <typeparam name="TResponse">响应消息类型 / The response message type</typeparam>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="metadata">环境参数字典（如 ServerId、PlayerId 等） / Environment parameter dictionary (e.g., ServerId, PlayerId)</param>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    Task<RemoteCallResult<TResponse>> CallWithMetadataAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        Dictionary<string, string> metadata,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage;

    // ─────────────────────────────────────────────────────────────
    //  SendOneWayAsync — 单向发送
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 单向发送消息，不等待响应。消息会写入网络流但不注册请求-响应匹配。
    /// 适用于日志收集、事件通知、指标上报等无需确认的场景。
    /// </summary>
    /// <remarks>
    /// Sends a message one-way without waiting for a response. The message is written to the network stream but no request-response matching is registered.
    /// Suitable for log collection, event notifications, metric reporting, and other scenarios where confirmation is not required.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">发送超时毫秒数 / The send timeout duration in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    /// <returns>异步任务 / An asynchronous task</returns>
    Task SendOneWayAsync(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        CancellationToken cancellationToken = default);

    // ─────────────────────────────────────────────────────────────
    //  IsServiceAvailableAsync — 服务可用性检查
    // ─────────────────────────────────────────────────────────────

    /// <summary>
    /// 检查目标服务是否可用（基于熔断器状态和健康评分判断）。
    /// 此方法不发送实际请求，仅检查本地维护的服务健康状态。
    /// </summary>
    /// <remarks>
    /// Checks whether the target service is available (based on circuit breaker state and health score).
    /// This method does not send an actual request; it only checks locally maintained service health state.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <returns>服务是否可用 / Whether the service is available</returns>
    Task<bool> IsServiceAvailableAsync(string serviceName);
}
