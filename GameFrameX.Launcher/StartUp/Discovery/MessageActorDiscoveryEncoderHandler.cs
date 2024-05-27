namespace GameFrameX.Launcher.StartUp.Discovery;

class MessageActorDiscoveryEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetActorMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetResponseActorMessageIdByType(messageType);
    }

    protected override int GetMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetResponseMessageIdByType(messageType);
    }

    protected override int GetRpcMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRespMessageIdByType(messageType);
    }
}