// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
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
//   Any disputes or liabilities arising from secondary development based on this project
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


using GameFrameX.Apps.Player.Mail;

namespace GameFrameX.Hotfix.Logic.Player.Mail
{
    /// <summary>
    /// 运营邮件 Campaign 列表查询载荷（<see cref="MailCampaignRegistry.QueryAll(MailCampaignQuery)"/> 的唯一查询形态）。
    /// 所有过滤条件为 AND 关系，字段缺省（null / 0）表示不限。
    /// <para>
    /// 维护约束：新增过滤维度只改本载荷与 <see cref="MailCampaignRegistry.QueryAll(MailCampaignQuery)"/> 的过滤管线，
    /// 不回退为可选位置参数。
    /// </para>
    /// </summary>
    /// <remarks>
    /// Query payload for listing operational mail campaigns (the sole query shape of MailCampaignRegistry.QueryAll).
    /// All filters combine with AND; a default field value (null / 0) means unconstrained.
    /// <para>
    /// Maintenance constraint: add new filter dimensions only to this payload and the QueryAll filter pipeline;
    /// do not regress to optional positional parameters.
    /// </para>
    /// </remarks>
    public sealed class MailCampaignQuery
    {
        /// <summary>
        /// 获取或设置状态过滤；null 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the status filter; null means unconstrained.</remarks>
        public MailCampaignStatus? Status { get; init; }

        /// <summary>
        /// 获取或设置邮件类型过滤；null 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the mail type filter; null means unconstrained.</remarks>
        public MailType? MailType { get; init; }

        /// <summary>
        /// 获取或设置服务器 ID 过滤；≤ 0 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the server id filter; &lt;= 0 means unconstrained.</remarks>
        public int ServerId { get; init; }

        /// <summary>
        /// 获取或设置渠道 ID 过滤；≤ 0 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the channel id filter; &lt;= 0 means unconstrained.</remarks>
        public int ChannelId { get; init; }

        /// <summary>
        /// 获取或设置最低等级过滤；≤ 0 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the minimum level filter; &lt;= 0 means unconstrained.</remarks>
        public int MinLevel { get; init; }

        /// <summary>
        /// 获取或设置创建时间下限（含，Unix 秒，UTC）；≤ 0 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the inclusive creation time lower bound (Unix seconds, UTC); &lt;= 0 means unconstrained.</remarks>
        public long CreatedFromUnixSeconds { get; init; }

        /// <summary>
        /// 获取或设置创建时间上限（含，Unix 秒，UTC）；≤ 0 表示不限。
        /// </summary>
        /// <remarks>Gets or sets the inclusive creation time upper bound (Unix seconds, UTC); &lt;= 0 means unconstrained.</remarks>
        public long CreatedToUnixSeconds { get; init; }

        /// <summary>
        /// 获取或设置返回条数上限；≤ 0 表示返回全部。
        /// </summary>
        /// <remarks>Gets or sets the maximum number of results; &lt;= 0 returns all.</remarks>
        public int Limit { get; init; }
    }
}
