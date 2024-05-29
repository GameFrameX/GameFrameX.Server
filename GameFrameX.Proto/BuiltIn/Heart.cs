using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.Proto;
using ProtoBuf;

namespace GameFrameX.Proto.BuiltIn;

/// <summary>
/// 请求心跳
/// </summary>
[MessageTypeHandler(((short.MaxValue - 1) << 16) + 1)]
[ProtoContract]
public partial class ReqActorHeartBeat : MessageObject, IRequestMessage, IReqHeartBeatMessage
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
[MessageTypeHandler(((short.MaxValue - 1) << 16) + 2)]
[ProtoContract]
public partial class RespActorHeartBeat : MessageObject, IResponseMessage, IRespHeartBeatMessage
{
    /// <summary>
    ///  时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}