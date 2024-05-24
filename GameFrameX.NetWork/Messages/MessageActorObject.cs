using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

[ProtoContract]
public abstract class MessageActorObject : BaseMessageObject, IActorMessage
{
    public string ToSendMessageString(ServerType srcServerType, ServerType destServerType)
    {
        return $"---发送[{srcServerType} To {destServerType}] {ToMessageString()}";
    }

    public string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType)
    {
        return $"---收到[{srcServerType} To {destServerType}] {ToMessageString()}";
    }
}