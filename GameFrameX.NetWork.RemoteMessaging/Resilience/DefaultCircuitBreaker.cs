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

using System.Collections.Concurrent;

namespace GameFrameX.NetWork.RemoteMessaging.Resilience;

/// <summary>
/// 默认熔断器实现。按服务维度管理三态（Closed/Open/HalfOpen）。
/// 连续失败达到阈值后熔断，冷却期后进入半开状态允许探测。
/// </summary>
/// <remarks>
/// Default circuit breaker implementation. Manages the three states (Closed/Open/HalfOpen) per service.
/// Trips when consecutive failures reach the threshold; enters half-open state after the cooldown period to allow probing.
/// </remarks>
internal sealed class DefaultCircuitBreaker : ICircuitBreaker
{
    private readonly int _failureThreshold;
    private readonly int _halfOpenMaxAttempts;
    private readonly int _openDurationMs;
    private readonly ConcurrentDictionary<string, CircuitStateTracker> _trackers = new();

    /// <summary>
    /// 初始化默认熔断器。
    /// </summary>
    /// <remarks>
    /// Initializes the default circuit breaker.
    /// </remarks>
    /// <param name="failureThreshold">连续失败阈值（默认 5 次） / Consecutive failure threshold (default 5)</param>
    /// <param name="openDurationMs">熔断开启持续时间毫秒（默认 30000ms） / Open duration in milliseconds (default 30000ms)</param>
    /// <param name="halfOpenMaxAttempts">半开状态最大探测次数（默认 3 次） / Maximum probing attempts in half-open state (default 3)</param>
    public DefaultCircuitBreaker(int failureThreshold = 5, int openDurationMs = 30000, int halfOpenMaxAttempts = 3)
    {
        _failureThreshold = failureThreshold;
        _openDurationMs = openDurationMs;
        _halfOpenMaxAttempts = halfOpenMaxAttempts;
    }

    
    public bool IsAllowed(string serviceName)
    {
        var tracker = _trackers.GetOrAdd(serviceName, _ => new CircuitStateTracker());
        lock (tracker)
        {
            switch (tracker.State)
            {
                case CircuitState.Closed:
                    return true;
                case CircuitState.Open:
                    if (Environment.TickCount - tracker.OpenedAtTick > _openDurationMs)
                    {
                        tracker.State = CircuitState.HalfOpen;
                        tracker.HalfOpenAttempts = 0;
                        tracker.FailureCount = 0;
                        return true;
                    }

                    return false;
                case CircuitState.HalfOpen:
                    return tracker.HalfOpenAttempts < _halfOpenMaxAttempts;
                default:
                    return true;
            }
        }
    }

    
    public void RecordSuccess(string serviceName)
    {
        var tracker = _trackers.GetOrAdd(serviceName, _ => new CircuitStateTracker());
        lock (tracker)
        {
            if (tracker.State == CircuitState.HalfOpen)
            {
                tracker.State = CircuitState.Closed;
                tracker.FailureCount = 0;
                tracker.HalfOpenAttempts = 0;
            }
        }
    }

    
    public void RecordFailure(string serviceName)
    {
        var tracker = _trackers.GetOrAdd(serviceName, _ => new CircuitStateTracker());
        lock (tracker)
        {
            tracker.FailureCount++;
            switch (tracker.State)
            {
                case CircuitState.HalfOpen:
                    tracker.HalfOpenAttempts++;
                    if (tracker.HalfOpenAttempts >= _halfOpenMaxAttempts)
                    {
                        tracker.State = CircuitState.Open;
                        tracker.OpenedAtTick = Environment.TickCount;
                        LogHelper.Warning("CircuitBreaker 触发熔断(半开→打开), Service: {serviceName}, Failures: {failureCount}", serviceName, tracker.FailureCount);
                    }

                    break;
                case CircuitState.Closed:
                    if (tracker.FailureCount >= _failureThreshold)
                    {
                        tracker.State = CircuitState.Open;
                        tracker.OpenedAtTick = Environment.TickCount;
                        LogHelper.Info("CircuitBreaker 触发熔断(关闭→打开), Service: {serviceName}, Failures: {failureCount}", serviceName, tracker.FailureCount);
                    }

                    break;
            }
        }
    }

    
    public CircuitState GetState(string serviceName)
    {
        if (_trackers.TryGetValue(serviceName, out var tracker))
        {
            lock (tracker)
            {
                return tracker.State;
            }
        }

        return CircuitState.Closed;
    }

    private sealed class CircuitStateTracker
    {
        public int FailureCount;
        public int HalfOpenAttempts;
        public int OpenedAtTick;
        public CircuitState State = CircuitState.Closed;
    }
}