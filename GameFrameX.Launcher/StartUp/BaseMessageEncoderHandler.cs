using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp;

public abstract class BaseMessageEncoderHandler : IMessageEncoderHandler, IPackageEncoder<IMessage>
{
    /// <summary>
    /// 获取消息ID
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    protected abstract int GetMessageId(Type messageType);

    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] Handler(IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            var messageType = message.GetType();

            var msgId = GetMessageId(messageType);
            messageObject.MessageId = msgId;
            var uniqueId = messageObject.UniqueId;
            var bytes = SerializerHelper.Serialize(messageObject);
            // len +uniqueId + msgId + bytes.length
            ushort len = (ushort)(2 + 4 + 4 + bytes.Length);
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
    /// 和服务器之间的消息 数据长度(4)+消息唯一ID(8)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="messageUniqueId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /*public byte[] RpcHandler(int messageUniqueId, IMessage message)
    {
        var bytes = SerializerHelper.Serialize(message);
        // len + UniqueId + msgId + bytes.length
        ushort len = (ushort)(2 + 4 + 4 + bytes.Length);
        var span = new byte[len];
        int offset = 0;
        span.WriteUShort(len, ref offset);
        span.WriteInt(messageUniqueId, ref offset);
        var messageType = message.GetType();
        var msgId = GetRpcMessageId(messageType);
        message.MessageId = msgId;
        span.WriteInt(msgId, ref offset);
        span.WriteBytesWithoutLength(bytes, ref offset);
        return span;
    }*/
    public int Encode(IBufferWriter<byte> writer, IMessage pack)
    {
        var bytes = Handler(pack);
        writer.Write(bytes);
        return bytes.Length;
    }
}