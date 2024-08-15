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
    /// 获取消息业务类型
    /// </summary>
    /// <param name="messageType">消息对象类型</param>
    /// <returns></returns>
    MessageOperationType GetMessageOperationType(Type messageType);

    /// <summary>
    /// 消息编码,当压缩消息处理器存在的时候将会被调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <returns></returns>
    byte[] Handler(INetworkMessage message);

    /// <summary>
    /// 设置压缩消息处理器
    /// </summary>
    /// <param name="compressHandler">压缩消息处理器</param>
    void SetCompressionHandler(IMessageCompressHandler compressHandler = null);

    /// <summary>
    /// 消息包头长度
    /// </summary>
    ushort PackageLength { get; }
}