using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp.Gateway;

class MessageActorGatewayDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    public IMessage Handler(byte[] data)
    {
        int readOffset = 0;
        var length = data.ReadInt(ref readOffset);
        var uniqueId = data.ReadLong(ref readOffset);
        var messageId = data.ReadInt(ref readOffset);
        // var messageUniqueData = data.ReadBytes(ref readOffset);
        var messageData = data.ReadBytes(ref readOffset);
        var messageType = ProtoMessageIdHandler.GetResponseMessageTypeById(messageId);
        if (messageType != null)
        {
            var messageObject = SerializerHelper.Deserialize(messageData, messageType);
            if (messageObject is MessageObject messageObject1)
            {
                messageObject1.MessageId = messageId;
                messageObject1.UniqueId = uniqueId;
                return messageObject1;
            }

            if (messageObject is MessageActorObject messageActorObject)
            {
                messageActorObject.MessageId = messageId;
                messageActorObject.UniqueId = uniqueId;
                return messageActorObject;
            }
        }

        LogHelper.Fatal("未知消息类型");
        return null;
    }

    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        var data = buffer.ToArray();
        var messageObject = Handler(data);
        return messageObject;
    }
}