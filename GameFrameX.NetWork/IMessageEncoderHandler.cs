using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

/// <summary>
/// 消息编码器接口定义
/// </summary>
public interface IMessageEncoderHandler
{
    /// <summary>
    /// 消息编码
    /// </summary>
    /// <param name="message"></param>
    /// <summary>
    /// 消息编码,当压缩消息处理器存在的时候将会被调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <returns></returns>
    byte[] Handler(IMessage message);

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