using GameFrameX.NetWork.Abstractions;
using GameFrameX.Setting;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息基类接口
/// </summary>
public interface IMessage : INetworkMessage
{
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