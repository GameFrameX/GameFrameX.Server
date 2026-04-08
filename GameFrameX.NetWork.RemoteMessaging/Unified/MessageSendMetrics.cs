// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
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

using System.Collections.Concurrent;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 统一消息发送指标记录。用于可观测性：成功率、耗时分位、重试率、超时率。
/// </summary>
/// <remarks>
/// Unified message sending metrics recorder. Used for observability: success rate, latency percentile, retry rate, timeout rate.
/// </remarks>
public sealed class MessageSendMetrics
{
    private readonly ConcurrentDictionary<string, ServiceMetrics> _serviceMetrics = new();

    /// <summary>
    /// 记录玩家消息发送结果
    /// </summary>
    /// <remarks>
    /// Records a player message send result.
    /// </remarks>
    /// <param name="targetType">目标类型 (player/server) / Target type</param>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <param name="playerId">玩家ID / Player ID</param>
    /// <param name="statusCode">状态码 / Status code</param>
    /// <param name="elapsedMs">耗时毫秒 / Elapsed milliseconds</param>
    /// <param name="retryCount">重试次数 / Retry count</param>
    /// <param name="traceId">追踪ID / Trace ID</param>
    public void Record(
        string targetType,
        string serviceName,
        long playerId,
        string statusCode,
        long elapsedMs,
        int retryCount,
        string traceId)
    {
        var metrics = _serviceMetrics.GetOrAdd($"{targetType}:{serviceName}", _ => new ServiceMetrics());
        metrics.Record(statusCode, elapsedMs, retryCount);
    }

    /// <summary>
    /// 获取所有服务的指标快照
    /// </summary>
    /// <remarks>
    /// Gets a snapshot of metrics for all services.
    /// </remarks>
    /// <returns>指标快照列表 / List of metric snapshots</returns>
    public List<ServiceMetricsSnapshot> GetSnapshots()
    {
        var result = new List<ServiceMetricsSnapshot>();
        foreach (var kvp in _serviceMetrics)
        {
            result.Add(kvp.Value.GetSnapshot(kvp.Key));
        }

        return result;
    }

    /// <summary>
    /// 单个服务的指标聚合
    /// </summary>
    /// <remarks>
    /// Metrics aggregation for a single service.
    /// </remarks>
    private sealed class ServiceMetrics
    {
        private long _totalCalls;
        private long _successCalls;
        private long _timeoutCalls;
        private long _retryCalls;
        private long _totalElapsedMs;
        private long _maxElapsedMs;
        private readonly ConcurrentQueue<long> _recentLatencies = new();

        internal void Record(string statusCode, long elapsedMs, int retryCount)
        {
            Interlocked.Increment(ref _totalCalls);

            if (statusCode == "Success" || statusCode == "LocalDelivered" || statusCode == "RemoteDelivered")
            {
                Interlocked.Increment(ref _successCalls);
            }

            if (statusCode == "Timeout")
            {
                Interlocked.Increment(ref _timeoutCalls);
            }

            if (retryCount > 0)
            {
                Interlocked.Increment(ref _retryCalls);
            }

            Interlocked.Add(ref _totalElapsedMs, elapsedMs);

            var currentMax = Interlocked.Read(ref _maxElapsedMs);
            if (elapsedMs > currentMax)
            {
                Interlocked.CompareExchange(ref _maxElapsedMs, elapsedMs, currentMax);
            }

            _recentLatencies.Enqueue(elapsedMs);
            while (_recentLatencies.Count > 1000)
            {
                _recentLatencies.TryDequeue(out _);
            }
        }

        internal ServiceMetricsSnapshot GetSnapshot(string key)
        {
            var latencies = _recentLatencies.ToArray();
            Array.Sort(latencies);

            long p50 = 0, p90 = 0, p99 = 0;
            if (latencies.Length > 0)
            {
                p50 = latencies[(int)(latencies.Length * 0.50)];
                p90 = latencies[(int)(latencies.Length * 0.90)];
                p99 = latencies[(int)(latencies.Length * 0.99)];
            }

            var total = Interlocked.Read(ref _totalCalls);
            return new ServiceMetricsSnapshot
            {
                Key = key,
                TotalCalls = total,
                SuccessCalls = Interlocked.Read(ref _successCalls),
                TimeoutCalls = Interlocked.Read(ref _timeoutCalls),
                RetryCalls = Interlocked.Read(ref _retryCalls),
                SuccessRate = total > 0 ? (double)Interlocked.Read(ref _successCalls) / total : 0,
                AvgElapsedMs = total > 0 ? (double)Interlocked.Read(ref _totalElapsedMs) / total : 0,
                MaxElapsedMs = Interlocked.Read(ref _maxElapsedMs),
                P50ElapsedMs = p50,
                P90ElapsedMs = p90,
                P99ElapsedMs = p99,
            };
        }
    }
}

/// <summary>
/// 服务指标快照
/// </summary>
/// <remarks>
/// Service metrics snapshot.
/// </remarks>
public sealed class ServiceMetricsSnapshot
{
    /// <summary>
    /// 指标键 (targetType:serviceName)
    /// </summary>
    /// <remarks>
    /// Metrics key (targetType:serviceName).
    /// </remarks>
    public string Key { get; init; }

    /// <summary>
    /// 总调用次数
    /// </summary>
    /// <remarks>
    /// Total call count.
    /// </remarks>
    public long TotalCalls { get; init; }

    /// <summary>
    /// 成功次数
    /// </summary>
    /// <remarks>
    /// Success count.
    /// </remarks>
    public long SuccessCalls { get; init; }

    /// <summary>
    /// 超时次数
    /// </summary>
    /// <remarks>
    /// Timeout count.
    /// </remarks>
    public long TimeoutCalls { get; init; }

    /// <summary>
    /// 重试次数
    /// </summary>
    /// <remarks>
    /// Retry count.
    /// </remarks>
    public long RetryCalls { get; init; }

    /// <summary>
    /// 成功率
    /// </summary>
    /// <remarks>
    /// Success rate.
    /// </remarks>
    public double SuccessRate { get; init; }

    /// <summary>
    /// 平均耗时毫秒
    /// </summary>
    /// <remarks>
    /// Average elapsed time in milliseconds.
    /// </remarks>
    public double AvgElapsedMs { get; init; }

    /// <summary>
    /// 最大耗时毫秒
    /// </summary>
    /// <remarks>
    /// Maximum elapsed time in milliseconds.
    /// </remarks>
    public long MaxElapsedMs { get; init; }

    /// <summary>
    /// P50 耗时
    /// </summary>
    /// <remarks>
    /// P50 latency.
    /// </remarks>
    public long P50ElapsedMs { get; init; }

    /// <summary>
    /// P90 耗时
    /// </summary>
    /// <remarks>
    /// P90 latency.
    /// </remarks>
    public long P90ElapsedMs { get; init; }

    /// <summary>
    /// P99 耗时
    /// </summary>
    /// <remarks>
    /// P99 latency.
    /// </remarks>
    public long P99ElapsedMs { get; init; }
}
