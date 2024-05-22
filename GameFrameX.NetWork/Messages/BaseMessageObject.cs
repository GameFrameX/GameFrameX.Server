using GameFrameX.Utility;
using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息基类
/// </summary>
[ProtoContract]
public abstract class BaseMessageObject
{
    /// <summary>
    /// 消息ID
    /// </summary>
    [JsonIgnore]
    [ProtoMember(998)]
    public int MessageId { get; set; }

    /// <summary>
    /// 消息的唯一ID
    /// </summary>
    [ProtoMember(999)]
    public long UniqueId { get; set; }

    public BaseMessageObject()
    {
        UniqueId = UtilityIdGenerator.GetNextUniqueId();
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }

    public string ToMessageString()
    {
        return $"消息ID:[{MessageId}] ==>消息类型:{GetType().Name} 消息内容:{JsonConvert.SerializeObject(this)}";
    }
}