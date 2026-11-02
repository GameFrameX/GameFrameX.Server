using System.Collections.Generic;
using System.Text;
using GameFrameX.Apps.Player.Mail;
using GameFrameX.Apps.Player.Mail.Entity;

namespace GameFrameX.Hotfix.Logic.Player.Mail;

/// <summary>
/// 邮件附件领取的纯函数集合（U1 §4.4 / B6）。集中承载领取前的查找、领取状态写入、附件整体维度派生与幂等 trace 构造，
/// 作为 <see cref="MailComponentAgent"/> 与单元测试的共同事实来源。
/// </summary>
/// <remarks>
/// 单向流转：<see cref="ClaimStatus.Claimable"/> → <see cref="ClaimStatus.Claimed"/>（终态，撤回不回滚，B3）；
/// 或 → <see cref="ClaimStatus.Discarded"/>（撤回 / 过期作废，B3 / B4）。重复领取视为幂等成功，不重复发奖（B6）。
/// </remarks>
public static class MailAttachmentClaim
{
    /// <summary>
    /// 查找并准备一次单附件领取。不写入任何状态，仅返回命中的邮件 / 附件引用（未命中为 null）。
    /// 不在此处判定错误码：调用方根据 <see cref="ClaimPrepareResult.Mail"/> / <see cref="ClaimPrepareResult.Attachment"/>
    /// 是否命中与 <see cref="MailAttachmentInstance.ClaimStatus"/> 直接给响应的 ErrorCode 赋值（成功不赋，框架默认 0）。
    /// </summary>
    /// <param name="state">玩家邮件箱状态。</param>
    /// <param name="mailId">邮件实例 ID。</param>
    /// <param name="slotId">附件槽位 ID。</param>
    public static ClaimPrepareResult Prepare(MailBoxState state, long mailId, int slotId)
    {
        var mail = TryFindMail(state, mailId);
        if (mail == null || mail.MailStatus == MailStatus.Deleted)
        {
            return ClaimPrepareResult.Empty;
        }

        // 终态邮件（撤回 / 过期）整体不可领：未领附件已被作废流程置 Discarded，统一由附件状态判定。
        var attachment = TryFindAttachment(mail, slotId);
        return new ClaimPrepareResult { Mail = mail, Attachment = attachment };
    }

    /// <summary>
    /// 将附件置为已领（终态）。一旦写入不可回退（B3 撤回不回滚已领附件）。
    /// </summary>
    public static void MarkClaimed(MailAttachmentInstance attachment, long now)
    {
        attachment.ClaimStatus = ClaimStatus.Claimed;
        attachment.ClaimTime = now;
    }

    /// <summary>
    /// 将附件置为作废（撤回 / 过期，B3 / B4）。已领附件不应调用本方法（由调用方保证）。
    /// </summary>
    public static void MarkDiscarded(MailAttachmentInstance attachment)
    {
        if (attachment.ClaimStatus == ClaimStatus.Claimed)
        {
            return; // B3：已领不动。
        }

        attachment.ClaimStatus = ClaimStatus.Discarded;
    }

    /// <summary>
    /// 重算邮件附件整体维度（U1 §4.3）。派生字段，单一事实来源为 <see cref="MailState.MailStatus"/> + 各附件 <see cref="MailAttachmentInstance.ClaimStatus"/>。
    /// </summary>
    public static AttachmentStatus ComputeAttachmentStatus(MailState mail)
    {
        if (mail.Attachments == null || mail.Attachments.Count == 0)
        {
            return AttachmentStatus.NoAttachment;
        }

        if (mail.MailStatus == MailStatus.Revoked || mail.MailStatus == MailStatus.Expired)
        {
            return AttachmentStatus.Unclaimable;
        }

        var claimed = 0;
        var claimable = 0;
        foreach (var att in mail.Attachments)
        {
            if (att.ClaimStatus == ClaimStatus.Claimed)
            {
                claimed++;
            }
            else if (att.ClaimStatus == ClaimStatus.Claimable)
            {
                claimable++;
            }
        }

        if (claimable == 0 && claimed == mail.Attachments.Count)
        {
            return AttachmentStatus.AllClaimed;
        }

        // 仍存在可领（未领）附件 → PartialClaimed（含 0 已领情况，用于 B2 删除门禁）。
        return AttachmentStatus.PartialClaimed;
    }

    /// <summary>
    /// 构造领取幂等追踪键（U1 §4.4 / B6）：以 <c>CampaignId:CampaignVersion:MailId:SlotId</c> 四元组唯一标识一次附件发放。
    /// 与 <c>RewardGrantComponentAgent</c> 的 <c>RoleId:SourceType:SourceId:TraceId</c> 共同组成全局幂等键。
    /// </summary>
    public static string BuildTrace(long campaignId, int campaignVersion, long mailId, int slotId)
    {
        var sb = new StringBuilder();
        sb.Append(campaignId).Append(':').Append(campaignVersion).Append(':').Append(mailId).Append(':').Append(slotId);
        return sb.ToString();
    }

    private static MailState TryFindMail(MailBoxState state, long mailId)
    {
        foreach (var mail in state.List)
        {
            if (mail.MailId == mailId)
            {
                return mail;
            }
        }

        return null;
    }

    private static MailAttachmentInstance TryFindAttachment(MailState mail, int slotId)
    {
        if (mail.Attachments == null)
        {
            return null;
        }

        foreach (var att in mail.Attachments)
        {
            if (att.SlotId == slotId)
            {
                return att;
            }
        }

        return null;
    }
}
