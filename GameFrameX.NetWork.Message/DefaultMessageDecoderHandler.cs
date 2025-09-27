using System.Buffers;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息解码处理器
/// </summary>
public class DefaultMessageDecoderHandler : BaseMessageDecoderHandler
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public override ushort PackageHeaderLength { get; } = sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(int) + sizeof(int);

    /// <summary>
    /// 消息解码
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public override IMessage Handler(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            // 消息总长度
            reader.TryReadBigEndianValue(out uint totalLength);
            // 操作类型
            reader.TryReadBigEndianValue(out byte operationType);
            // 压缩标记
            reader.TryReadBigEndianValue(out byte zipFlag);
            // 唯一ID
            reader.TryReadBigEndianValue(out int uniqueId);
            // 消息ID
            reader.TryReadBigEndianValue(out int messageId);
            // 消息对象头
            var messageObjectHeader = new MessageObjectHeader
            {
                OperationType = (MessageOperationType)operationType,
                ZipFlag = zipFlag,
                UniqueId = uniqueId,
                MessageId = messageId,
            };
            // 消息内容
            reader.TryReadBytesValue((int)(totalLength - PackageHeaderLength), out var messageData);
            if (messageObjectHeader.ZipFlag > 0)
            {
                ArgumentNullException.ThrowIfNull(DecompressHandler, nameof(DecompressHandler));
                messageData = DecompressHandler.Handler(messageData);
            }

            var messageType = MessageProtoHelper.GetMessageTypeById(messageObjectHeader.MessageId);

            if (messageObjectHeader.MessageId >= 0)
            {
                // 外部消息
                return OuterNetworkMessage.Create(messageObjectHeader, messageData, messageType);
            }

            // 内部消息
            return InnerNetworkMessage.Create(messageObjectHeader, messageData, messageType);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }
}