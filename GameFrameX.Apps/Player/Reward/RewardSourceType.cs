namespace GameFrameX.Apps.Player.Reward;

/// <summary>
/// 统一奖励发放的来源类型，仅用于拼接幂等键与审计，不影响发放路由。
/// </summary>
public enum RewardSourceType
{
    /// <summary>
    /// 未指定：占位值，禁止用于幂等键（会导致不同发放被合并为同一条记录）。
    /// </summary>
    None = 0,

    /// <summary>
    /// 邮件附件领取。建议 <c>SourceId</c> 形如 <c>mail:&lt;MailId&gt;:attachment:&lt;AttachmentId&gt;</c>。
    /// </summary>
    Mail = 1,

    /// <summary>
    /// 运营补偿。
    /// </summary>
    Compensation = 2,

    /// <summary>
    /// 兑换码兑换。
    /// </summary>
    ExchangeCode = 3,

    /// <summary>
    /// 活动奖励。
    /// </summary>
    Activity = 4,

    /// <summary>
    /// 后台手动发放。
    /// </summary>
    Manual = 5,
}
