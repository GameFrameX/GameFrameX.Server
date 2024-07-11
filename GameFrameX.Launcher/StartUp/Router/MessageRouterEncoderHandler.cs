using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.ProtoBuf.Net;

namespace GameFrameX.Launcher.StartUp.Router;

class MessageRouterEncoderHandler : BaseMessageEncoderHandler
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] InnerHandler(IInnerMessage message)
    {
        var    msgId  = MessageProtoHelper.GetMessageIdByType(message.MessageType);
        ushort len    = (ushort)(PackageLength + message.MessageDataLength);
        var    buffer = new byte[len];
        int    offset = 0;
        buffer.WriteUShort(len, ref offset);
        buffer.WriteByte((byte)message.OperationType, ref offset);
        buffer.WriteInt(message.UniqueId, ref offset);
        buffer.WriteInt(msgId, ref offset);
        buffer.WriteBytesWithoutLength(message.MessageData, ref offset);
        return buffer;
    }

    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override byte[] Handler(INetworkMessage message)
    {
        if (message is MessageObject messageObject)
        {
            var messageType = message.GetType();

            var messageId     = MessageProtoHelper.GetMessageIdByType(messageType);
            var operationType = (byte)(MessageProtoHelper.IsHeartbeat(messageType) ? MessageOperationType.HeartBeat : MessageOperationType.Game);
            messageObject.SetMessageId(messageId);
            var bytes = ProtoBufSerializerHelper.Serialize(messageObject);
            // len +uniqueId + msgId + bytes.length
            ushort len    = (ushort)(PackageLength + bytes.Length);
            var    span   = new byte[len];
            int    offset = 0;
            span.WriteUShort(len, ref offset);
            span.WriteByte(operationType, ref offset);
            span.WriteByte(byte.MinValue, ref offset);
            span.WriteInt(messageObject.UniqueId, ref offset);
            span.WriteInt(messageObject.MessageId, ref offset);
            span.WriteBytesWithoutLength(bytes, ref offset);
            return span;
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }
}