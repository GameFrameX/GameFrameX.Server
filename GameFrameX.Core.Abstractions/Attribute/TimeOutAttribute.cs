namespace GameFrameX.Core.Abstractions.Attribute;

/// <summary>
/// 超时时间(毫秒)
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class TimeOutAttribute : System.Attribute
{
    /// <summary>
    /// 超时时间
    /// </summary>
    public int TimeOut { get; }

    /// <summary>
    /// 超时时间
    /// </summary>
    /// <param name="timeOut">时间.单位毫秒</param>
    public TimeOutAttribute(int timeOut)
    {
        TimeOut = timeOut;
    }

    /// <summary>
    /// 超时时间
    /// </summary>
    /// <param name="timeout"></param>
    public TimeOutAttribute(TimeSpan timeout) : this(timeout.Milliseconds)
    {
    }
}