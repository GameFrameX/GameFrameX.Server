namespace GameFrameX.NetWork.Messages;

public interface IOuterMessage : IMessage
{
    /// <summary>
    /// 消息操作业务类型
    /// </summary>
    MessageOperationType OperationType { get; }

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