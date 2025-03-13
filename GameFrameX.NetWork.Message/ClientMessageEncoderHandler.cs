using System.Buffers;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Utility.Extensions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public sealed class ClientMessageEncoderHandler : BaseMessageEncoderHandler
{
    /// <summary>
    /// totalLength + operationType + zipFlag + uniqueId + messageId
    /// </summary>
    public override ushort PackageHeaderLength { get; } = sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(int) + sizeof(int);

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public override byte[] Handler(IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            MessageProtoHelper.SetMessageId(messageObject);
            messageObject.SetOperationType(MessageProtoHelper.GetMessageOperationType(messageObject));
            var messageBodyData = ProtoBufSerializerHelper.Serialize(messageObject);
            var isHeartbeat = MessageProtoHelper.IsHeartbeat(messageObject.GetType());
            byte zipFlag = 0;
            BytesCompressHandler(ref messageBodyData, ref zipFlag);
            var totalLength = (ushort)(PackageHeaderLength + messageBodyData.Length);
            var buffer = ArrayPool<byte>.Shared.Rent(totalLength);
            var offset = 0;
            // 总长度
            buffer.WriteUInt(totalLength, ref offset);
            // operationType
            buffer.WriteByte((byte)(isHeartbeat ? MessageOperationType.HeartBeat : MessageOperationType.Game), ref offset);
            // zipFlag
            buffer.WriteByte(zipFlag, ref offset);
            // uniqueId
            buffer.WriteInt(messageObject.UniqueId, ref offset);
            // MessageId
            buffer.WriteInt(messageObject.MessageId, ref offset);
            buffer.WriteBytesWithoutLength(messageBodyData, ref offset);
            var result = buffer.AsSpan(0, totalLength).ToArray();
            ArrayPool<byte>.Shared.Return(buffer);
            return result;
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }
}