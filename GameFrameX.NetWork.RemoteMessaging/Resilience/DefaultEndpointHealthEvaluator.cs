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
using GameFrameX.NetWork.RemoteMessaging.Contracts;

namespace GameFrameX.NetWork.RemoteMessaging.Resilience;

/// <summary>
/// 默认端点健康评估器。基于连续失败次数和恢复时间计算健康评分。
/// </summary>
internal sealed class DefaultEndpointHealthEvaluator : IEndpointHealthEvaluator
{
    private readonly ConcurrentDictionary<string, HealthTracker> _trackers = new();

    /// <inheritdoc />
    public int GetHealthScore(string serviceName)
    {
        if (!_trackers.TryGetValue(serviceName, out var tracker))
        {
            return 100;
        }

        lock (tracker)
        {
            if (tracker.ConsecutiveFailures == 0)
            {
                return 100;
            }

            // 最近有成功记录则恢复
            var elapsedSinceRecovery = Environment.TickCount - tracker.LastSuccessTick;
            if (tracker.LastSuccessTick > 0 && elapsedSinceRecovery < 60000)
            {
                return 80;
            }

            // 按连续失败次数衰减
            var score = Math.Max(0, 100 - tracker.ConsecutiveFailures * 20);
            return score;
        }
    }

    /// <inheritdoc />
    public void MarkUnavailable(string serviceName, string reason)
    {
        var tracker = _trackers.GetOrAdd(serviceName, _ => new HealthTracker());
        lock (tracker)
        {
            tracker.ConsecutiveFailures++;
            tracker.LastFailureTick = Environment.TickCount;
            tracker.LastFailureReason = reason;
        }
    }

    /// <inheritdoc />
    public void MarkHealthy(string serviceName)
    {
        var tracker = _trackers.GetOrAdd(serviceName, _ => new HealthTracker());
        lock (tracker)
        {
            tracker.ConsecutiveFailures = 0;
            tracker.LastSuccessTick = Environment.TickCount;
            tracker.LastFailureReason = null;
        }
    }

    private sealed class HealthTracker
    {
        public int ConsecutiveFailures;
        public int LastFailureTick;
        public int LastSuccessTick;
        public string LastFailureReason;
    }
}
