namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 内部消息
/// </summary>
public interface IInnerNetworkMessage : IMessage
{
    /// <summary>
    /// 消息数据
    /// </summary>
    byte[] MessageData { get; }

    /// <summary>
    /// 消息头对象
    /// </summary>
    INetworkMessageHeader Header { get; }

    /// <summary>
    /// 消息唯一ID
    /// </summary>
    string UniqueId { get; }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
    INetworkMessage DeserializeMessageObject();

    /// <summary>
    /// 设置消息数据
    /// </summary>
    /// <param name="messageData"></param>
    void SetMessageData(byte[] messageData);

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