using GameFrameX.Proto.BuiltIn;
using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp;

public abstract class BaseMessageEncoderHandler : IMessageEncoderHandler, IPackageEncoder<IMessage>
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageId"></param>
    /// <returns></returns>
    public byte[] HeartBeatHandler(ReqHeartBeat message, out int messageId)
    {
        var messageType = message.GetType();
        messageId = MessageProtoHelper.GetMessageIdByType(messageType);
        var buffer = MessageSerializerHelper.Serialize(message);
        return buffer;
    }

    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public virtual byte[] Handler(IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            var messageType = message.GetType();

            var msgId = MessageProtoHelper.GetMessageIdByType(messageType);
            messageObject.MessageId = msgId;
            var uniqueId = messageObject.UniqueId;
            var bytes = MessageSerializerHelper.Serialize(messageObject);
            // len +uniqueId + msgId + bytes.length
            ushort len = (ushort)(PackageLength + bytes.Length);
            var span = new byte[len];
            int offset = 0;
            span.WriteUShort(len, ref offset);
            span.WriteInt(uniqueId, ref offset);
            span.WriteInt(msgId, ref offset);
            span.WriteBytesWithoutLength(bytes, ref offset);
            return span;
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }

    /// <summary>
    /// len +cmdType+uniqueId + msgId + bytes.length
    /// </summary>
    public virtual ushort PackageLength { get; } = 2 + 1 + 4 + 4;

    public int Encode(IBufferWriter<byte> writer, IMessage pack)
    {
        var bytes = Handler(pack);
        writer.Write(bytes);
        return bytes.Length;
    }
}