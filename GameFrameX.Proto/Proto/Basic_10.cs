using System;
using ProtoBuf;
using System.Collections.Generic;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto.Proto
{
	/// <summary>
	/// 请求心跳
	/// </summary>
	[ProtoContract]
	[MessageTypeHandler(655361)]
	public sealed class ReqHeartBeat : MessageObject, IRequestMessage
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
	[MessageTypeHandler(655362)]
	public sealed class NotifyHeartBeat : MessageObject, INotifyMessage
	{
		/// <summary>
		/// 时间戳
		/// </summary>
		[ProtoMember(1)]
		public long Timestamp { get; set; }

	}

	/// <summary>
	/// 
	/// </summary>
	[ProtoContract]
	[MessageTypeHandler(655363)]
	public sealed class NotifyTestA : MessageObject, INotifyMessage
	{
		/// <summary>
		/// 时间戳
		/// </summary>
		[ProtoMember(1)]
		public long Timestamp { get; set; }

	}

	/// <summary>
	/// 
	/// </summary>
	[ProtoContract]
	[MessageTypeHandler(655364)]
	public sealed class NotifyTest : MessageObject, INotifyMessage
	{
		/// <summary>
		/// 时间戳
		/// </summary>
		[ProtoMember(1)]
		public NotifyTestA? Timestamp { get; set; }

	}

}
