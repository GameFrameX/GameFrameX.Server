using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 基础消息编码处理器
/// </summary>
public abstract class BaseMessageEncoderHandler : IMessageEncoderHandler
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
    /// 设置压缩消息处理器
    /// </summary>
    /// <param name="compressHandler">压缩消息处理器</param>
    public void SetCompressionHandler(IMessageCompressHandler compressHandler = null)
    {
        CompressHandler = compressHandler;
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