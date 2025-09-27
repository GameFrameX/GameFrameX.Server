using System.Buffers;
using GameFrameX.Foundation.Extensions;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
// using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 对外部客户端的消息解码处理器
/// </summary>
[Obsolete("建议使用 DefaultMessageDecoderHandler 替代")]
public sealed class ClientMessageDecoderHandler : DefaultMessageDecoderHandler
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public override uint PackageHeaderLength { get; } = sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(int) + sizeof(int);


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
            // 消息对象头
            var messageObjectHeader = new MessageObjectHeader();
            reader.TryReadBigEndianValue(out byte operationType);
            reader.TryReadBigEndianValue(out byte zipFlag);
            reader.TryReadBigEndianValue(out int uniqueId);
            reader.TryReadBigEndianValue(out int messageId);
            messageObjectHeader.OperationType = (MessageOperationType)operationType;
            messageObjectHeader.ZipFlag = zipFlag;
            messageObjectHeader.UniqueId = uniqueId;
            messageObjectHeader.MessageId = messageId;
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

            throw new Exception("不支持的消息类型,消息ID:" + messageObjectHeader.MessageId);
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }
}