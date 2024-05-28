namespace GameFrameX.Launcher.StartUp.Gateway;

class MessageActorGatewayEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestMessageIdByType(messageType);
    }
}