namespace GameFrameX.Hotfix.Logic.Player.Mail
{
    /// <summary>
    /// 运营邮件 Campaign HTTP 接口错误码。
    /// 仅用于 Admin HTTP API（发布 / 预览 / 撤回 / 查询），不进入客户端协议，故不走 protobuf，直接定义为 C# 枚举。
    /// 由 <see cref="MailCampaignRegistry"/> 产生，HTTP handler 透传到响应。
    /// </summary>
    public enum MailCampaignErrorCode
    {
        /// <summary>成功</summary>
        Ok = 0,

        /// <summary>Campaign 已发布或已撤回，主体字段不可修改（B1）。</summary>
        HasExist = 1,

        /// <summary>Campaign 不存在（查询 / 撤回时未找到对应 CampaignId）。</summary>
        CampaignNotFound = 2,

        /// <summary>Campaign 已撤回，不可重复撤回或修改。</summary>
        CampaignAlreadyRevoked = 3,

        /// <summary>Campaign 参数非法（必填缺失、过滤条件矛盾、附件数量非正等）。</summary>
        InvalidCampaignParameter = 4,
    }
}
