using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp.Discovery;

class MessageActorDiscoveryEncoderHandler : IPackageEncoder<IMessage>
{
    public byte[] Handler(IMessage message)
    {
        var bytes = SerializerHelper.Serialize(message);
        var messageType = message.GetType();
        long uniqueId = 0;
        int msgId = 0;
        if (message is MessageObject messageObject)
        {
            msgId = ProtoMessageIdHandler.GetRespMessageIdByType(messageType);
            messageObject.MessageId = msgId;
            uniqueId = messageObject.UniqueId;
            bytes = SerializerHelper.Serialize(messageObject);
        }
        else if (message is MessageActorObject messageActorObject)
        {
            msgId = ProtoMessageIdHandler.GetResponseActorMessageIdByType(messageType);
            messageActorObject.MessageId = msgId;
            uniqueId = messageActorObject.UniqueId;
            bytes = SerializerHelper.Serialize(messageActorObject);
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

    public int Encode(IBufferWriter<byte> writer, IMessage messageObject)
    {
        var bytes = Handler(messageObject);
        LogHelper.Debug($"---发送  {messageObject.ToMessageString()}");
        writer.Write(bytes);
        return bytes.Length;
    }
}