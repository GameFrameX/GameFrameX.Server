namespace GameFrameX.Apps.Player.Mail.Entity;

/// <summary>
/// 玩家邮件箱状态。每玩家一份，挂在玩家 Actor 的 <see cref="Component.MailComponent"/> 上。
/// </summary>
/// <remarks>
/// 懒创建幂等（B5）以 <see cref="CreatedCampaignVersions"/>（<c>CampaignId@Version</c>）为去重键：同一 Campaign 仅实例化一次；
/// 含已撤回 Campaign，防撤回后重实例化；渠道条件未补齐的 Campaign **不记 key**（条件待定，区别于撤回的永久跳过）。
/// </remarks>
public sealed class MailBoxState : BaseCacheState
{
    /// <summary>玩家个人邮件实例集合。</summary>
    public List<MailState> List { get; set; } = new List<MailState>();

    /// <summary>
    /// 已实例化的 <c>CampaignId@Version</c> 集合。懒创建去重键（B5）。
    /// 含已撤回 Campaign（防撤回后重实例化）；渠道未补齐的 Campaign 不记入此集合（待补齐后再判断）。
    /// </summary>
    public HashSet<string> CreatedCampaignVersions { get; set; } = new HashSet<string>();

    /// <summary>未读计数。冗余字段，由读信流程维护，仅作展示。</summary>
    public int UnreadCount { get; set; }

    /// <summary>上次懒同步游标（<c>PublishedAt</c> 增量扫描）。用于避免每次全量扫描发布记录。</summary>
    public long LastSyncCampaignTime { get; set; }

    /// <summary>上次过期清理时间。用于定时清理调度。</summary>
    public long LastCleanupTime { get; set; }

    /// <summary>
    /// 玩家内邮件实例 ID 分配序列。玩家 Actor 单线程驱动，自增即可保证玩家内唯一。
    /// </summary>
    public long MailIdSeq { get; set; }
}
