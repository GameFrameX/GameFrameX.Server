namespace GameFrameX.Apps.Player.Mail.Entity
{
    /// <summary>
    /// 邮件多语言文本项。Key 为语言代码（如 <c>zh-CN</c> / <c>en-US</c>），Value 为对应语言的展示文案。
    /// 发布接口要求至少提供默认语言文案；缺失语言由客户端回退到默认语言。
    /// </summary>
    public sealed class MailLocalizedContent
    {
        /// <summary>
        /// 语言代码（小写 BCP 47 tag，如 <c>zh-cn</c>、<c>en-us</c>）。
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// 该语言下的展示文案。允许内嵌参数占位符（如 <c>{0}</c>），由客户端按上下文替换。
        /// </summary>
        public string Text { get; set; }
    }
}
