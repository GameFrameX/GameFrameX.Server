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

    /// <summary>
    /// 消息ID
    /// </summary>
    [ProtoMember(2)]
    public int MessageId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    [ProtoMember(3)]
    public byte OperationType { get; set; }
}