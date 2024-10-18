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
public sealed class DefaultMessageDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public int MessageHeaderLength { get; } = 6;

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public IMessage Handler(byte[] data)
    {
        ReadOnlySequence<byte> sequence = new ReadOnlySequence<byte>(data);
        return Handler(ref sequence);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public IMessage Handler(ref ReadOnlySequence<byte> sequence)
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
            var messageObjectHeader = DecodeHeaderNetworkMessage(messageHeaderData);

            // 消息内容
            reader.TryReadBytes(totalLength - headerLength - MessageHeaderLength, out var messageData);

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

    private INetworkMessageHeader DecodeHeaderNetworkMessage(byte[] messageData)
    {
        var messageObjectHeader = (INetworkMessageHeader)ProtoBufSerializerHelper.Deserialize(messageData, typeof(MessageObjectHeader));
        return messageObjectHeader;
    }

    /// <summary>
    /// 解压消息处理器
    /// </summary>
    private IMessageDecompressHandler DecompressHandler { get; set; }

    /// <summary>
    /// 设置解压消息处理器
    /// </summary>
    /// <param name="decompressHandler">解压消息处理器</param>
    public void SetDecompressionHandler(IMessageDecompressHandler decompressHandler = null)
    {
        DecompressHandler = decompressHandler;
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="buffer"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        return Handler(ref buffer);
    }
}