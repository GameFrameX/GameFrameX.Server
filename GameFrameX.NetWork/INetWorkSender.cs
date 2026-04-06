namespace GameFrameX.NetWork;

/// <summary>
/// 网络发送器接口。
/// </summary>
/// <remarks>
/// Encapsulates transport-specific send behavior for network channels.
/// </remarks>
public interface INetWorkSender
{
    /// <summary>
    /// 异步发送消息数据。
    /// </summary>
    /// <param name="messageData">消息二进制数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    ValueTask SendAsync(byte[] messageData, CancellationToken cancellationToken);

    /// <summary>
    /// 异步发送消息数据。
    /// </summary>
    /// <param name="messageData">消息二进制数据</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>异步任务</returns>
    ValueTask SendAsync(ReadOnlyMemory<byte> messageData, CancellationToken cancellationToken);
}