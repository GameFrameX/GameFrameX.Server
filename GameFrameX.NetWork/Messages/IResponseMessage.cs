namespace GameFrameX.NetWork.Messages;

public interface IResponseMessage : IMessage
{
    /// <summary>
    /// 错误码，非 0 表示错误
    /// </summary>
    int ErrorCode { get; }
}