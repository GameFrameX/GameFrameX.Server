using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using ProtoBuf;

namespace GameFrameX.Proto.BuiltIn;

/// <summary>
/// 请求心跳
/// </summary>
[ProtoContract]
[MessageTypeHandler((-1 << 16) + 100, MessageOperationType.HeartBeat)]
public sealed class ReqActorHeartBeat : MessageObject, IRequestMessage, IHeartBeatMessage
{
    /// <summary>
    /// 时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}

/// <summary>
/// 服务器通知心跳结果，因为有些业务需要对心跳结果做处理所以不做成RPC的方式处理
/// </summary>
[ProtoContract]
[MessageTypeHandler((-1 << 16) + 101, MessageOperationType.HeartBeat)]
public sealed class NotifyActorHeartBeat : MessageObject, INotifyMessage, IHeartBeatMessage
{
    /// <summary>
    /// 时间戳
    /// </summary>
    [ProtoMember(1)]
    public long Timestamp { get; set; }
}