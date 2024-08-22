namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 响应消息
/// </summary>
public interface IResponseMessage : INetworkMessage
{
    /// <summary>
    /// 错误码，非 0 表示错误
    /// </summary>
    int ErrorCode { get; set; }
}