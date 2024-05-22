using GameFrameX.Serialize.Serialize;

namespace GameFrameX.Launcher.Message;

class MessageActorGatewayEncoderHandler : IMessageEncoderHandler
{
    public byte[] Handler(IMessage message)
    {
        byte[] bytes = null;
        var messageType = message.GetType();
        long uniqueId = 0;
        int msgId = 0;
        if (message is MessageObject messageObject)
        {
            msgId = ProtoMessageIdHandler.GetReqMessageIdByType(messageType);
            messageObject.MessageId = msgId;
            uniqueId = messageObject.UniqueId;
            bytes = SerializerHelper.Serialize(messageObject);
        }
        else if (message is MessageActorObject messageActorObject)
        {
            msgId = ProtoMessageIdHandler.GetRequestActorMessageIdByType(messageType);
            messageActorObject.MessageId = msgId;
            uniqueId = messageActorObject.UniqueId;
            bytes = SerializerHelper.Serialize(messageActorObject);
        }

        if (bytes == null)
        {
            LogHelper.Error("消息对象为空，编码异常");
            return null;
        }

        // len +timestamp + msgId + bytes.length
        int len = 4 + 8 + 4 + 4 + bytes.Length;
        var span = new byte[len];
        int offset = 0;
        span.WriteInt(len, ref offset);
        span.WriteLong(uniqueId, ref offset);

        span.WriteInt(msgId, ref offset);
        span.WriteBytes(bytes, ref offset);
        return span;
    }

    public byte[] RpcReplyHandler(long msgUniqueId, IMessage message)
    {
        var bytes = SerializerHelper.Serialize(message);

        // len +UniqueId + msgId + bytes.length
        int len = 4 + 8 + 4 + bytes.Length;
        var span = ArrayPool<byte>.Shared.Rent(len);
        int offset = 0;
        span.WriteInt(len, ref offset);
        span.WriteLong(msgUniqueId, ref offset);
        var messageType = message.GetType();
        var msgId = ProtoMessageIdHandler.GetRespMessageIdByType(messageType);
        span.WriteInt(msgId, ref offset);
        span.WriteBytes(bytes, ref offset);
        return span;
    }

    public int Encode(IBufferWriter<byte> writer, IMessage messageObject)
    {
        var bytes = Handler(messageObject);
        LogHelper.Debug($"---发送消息 ==>消息类型:{messageObject.GetType()} 消息内容:{messageObject}");
        writer.Write(bytes);
        ArrayPool<byte>.Shared.Return(bytes);
        return bytes.Length;
    }
}