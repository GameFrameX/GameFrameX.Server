using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 外部消息
/// </summary>
public interface IOuterMessage : INetworkMessage
{
    /// <summary>
    /// 消息数据
    /// </summary>
    byte[] MessageData { get; }

    /// <summary>
    /// 消息类型
    /// </summary>
    Type MessageType { get; }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
    MessageObject DeserializeMessageObject();
}