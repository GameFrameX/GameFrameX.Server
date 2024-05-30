using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface IMessageEncoderHandler
{
    byte[] Handler(IMessage message);

    /// <summary>
    /// 消息包头长度
    /// </summary>
    ushort PackageLength { get; }
}