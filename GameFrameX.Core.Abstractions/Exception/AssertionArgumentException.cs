namespace GameFrameX.Core.Abstractions.Exception;

/// <summary>
/// 断言异常
/// </summary>
public abstract class AssertionArgumentException : System.Exception
{
    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="errorCode">错误码</param>
    /// <param name="message">错误信息</param>
    protected AssertionArgumentException(int errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// 错误码
    /// </summary>
    public int ErrorCode { get; private set; }
}