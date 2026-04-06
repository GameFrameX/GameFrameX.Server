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


using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Logger;
using System.Diagnostics;

namespace GameFrameX.DataBase.Mongo;

public sealed partial class MongoDbService
{
    /// <summary>
    /// 获取当前健康状态名称。
    /// </summary>
    /// <returns>健康状态名称。</returns>
    public string GetAvailabilityStateName()
    {
        return AvailabilityState.ToString();
    }

    /// <summary>
    /// 记录一次成功操作并根据状态机更新状态。
    /// </summary>
    /// <param name="operationName">操作名称。</param>
    /// <param name="operationType">操作类型。</param>
    private void RecordOperationSuccess(string operationName, string operationType)
    {
        lock (_availabilityLock)
        {
            _consecutiveFailures = 0;
            _consecutiveSuccesses++;
            if (AvailabilityState == DatabaseAvailabilityState.Recovering)
            {
                _recoveringProbeSuccessCount++;
                if (_recoveringProbeSuccessCount >= _recoveringToHealthySuccessThreshold)
                {
                    var unhealthySinceTicks = Interlocked.Read(ref _unhealthySinceTicks);
                    if (unhealthySinceTicks > 0)
                    {
                        var recoveryDuration = TimeSpan.FromTicks(DateTime.UtcNow.Ticks - unhealthySinceTicks);
                        DbRecoveryDurationMilliseconds.Record(recoveryDuration.TotalMilliseconds, new TagList { { "source", "half_open_success" }, { "op", operationType }, { "name", operationName }, });
                    }

                    ChangeAvailabilityState(DatabaseAvailabilityState.Healthy, "recovering_probe_success");
                }
            }
            else if (AvailabilityState == DatabaseAvailabilityState.Degraded && _consecutiveSuccesses >= _degradedToHealthySuccessThreshold)
            {
                ChangeAvailabilityState(DatabaseAvailabilityState.Healthy, "degraded_success_recovered");
            }
        }
    }

    /// <summary>
    /// 记录一次可重试失败并根据状态机更新状态。
    /// </summary>
    /// <param name="operationName">操作名称。</param>
    /// <param name="operationType">操作类型。</param>
    /// <param name="exception">异常信息。</param>
    private void RecordRetryableFailure(string operationName, string operationType, Exception exception)
    {
        lock (_availabilityLock)
        {
            _consecutiveFailures++;
            _consecutiveSuccesses = 0;
            if (AvailabilityState == DatabaseAvailabilityState.Recovering)
            {
                _recoveringProbeSuccessCount = 0;
                ChangeAvailabilityState(DatabaseAvailabilityState.Unhealthy, "recovering_probe_failed");
                EnsureRecoveryTaskStarted();
                return;
            }

            if (AvailabilityState == DatabaseAvailabilityState.Healthy && _consecutiveFailures >= _healthyToDegradedFailureThreshold)
            {
                ChangeAvailabilityState(DatabaseAvailabilityState.Degraded, $"consecutive_failures={_consecutiveFailures}");
            }

            if (_consecutiveFailures >= _degradedToUnhealthyFailureThreshold)
            {
                ChangeAvailabilityState(DatabaseAvailabilityState.Unhealthy, $"consecutive_failures={_consecutiveFailures};reason={GetFailureReason(exception)}");
                EnsureRecoveryTaskStarted();
            }
        }
    }

    /// <summary>
    /// 切换数据库可用性状态并记录审计指标。
    /// </summary>
    /// <param name="newState">目标状态。</param>
    /// <param name="reason">切换原因。</param>
    private void ChangeAvailabilityState(DatabaseAvailabilityState newState, string reason)
    {
        var oldState = AvailabilityState;
        if (oldState == newState)
        {
            return;
        }

        Volatile.Write(ref _availabilityState, (int)newState);
        Interlocked.Exchange(ref _healthStatusValue, (int)newState);
        if (newState == DatabaseAvailabilityState.Unhealthy)
        {
            Interlocked.Exchange(ref _unhealthySinceTicks, DateTime.UtcNow.Ticks);
        }
        else if (newState == DatabaseAvailabilityState.Healthy)
        {
            Interlocked.Exchange(ref _unhealthySinceTicks, 0);
            _recoveringProbeSuccessCount = 0;
        }
        else if (newState == DatabaseAvailabilityState.Recovering)
        {
            _recoveringProbeSuccessCount = 0;
        }

        DbHealthStateTransitionTotal.Add(1, new TagList { { "from", oldState.ToString() }, { "to", newState.ToString() }, { "reason", reason ?? "unknown" }, });
        LogHelper.Warning("MongoDbService.StateTransition {from} -> {to}. reason={reason}", oldState, newState, reason);
    }

    /// <summary>
    /// 判断恢复中的请求是否允许进入半开探测窗口。
    /// </summary>
    /// <returns>允许返回 true；否则返回 false。</returns>
    private bool TryAcquireRecoveringProbeSlot()
    {
        var nowTicks = DateTime.UtcNow.Ticks;
        var windowStartTicks = Interlocked.Read(ref _recoveringProbeWindowStartTicks);
        if (windowStartTicks == 0 || nowTicks - windowStartTicks >= _recoveringProbeWindow.Ticks)
        {
            lock (_availabilityLock)
            {
                windowStartTicks = Interlocked.Read(ref _recoveringProbeWindowStartTicks);
                if (windowStartTicks == 0 || nowTicks - windowStartTicks >= _recoveringProbeWindow.Ticks)
                {
                    Interlocked.Exchange(ref _recoveringProbeWindowStartTicks, nowTicks);
                    Interlocked.Exchange(ref _recoveringProbeWindowCount, 0);
                }
            }
        }

        var currentCount = Interlocked.Increment(ref _recoveringProbeWindowCount);
        return currentCount <= _recoveringMaxProbePerSecond;
    }

    /// <summary>
    /// 判断并执行非核心读降级返回。
    /// </summary>
    /// <typeparam name="T">返回值类型。</typeparam>
    /// <param name="operationName">操作名称。</param>
    /// <param name="fallbackValueFactory">降级值工厂。</param>
    /// <param name="fallbackValue">降级值。</param>
    /// <returns>若触发降级则返回 true。</returns>
    private bool TryReturnDegradedReadFallback<T>(string operationName, Func<T> fallbackValueFactory, out T fallbackValue)
    {
        fallbackValue = default;
        if (!NonCriticalReadOperationWhiteList.Contains(operationName))
        {
            return false;
        }

        var state = AvailabilityState;
        if (state == DatabaseAvailabilityState.Healthy || state == DatabaseAvailabilityState.Degraded)
        {
            return false;
        }

        if (state == DatabaseAvailabilityState.Recovering && TryAcquireRecoveringProbeSlot())
        {
            return false;
        }

        fallbackValue = fallbackValueFactory != null ? fallbackValueFactory() : default;
        DbDegradeActionTotal.Add(1, new TagList { { "kind", "read_fallback" }, { "state", state.ToString() }, { "name", operationName }, });
        LogHelper.Warning("MongoDbService.ReadFallback {operationName} state={state}", operationName, state);
        return true;
    }

    /// <summary>
    /// 判断核心写操作是否应快速失败。
    /// </summary>
    /// <param name="operationName">操作名称。</param>
    /// <returns>允许执行返回 true。</returns>
    private bool IsCoreWriteAllowed(string operationName)
    {
        if (!CoreWriteOperationWhiteList.Contains(operationName))
        {
            return true;
        }

        var state = AvailabilityState;
        if (state == DatabaseAvailabilityState.Unhealthy)
        {
            DbDegradeActionTotal.Add(1, new TagList { { "kind", "write_fast_fail" }, { "state", state.ToString() }, { "name", operationName }, });
            return false;
        }

        if (state == DatabaseAvailabilityState.Recovering && !TryAcquireRecoveringProbeSlot())
        {
            DbDegradeActionTotal.Add(1, new TagList { { "kind", "write_half_open_limited" }, { "state", state.ToString() }, { "name", operationName }, });
            return false;
        }

        return true;
    }
}
