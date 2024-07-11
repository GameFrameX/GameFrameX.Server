using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;

namespace GameFrameX.Launcher.StartUp.Router;

/// <summary>
/// 消息结构 4 + 4 + 4 + bytes.length
/// </summary>
public class MessageRouterDecoderHandler : BaseMessageDecoderHandler
{
    public override IMessage Handler(byte[] data)
    {
        OuterMessage outerMessage = new OuterMessage();
        try
        {
            int readOffset = 0;
            var length = data.ReadUShort(ref readOffset);
            var messageOperationType = data.ReadByte(ref readOffset);
            var uniqueId = data.ReadInt(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            var messageData = data.ReadBytes(readOffset, length - readOffset);
            var messageType = MessageProtoHelper.GetMessageTypeById(messageId);
            outerMessage.SetMessageType(messageType);
            outerMessage.SetOperationType((MessageOperationType)messageOperationType);
            outerMessage.SetUniqueId(uniqueId);
            outerMessage.SetMessageId(messageId);
            outerMessage.SetMessageData(messageData);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }

        return outerMessage;
    }
}