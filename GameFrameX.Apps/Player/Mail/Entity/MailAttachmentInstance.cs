namespace GameFrameX.Apps.Player.Mail.Entity;

/// <summary>
/// 玩家邮件内的单个附件实例。实例化时从 <see cref="MailAttachmentState"/> 复制元信息，并承载领取状态（U1 §3.5）。
/// </summary>
/// <remarks>
/// 领取状态 <see cref="ClaimStatus"/> 单向流转：<see cref="ClaimStatus.Claimable"/> → <see cref="ClaimStatus.Claimed"/>（已领，终态，撤回不回滚，B3）；
/// 或 → <see cref="ClaimStatus.Discarded"/>（撤回 / 过期作废，B3 / B4）。领取幂等（B6）由统一发放接口保证。
/// </remarks>
public sealed class MailAttachmentInstance
{
    /// <summary>附件槽位 ID。邮件内唯一，实例化时从 <see cref="MailAttachmentState.SlotId"/> 复制。</summary>
    public int SlotId { get; set; }

    /// <summary>奖励类型。实例化时复制。</summary>
    public int RewardType { get; set; }

    /// <summary>物品 ID。实例化时复制。</summary>
    public int ItemId { get; set; }

    /// <summary>数量。实例化时复制。</summary>
    public long Amount { get; set; }

    /// <summary>展示用图标 ID。实例化时复制。</summary>
    public int IconId { get; set; }

    /// <summary>领取状态。单向流转，终态不可回退（B3 / B6）。</summary>
    public ClaimStatus ClaimStatus { get; set; } = ClaimStatus.Claimable;

    /// <summary>首次领取时间（unix 秒）。空表示未领取。一旦写入不可清空（B3 不回滚）。</summary>
    public long? ClaimTime { get; set; }
}
