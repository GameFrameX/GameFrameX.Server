using GameFrameX.Launcher.StartUp;

namespace GameFrameX.Hotfix.Launcher;

public class MessageGameEncoderHandler : BaseMessageEncoderHandler
{
    protected override int GetMessageId(Type messageType)
    {
        return ProtoMessageIdHandler.GetRequestMessageIdByType(messageType);
    }
}