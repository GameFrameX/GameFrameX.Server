using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp.Router;

class MessageRouterEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetActorMessageId(Type messageType)
    {
        throw new NotImplementedException();
    }

    protected override int GetMessageId(Type messageType)
    {
        throw new NotImplementedException();
    }

    protected override int GetRpcMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestMessageIdByType(messageType);
    }
}