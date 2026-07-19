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

namespace GameFrameX.Apps.Player.Mail.Entity;

/// <summary>
/// 运营邮件发布记录的多语言文案快照条目。按语言代码（如 <c>zh-CN</c> / <c>en-US</c>）索引标题与正文。
/// </summary>
/// <remarks>
/// 发布即冻结：Admin 后续修改模板不回灌已发布邮件（U1 §3.2 / B1）。
/// </remarks>
public sealed class MailLocalizedText
{
    /// <summary>渲染后标题。</summary>
    public string Title { get; set; }

    /// <summary>渲染后正文。</summary>
    public string Content { get; set; }
}

/// <summary>
/// 运营邮件发布记录（Campaign 不可变快照）。Server 端 <see cref="Status"/> 仅 <see cref="MailCampaignStatus.Published"/> / <see cref="MailCampaignStatus.Revoked"/>。
/// </summary>
/// <remarks>
/// 不可变边界（B1）：一旦发布，全字段冻结，不存在修改入口；改文案必须发新 <see cref="CampaignVersion"/>。
/// 命中判断：<see cref="ServerInstanceIds"/> 空 = 不限；非空 = 含当前服 <c>Setting.ServerId</c>（业务 <c>ServerInstanceId</c> 即 <c>Setting.ServerId</c>，与协议层 <c>ServerId</c> 区分，U1 §3.2 / §6）。
/// 撤回不可逆（B3）：仅置 <see cref="RevokedAt"/> 与 <see cref="Status"/>，不回滚已领取资产。
/// </remarks>
public sealed class MailCampaignState
{
    /// <summary>运营邮件活动 ID，全局唯一。</summary>
    public string CampaignId { get; set; }

    /// <summary>发布版本号。同一 <see cref="CampaignId"/> 允许多版本，单调递增，发布即冻结（B1）。</summary>
    public long CampaignVersion { get; set; }

    /// <summary>命中服务器实例集合（业务 <c>ServerInstanceId</c>，即 <c>Setting.ServerId</c>，long）。空集合 = 不限；非空 = 含当前服 <c>Setting.ServerId</c> 才命中。</summary>
    public HashSet<long> ServerInstanceIds { get; set; } = new HashSet<long>();

    /// <summary>渠道集合。空集合 = 不限。游戏服不持有渠道运行参数，带渠道条件的 Campaign 一期默认不实例化（U1 §4.5）。</summary>
    public HashSet<string> ChannelIds { get; set; } = new HashSet<string>();

    /// <summary>等级区间下限（含）。空 = 不限。读取自 <c>PlayerState.Level</c>。</summary>
    public int? MinLevel { get; set; }

    /// <summary>等级区间上限（含）。空 = 不限。读取自 <c>PlayerState.Level</c>。</summary>
    public int? MaxLevel { get; set; }

    /// <summary>玩家创建时间下限（unix 秒）。空 = 不限。读取自 <c>CacheState.CreatedTime</c>。</summary>
    public long? PlayerCreatedAfter { get; set; }

    /// <summary>玩家创建时间上限（unix 秒）。空 = 不限。读取自 <c>CacheState.CreatedTime</c>。</summary>
    public long? PlayerCreatedBefore { get; set; }

    /// <summary>邮件业务类型。仅分类展示，不影响状态机。</summary>
    public MailType MailType { get; set; }

    /// <summary>模板 ID。发布即冻结。</summary>
    public int TemplateId { get; set; }

    /// <summary>模板版本。发布即冻结，模板后续编辑不影响已发布邮件。</summary>
    public long TemplateVersion { get; set; }

    /// <summary>模板参数。与模板版本一同冻结。</summary>
    public Dictionary<string, string> TemplateArgs { get; set; } = new Dictionary<string, string>();

    /// <summary>多语言文案快照。key = 语言代码。发布即冻结（B1）。</summary>
    public Dictionary<string, MailLocalizedText> LocalizedContentSnapshot { get; set; } = new Dictionary<string, MailLocalizedText>();

    /// <summary>附件定义列表（Campaign 侧元信息，不含领取状态）。</summary>
    public List<MailAttachmentState> Attachments { get; set; } = new List<MailAttachmentState>();

    /// <summary>绝对过期时间（unix 秒）。空表示用 <see cref="ExpireDays"/> 相对计算。</summary>
    public long? ExpireAt { get; set; }

    /// <summary>自玩家实例化起算的相对过期天数。与 <see cref="ExpireAt"/> 二选一，<see cref="ExpireAt"/> 优先。</summary>
    public int? ExpireDays { get; set; }

    /// <summary>过期附件策略。默认 <see cref="ExpireAttachmentPolicy.DiscardUnclaimed"/>（B4）。</summary>
    public ExpireAttachmentPolicy ExpireAttachmentPolicy { get; set; } = ExpireAttachmentPolicy.DiscardUnclaimed;

    /// <summary>Campaign 状态。<see cref="MailCampaignStatus.Published"/> → <see cref="MailCampaignStatus.Revoked"/> 单向不可逆（B1）。</summary>
    public MailCampaignStatus Status { get; set; } = MailCampaignStatus.Published;

    /// <summary>发布时间（unix 秒）。不可变。</summary>
    public long PublishedAt { get; set; }

    /// <summary>撤回时间（unix 秒）。空表示未撤回；非空表示已撤回（终态，B3）。</summary>
    public long? RevokedAt { get; set; }
}
