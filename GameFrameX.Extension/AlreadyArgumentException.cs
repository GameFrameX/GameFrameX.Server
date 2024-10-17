namespace GameFrameX.Extension;

/// <summary>
/// 已经存在参数重复异常
/// </summary>
public sealed class AlreadyArgumentException : Exception
{
    /// <summary>
    /// 已经存在参数重复
    /// </summary>
    /// <param name="message">异常消息</param>
    public AlreadyArgumentException(string message) : base(message)
    {
    }
}