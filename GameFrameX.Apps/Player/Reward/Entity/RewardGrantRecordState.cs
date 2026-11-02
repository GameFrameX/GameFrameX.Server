namespace GameFrameX.Apps.Player.Reward.Entity;

/// <summary>
/// 玩家统一奖励发放幂等账本状态。每玩家一份，挂在玩家 Actor 的 <see cref="Component.RewardGrantComponent"/> 上。
/// </summary>
/// <remarks>
/// 账本键为发放幂等键 <c>RoleId:SourceType:SourceId:TraceId</c>（B6）。
/// 一期无 TTL / 容量回收（邮件附件每玩家有限，实际增长受玩法约束；retention 留 follow-up）。
/// </remarks>
public sealed class RewardGrantRecordState : BaseCacheState
{
    /// <summary>
    /// 幂等键到发放记录的映射。键格式 <c>RoleId:SourceType:SourceId:TraceId</c>。
    /// </summary>
    public Dictionary<string, RewardGrantRecord> Records { get; set; } = new Dictionary<string, RewardGrantRecord>();
}
