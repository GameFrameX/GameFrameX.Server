using SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

public class DefaultNetWorkChannel : BaseNetWorkChannel
{
    public DefaultNetWorkChannel(IGameAppSession session, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession = null, bool isWebSocket = false) : base(session, messageEncoder, rpcSession, isWebSocket)
    {
    }
}