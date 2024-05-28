namespace GameFrameX.Launcher.StartUp.Discovery;

class MessageActorDiscoveryEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetResponseMessageIdByType(messageType);
    }
}