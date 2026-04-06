using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

/// <summary>
/// 基于普通会话的网络发送器。
/// </summary>
internal sealed class SessionNetWorkSender : INetWorkSender
{
    private readonly IGameAppSession _session;

    public SessionNetWorkSender(IGameAppSession session)
    {
        _session = session;
    }

    public ValueTask SendAsync(byte[] messageData, CancellationToken cancellationToken)
    {
        return _session.SendAsync(messageData, cancellationToken);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> messageData, CancellationToken cancellationToken)
    {
        return _session.SendAsync(messageData, cancellationToken);
    }
}
