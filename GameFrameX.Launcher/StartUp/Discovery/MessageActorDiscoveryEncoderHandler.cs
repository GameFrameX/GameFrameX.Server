namespace GameFrameX.Launcher.StartUp.Discovery;

class MessageActorDiscoveryEncoderHandler : BaseMessageEncoderHandler
{
    /// <summary>
    /// 和客户端之间的消息 数据长度(4)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] InnerHandler(IInnerMessage message)
    {
        var msgId = MessageProtoHelper.GetMessageIdByType(message.MessageType);
        ushort len = (ushort)(PackageLength + message.MessageDataLength);
        var buffer = new byte[len];
        int offset = 0;
        message.SetMessageId(msgId);
        buffer.WriteUShort(len, ref offset);
        buffer.WriteByte((byte)message.OperationType, ref offset);
        buffer.WriteInt(message.UniqueId, ref offset);
        buffer.WriteInt(msgId, ref offset);
        buffer.WriteBytesWithoutLength(message.MessageData, ref offset);
        return buffer;
    }
}