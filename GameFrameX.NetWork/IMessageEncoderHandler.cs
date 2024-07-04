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
    /// <returns></returns>
    byte[] Handler(IMessage message);

    /// <summary>
    /// 消息包头长度
    /// </summary>
    ushort PackageLength { get; }
}