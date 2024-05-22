using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

[ProtoContract]
public abstract class MessageActorObject :BaseMessageObject, IActorMessage
{

}