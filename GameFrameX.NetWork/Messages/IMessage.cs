using GameFrameX.Setting;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息基类接口
/// </summary>
public interface IMessage
{
    /// <summary>
    /// 获取消息字符串
    /// </summary>
    /// <returns></returns>
    string ToMessageString();

    /// <summary>
    /// 获取发送消息字符串
    /// </summary>
    /// <param name="srcServerType"></param>
    /// <param name="destServerType"></param>
    /// <returns></returns>
    string ToSendMessageString(ServerType srcServerType, ServerType destServerType);

    /// <summary>
    /// 获取接收消息字符串
    /// </summary>
    /// <param name="srcServerType"></param>
    /// <param name="destServerType"></param>
    /// <returns></returns>
    string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType);
}