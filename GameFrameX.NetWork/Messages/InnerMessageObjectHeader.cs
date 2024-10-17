using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 内部消息对象头
/// </summary>
[ProtoContract]
public sealed class InnerMessageObjectHeader : MessageObjectHeader
{
    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(5)]
    public int ServerId { get; set; }
}