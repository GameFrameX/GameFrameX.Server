namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 响应消息
/// </summary>
public interface IResponseMessage : IMessage
{
    /// <summary>
    /// 错误码，非 0 表示错误
    /// </summary>
    int ErrorCode { get; set; }
}