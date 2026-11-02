namespace GameFrameX.Apps.Player.Mail
{
    /// <summary>
    /// 邮件类型：决定展示样式与领取限制。编号发布后不可变，新增类型只追加不复用。
    /// </summary>
    public enum MailType
    {
        /// <summary>
        /// 系统邮件：系统自动触发（如维护补偿），无运营 Campaign 概念时也走相同展示通道。
        /// </summary>
        System = 0,

        /// <summary>
        /// 运营邮件：由 Admin 发布接口创建 Campaign，按过滤条件命中懒创建到玩家邮箱。
        /// </summary>
        Operation = 1,

        /// <summary>
        /// 补偿邮件：针对单角色或小范围的定向补偿，复用 Campaign 落库但不走全服过滤命中。
        /// </summary>
        Compensation = 2,
    }
}
