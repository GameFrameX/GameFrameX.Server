using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

public class DefaultNetChannel : BaseNetChannel
{
    public DefaultNetChannel(IAppSession session, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession, bool isWebSocket = false) : base(session, messageEncoder, rpcSession, isWebSocket)
    {
    }
}