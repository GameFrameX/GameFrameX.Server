using System.Buffers;
using GameFrameX.Extension;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public abstract class BaseMessageEncoderHandler : IMessageEncoderHandler, IPackageEncoder<IMessage>
{
    /// <summary>
    /// 压缩消息处理器
    /// </summary>
    protected IMessageCompressHandler CompressHandler { get; private set; }

    /// <summary>
    /// 超过多少字节长度才启用压缩,默认512
    /// </summary>
    public virtual uint LimitCompressLength { get; } = 512;

    /// <summary>
    /// totalLength + headerLength
    /// </summary>
    public virtual ushort PackageHeaderLength { get; } = 4 + 2;

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public abstract byte[] Handler(IMessage message);

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
        return InnerBufferHandler(message.MessageData, ref messageHeaderData);
    }


    /// <summary>
    /// 设置压缩消息处理器
    /// </summary>
    /// <param name="compressHandler">压缩消息处理器</param>
    public void SetCompressionHandler(IMessageCompressHandler compressHandler = null)
    {
        CompressHandler = compressHandler;
    }


    /// <summary>
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

    /// <summary>
    /// 内部消息结构写入
    /// 结构为 totalLength(uint) + headerLength(ushort) + header 数组 + body 数组
    /// </summary>
    /// <param name="messageHeaderData">消息头数组</param>
    /// <param name="messageBodyData">内容数组</param>
    /// <returns></returns>
    protected byte[] InnerBufferHandler(byte[] messageBodyData, ref byte[] messageHeaderData)
    {
        var totalLength = (ushort)(PackageHeaderLength + messageBodyData.Length + messageHeaderData.Length);

        var buffer = ArrayPool<byte>.Shared.Rent(totalLength);

        var offset = 0;
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
    /// 消息压缩处理
    /// </summary>
    /// <param name="bytes">压缩前的数据</param>
    /// <param name="zipFlag">压缩标记</param>
    /// <returns></returns>
    protected void BytesCompressHandler(ref byte[] bytes, ref byte zipFlag)
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
}