using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

/// <summary>
/// 普通会话发送器提供者。
/// </summary>
internal sealed class SessionNetWorkSenderProvider : INetWorkSenderProvider
{
    public Type SessionType => typeof(IGameAppSession);

    public INetWorkSender Create(IGameAppSession session)
    {
        ArgumentNullException.ThrowIfNull(session, nameof(session));
        return new SessionNetWorkSender(session);
    }
}
