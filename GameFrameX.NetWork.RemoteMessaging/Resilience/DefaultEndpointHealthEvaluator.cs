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

using System.Collections.Concurrent;

namespace GameFrameX.NetWork.RemoteMessaging.Resilience;

/// <summary>
/// 默认端点健康评估器。基于连续失败次数和恢复时间计算健康评分。
/// </summary>
/// <remarks>
/// Default endpoint health evaluator. Computes health scores based on consecutive failure count and recovery time.
/// </remarks>
internal sealed class DefaultEndpointHealthEvaluator : IEndpointHealthEvaluator
{
    private readonly ConcurrentDictionary<string, HealthTracker> _trackers = new();

    /// <summary>
    /// 获取指定服务的健康评分（0-100）。
    /// </summary>
    /// <remarks>
    /// Gets the health score (0-100) for the specified service.
    /// </remarks>
    /// <param name="serviceName">目标服务名 / Target service name</param>
    /// <returns>健康评分：100=完全健康，0=完全不可用 / Health score: 100 = fully healthy, 0 = completely unavailable</returns>
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

    /// <summary>
    /// 标记指定服务为不可用。
    /// </summary>
    /// <remarks>
    /// Marks the specified service as unavailable.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <param name="reason">不可用原因 / Reason for unavailability</param>
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

    /// <summary>
    /// 标记指定服务为健康。
    /// </summary>
    /// <remarks>
    /// Marks the specified service as healthy.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
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
        public string LastFailureReason;
        public int LastFailureTick;
        public int LastSuccessTick;
    }
}