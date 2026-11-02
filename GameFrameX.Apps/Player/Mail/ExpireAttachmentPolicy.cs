namespace GameFrameX.Apps.Player.Mail
{
    /// <summary>
    /// 邮件附件过期处置策略（B4：过期未领取附件默认作废，可配置）。
    /// 决定 Campaign 撤回或自然过期后，玩家尚未领取的附件如何处理。
    /// </summary>
    public enum ExpireAttachmentPolicy
    {
        /// <summary>
        /// 默认作废：未领取附件直接丢弃，不进入玩家任何资产账户，日志留痕用于审计。
        /// </summary>
        DiscardUnclaimed = 0,

        /// <summary>
        /// 保留未领取：附件保留在邮件中，玩家仍可领取，仅停止向新玩家下发（用于「停发但已命中可继续领」场景）。
        /// </summary>
        KeepUnclaimed = 1,

        /// <summary>
        /// 自动入袋：未领取附件通过统一奖励发放接口自动发放到玩家账户（需谨慎，可能突破发放窗口限制）。
        /// </summary>
        AutoClaim = 2,
    }
}
