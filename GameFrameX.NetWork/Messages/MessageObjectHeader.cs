using GameFrameX.NetWork.Abstractions;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象头
/// </summary>
[ProtoContract]
public class MessageObjectHeader : INetworkMessageHeader
{
    /// <summary>
    /// 消息ID
    /// </summary>
    [ProtoMember(1)]
    public int MessageId { get; set; }

    /// <summary>
    /// 操作类型
    /// </summary>
    [ProtoMember(2)]
    public MessageOperationType OperationType { get; set; }

    /// <summary>
    /// 压缩标记
    /// </summary>
    [ProtoMember(3)]
    public byte ZipFlag { get; set; }

    /// <summary>
    /// 唯一消息序列ID
    /// </summary>
    [ProtoMember(4)]
    public int UniqueId { get; set; }
}