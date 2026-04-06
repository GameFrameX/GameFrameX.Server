// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Diagnostics;
using System.Net.Sockets;

namespace GameFrameX.NetWork.RemoteMessaging.Composition;

/// <summary>
/// 统一远程消息调用客户端。串联服务解析、连接管理、编解码、请求匹配、拦截器、重试、熔断和健康探测。
/// </summary>
/// <remarks>
/// Unified remote message call client. Orchestrates service resolution, connection management, encoding/decoding, request-response matching, interceptors, retry, circuit breaking, and health probing.
/// </remarks>
internal sealed class RemoteMessageClient : IRemoteMessageClient
{
    private readonly SemaphoreSlim _callSemaphore = new(1, 1);
    private readonly ICircuitBreaker _circuitBreaker;
    private readonly IConnectionProvider _connectionProvider;
    private readonly IServiceEndpointResolver _endpointResolver;
    private readonly IEndpointHealthEvaluator _healthEvaluator;
    private readonly IRemoteCallInterceptor[] _interceptors;
    private readonly IMessageCodec _messageCodec;
    private readonly IProtocolVersionNegotiator _protocolVersionNegotiator;
    private readonly IRequestResponseMatcher _requestResponseMatcher;
    private readonly IRetryPolicy _retryPolicy;

    /// <summary>
    /// 初始化统一远程消息调用客户端。
    /// </summary>
    /// <remarks>
    /// Initializes the unified remote message call client.
    /// </remarks>
    /// <param name="endpointResolver">服务端点解析器 / Service endpoint resolver</param>
    /// <param name="connectionProvider">连接提供器 / Connection provider</param>
    /// <param name="messageCodec">消息编解码器 / Message codec</param>
    /// <param name="requestResponseMatcher">请求-响应匹配器 / Request-response matcher</param>
    /// <param name="protocolVersionNegotiator">协议版本协商器 / Protocol version negotiator</param>
    /// <param name="interceptors">远程调用拦截器数组 / Array of remote call interceptors</param>
    /// <param name="retryPolicy">重试策略（可为 null） / Retry policy (can be null)</param>
    /// <param name="circuitBreaker">熔断器 / Circuit breaker</param>
    /// <param name="healthEvaluator">端点健康评估器 / Endpoint health evaluator</param>
    public RemoteMessageClient(
        IServiceEndpointResolver endpointResolver,
        IConnectionProvider connectionProvider,
        IMessageCodec messageCodec,
        IRequestResponseMatcher requestResponseMatcher,
        IProtocolVersionNegotiator protocolVersionNegotiator,
        IRemoteCallInterceptor[] interceptors,
        IRetryPolicy retryPolicy,
        ICircuitBreaker circuitBreaker,
        IEndpointHealthEvaluator healthEvaluator)
    {
        _endpointResolver = endpointResolver;
        _connectionProvider = connectionProvider;
        _messageCodec = messageCodec;
        _requestResponseMatcher = requestResponseMatcher;
        _protocolVersionNegotiator = protocolVersionNegotiator;
        _interceptors = interceptors ?? Array.Empty<IRemoteCallInterceptor>();
        _retryPolicy = retryPolicy;
        _circuitBreaker = circuitBreaker;
        _healthEvaluator = healthEvaluator;
    }

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
    public Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage)
        where TResponse : class, IResponseMessage
    {
        return CallAsync<TResponse>(serviceName, requestMessage, RemoteCallContext.DefaultTimeoutMs);
    }

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
    public Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        CancellationToken cancellationToken)
        where TResponse : class, IResponseMessage
    {
        return CallAsync<TResponse>(serviceName, requestMessage, RemoteCallContext.DefaultTimeoutMs, cancellationToken);
    }

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
    public async Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.Create(serviceName, timeoutMs, cancellationToken);
        var result = await CallWithResultAsync<TResponse>(context, requestMessage);
        return result.Response;
    }

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
    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.Create(serviceName);
        return CallWithResultAsync<TResponse>(context, requestMessage);
    }

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
    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.Create(serviceName, timeoutMs);
        return CallWithResultAsync<TResponse>(context, requestMessage);
    }

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
    public Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs,
        CancellationToken cancellationToken)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.Create(serviceName, timeoutMs, cancellationToken);
        return CallWithResultAsync<TResponse>(context, requestMessage);
    }

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
    public Task<RemoteCallResult<TResponse>> CallWithRetryAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        int maxRetryCount = RemoteCallContext.DefaultMaxRetryCount,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.CreateIdempotent(serviceName, timeoutMs, maxRetryCount, cancellationToken);
        return CallWithResultAsync<TResponse>(context, requestMessage);
    }

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
    public Task<RemoteCallResult<TResponse>> CallWithoutRetryAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.CreateNonIdempotent(serviceName, timeoutMs, cancellationToken);
        return CallWithResultAsync<TResponse>(context, requestMessage);
    }

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
    public Task<RemoteCallResult<TResponse>> CallWithMetadataAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        Dictionary<string, string> metadata,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : class, IResponseMessage
    {
        var context = RemoteCallContext.CreateWithMetadata(serviceName, metadata, timeoutMs, cancellationToken);
        return CallWithResultAsync<TResponse>(context, requestMessage);
    }

    /// <summary>
    /// 发送请求并返回结构化结果（包含状态码、耗时、重试信息）。
    /// </summary>
    /// <remarks>
    /// Sends a request and returns a structured result containing status code, elapsed time, and retry information.
    /// </remarks>
    /// <param name="context">调用上下文（含超时、重试策略、追踪信息） / The call context (including timeout, retry strategy, and tracing info)</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <returns>结构化调用结果 / The structured call result</returns>
    public async Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        RemoteCallContext context,
        MessageObject requestMessage)
        where TResponse : class, IResponseMessage
    {
        var stopwatch = Stopwatch.StartNew();
        var attemptCount = 0;

        while (true)
        {
            attemptCount++;
            stopwatch.Restart();
            _requestResponseMatcher?.CleanupExpired();

            // 熔断检查
            if (!_circuitBreaker.IsAllowed(context.ServiceName))
            {
                return RemoteCallResult<TResponse>.Fail(
                    RemoteStatusCode.CircuitOpen,
                    $"Circuit breaker is open for service: {context.ServiceName}",
                    stopwatch.ElapsedMilliseconds,
                    context.TraceId);
            }

            // 健康探测检查
            var healthScore = _healthEvaluator.GetHealthScore(context.ServiceName);
            if (healthScore <= 0)
            {
                return RemoteCallResult<TResponse>.Fail(
                    RemoteStatusCode.ServiceUnavailable,
                    $"Service health score is {healthScore}, considered unavailable: {context.ServiceName}",
                    stopwatch.ElapsedMilliseconds,
                    context.TraceId);
            }

            try
            {
                await RunBeforeInterceptorsAsync(context, requestMessage);

                if (_protocolVersionNegotiator != null && !_protocolVersionNegotiator.IsCompatible(requestMessage.GetType()))
                {
                    return RemoteCallResult<TResponse>.Fail(
                        RemoteStatusCode.UnexpectedResponse,
                        $"Protocol version incompatible for message type: {requestMessage.GetType().Name}",
                        stopwatch.ElapsedMilliseconds,
                        context.TraceId);
                }

                var endpoint = _endpointResolver.ResolveTcpEndpoint(context.ServiceName);
                if (!TryParseTcpEndpoint(endpoint, out var host, out var port))
                {
                    LogHelper.Error("RemoteMessageClient.CallWithResultAsync 解析服务 TCP 地址失败, Service: {serviceName}, Endpoint: {endpoint}", context.ServiceName, endpoint);
                    return RemoteCallResult<TResponse>.Fail(RemoteStatusCode.EndpointNotFound, "Failed to resolve service endpoint", stopwatch.ElapsedMilliseconds, context.TraceId);
                }

                await _callSemaphore.WaitAsync(context.CancellationToken);
                try
                {
                    var stream = await _connectionProvider.GetOrCreateStreamAsync(host, port, context.CancellationToken);
                    if (stream == null)
                    {
                        RecordFailures(context.ServiceName, "Failed to create connection");
                        return RemoteCallResult<TResponse>.Fail(RemoteStatusCode.ConnectionFailed, "Failed to create connection", stopwatch.ElapsedMilliseconds, context.TraceId);
                    }

                    var requestUniqueId = _requestResponseMatcher?.RegisterPendingRequest(context.TimeoutMs) ?? IdGenerator.GetNextUniqueIntId();
                    PrepareRequestMessage(requestMessage, requestUniqueId);

                    using var requestBuffer = _messageCodec.Encode(requestMessage);
                    await stream.WriteAsync(requestBuffer.Memory, context.CancellationToken);

                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken);
                    timeoutCts.CancelAfter(context.TimeoutMs);
                    while (true)
                    {
                        var responseMessage = await _messageCodec.DecodeAsync(stream, timeoutCts.Token);
                        if (responseMessage == null)
                        {
                            RecordFailures(context.ServiceName, "Connection closed during response");
                            return RemoteCallResult<TResponse>.Fail(RemoteStatusCode.ConnectionClosed, "Connection closed during response", stopwatch.ElapsedMilliseconds, context.TraceId);
                        }

                        _requestResponseMatcher?.TryComplete(responseMessage.UniqueId, responseMessage);
                        if (responseMessage.UniqueId != requestMessage.UniqueId)
                        {
                            continue;
                        }

                        stopwatch.Stop();
                        var matchedResponse = responseMessage;
                        if (matchedResponse is TResponse typedResponse)
                        {
                            RecordSuccess(context.ServiceName);
                            await RunAfterInterceptorsAsync(context, requestMessage, responseMessage, stopwatch.ElapsedMilliseconds);
                            return RemoteCallResult<TResponse>.Ok(typedResponse, stopwatch.ElapsedMilliseconds, context.TraceId, attemptCount - 1);
                        }

                        return RemoteCallResult<TResponse>.Fail(RemoteStatusCode.UnexpectedResponse, "Response type mismatch", stopwatch.ElapsedMilliseconds, context.TraceId);
                    }
                }
                finally
                {
                    _callSemaphore.Release();
                }
            }
            catch (OperationCanceledException)
            {
                stopwatch.Stop();
                var statusCode = context.CancellationToken.IsCancellationRequested ? RemoteStatusCode.Cancelled : RemoteStatusCode.Timeout;
                var errorMessage = statusCode == RemoteStatusCode.Cancelled ? "Request was cancelled" : $"Request timed out after {context.TimeoutMs}ms";

                RecordFailures(context.ServiceName, errorMessage);
                await RunExceptionInterceptorsAsync(context, requestMessage, new TimeoutException(errorMessage), stopwatch.ElapsedMilliseconds);

                if (ShouldRetry(context, statusCode, attemptCount))
                {
                    await Task.Delay(_retryPolicy.GetRetryDelayMs(attemptCount));
                    continue;
                }

                return RemoteCallResult<TResponse>.Fail(statusCode, errorMessage, stopwatch.ElapsedMilliseconds, context.TraceId);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _connectionProvider.Invalidate();

                RecordFailures(context.ServiceName, ex.Message);
                await RunExceptionInterceptorsAsync(context, requestMessage, ex, stopwatch.ElapsedMilliseconds);

                var statusCode = IsConnectionException(ex) ? RemoteStatusCode.ConnectionFailed : RemoteStatusCode.UnknownError;
                if (ShouldRetry(context, statusCode, attemptCount))
                {
                    await Task.Delay(_retryPolicy.GetRetryDelayMs(attemptCount));
                    continue;
                }

                return RemoteCallResult<TResponse>.Fail(statusCode, ex.Message, stopwatch.ElapsedMilliseconds, context.TraceId);
            }
        }
    }

    /// <summary>
    /// 单向发送消息，不等待响应。
    /// </summary>
    /// <remarks>
    /// Sends a message one-way without waiting for a response.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="requestMessage">请求消息对象 / The request message object</param>
    /// <param name="timeoutMs">发送超时毫秒数 / The send timeout duration in milliseconds</param>
    /// <param name="cancellationToken">取消令牌 / The cancellation token</param>
    public async Task SendOneWayAsync(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs = RemoteCallContext.DefaultTimeoutMs,
        CancellationToken cancellationToken = default)
    {
        if (!_circuitBreaker.IsAllowed(serviceName))
        {
            LogHelper.Warning("SendOneWayAsync: 熔断器已打开, Service: {serviceName}", serviceName);
            return;
        }

        var healthScore = _healthEvaluator.GetHealthScore(serviceName);
        if (healthScore <= 0)
        {
            LogHelper.Warning("SendOneWayAsync: 服务健康评分过低, Service: {serviceName}, Score: {healthScore}", serviceName, healthScore);
            return;
        }

        var context = RemoteCallContext.Create(serviceName, timeoutMs, cancellationToken);

        try
        {
            await RunBeforeInterceptorsAsync(context, requestMessage);

            if (_protocolVersionNegotiator != null && !_protocolVersionNegotiator.IsCompatible(requestMessage.GetType()))
            {
                LogHelper.Warning("SendOneWayAsync: 协议版本不兼容, MessageType: {messageType}", requestMessage.GetType().Name);
                return;
            }

            var endpoint = _endpointResolver.ResolveTcpEndpoint(serviceName);
            if (!TryParseTcpEndpoint(endpoint, out var host, out var port))
            {
                LogHelper.Error("SendOneWayAsync: 解析服务 TCP 地址失败, Service: {serviceName}, Endpoint: {endpoint}", serviceName, endpoint);
                return;
            }

            await _callSemaphore.WaitAsync(cancellationToken);
            try
            {
                var stream = await _connectionProvider.GetOrCreateStreamAsync(host, port, cancellationToken);
                if (stream == null)
                {
                    RecordFailures(serviceName, "SendOneWayAsync: Failed to create connection");
                    return;
                }

                var requestUniqueId = _requestResponseMatcher?.RegisterPendingRequest(timeoutMs) ?? IdGenerator.GetNextUniqueIntId();
                PrepareRequestMessage(requestMessage, requestUniqueId);

                using var requestBuffer = _messageCodec.Encode(requestMessage);
                await stream.WriteAsync(requestBuffer.Memory, cancellationToken);

                RecordSuccess(serviceName);
                await RunAfterInterceptorsAsync(context, requestMessage, null, 0);
            }
            finally
            {
                _callSemaphore.Release();
            }
        }
        catch (OperationCanceledException)
        {
            RecordFailures(serviceName, cancellationToken.IsCancellationRequested ? "SendOneWayAsync cancelled" : "SendOneWayAsync timed out");
        }
        catch (Exception ex)
        {
            _connectionProvider.Invalidate();
            RecordFailures(serviceName, $"SendOneWayAsync error: {ex.Message}");
        }
    }

    /// <summary>
    /// 检查目标服务是否可用。
    /// </summary>
    /// <remarks>
    /// Checks whether the target service is available.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <returns>服务是否可用 / Whether the service is available</returns>
    public Task<bool> IsServiceAvailableAsync(string serviceName)
    {
        if (!_circuitBreaker.IsAllowed(serviceName))
        {
            return Task.FromResult(false);
        }

        var healthScore = _healthEvaluator.GetHealthScore(serviceName);
        if (healthScore <= 0)
        {
            return Task.FromResult(false);
        }

        var endpoint = _endpointResolver.ResolveTcpEndpoint(serviceName);
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return Task.FromResult(false);
        }

        return Task.FromResult(true);
    }

    private void RecordSuccess(string serviceName)
    {
        _circuitBreaker.RecordSuccess(serviceName);
        _healthEvaluator.MarkHealthy(serviceName);
    }

    private void RecordFailures(string serviceName, string reason)
    {
        _circuitBreaker.RecordFailure(serviceName);
        _healthEvaluator.MarkUnavailable(serviceName, reason);
    }

    private bool ShouldRetry(RemoteCallContext context, RemoteStatusCode statusCode, int attemptCount)
    {
        return _retryPolicy != null && _retryPolicy.ShouldRetry(context, statusCode, attemptCount);
    }

    private static bool IsConnectionException(Exception ex)
    {
        return ex is IOException || ex is SocketException;
    }

    private async Task RunBeforeInterceptorsAsync(RemoteCallContext context, MessageObject request)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.OnBeforeCallAsync(context, request);
        }
    }

    private async Task RunAfterInterceptorsAsync(RemoteCallContext context, MessageObject request, MessageObject response, long elapsedMs)
    {
        foreach (var interceptor in _interceptors)
        {
            await interceptor.OnAfterCallAsync(context, request, response, elapsedMs);
        }
    }

    private async Task RunExceptionInterceptorsAsync(RemoteCallContext context, MessageObject request, Exception exception, long elapsedMs)
    {
        foreach (var interceptor in _interceptors)
        {
            try
            {
                await interceptor.OnExceptionAsync(context, request, exception, elapsedMs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, "Interceptor OnExceptionAsync failed");
            }
        }
    }

    /// <summary>
    /// 准备请求消息的元数据（消息ID、操作类型、唯一ID）。
    /// </summary>
    /// <remarks>
    /// Prepares request message metadata (message ID, operation type, unique ID).
    /// </remarks>
    private static void PrepareRequestMessage(MessageObject message, int uniqueId)
    {
        MessageProtoHelper.SetMessageId(message);
        message.SetOperationType(MessageProtoHelper.GetMessageOperationType(message));
        message.SetUniqueId(uniqueId);
    }

    /// <summary>
    /// 尝试将服务地址解析为主机和端口。
    /// </summary>
    /// <remarks>
    /// Attempts to parse a service address into host and port components.
    /// </remarks>
    private static bool TryParseTcpEndpoint(string endpoint, out string host, out int port)
    {
        host = string.Empty;
        port = 0;
        if (string.IsNullOrWhiteSpace(endpoint))
        {
            return false;
        }

        var normalized = endpoint.Trim();
        if (!normalized.Contains("://", StringComparison.Ordinal))
        {
            normalized = $"tcp://{normalized}";
        }

        if (Uri.TryCreate(normalized, UriKind.Absolute, out var uri))
        {
            host = uri.Host;
            port = uri.Port;
            return !string.IsNullOrWhiteSpace(host) && port > 0;
        }

        var lastSeparatorIndex = endpoint.LastIndexOf(':');
        if (lastSeparatorIndex <= 0 || lastSeparatorIndex >= endpoint.Length - 1)
        {
            return false;
        }

        host = endpoint[..lastSeparatorIndex];
        return int.TryParse(endpoint[(lastSeparatorIndex + 1)..], out port) && port > 0;
    }
}
