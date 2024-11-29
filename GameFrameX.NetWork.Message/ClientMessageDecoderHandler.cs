using System.Buffers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 对外部客户端的消息解码处理器
/// </summary>
public sealed class ClientMessageDecoderHandler : DefaultMessageDecoderHandler
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public override int PackageHeaderLength { get; } = sizeof(uint) + sizeof(byte) + sizeof(byte) + sizeof(int) + sizeof(int);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public override IMessage Handler(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            // 消息总长度
            reader.TryReadBigEndian(out uint totalLength);
            // 消息对象头
            var messageObjectHeader = new MessageObjectHeader();
            reader.TryReadBigEndian(out byte operationType);
            reader.TryReadBigEndian(out byte zipFlag);
            reader.TryReadBigEndian(out int uniqueId);
            reader.TryReadBigEndian(out int messageId);
            messageObjectHeader.OperationType = (MessageOperationType)operationType;
            messageObjectHeader.ZipFlag = zipFlag;
            messageObjectHeader.UniqueId = uniqueId;
            messageObjectHeader.MessageId = messageId;
            // 消息内容
            reader.TryReadBytes((int)(totalLength - PackageHeaderLength), out var messageData);
            if (messageObjectHeader.ZipFlag > 0)
            {
                DecompressHandler.CheckNotNull(nameof(DecompressHandler));
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