using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

[ProtoContract]
public abstract class MessageActorObject : MessageObject
{
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}