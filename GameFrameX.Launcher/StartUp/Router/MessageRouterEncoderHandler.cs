using GameFrameX.Serialize.Serialize;

namespace GameFrameX.Launcher.StartUp.Router;

class MessageRouterEncoderHandler : BaseMessageEncoderHandler
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override byte[] Handler(IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            var messageType = message.GetType();

            var msgId = MessageProtoHelper.GetMessageIdByType(messageType);
            var operationType = (byte)(MessageProtoHelper.HasHeartbeat(messageType) ? MessageOperationType.HeartBeat : MessageOperationType.Game);
            messageObject.MessageId = msgId;
            var uniqueId = messageObject.UniqueId;
            var bytes = SerializerHelper.Serialize(messageObject);
            // len +uniqueId + msgId + bytes.length
            ushort len = (ushort)(PackageLength + bytes.Length);
            var span = new byte[len];
            int offset = 0;
            span.WriteUShort(len, ref offset);
            span.WriteByte(operationType, ref offset);
            span.WriteInt(uniqueId, ref offset);
            span.WriteInt(msgId, ref offset);
            span.WriteBytesWithoutLength(bytes, ref offset);
            return span;
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }

    public override ushort PackageLength { get; } = 2 + 1 + 4 + 4;
}