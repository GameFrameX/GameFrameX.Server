using GameFrameX.SuperSocket.WebSocket.Server;

namespace GameFrameX.NetWork;

/// <summary>
/// 基于 WebSocket 会话的网络发送器。
/// </summary>
internal sealed class WebSocketNetWorkSender : INetWorkSender
{
    private readonly WebSocketSession _webSocketSession;

    public WebSocketNetWorkSender(WebSocketSession webSocketSession)
    {
        _webSocketSession = webSocketSession ?? throw new ArgumentNullException(nameof(webSocketSession));
    }

    public ValueTask SendAsync(byte[] messageData, CancellationToken cancellationToken)
    {
        return _webSocketSession.SendAsync(messageData, cancellationToken);
    }

    public ValueTask SendAsync(ReadOnlyMemory<byte> messageData, CancellationToken cancellationToken)
    {
        return _webSocketSession.SendAsync(messageData, cancellationToken);
    }
}
