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
using System.Diagnostics.Metrics;

namespace GameFrameX.NetWork.RemoteMessaging.Observability;

/// <summary>
/// 基于 System.Diagnostics.Metrics 的远程调用指标采集器。
/// 对接 OpenTelemetry，提供 QPS、成功率、超时率、重试率、P95/P99 耗时分布。
/// </summary>
/// <remarks>
/// Remote call metrics collector based on System.Diagnostics.Metrics.
/// Integrates with OpenTelemetry to provide QPS, success rate, timeout rate, retry rate,
/// and P95/P99 latency distribution.
/// </remarks>
internal sealed class DiagnosticsRemoteCallMetrics : IRemoteCallMetrics
{
    private const string MeterName = "GameFrameX.RemoteMessaging";
    private const string MeterVersion = "1.0.0";
    private readonly Histogram<double> _callDurationHistogram;
    private readonly Counter<long> _callFailureCounter;
    private readonly Counter<long> _callRetryCounter;
    private readonly Counter<long> _callSuccessCounter;
    private readonly Counter<long> _callTimeoutCounter;
    private readonly Counter<long> _callTotalCounter;

    private readonly Meter _meter;

    /// <summary>
    /// 初始化 <see cref="DiagnosticsRemoteCallMetrics"/> 的新实例。
    /// 创建 Meter 对象并注册所有计数器和直方图指标。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of <see cref="DiagnosticsRemoteCallMetrics"/>.
    /// Creates the Meter object and registers all counter and histogram instruments.
    /// </remarks>
    public DiagnosticsRemoteCallMetrics()
    {
        _meter = new Meter(MeterName, MeterVersion);

        _callTotalCounter = _meter.CreateCounter<long>(
            "remotemessaging.calls.total",
            description: "Total number of remote messaging calls");

        _callSuccessCounter = _meter.CreateCounter<long>(
            "remotemessaging.calls.success",
            description: "Number of successful remote messaging calls");

        _callFailureCounter = _meter.CreateCounter<long>(
            "remotemessaging.calls.failure",
            description: "Number of failed remote messaging calls");

        _callTimeoutCounter = _meter.CreateCounter<long>(
            "remotemessaging.calls.timeout",
            description: "Number of timed out remote messaging calls");

        _callRetryCounter = _meter.CreateCounter<long>(
            "remotemessaging.calls.retry",
            description: "Number of retried remote messaging calls");

        _callDurationHistogram = _meter.CreateHistogram<double>(
            "remotemessaging.calls.duration",
            "ms",
            "Duration of remote messaging calls in milliseconds");
    }

    /// <summary>
    /// 记录一次成功的调用指标。
    /// </summary>
    /// <remarks>
    /// Records a successful remote call invocation metric.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="messageType">消息类型名 / The message type name</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <param name="retryCount">重试次数 / The number of retries performed</param>
    public void RecordSuccess(string serviceName, string messageType, long elapsedMs, int retryCount = 0)
    {
        var tags = new TagList
        {
            { "service", serviceName },
            { "message_type", messageType },
        };

        _callTotalCounter.Add(1, tags);
        _callSuccessCounter.Add(1, tags);
        _callDurationHistogram.Record(elapsedMs, tags);

        if (retryCount > 0)
        {
            _callRetryCounter.Add(retryCount, tags);
        }
    }

    /// <summary>
    /// 记录一次失败的调用指标。
    /// </summary>
    /// <remarks>
    /// Records a failed remote call invocation metric.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="messageType">消息类型名 / The message type name</param>
    /// <param name="statusCode">状态码 / The status code of the failure</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    public void RecordFailure(string serviceName, string messageType, RemoteStatusCode statusCode, long elapsedMs)
    {
        var tags = new TagList
        {
            { "service", serviceName },
            { "message_type", messageType },
            { "status_code", statusCode.ToString() },
        };

        _callTotalCounter.Add(1, tags);
        _callFailureCounter.Add(1, tags);
        _callDurationHistogram.Record(elapsedMs, tags);

        if (statusCode == RemoteStatusCode.Timeout)
        {
            _callTimeoutCounter.Add(1, tags);
        }
    }

    /// <summary>
    /// 记录一次重试指标。
    /// </summary>
    /// <remarks>
    /// Records a retry attempt metric.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / The target service name</param>
    /// <param name="messageType">消息类型名 / The message type name</param>
    /// <param name="attemptCount">第几次重试 / The retry attempt number</param>
    public void RecordRetry(string serviceName, string messageType, int attemptCount)
    {
        var tags = new TagList
        {
            { "service", serviceName },
            { "message_type", messageType },
        };

        _callRetryCounter.Add(1, tags);
    }
}