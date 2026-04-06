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
using GameFrameX.NetWork.RemoteMessaging.Abstractions;

namespace GameFrameX.NetWork.RemoteMessaging.Internal;

/// <summary>
/// 基于 System.Diagnostics.Metrics 的远程调用指标采集器。
/// 对接 OpenTelemetry，提供 QPS、成功率、超时率、重试率、P95/P99 耗时分布。
/// </summary>
internal sealed class DiagnosticsRemoteCallMetrics : IRemoteCallMetrics
{
    private const string MeterName = "GameFrameX.RemoteMessaging";
    private const string MeterVersion = "1.0.0";

    private readonly Meter _meter;
    private readonly Counter<long> _callTotalCounter;
    private readonly Counter<long> _callSuccessCounter;
    private readonly Counter<long> _callFailureCounter;
    private readonly Counter<long> _callTimeoutCounter;
    private readonly Counter<long> _callRetryCounter;
    private readonly Histogram<double> _callDurationHistogram;

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
            unit: "ms",
            description: "Duration of remote messaging calls in milliseconds");
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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
