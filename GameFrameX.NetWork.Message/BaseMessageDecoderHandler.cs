using System.Buffers;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.ProtoBase;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息解码处理器
/// </summary>
public abstract class BaseMessageDecoderHandler : IMessageDecoderHandler, IPackageDecoder<IMessage>
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    public virtual int PackageHeaderLength { get; } = 6;

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
    /// 消息解码
    /// </summary>
    /// <param name="sequence"></param>
    /// <returns></returns>
    public abstract IMessage Handler(ref ReadOnlySequence<byte> sequence);

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
    public IMessage Decode(ref ReadOnlySequence<byte> buffer, object context)
    {
        return Handler(ref buffer);
    }
}