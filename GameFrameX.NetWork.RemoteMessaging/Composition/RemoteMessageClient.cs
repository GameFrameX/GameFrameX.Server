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

using System.Diagnostics;
using System.Net.Sockets;
using GameFrameX.NetWork.RemoteMessaging.Contracts;

namespace GameFrameX.NetWork.RemoteMessaging.Composition;

/// <summary>
/// 统一远程消息调用客户端。串联服务解析、连接管理、编解码、请求匹配、拦截器、重试、熔断和健康探测。
/// </summary>
internal sealed class RemoteMessageClient : IRemoteMessageClient
{
    private readonly IServiceEndpointResolver _endpointResolver;
    private readonly IConnectionProvider _connectionProvider;
    private readonly IMessageCodec _messageCodec;
    private readonly IRequestResponseMatcher _requestResponseMatcher;
    private readonly IProtocolVersionNegotiator _protocolVersionNegotiator;
    private readonly IRemoteCallInterceptor[] _interceptors;
    private readonly IRetryPolicy _retryPolicy;
    private readonly ICircuitBreaker _circuitBreaker;
    private readonly IEndpointHealthEvaluator _healthEvaluator;
    private readonly SemaphoreSlim _callSemaphore = new(1, 1);

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

    /// <inheritdoc />
    public async Task<TResponse> CallAsync<TResponse>(
        string serviceName,
        MessageObject requestMessage,
        int timeoutMs,
        CancellationToken cancellationToken = default)
        where TResponse : MessageObject
    {
        var context = RemoteCallContext.Create(serviceName, timeoutMs, cancellationToken);
        var result = await CallWithResultAsync<TResponse>(context, requestMessage);
        return result.Response;
    }

    /// <inheritdoc />
    public async Task<RemoteCallResult<TResponse>> CallWithResultAsync<TResponse>(
        RemoteCallContext context,
        MessageObject requestMessage)
        where TResponse : class
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

                    var requestBuffer = _messageCodec.Encode(requestMessage);
                    await stream.WriteAsync(requestBuffer, 0, requestBuffer.Length, context.CancellationToken);

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
    private static void PrepareRequestMessage(MessageObject message, int uniqueId)
    {
        MessageProtoHelper.SetMessageId(message);
        message.SetOperationType(MessageProtoHelper.GetMessageOperationType(message));
        message.SetUniqueId(uniqueId);
    }

    /// <summary>
    /// 尝试将服务地址解析为主机和端口。
    /// </summary>
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
