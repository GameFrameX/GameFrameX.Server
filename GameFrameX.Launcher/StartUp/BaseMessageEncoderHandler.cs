using GameFrameX.Serialize.Serialize;
using SuperSocket.ProtoBase;

namespace GameFrameX.Launcher.StartUp;

abstract class BaseMessageEncoderHandler : IMessageEncoderHandler, IPackageEncoder<IMessage>
{
    /// <summary>
    /// 获取Actor消息ID
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    protected abstract int GetActorMessageId(Type messageType);

    /// <summary>
    /// 获取消息ID
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    protected abstract int GetMessageId(Type messageType);

    /// <summary>
    /// 获取Rpc消息ID
    /// </summary>
    /// <param name="messageType"></param>
    /// <returns></returns>
    protected abstract int GetRpcMessageId(Type messageType);

    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] Handler(IMessage message)
    {
        byte[] bytes = null;
        var messageType = message.GetType();
        long uniqueId = 0;
        int msgId = 0;
        if (message is MessageObject messageObject)
        {
            msgId = GetMessageId(messageType);
            messageObject.MessageId = msgId;
            uniqueId = messageObject.UniqueId;
            bytes = SerializerHelper.Serialize(messageObject);
        }
        else if (message is MessageActorObject messageActorObject)
        {
            msgId = GetActorMessageId(messageType);
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

    /// <summary>
    /// 和服务器之间的消息 数据长度(4)+消息唯一ID(8)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="messageUniqueId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] RpcHandler(long messageUniqueId, IMessage message)
    {
        var bytes = SerializerHelper.Serialize(message);
        // len + UniqueId + msgId + bytes.length
        int len = 4 + 8 + 4 + 4 + bytes.Length;
        var span = new byte[len];
        int offset = 0;
        span.WriteInt(len, ref offset);
        span.WriteLong(messageUniqueId, ref offset);
        var messageType = message.GetType();
        var msgId = GetRpcMessageId(messageType);
        if (message is BaseMessageObject baseMessageObject)
        {
            baseMessageObject.MessageId = msgId;
        }

        span.WriteInt(msgId, ref offset);
        span.WriteBytes(bytes, ref offset);
        return span;
    }

    public int Encode(IBufferWriter<byte> writer, IMessage pack)
    {
        var bytes = Handler(pack);
        writer.Write(bytes);
        return bytes.Length;
    }
}