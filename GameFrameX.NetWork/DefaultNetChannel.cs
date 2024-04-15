namespace GameFrameX.NetWork;

public class DefaultNetChannel : BaseNetChannel
{
    public DefaultNetChannel(IGameSession session, IMessageEncoderHandler messageEncoder) : base(session, messageEncoder)
    {
    }
}