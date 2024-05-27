namespace GameFrameX.Launcher.StartUp.Gateway;

class MessageActorGatewayEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetActorMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetResponseActorMessageIdByType(messageType);
    }

    protected override int GetMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetReqMessageIdByType(messageType);
    }

    protected override int GetRpcMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestMessageIdByType(messageType);
    }
}