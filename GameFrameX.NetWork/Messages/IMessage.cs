using GameFrameX.Setting;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息基类接口
/// </summary>
public interface IMessage
{
    /// <summary>
    /// 消息ID
    /// </summary>
    int MessageId { get; set; }

    /// <summary>
    /// 消息的唯一ID
    /// </summary>
    int UniqueId { get; set; }

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
    /// 获取消息字符串
    /// </summary>
    /// <returns></returns>
    string ToMessageString();

    /// <summary>
    /// 获取发送消息字符串
    /// </summary>
    /// <param name="srcServerType">发送方</param>
    /// <param name="destServerType">接收方</param>
    /// <returns></returns>
    string ToSendMessageString(ServerType srcServerType, ServerType destServerType);

    /// <summary>
    /// 获取接收消息字符串
    /// </summary>
    /// <param name="srcServerType">发送方</param>
    /// <param name="destServerType">接收方</param>
    /// <returns></returns>
    string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType);
}