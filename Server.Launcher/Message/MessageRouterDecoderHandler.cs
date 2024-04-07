using Server.Extension;
using Server.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace Server.Launcher.Message;

class MessageRouterDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    public IMessage Handler(Span<byte> data)
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
            else
            {
                LogHelper.Fatal("未知消息类型");
                return null;
            }
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }


    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var data = buffer.ToArray();
        LogHelper.Info(data.ToArrayString());
        var messageObject = Handler(data);
        return messageObject;
    }
}