namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络消息接口
/// </summary>
public interface INetworkMessage : IMessage
{
    /// <summary>
    /// 设置消息ID
    /// </summary>
    /// <param name="messageId"></param>
    void SetMessageId(int messageId);

    /// <summary>
    /// 消息的唯一ID
    /// </summary>
    int UniqueId { get; }

    /// <summary>
    /// 更新唯一消息ID
    /// </summary>
    void UpdateUniqueId();

    /// <summary>
    /// 设置唯一消息ID
    /// </summary>
    /// <param name="uniqueId"></param>
    void SetUniqueId(int uniqueId);

    /// <summary>
    /// 获取JSON格式化后的消息字符串
    /// </summary>
    /// <returns></returns>
    string ToJsonString();
}