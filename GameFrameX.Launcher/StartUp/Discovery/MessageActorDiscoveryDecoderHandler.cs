using GameFrameX.NetWork.Message;

namespace GameFrameX.Launcher.StartUp.Discovery;

class MessageActorDiscoveryDecoderHandler : BaseMessageDecoderHandler
{
    public override IMessage Handler(byte[] data)
    {
        var innerMessage = new InnerMessage();
        try
        {
            int readOffset = 0;
            var length = data.ReadUShort(ref readOffset);
            var messageOperationType = data.ReadByte(ref readOffset);
            var uniqueId = data.ReadInt(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(readOffset, length - readOffset);
            var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
            innerMessage.SetMessageType(messageType);
            innerMessage.SetOperationType((MessageOperationType)messageOperationType);
            innerMessage.SetUniqueId(uniqueId);
            innerMessage.SetMessageId(messageId);
            innerMessage.SetMessageData(messageData);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }

        return innerMessage;
    }
}