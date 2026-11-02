// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System;
using System.Collections.Generic;
using GameFrameX.Apps.Player.Mail;
using GameFrameX.Apps.Player.Mail.Entity;
using GameFrameX.Hotfix.Logic.Player.Mail;

namespace GameFrameX.Hotfix.Logic.Http.Mail
{
    /// <summary>
    /// 预览 / 校验 Campaign 请求体。字段与发布请求一致，但不分配 CampaignId。
    /// </summary>
    public sealed class PreviewMailCampaignRequest : HttpMessageRequestBase
    {
        /// <summary>邮件类型。</summary>
        [Required]
        [Description("邮件类型")]
        public MailType MailType { get; set; }

        /// <summary>多语言标题，至少 1 项。</summary>
        [Required]
        [Description("多语言标题")]
        public List<MailLocalizedContent> Titles { get; set; } = new List<MailLocalizedContent>();

        /// <summary>多语言正文。</summary>
        [Description("多语言正文")]
        public List<MailLocalizedContent> Contents { get; set; } = new List<MailLocalizedContent>();

        /// <summary>附件列表。</summary>
        [Description("附件列表")]
        public List<MailAttachmentState> Attachments { get; set; } = new List<MailAttachmentState>();

        /// <summary>服务器 ID 白名单。</summary>
        [Description("服务器 ID 白名单")]
        public List<int> ServerIds { get; set; } = new List<int>();

        /// <summary>渠道 ID 白名单。</summary>
        [Description("渠道 ID 白名单")]
        public List<int> ChannelIds { get; set; } = new List<int>();

        /// <summary>最低等级。</summary>
        [Description("最低等级")]
        public int MinLevel { get; set; }

        /// <summary>最高等级。</summary>
        [Description("最高等级")]
        public int MaxLevel { get; set; }

        /// <summary>附件过期处置策略。</summary>
        [Description("附件过期处置策略")]
        public ExpireAttachmentPolicy ExpireAttachmentPolicy { get; set; }

        /// <summary>过期时间（Unix 秒，UTC）。</summary>
        [Description("过期时间")]
        public long ExpireAt { get; set; }
    }
}
