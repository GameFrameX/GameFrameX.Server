using System.Buffers;
using GameFrameX.Foundation.Extensions;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public sealed class DefaultMessageEncoderHandler : BaseMessageEncoderHandler
{
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
            try
            {
                var messageBodyData = ProtoBufSerializerHelper.Serialize(messageObject);
                byte zipFlag = 0;
                BytesCompressHandler(ref messageBodyData, ref zipFlag);
                var totalLength = (ushort)(PackageHeaderLength + messageBodyData.Length);
                var buffer = ArrayPool<byte>.Shared.Rent(totalLength);
                var offset = 0;
                // 总长度
                buffer.WriteUIntValue(totalLength, ref offset);
                // operationType 操作类型
                buffer.WriteByteValue((byte)messageObject.OperationType, ref offset);
                // zipFlag 压缩标记
                buffer.WriteByteValue(zipFlag, ref offset);
                // uniqueId 唯一ID
                buffer.WriteIntValue(messageObject.UniqueId, ref offset);
                // MessageId 消息ID
                buffer.WriteIntValue(messageObject.MessageId, ref offset);
                buffer.WriteBytesWithoutLength(messageBodyData, ref offset);
                var result = buffer.AsSpan(0, totalLength).ToArray();
                ArrayPool<byte>.Shared.Return(buffer);
                return result;
            }
            catch (Exception e)
            {
                LogHelper.Error("消息对象编码异常,请检查错误日志");
                LogHelper.Error(e);
                return null;
            }
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }
}