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

using GameFrameX.NetWork.RemoteMessaging.Transport;

namespace GameFrameX.NetWork.RemoteMessaging;

/// <summary>
/// 远程消息通信配置选项。通过环境变量或代码配置控制行为。
/// 环境变量前缀: RemoteMessaging__
/// </summary>
/// <remarks>
/// Remote messaging configuration options. Controls behavior through environment variables or code configuration.
/// Environment variable prefix: RemoteMessaging__
/// </remarks>
public sealed class RemoteMessagingOptions
{
    /// <summary>
    /// 配置键前缀
    /// </summary>
    /// <remarks>
    /// Configuration key prefix.
    /// </remarks>
    private const string EnvPrefix = "RemoteMessaging__";

    /// <summary>
    /// 是否启用统一客户端（默认 true）。设为 false 可回退到旧实现。
    /// </summary>
    /// <remarks>
    /// Whether the unified client is enabled (default true). Set to false to fall back to the legacy implementation.
    /// </remarks>
    /// <value>是否启用统一客户端 / Whether the unified client is enabled</value>
    public bool EnableUnifiedClient { get; set; } = true;

    /// <summary>
    /// 是否启用重试（默认 true）。
    /// </summary>
    /// <remarks>
    /// Whether retry is enabled (default true).
    /// </remarks>
    /// <value>是否启用重试 / Whether retry is enabled</value>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    /// 默认超时毫秒数（默认 5000）。
    /// </summary>
    /// <remarks>
    /// Default timeout in milliseconds (default 5000).
    /// </remarks>
    /// <value>默认超时毫秒数 / The default timeout in milliseconds</value>
    public int DefaultTimeoutMs { get; set; } = 5000;

    /// <summary>
    /// 最大重试次数（默认 2）。
    /// </summary>
    /// <remarks>
    /// Maximum number of retry attempts (default 2).
    /// </remarks>
    /// <value>最大重试次数 / The maximum number of retry attempts</value>
    public int MaxRetryCount { get; set; } = 2;

    /// <summary>
    /// 重试基础延迟毫秒数（默认 200）。
    /// </summary>
    /// <remarks>
    /// Base delay in milliseconds between retries (default 200).
    /// </remarks>
    /// <value>重试基础延迟毫秒数 / The base delay in milliseconds between retries</value>
    public int RetryBaseDelayMs { get; set; } = 200;

    /// <summary>
    /// 熔断失败阈值（默认 5 次连续失败触发熔断）。
    /// </summary>
    /// <remarks>
    /// Circuit breaker failure threshold (default 5 consecutive failures trigger the circuit breaker).
    /// </remarks>
    /// <value>熔断失败阈值 / The consecutive failure threshold that triggers the circuit breaker</value>
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>
    /// 熔断开启持续时间毫秒（默认 30000ms）。
    /// </summary>
    /// <remarks>
    /// Duration in milliseconds the circuit breaker stays open (default 30000ms).
    /// </remarks>
    /// <value>熔断开启持续时间毫秒 / The duration in milliseconds the circuit breaker stays open</value>
    public int CircuitBreakerOpenDurationMs { get; set; } = 30000;

    /// <summary>
    /// 默认压缩算法 ID（默认 Deflate=1，0 表示不压缩）。
    /// </summary>
    /// <remarks>
    /// Default compression algorithm ID (default Deflate=1, 0 means no compression).
    /// </remarks>
    /// <value>默认压缩算法 ID / Default compression algorithm ID</value>
    public byte DefaultCompressionAlgorithmId { get; set; } = DeflateMessageCompressionAlgorithm.Id;

    /// <summary>
    /// 压缩阈值（字节）。仅超过该值才尝试压缩，默认 512。
    /// </summary>
    /// <remarks>
    /// Compression threshold in bytes. Only attempts compression when payload exceeds this value (default 512).
    /// </remarks>
    /// <value>压缩阈值 / Compression threshold in bytes</value>
    public int CompressionThreshold { get; set; } = 512;

    /// <summary>
    /// 压缩算法注册表。为空时将使用默认注册表。
    /// </summary>
    /// <remarks>
    /// Compression algorithm registry. When null, the default registry will be used.
    /// </remarks>
    /// <value>压缩算法注册表 / Compression algorithm registry</value>
    public IMessageCompressionRegistry CompressionRegistry { get; set; }

    /// <summary>
    /// 是否启用故障注入（默认 false）。仅开发环境使用。
    /// </summary>
    /// <remarks>
    /// Whether fault injection is enabled (default false). For development use only.
    /// </remarks>
    /// <value>是否启用故障注入 / Whether fault injection is enabled</value>
    public bool EnableFaultInjection { get; set; }

    /// <summary>
    /// 故障注入类型。
    /// </summary>
    /// <remarks>
    /// The fault injection type.
    /// </remarks>
    /// <value>故障注入类型 / The fault injection type</value>
    public string FaultInjectionType { get; set; } = "None";

    /// <summary>
    /// 故障注入延迟毫秒数。
    /// </summary>
    /// <remarks>
    /// The fault injection delay in milliseconds.
    /// </remarks>
    /// <value>故障注入延迟毫秒数 / The fault injection delay in milliseconds</value>
    public int FaultInjectionDelayMs { get; set; }

    /// <summary>
    /// 从环境变量加载配置。
    /// </summary>
    /// <remarks>
    /// Loads configuration from environment variables.
    /// </remarks>
    /// <returns>配置选项 / The loaded configuration options</returns>
    public static RemoteMessagingOptions LoadFromEnvironment()
    {
        var options = new RemoteMessagingOptions
        {
            EnableUnifiedClient = GetEnvBool("EnableUnifiedClient", true),
            EnableRetry = GetEnvBool("EnableRetry", true),
            DefaultTimeoutMs = GetEnvInt("DefaultTimeoutMs", 5000),
            MaxRetryCount = GetEnvInt("MaxRetryCount", 2),
            RetryBaseDelayMs = GetEnvInt("RetryBaseDelayMs", 200),
            CircuitBreakerFailureThreshold = GetEnvInt("CircuitBreakerFailureThreshold", 5),
            CircuitBreakerOpenDurationMs = GetEnvInt("CircuitBreakerOpenDurationMs", 30000),
            DefaultCompressionAlgorithmId = GetEnvByte("DefaultCompressionAlgorithmId", DeflateMessageCompressionAlgorithm.Id),
            CompressionThreshold = GetEnvInt("CompressionThreshold", 512),
            EnableFaultInjection = GetEnvBool("EnableFaultInjection", false),
            FaultInjectionType = Environment.GetEnvironmentVariable("RemoteMessaging__FaultInjection__Type") ?? "None",
            FaultInjectionDelayMs = GetEnvInt("FaultInjectionDelayMs", 0),
        };
        return options;
    }

    private static bool GetEnvBool(string key, bool defaultValue)
    {
        var value = Environment.GetEnvironmentVariable($"RemoteMessaging__{key}");
        if (value == null)
        {
            return defaultValue;
        }

        return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(value, "1", StringComparison.OrdinalIgnoreCase);
    }

    private static int GetEnvInt(string key, int defaultValue)
    {
        var value = Environment.GetEnvironmentVariable($"RemoteMessaging__{key}");
        if (value == null || !int.TryParse(value, out var result))
        {
            return defaultValue;
        }

        return result;
    }

    private static byte GetEnvByte(string key, byte defaultValue)
    {
        var value = Environment.GetEnvironmentVariable($"RemoteMessaging__{key}");
        if (value == null || !byte.TryParse(value, out var result))
        {
            return defaultValue;
        }

        return result;
    }
}
