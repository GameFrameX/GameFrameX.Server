using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.WebSocket.Server;

namespace GameFrameX.NetWork;

/// <summary>
/// WebSocket 发送器提供者。
/// </summary>
internal sealed class WebSocketNetWorkSenderProvider : INetWorkSenderProvider
{
    public Type SessionType => typeof(WebSocketSession);

    public INetWorkSender Create(IGameAppSession session)
    {
        ArgumentNullException.ThrowIfNull(session, nameof(session));
        var webSocketSession = session as WebSocketSession
                               ?? throw new InvalidCastException($"Session type '{session.GetType().FullName}' can not be cast to '{typeof(WebSocketSession).FullName}'.");
        return new WebSocketNetWorkSender(webSocketSession);
    }
}
