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
public class InnerMessageDecoderHandler : IMessageDecoderHandler, IPackageDecoder<InnerNetworkMessage>
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public virtual int MessageHeaderLength { get; } = 6;

    /// <summary>
    /// 和客户端之间的消息 数据长度(2)+消息唯一ID(4)+消息ID(4)+消息内容
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public virtual INetworkMessage Handler(byte[] data)
    {
        return default;
    }

    private bool DecodeHeaderNetworkMessage(byte[] messageData, out InnerNetworkMessage networkMessage)
    {
        networkMessage = (InnerNetworkMessage)ProtoBufSerializerHelper.Deserialize(messageData, typeof(MessageObjectHeader));
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public INetworkMessage Handler(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            reader.TryReadBigEndian(out int totalLength);
            // 消息头长度
            reader.TryReadBigEndian(out int headerLength);

            reader.TryReadBytes(headerLength, out var messageHeaderData);

            if (DecodeHeaderNetworkMessage(messageHeaderData, out var networkMessage))
            {
                return networkMessage;
            }

            LogHelper.Fatal("未知消息类型");
            return null;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    InnerNetworkMessage InnerHandler(ref ReadOnlySequence<byte> sequence)
    {
        var reader = new SequenceReader<byte>(sequence);
        try
        {
            reader.TryReadBigEndian(out int totalLength);
            // 消息头长度
            reader.TryReadBigEndian(out ushort headerLength);

            reader.TryReadBytes(headerLength, out var messageHeaderData);

            if (DecodeHeaderNetworkMessage(messageHeaderData, out var networkMessage))
            {
                return networkMessage;
            }

            LogHelper.Fatal("未知消息类型");
            return null;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            return null;
        }
    }

    /// <summary>
    /// 解压消息处理器
    /// </summary>
    protected IMessageDecompressHandler DecompressHandler { get; private set; }

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
    public InnerNetworkMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        return InnerHandler(ref buffer);
    }
}