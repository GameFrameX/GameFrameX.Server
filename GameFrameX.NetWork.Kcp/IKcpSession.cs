using System.Net;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP session interface / KCP 会话接口
/// </summary>
public interface IKcpSession
{
    /// <summary>
    /// Conversation ID / 会话 ID
    /// </summary>
    uint ConversationId { get; }

    /// <summary>
    /// Remote endpoint / 远程端点
    /// </summary>
    EndPoint RemoteEndPoint { get; }

    /// <summary>
    /// Is connection active / 连接是否活跃
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Last active time / 最后活跃时间
    /// </summary>
    DateTime LastActiveTime { get; }

    /// <summary>
    /// Input data received from UDP / 输入从 UDP 接收的数据
    /// </summary>
    void Input(ReadOnlySpan<byte> data);

    /// <summary>
    /// Send data through KCP / 通过 KCP 发送数据
    /// </summary>
    int Send(ReadOnlySpan<byte> data);

    /// <summary>
    /// Receive data from KCP / 从 KCP 接收数据
    /// </summary>
    int Recv(Span<byte> buffer);

    /// <summary>
    /// Try peek receive size / 尝试获取接收大小
    /// </summary>
    int PeekSize();

    /// <summary>
    /// Close connection / 关闭连接
    /// </summary>
    void Close();
}