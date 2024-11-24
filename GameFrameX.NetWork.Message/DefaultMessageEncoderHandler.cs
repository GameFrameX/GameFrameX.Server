using System.Buffers;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ProtoBase;
using Microsoft.Extensions.ObjectPool;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public sealed class DefaultMessageEncoderHandler : IMessageEncoderHandler, IPackageEncoder<IMessage>
{
    private readonly ObjectPool<MessageObjectHeader> _messageObjectHeaderObjectPool;

    /// <summary>
    /// 
    /// </summary>
    public DefaultMessageEncoderHandler()
    {
        _messageObjectHeaderObjectPool = new DefaultObjectPoolProvider().Create<MessageObjectHeader>();
    }

    /// <summary>
    /// 超过多少字节长度才启用压缩,默认100
    /// </summary>
    public uint LimitCompressLength { get; } = 100;

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] Handler(IMessage message)
    {
        if (message is MessageObject messageObject)
        {
            MessageProtoHelper.SetMessageIdAndOperationType(messageObject);
            var messageBodyData = ProtoBufSerializerHelper.Serialize(messageObject);
            byte zipFlag = 0;
            BytesCompressHandler(ref messageBodyData, ref zipFlag);
            var messageObjectHeader = _messageObjectHeaderObjectPool.Get();
            messageObjectHeader.OperationType = messageObject.OperationType;
            messageObjectHeader.UniqueId = messageObject.UniqueId;
            messageObjectHeader.MessageId = messageObject.MessageId;
            messageObjectHeader.ZipFlag = zipFlag;
            var messageHeaderData = ProtoBufSerializerHelper.Serialize(messageObjectHeader);
            _messageObjectHeaderObjectPool.Return(messageObjectHeader);
            return BufferBytes(messageBodyData, ref messageHeaderData);
        }

        LogHelper.Error("消息对象为空，编码异常");
        return null;
    }

    private byte[] BufferBytes(byte[] messageBodyData, ref byte[] messageHeaderData)
    {
        var totalLength = (ushort)(PackageLength + messageBodyData.Length + messageHeaderData.Length);

        var buffer = ArrayPool<byte>.Shared.Rent(totalLength);
        
        int offset = 0;
        // 总长度
        buffer.WriteUInt(totalLength, ref offset);
        // 消息头长度
        buffer.WriteUShort((ushort)messageHeaderData.Length, ref offset);
        // 消息头
        buffer.WriteBytesWithoutLength(messageHeaderData, ref offset);
        // 消息体
        buffer.WriteBytesWithoutLength(messageBodyData, ref offset);
        var result = buffer.AsSpan(0, totalLength).ToArray();
        ArrayPool<byte>.Shared.Return(buffer);
        return result;
    }

    /// <summary>
    /// 内部消息
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public byte[] Handler(IInnerNetworkMessage message)
    {
        var innerNetworkMessage = message;
        var headerZipFlag = innerNetworkMessage.Header.ZipFlag;
        var messageData = innerNetworkMessage.MessageData;
        BytesCompressHandler(ref messageData, ref headerZipFlag);
        innerNetworkMessage.Header.ZipFlag = headerZipFlag;
        innerNetworkMessage.SetMessageData(messageData);
        var messageHeaderData = ProtoBufSerializerHelper.Serialize(innerNetworkMessage.Header);
        return BufferBytes(message.MessageData, ref messageHeaderData);
    }


    /// <summary>
    /// 消息压缩处理
    /// </summary>
    /// <param name="bytes">压缩前的数据</param>
    /// <param name="zipFlag">压缩标记</param>
    /// <returns></returns>
    private void BytesCompressHandler(ref byte[] bytes, ref byte zipFlag)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(zipFlag);
        if (CompressHandler != null && bytes.Length > LimitCompressLength)
        {
            zipFlag = 1;
            // 压缩
            bytes = CompressHandler.Handler(bytes);
        }
        else
        {
            zipFlag = 0;
        }
    }


    /// <summary>
    /// 压缩消息处理器
    /// </summary>
    private IMessageCompressHandler CompressHandler { get; set; }

    /// <summary>
    /// 设置压缩消息处理器
    /// </summary>
    /// <param name="compressHandler">压缩消息处理器</param>
    public void SetCompressionHandler(IMessageCompressHandler compressHandler = null)
    {
        CompressHandler = compressHandler;
    }

    /// <summary>
    /// totalLength + headerLength
    /// </summary>
    public ushort PackageLength { get; } = 4 + 2;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="pack"></param>
    /// <returns></returns>
    public int Encode(IBufferWriter<byte> writer, IMessage pack)
    {
        var bytes = Handler(pack);
        writer.Write(bytes);
        return bytes.Length;
    }
}