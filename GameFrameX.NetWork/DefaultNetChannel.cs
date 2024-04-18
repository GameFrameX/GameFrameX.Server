using SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

public class DefaultNetChannel : BaseNetChannel
{
    public DefaultNetChannel(IAppSession session, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession) : base(session, messageEncoder, rpcSession)
    {
    }
}