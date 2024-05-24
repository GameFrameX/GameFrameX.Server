using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// RPC 消息
/// </summary>
public abstract class MessageRpcObject : BaseMessageObject, IActorMessage
{
    /// <summary>
    /// 消息唯一id
    /// </summary>
    [JsonIgnore]
    public int UniId { get; set; }

    public string ToSendMessageString(ServerType srcServerType, ServerType destServerType)
    {
        return $"---发送[{srcServerType} To {destServerType}] {ToMessageString()}";
    }

    public string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType)
    {
        return $"---收到[{srcServerType} To {destServerType}] {ToMessageString()}";
    }
}