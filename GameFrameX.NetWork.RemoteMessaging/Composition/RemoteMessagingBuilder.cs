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

namespace GameFrameX.NetWork.RemoteMessaging;

/// <summary>
/// 远程消息通信 SDK 构建器。业务侧只需调用 <c>RemoteMessagingBuilder.Build()</c> 即可获得完整功能的客户端。
/// </summary>
/// <remarks>
/// Remote messaging SDK builder. Business code only needs to call <c>RemoteMessagingBuilder.Build()</c> to obtain a fully functional client.
/// </remarks>
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
    private Func<IServiceEndpointResolver, IConnectionProvider, ITransportProtocolAdapter> _transportAdapterFactory;

    /// <summary>
    /// 设置默认超时时间。
    /// </summary>
    /// <remarks>
    /// Sets the default timeout duration.
    /// </remarks>
    /// <param name="timeoutMs">超时毫秒数 / The timeout duration in milliseconds</param>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder WithTimeoutMs(int timeoutMs)
    {
        _options.DefaultTimeoutMs = timeoutMs;
        return this;
    }

    /// <summary>
    /// 设置重试策略。
    /// </summary>
    /// <remarks>
    /// Configures the retry policy.
    /// </remarks>
    /// <param name="maxRetryCount">最大重试次数 / The maximum number of retry attempts</param>
    /// <param name="baseDelayMs">基础延迟毫秒 / The base delay in milliseconds between retries</param>
    /// <returns>构建器实例 / The builder instance</returns>
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
    /// <remarks>
    /// Configures the circuit breaker parameters.
    /// </remarks>
    /// <param name="failureThreshold">连续失败阈值 / The consecutive failure threshold that triggers the circuit breaker</param>
    /// <param name="openDurationMs">熔断开启持续时间毫秒 / The duration in milliseconds the circuit breaker stays open</param>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder WithCircuitBreaker(int failureThreshold = 5, int openDurationMs = 30000)
    {
        _options.CircuitBreakerFailureThreshold = failureThreshold;
        _options.CircuitBreakerOpenDurationMs = openDurationMs;
        return this;
    }

    /// <summary>
    /// 配置默认压缩算法与压缩阈值。
    /// </summary>
    /// <remarks>
    /// Configures the default compression algorithm and compression threshold.
    /// </remarks>
    /// <param name="defaultAlgorithmId">默认压缩算法 ID（0 表示禁用压缩） / Default compression algorithm ID (0 to disable compression)</param>
    /// <param name="threshold">压缩阈值（字节） / Compression threshold in bytes</param>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder WithCompression(byte defaultAlgorithmId, int threshold = 512)
    {
        _options.DefaultCompressionAlgorithmId = defaultAlgorithmId;
        _options.CompressionThreshold = threshold;
        return this;
    }

    /// <summary>
    /// 配置压缩算法注册表。
    /// </summary>
    /// <remarks>
    /// Configures the compression algorithm registry.
    /// </remarks>
    /// <param name="registry">压缩算法注册表 / The compression algorithm registry</param>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder WithCompressionRegistry(IMessageCompressionRegistry registry)
    {
        _options.CompressionRegistry = registry;
        return this;
    }

    /// <summary>
    /// 配置传输协议适配器工厂。可用于切换 TCP/KCP/QUIC 等不同协议实现。
    /// </summary>
    /// <remarks>
    /// Configures the transport protocol adapter factory. Can be used to switch protocol implementations like TCP/KCP/QUIC.
    /// </remarks>
    /// <param name="factory">适配器工厂 / Adapter factory</param>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder WithTransportAdapterFactory(Func<IServiceEndpointResolver, IConnectionProvider, ITransportProtocolAdapter> factory)
    {
        _transportAdapterFactory = factory;
        return this;
    }

    /// <summary>
    /// 显式使用默认 TCP 适配器。
    /// </summary>
    /// <remarks>
    /// Explicitly uses the default TCP adapter.
    /// </remarks>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder UseTcpTransportAdapter()
    {
        _transportAdapterFactory = static (resolver, provider) => new TcpTransportProtocolAdapter(resolver, provider);
        return this;
    }

    /// <summary>
    /// 使用 KCP 适配器占位实现（当前仅用于接线验证，不可用于生产流量）。
    /// </summary>
    /// <remarks>
    /// Uses the placeholder KCP adapter (for wiring verification only, not for production traffic).
    /// </remarks>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder UseKcpTransportAdapterPlaceholder()
    {
        _transportAdapterFactory = static (resolver, _) => new KcpTransportProtocolAdapter(resolver);
        return this;
    }

    /// <summary>
    /// 从环境变量加载配置（覆盖之前的设置）。
    /// </summary>
    /// <remarks>
    /// Loads configuration from environment variables, overriding any previous settings.
    /// </remarks>
    /// <returns>构建器实例 / The builder instance</returns>
    public RemoteMessagingBuilder LoadFromEnvironment()
    {
        _options = RemoteMessagingOptions.LoadFromEnvironment();
        return this;
    }

    /// <summary>
    /// 构建并返回配置好的远程消息客户端。
    /// </summary>
    /// <remarks>
    /// Builds and returns a configured remote message client.
    /// </remarks>
    /// <returns>远程消息客户端实例 / The configured remote message client instance</returns>
    public IRemoteMessageClient Build()
    {
        var endpointResolver = new AspireEndpointResolver();
        var connectionProvider = new TcpConnectionProvider();
        var transportProtocolAdapter = _transportAdapterFactory?.Invoke(endpointResolver, connectionProvider)
                                     ?? new TcpTransportProtocolAdapter(endpointResolver, connectionProvider);
        var compressionRegistry = _options.CompressionRegistry ?? new DefaultMessageCompressionRegistry();
        var messageCodec = new DefaultMessageCodec(
            compressionRegistry,
            _options.DefaultCompressionAlgorithmId,
            _options.CompressionThreshold);
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
            transportProtocolAdapter,
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
    /// <remarks>
    /// Convenience method: builds a client using default configuration.
    /// </remarks>
    /// <returns>远程消息客户端实例 / The remote message client instance</returns>
    public static IRemoteMessageClient BuildDefault()
    {
        return new RemoteMessagingBuilder().Build();
    }

    /// <summary>
    /// 快捷方法：从环境变量加载配置并构建客户端。
    /// </summary>
    /// <remarks>
    /// Convenience method: loads configuration from environment variables and builds a client.
    /// </remarks>
    /// <returns>远程消息客户端实例 / The remote message client instance</returns>
    public static IRemoteMessageClient BuildFromEnvironment()
    {
        return new RemoteMessagingBuilder().LoadFromEnvironment().Build();
    }
}
