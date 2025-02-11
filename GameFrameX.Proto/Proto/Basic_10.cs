using System;
using ProtoBuf;
using System.Collections.Generic;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto.Proto
{
    /// <summary>
    /// 请求心跳
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求心跳")]
    [MessageTypeHandler(655370)]
    public sealed class ReqHeartBeat : MessageObject, IRequestMessage, IHeartBeatMessage
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("时间戳")]
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// 服务器通知心跳结果，因为有些业务需要对心跳结果做处理所以不做成RPC的方式处理
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("服务器通知心跳结果，因为有些业务需要对心跳结果做处理所以不做成RPC的方式处理")]
    [MessageTypeHandler(655371)]
    public sealed class NotifyHeartBeat : MessageObject, INotifyMessage, IHeartBeatMessage
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("时间戳")]
        public long Timestamp { get; set; }
    }

    /// <summary>
    /// 通知客户端服务器人数已达上限
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("通知客户端服务器人数已达上限")]
    [MessageTypeHandler(655372)]
    public sealed class NotifyServerFullyLoaded : MessageObject, INotifyMessage
    {
    }

}
