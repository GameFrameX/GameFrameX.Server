using GameFrameX.NetWork.Messages;
using ProtoBuf;

namespace GameFrameX.Proto.BuiltIn;

/// <summary>
/// 请求Cache连接 目标
/// </summary>
[MessageTypeHandler(2000001)]
public partial class ReqActorCacheTarget : MessageActorObject, IActorRequestMessage
{
    /// <summary>
    ///  时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}

/// <summary>
/// 返回Cache连接 目标
/// </summary>
[MessageTypeHandler(2000001)]
[ProtoContract]
public partial class RespActorCacheTarget : MessageActorObject, IActorResponseMessage
{
    /// <summary>
    ///  时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}

/// <summary>
/// 请求心跳
/// </summary>
[MessageTypeHandler(1000001)]
public partial class ReqActorHeartBeat : MessageActorObject, IActorRequestMessage
{
    /// <summary>
    ///  时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}

/// <summary>
/// 返回心跳
/// </summary>
[MessageTypeHandler(1000001)]
[ProtoContract]
public partial class RespActorHeartBeat : MessageActorObject, IActorResponseMessage
{
    /// <summary>
    ///  时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}