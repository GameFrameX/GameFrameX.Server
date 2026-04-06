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

namespace GameFrameX.NetWork.RemoteMessaging;

/// <summary>
/// 远程消息通信 SDK 构建器。业务侧只需调用 <c>RemoteMessagingBuilder.Build()</c> 即可获得完整功能的客户端。
/// </summary>
/// <example>
///     <code>
/// // 最简用法
/// var client = RemoteMessagingBuilder.Build();
/// 
/// // 自定义配置
/// var client = new RemoteMessagingBuilder()
///     .WithTimeoutMs(10000)
///     .WithRetry(maxRetryCount: 3)
///     .Build();
/// </code>
/// </example>
public sealed class RemoteMessagingBuilder
{
    private RemoteMessagingOptions _options = new();

    /// <summary>
    /// 设置默认超时时间。
    /// </summary>
    /// <param name="timeoutMs">超时毫秒数</param>
    /// <returns>构建器实例</returns>
    public RemoteMessagingBuilder WithTimeoutMs(int timeoutMs)
    {
        _options.DefaultTimeoutMs = timeoutMs;
        return this;
    }

    /// <summary>
    /// 设置重试策略。
    /// </summary>
    /// <param name="maxRetryCount">最大重试次数</param>
    /// <param name="baseDelayMs">基础延迟毫秒</param>
    /// <returns>构建器实例</returns>
    public RemoteMessagingBuilder WithRetry(int maxRetryCount, int baseDelayMs = 200)
    {
        _options.MaxRetryCount = maxRetryCount;
        _options.RetryBaseDelayMs = baseDelayMs;
        _options.EnableRetry = maxRetryCount > 0;
        return this;
    }

    /// <summary>
    /// 设置熔断器参数。
    /// </summary>
    /// <param name="failureThreshold">连续失败阈值</param>
    /// <param name="openDurationMs">熔断开启持续时间毫秒</param>
    /// <returns>构建器实例</returns>
    public RemoteMessagingBuilder WithCircuitBreaker(int failureThreshold = 5, int openDurationMs = 30000)
    {
        _options.CircuitBreakerFailureThreshold = failureThreshold;
        _options.CircuitBreakerOpenDurationMs = openDurationMs;
        return this;
    }

    /// <summary>
    /// 从环境变量加载配置（覆盖之前的设置）。
    /// </summary>
    /// <returns>构建器实例</returns>
    public RemoteMessagingBuilder LoadFromEnvironment()
    {
        _options = RemoteMessagingOptions.LoadFromEnvironment();
        return this;
    }

    /// <summary>
    /// 构建并返回配置好的远程消息客户端。
    /// </summary>
    /// <returns>远程消息客户端实例</returns>
    public IRemoteMessageClient Build()
    {
        var endpointResolver = new AspireEndpointResolver();
        var connectionProvider = new TcpConnectionProvider();
        var messageCodec = new DefaultMessageCodec();
        var requestResponseMatcher = new RequestResponseMatcher();
        var protocolVersionNegotiator = new DefaultProtocolVersionNegotiator();

        var interceptors = new List<IRemoteCallInterceptor>
        {
            new LoggingRemoteCallInterceptor(),
        };

        ICircuitBreaker circuitBreaker;
        IEndpointHealthEvaluator healthEvaluator;

        if (_options.EnableUnifiedClient)
        {
            var metrics = new DiagnosticsRemoteCallMetrics();
            interceptors.Add(new MetricsRemoteCallInterceptor(metrics));
            interceptors.Add(new TraceRemoteCallInterceptor());

            if (_options.EnableFaultInjection)
            {
                interceptors.Add(new FaultInjectionRemoteCallInterceptor());
            }

            circuitBreaker = new DefaultCircuitBreaker(
                _options.CircuitBreakerFailureThreshold,
                _options.CircuitBreakerOpenDurationMs);
            healthEvaluator = new DefaultEndpointHealthEvaluator();
        }
        else
        {
            circuitBreaker = new PassThroughCircuitBreaker();
            healthEvaluator = new PassThroughEndpointHealthEvaluator();
        }

        var retryPolicy = _options.EnableUnifiedClient && _options.EnableRetry
                              ? new DefaultRetryPolicy(_options.MaxRetryCount, _options.RetryBaseDelayMs)
                              : null;

        return new RemoteMessageClient(
            endpointResolver,
            connectionProvider,
            messageCodec,
            requestResponseMatcher,
            protocolVersionNegotiator,
            interceptors.ToArray(),
            retryPolicy,
            circuitBreaker,
            healthEvaluator);
    }

    /// <summary>
    /// 快捷方法：使用默认配置构建客户端。
    /// </summary>
    /// <returns>远程消息客户端实例</returns>
    public static IRemoteMessageClient BuildDefault()
    {
        return new RemoteMessagingBuilder().Build();
    }

    /// <summary>
    /// 快捷方法：从环境变量加载配置并构建客户端。
    /// </summary>
    /// <returns>远程消息客户端实例</returns>
    public static IRemoteMessageClient BuildFromEnvironment()
    {
        return new RemoteMessagingBuilder().LoadFromEnvironment().Build();
    }
}