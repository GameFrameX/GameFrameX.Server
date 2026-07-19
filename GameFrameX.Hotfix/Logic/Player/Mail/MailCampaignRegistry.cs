// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Apps.Player.Mail;
using GameFrameX.Apps.Player.Mail.Entity;

namespace GameFrameX.Hotfix.Logic.Player.Mail;

/// <summary>
/// 运营邮件发布记录的进程内只读源（Campaign 不可变快照存储）。
/// </summary>
/// <remarks>
/// 一期为进程内单例（懒创建与撤回作废的查询入口）；Admin 发布 / 撤回 HTTP API（后续 derived change）向本注册表写入，
/// 运行时无 Campaign 时懒创建空跑（不实例化任何邮件）。
/// 写入与查询做了 <c>lock</c> 互斥，保证并发安全；Campaign 持久化（落库）不在本 change 范围。
/// </remarks>
public static class MailCampaignRegistry
{
    private static readonly object LockObj = new object();
    private static readonly Dictionary<string, MailCampaignState> Campaigns = new Dictionary<string, MailCampaignState>();

    /// <summary>
    /// 构造 Campaign 去重键 <c>CampaignId@Version</c>（B5 幂等键）。
    /// </summary>
    public static string BuildKey(string campaignId, long campaignVersion)
    {
        return campaignId + "@" + campaignVersion;
    }

    /// <summary>
    /// 写入或替换 Campaign 快照（幂等：同 <c>CampaignId@Version</c> 覆盖）。
    /// 供后续 Admin 发布 HTTP API 与测试调用；本 change 运行时不主动调用。
    /// </summary>
    public static void PublishOrUpdate(MailCampaignState campaign)
    {
        if (campaign == null || string.IsNullOrEmpty(campaign.CampaignId))
        {
            return;
        }

        lock (LockObj)
        {
            Campaigns[BuildKey(campaign.CampaignId, campaign.CampaignVersion)] = campaign;
        }
    }

    /// <summary>
    /// 置 Campaign 撤回（<see cref="MailCampaignStatus.Revoked"/>，终态，B1 / B3）。
    /// 供后续 Admin 撤回 HTTP API调用；本 change 运行时不主动调用。在线玩家的邮件作废在玩家下次懒同步时由 <c>MailComponentAgent</c> 兜底。
    /// </summary>
    /// <returns>命中并撤回返回 true；Campaign 不存在返回 false。</returns>
    public static bool Revoke(string campaignId, long campaignVersion, long revokedAt)
    {
        if (string.IsNullOrEmpty(campaignId))
        {
            return false;
        }

        lock (LockObj)
        {
            if (!Campaigns.TryGetValue(BuildKey(campaignId, campaignVersion), out var campaign))
            {
                return false;
            }

            // Published → Revoked 单向不可逆（B1）：已是 Revoked 则保持原 RevokedAt 不变。
            if (campaign.Status == MailCampaignStatus.Revoked)
            {
                return true;
            }

            campaign.Status = MailCampaignStatus.Revoked;
            campaign.RevokedAt = revokedAt;
            return true;
        }
    }

    /// <summary>
    /// 按 <c>PublishedAt</c> 升序返回 <c>PublishedAt &gt; since</c> 的 Campaign（含已撤回，供懒创建防重实例化分支判断，B5）。
    /// </summary>
    public static List<MailCampaignState> QueryPublishedSince(long since)
    {
        lock (LockObj)
        {
            var result = new List<MailCampaignState>();
            foreach (var campaign in Campaigns.Values)
            {
                if (campaign.PublishedAt > since)
                {
                    result.Add(campaign);
                }
            }

            result.Sort((a, b) => a.PublishedAt.CompareTo(b.PublishedAt));
            return result;
        }
    }

    /// <summary>
    /// 返回当前所有已撤回 Campaign（供玩家懒同步时作废已实例化邮件，B3）。调用方按需拷贝快照后处理。
    /// </summary>
    public static List<MailCampaignState> GetRevokedCampaigns()
    {
        lock (LockObj)
        {
            var result = new List<MailCampaignState>();
            foreach (var campaign in Campaigns.Values)
            {
                if (campaign.Status == MailCampaignStatus.Revoked)
                {
                    result.Add(campaign);
                }
            }

            return result;
        }
    }
}
