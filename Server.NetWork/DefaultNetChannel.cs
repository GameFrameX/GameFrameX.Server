using SuperSocket;

namespace Server.NetWork;

public class DefaultNetChannel : BaseNetChannel
{
    public DefaultNetChannel(IAppSession session, IMessageEncoderHandler messageEncoder) : base(session, messageEncoder)
    {
    }
}