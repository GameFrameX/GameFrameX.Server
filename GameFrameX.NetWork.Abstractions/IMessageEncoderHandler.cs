namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息编码器接口定义
/// </summary>
public interface IMessageEncoderHandler
{
    /// <summary>
    /// 超过多少字节长度才启用压缩
    /// </summary>
    uint LimitCompressLength { get; }

    /// <summary>
    /// 消息编码,当压缩消息处理器存在的时候将会被调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <returns></returns>
    byte[] Handler(IMessage message);

    /// <summary>
    /// 内部消息
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    byte[] Handler(IInnerNetworkMessage message);

    /// <summary>
    /// 设置压缩消息处理器
    /// </summary>
    /// <param name="compressHandler">压缩消息处理器</param>
    void SetCompressionHandler(IMessageCompressHandler compressHandler = null);

    /// <summary>
    /// 消息包头长度
    /// </summary>
    ushort PackageHeaderLength { get; }
}