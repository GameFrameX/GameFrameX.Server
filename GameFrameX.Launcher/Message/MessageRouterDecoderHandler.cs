using GameFrameX.Extension;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;
using GameFrameX.Serialize.Serialize;

namespace GameFrameX.Launcher.Message;

public class MessageRouterDecoderHandler : IMessageDecoderHandler
{
    public IMessage Handler(byte[] data)
    {
        try
        {
            int readOffset = 0;
            var length = data.ReadInt(ref readOffset);
            var timestamp = data.ReadLong(ref readOffset);
            var messageId = data.ReadInt(ref readOffset);
            // var messageUniqueData = data.ReadBytes(ref readOffset);
            var messageData = data.ReadBytes(ref readOffset);
            var messageType = ProtoMessageIdHandler.GetReqTypeById(messageId);
            if (messageType != null)
            {
                var messageObject = (MessageObject)SerializerHelper.Deserialize(messageData, messageType);
                messageObject.MessageId = messageId;
                return messageObject;
            }

            LogHelper.Fatal("未知消息类型");
            return null;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }
}