namespace GameFrameX.NetWork.Messages;

public interface IInnerMessage : IMessage
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
    /// 消息数据长度
    /// </summary>
    ushort MessageDataLength { get; }

    /// <summary>
    /// 消息类型
    /// </summary>
    Type MessageType { get; }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
    MessageObject DeserializeMessageObject();

    /// <summary>
    /// 设置消息数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void SetData(string key, object value);

    /// <summary>
    /// 获取消息数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    object GetData(string key);

    /// <summary>
    /// 清除消息数据
    /// </summary>
    void ClearData();
}