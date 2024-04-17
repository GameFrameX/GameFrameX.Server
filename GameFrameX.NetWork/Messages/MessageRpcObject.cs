using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// RPC 消息
/// </summary>
public abstract class MessageRpcObject : IActorMessage
{
    /// <summary>
    /// 消息唯一id
    /// </summary>
    [JsonIgnore]
    public int UniId { get; set; }

    /// <summary>
    /// 消息ID
    /// </summary>
    [ProtoMember(998)]
    public int MessageId { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    /// <summary>
    /// 消息的唯一ID
    /// </summary>
    [ProtoMember(9999)]
    public long UniqueId { get; set; }

    public MessageRpcObject()
    {
        UniqueId = UtilityIdGenerator.GetNextUniqueId();
    }
}