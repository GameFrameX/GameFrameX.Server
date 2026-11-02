// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System;
using ProtoBuf;
using System.Collections.Generic;
using GameFrameX.Network.Abstractions;
using GameFrameX.Network.Messages;

namespace GameFrameX.Proto.Proto
{
	/// <summary>
	/// 权限码白名单枚举。
	/// </summary>
	[System.ComponentModel.Description("权限码白名单枚举。")]
	public enum AuthPermission
	{
		/// <summary>
		/// 登录权限
		/// </summary>
		[System.ComponentModel.Description("登录权限")]
		Login = 1,

		/// <summary>
		/// 聊天权限
		/// </summary>
		[System.ComponentModel.Description("聊天权限")]
		Chat = 2,

		/// <summary>
		/// 交易权限
		/// </summary>
		[System.ComponentModel.Description("交易权限")]
		Trade = 3,

		/// <summary>
		/// GM 指令权限
		/// </summary>
		[System.ComponentModel.Description("GM 指令权限")]
		GmCommand = 4,
	}

	/// <summary>
	/// 权限授予业务错误码（6 位 = 模块ID 220 + 3 位编号）。客户端协议 ErrorCode 以 int 透传：发生错误时赋具体码，成功不赋值（框架默认 0）。
	/// </summary>
	[System.ComponentModel.Description("权限授予业务错误码（6 位 = 模块ID 220 + 3 位编号）。客户端协议 ErrorCode 以 int 透传：发生错误时赋具体码，成功不赋值（框架默认 0）。")]
	public enum AuthErrorCode
	{
		/// <summary>
		/// 会话未绑定玩家（未登录）
		/// </summary>
		[System.ComponentModel.Description("会话未绑定玩家（未登录）")]
		NotLoggedIn = 220001,

		/// <summary>
		/// 权限码未在白名单定义
		/// </summary>
		[System.ComponentModel.Description("权限码未在白名单定义")]
		PermissionUnknown = 220002,

		/// <summary>
		/// 权限已授予（重复授予幂等报错）
		/// </summary>
		[System.ComponentModel.Description("权限已授予（重复授予幂等报错）")]
		AlreadyGranted = 220003,

		/// <summary>
		/// 权限未被授予（撤销不存在）
		/// </summary>
		[System.ComponentModel.Description("权限未被授予（撤销不存在）")]
		NotGranted = 220004,
	}

	/// <summary>
	/// 请求授予玩家权限（操作者=请求者）
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求授予玩家权限（操作者=请求者）")]
	[MessageTypeHandler(((220) << 16) + 10)]
	public sealed class ReqAuthGrant : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 目标玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("目标玩家ID")]
		public long TargetPlayerId { get; set; }

		/// <summary>
		/// 权限码（AuthPermission 白名单）
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("权限码（AuthPermission 白名单）")]
		public AuthPermission Permission { get; set; }

		public override void Clear()
		{
			TargetPlayerId = default;
			Permission = default;
		}
	}

	/// <summary>
	/// 返回授予玩家权限结果
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回授予玩家权限结果")]
	[MessageTypeHandler(((220) << 16) + 11)]
	public sealed class RespAuthGrant : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 请求撤销玩家权限（操作者=请求者）
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求撤销玩家权限（操作者=请求者）")]
	[MessageTypeHandler(((220) << 16) + 12)]
	public sealed class ReqAuthRevoke : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 目标玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("目标玩家ID")]
		public long TargetPlayerId { get; set; }

		/// <summary>
		/// 权限码（AuthPermission 白名单）
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("权限码（AuthPermission 白名单）")]
		public AuthPermission Permission { get; set; }

		public override void Clear()
		{
			TargetPlayerId = default;
			Permission = default;
		}
	}

	/// <summary>
	/// 返回撤销玩家权限结果
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回撤销玩家权限结果")]
	[MessageTypeHandler(((220) << 16) + 13)]
	public sealed class RespAuthRevoke : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 请求查询玩家是否持有权限
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求查询玩家是否持有权限")]
	[MessageTypeHandler(((220) << 16) + 14)]
	public sealed class ReqAuthHas : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 目标玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("目标玩家ID")]
		public long TargetPlayerId { get; set; }

		/// <summary>
		/// 权限码（AuthPermission 白名单）
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("权限码（AuthPermission 白名单）")]
		public AuthPermission Permission { get; set; }

		public override void Clear()
		{
			TargetPlayerId = default;
			Permission = default;
		}
	}

	/// <summary>
	/// 返回玩家是否持有权限
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回玩家是否持有权限")]
	[MessageTypeHandler(((220) << 16) + 15)]
	public sealed class RespAuthHas : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 是否已授予
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("是否已授予")]
		public bool Granted { get; set; }

		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			Granted = default;
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 请求列举玩家持有的全部权限
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求列举玩家持有的全部权限")]
	[MessageTypeHandler(((220) << 16) + 16)]
	public sealed class ReqAuthListPermissions : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 目标玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("目标玩家ID")]
		public long TargetPlayerId { get; set; }

		public override void Clear()
		{
			TargetPlayerId = default;
		}
	}

	/// <summary>
	/// 返回玩家持有的全部权限列表
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回玩家持有的全部权限列表")]
	[MessageTypeHandler(((220) << 16) + 17)]
	public sealed class RespAuthListPermissions : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 权限列表
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("权限列表")]
		public List<AuthPermission> Permissions { get; set; } = new List<AuthPermission>();

		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			Permissions.Clear();
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 会话恢复成员快照项
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("会话恢复成员快照项")]
	public sealed class SessionResumeMemberInfo : MessageObject
	{
		/// <summary>
		/// 玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("玩家ID")]
		public long PlayerId { get; set; }

		/// <summary>
		/// 成员状态（OnlineMatchMemberState：0=Joined 1=Ready 2=Playing 3=Disconnected 4=Left 5=Kicked）
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("成员状态（OnlineMatchMemberState：0=Joined 1=Ready 2=Playing 3=Disconnected 4=Left 5=Kicked）")]
		public int State { get; set; }

		public override void Clear()
		{
			PlayerId = default;
			State = default;
		}
	}

	/// <summary>
	/// 请求会话恢复（新物理连接携带登录时获得的会话凭据恢复身份与对局）
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求会话恢复（新物理连接携带登录时获得的会话凭据恢复身份与对局）")]
	[MessageTypeHandler(((220) << 16) + 18)]
	public sealed class ReqSessionResume : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 会话凭据（登录响应下发的 SessionToken 明文）
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("会话凭据（登录响应下发的 SessionToken 明文）")]
		public string Token { get; set; }

		/// <summary>
		/// 客户端最后确认的服务器序号（恢复增量的下界；0 = 无进度，回退全量快照）
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("客户端最后确认的服务器序号（恢复增量的下界；0 = 无进度，回退全量快照）")]
		public long LastAcknowledgedSequence { get; set; }

		public override void Clear()
		{
			Token = default;
			LastAcknowledgedSequence = default;
		}
	}

	/// <summary>
	/// 返回会话恢复（恢复后的身份绑定、轮换新凭据与对局快照/增量）
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回会话恢复（恢复后的身份绑定、轮换新凭据与对局快照/增量）")]
	[MessageTypeHandler(((220) << 16) + 19)]
	public sealed class RespSessionResume : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 恢复的玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("恢复的玩家ID")]
		public long PlayerId { get; set; }

		/// <summary>
		/// 轮换后的新会话凭据（旧凭据即刻失效；后续恢复须使用本值）
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("轮换后的新会话凭据（旧凭据即刻失效；后续恢复须使用本值）")]
		public string NewSessionToken { get; set; }

		/// <summary>
		/// 是否恢复了进行中的对局（false = 无可恢复对局，身份恢复仍成功）
		/// </summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("是否恢复了进行中的对局（false = 无可恢复对局，身份恢复仍成功）")]
		public bool Recovered { get; set; }

		/// <summary>
		/// 恢复的对局数值标识（未恢复为 0）
		/// </summary>
		[ProtoMember(4)]
		[System.ComponentModel.Description("恢复的对局数值标识（未恢复为 0）")]
		public long MatchId { get; set; }

		/// <summary>
		/// 对局状态（OnlineMatchState：0=Created 1=Waiting 2=Ready 3=Running 4=Settling 5=Completed 6=Closed 7=Cancelled 8=Timeout 9=Failed 10=Aborted 11=SettlementFailed）
		/// </summary>
		[ProtoMember(5)]
		[System.ComponentModel.Description("对局状态（OnlineMatchState：0=Created 1=Waiting 2=Ready 3=Running 4=Settling 5=Completed 6=Closed 7=Cancelled 8=Timeout 9=Failed 10=Aborted 11=SettlementFailed）")]
		public int MatchState { get; set; }

		/// <summary>
		/// 恢复点服务器序号（快照切片序号，后续增量起点）
		/// </summary>
		[ProtoMember(6)]
		[System.ComponentModel.Description("恢复点服务器序号（快照切片序号，后续增量起点）")]
		public long ServerSequence { get; set; }

		/// <summary>
		/// 增量是否可用（false = 客户端必须以快照重建本地状态）
		/// </summary>
		[ProtoMember(7)]
		[System.ComponentModel.Description("增量是否可用（false = 客户端必须以快照重建本地状态）")]
		public bool DeltaAvailable { get; set; }

		/// <summary>
		/// 对局成员快照（含各成员当前状态）
		/// </summary>
		[ProtoMember(8)]
		[System.ComponentModel.Description("对局成员快照（含各成员当前状态）")]
		public List<SessionResumeMemberInfo> Members { get; set; } = new List<SessionResumeMemberInfo>();

		/// <summary>
		/// 玩法私有状态载荷（不透明字节；服务端权威快照切片）
		/// </summary>
		[ProtoMember(9)]
		[System.ComponentModel.Description("玩法私有状态载荷（不透明字节；服务端权威快照切片）")]
		public byte[] GameState { get; set; }

		/// <summary>
		/// 处置说明（增量/快照使用方式）
		/// </summary>
		[ProtoMember(10)]
		[System.ComponentModel.Description("处置说明（增量/快照使用方式）")]
		public string Instruction { get; set; }

		/// <summary>
		/// 对局业务结果码（0 = 无对局失败；非 0 为 Online 显式段负值，如超窗 / 已结束 / 已离局）。
		/// 与 RPC 层 ErrorCode 解耦：恢复流程成功时 ErrorCode = 0，对局结果仅经本字段传达，
		/// 客户端始终能读取 NewSessionToken。
		/// </summary>
		[ProtoMember(11)]
		[System.ComponentModel.Description("对局业务结果码（0 = 无对局失败；非 0 为 Online 显式段负值）。与 RPC 层 ErrorCode 解耦，恢复流程成功时 ErrorCode = 0 且 NewSessionToken 必达")]
		public int MatchResultCode { get; set; }

		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			PlayerId = default;
			NewSessionToken = default;
			Recovered = default;
			MatchId = default;
			MatchState = default;
			ServerSequence = default;
			DeltaAvailable = default;
			Members.Clear();
			GameState = default;
		Instruction = default;
		MatchResultCode = default;
		ErrorCode = default;
		}
	}

}
