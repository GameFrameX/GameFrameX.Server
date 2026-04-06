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

namespace GameFrameX.NetWork.RemoteMessaging.Resilience;

/// <summary>
/// 透传健康评估器。用于关闭统一能力时保持老行为：健康分始终满分。
/// </summary>
/// <remarks>
/// Pass-through health evaluator. Used when the unified resilience capability is disabled to preserve legacy behavior:
/// always returns a perfect health score.
/// </remarks>
internal sealed class PassThroughEndpointHealthEvaluator : IEndpointHealthEvaluator
{
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
        return 100;
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
    }
}