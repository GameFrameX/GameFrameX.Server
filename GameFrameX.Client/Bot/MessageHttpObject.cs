using Newtonsoft.Json;
using ProtoBuf;

namespace GameFrameX.Bot;

/// <summary>
/// HTTP消息包装基类
/// </summary>
[ProtoContract]
public class MessageHttpObject
{
    /// <summary>
    /// 消息ID
    /// </summary>
    [ProtoMember(1)]
    public int Id { get; set; }

    /// <summary>
    /// 消息序列号
    /// </summary>
    [ProtoMember(2)]
    public int UniqueId { get; set; }

    [JsonIgnore] [ProtoMember(3)] public byte[] Body { get; set; }

    public override string ToString()
    {
        return Utility.JsonHelper.SerializeFormat(this);
    }
}