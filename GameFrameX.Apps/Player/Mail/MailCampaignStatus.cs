namespace GameFrameX.Apps.Player.Mail
{
    /// <summary>
    /// 运营邮件发布记录（Campaign）的生命周期状态。
    /// 发布成功后 Campaign 处于 <see cref="Published"/>；调用 Admin 撤回接口后转为 <see cref="Revoked"/>。
    /// 一旦发布，Campaign 主体字段不可修改（B1：发布后不可修改），仅状态字段可流转。
    /// </summary>
    public enum MailCampaignStatus
    {
        /// <summary>
        /// 已发布：Campaign 已落库，玩家可在过滤命中后懒创建邮件实例。
        /// </summary>
        Published = 0,

        /// <summary>
        /// 已撤回：运营通过 Admin 接口撤回；新玩家不再命中，已发放但未领取的附件按 <see cref="ExpireAttachmentPolicy"/> 处理，已领取资产不回滚（B3）。
        /// </summary>
        Revoked = 1,
    }
}
