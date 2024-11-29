using System.Buffers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息解码处理器
/// </summary>
public class DefaultMessageDecoderHandler : BaseMessageDecoderHandler
{
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
            reader.TryReadBigEndian(out int totalLength);
            // 消息头长度
            reader.TryReadBigEndian(out ushort headerLength);
            // 消息头字节数组
            reader.TryReadBytes(headerLength, out var messageHeaderData);
            // 消息对象头
            var messageObjectHeader = (INetworkMessageHeader)ProtoBufSerializerHelper.Deserialize(messageHeaderData, typeof(MessageObjectHeader));

            // 消息内容
            reader.TryReadBytes(totalLength - headerLength - PackageHeaderLength, out var messageData);
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