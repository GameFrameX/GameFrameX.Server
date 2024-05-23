using SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

public class DefaultNetWorkChannel : BaseNetWorkChannel
{
    public DefaultNetWorkChannel(IAppSession session, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession, bool isWebSocket = false) : base(session, messageEncoder, rpcSession, isWebSocket)
    {
    }
}