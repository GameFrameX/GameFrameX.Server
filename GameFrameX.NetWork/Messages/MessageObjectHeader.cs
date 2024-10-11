using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象头
/// </summary>
[ProtoContract]
public sealed class MessageObjectHeader
{
    /// <summary>
    /// 服务器ID
    /// </summary>
    [ProtoMember(1)]
    public int ServerId { get; set; }
}