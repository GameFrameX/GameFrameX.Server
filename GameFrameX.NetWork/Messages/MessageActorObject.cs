using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

[ProtoContract]
public abstract class MessageActorObject : IActorMessage
{
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public int MessageId { get; set; }
    public long UniqueId { get; set; }
}