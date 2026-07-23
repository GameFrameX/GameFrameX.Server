// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
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
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto.Proto
{
	/// <summary>
	/// 掷骰子比大小玩家信息
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("掷骰子比大小玩家信息")]
	public sealed class DiceBattlePlayerInfo
	{
		/// <summary>
		/// 玩家ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("玩家ID")]
		public long RoleId { get; set; }

		/// <summary>
		/// 是否已掷骰
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("是否已掷骰")]
		public bool HasRolled { get; set; }

		/// <summary>
		/// 骰子点数(1-6)。未结算前对外为0，结算后显示真实点数。
		/// </summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("骰子点数(1-6)。未结算前对外为0，结算后显示真实点数。")]
		public int DiceValue { get; set; }
	}

	/// <summary>
	/// 掷骰子比大小游戏信息
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("掷骰子比大小游戏信息")]
	public sealed class DiceBattleGameInfo
	{
		/// <summary>
		/// 房间ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("房间ID")]
		public long RoomId { get; set; }

		/// <summary>
		/// 当前局数
		/// </summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("当前局数")]
		public int Round { get; set; }

		/// <summary>
		/// 胜利玩家ID列表。点数最大者获胜，并列时全部返回；未结算前为空。
		/// </summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("胜利玩家ID列表。点数最大者获胜，并列时全部返回；未结算前为空。")]
		public List<long> WinnerRoleIds { get; set; } = new List<long>();

		/// <summary>
		/// 最大骰子点数。未结算前为0。
		/// </summary>
		[ProtoMember(4)]
		[System.ComponentModel.Description("最大骰子点数。未结算前为0。")]
		public int MaxDiceValue { get; set; }

		/// <summary>
		/// 玩家列表
		/// </summary>
		[ProtoMember(5)]
		[System.ComponentModel.Description("玩家列表")]
		public List<DiceBattlePlayerInfo> Players { get; set; } = new List<DiceBattlePlayerInfo>();
	}

	/// <summary>
	/// 请求掷骰子比大小游戏信息
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求掷骰子比大小游戏信息")]
	[MessageTypeHandler(((420) << 16) + 10)]
	public sealed class ReqDiceBattleGameInfo : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 房间ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("房间ID")]
		public long RoomId { get; set; }

		public override void Clear()
		{
			RoomId = default;
		}
	}

	/// <summary>
	/// 返回掷骰子比大小游戏信息
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回掷骰子比大小游戏信息")]
	[MessageTypeHandler(((420) << 16) + 11)]
	public sealed class RespDiceBattleGameInfo : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 游戏信息
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("游戏信息")]
		public DiceBattleGameInfo GameInfo { get; set; }

		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			GameInfo = default;
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 请求掷骰子（服务器端生成点数）
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求掷骰子（服务器端生成点数）")]
	[MessageTypeHandler(((420) << 16) + 12)]
	public sealed class ReqRollDiceBattle : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 房间ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("房间ID")]
		public long RoomId { get; set; }

		public override void Clear()
		{
			RoomId = default;
		}
	}

	/// <summary>
	/// 返回掷骰子
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回掷骰子")]
	[MessageTypeHandler(((420) << 16) + 13)]
	public sealed class RespRollDiceBattle : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 游戏信息
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("游戏信息")]
		public DiceBattleGameInfo GameInfo { get; set; }

		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			GameInfo = default;
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 请求掷骰子比大小再来一局
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("请求掷骰子比大小再来一局")]
	[MessageTypeHandler(((420) << 16) + 14)]
	public sealed class ReqRestartDiceBattle : MessageObject, IRequestMessage
	{
		/// <summary>
		/// 房间ID
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("房间ID")]
		public long RoomId { get; set; }

		public override void Clear()
		{
			RoomId = default;
		}
	}

	/// <summary>
	/// 返回掷骰子比大小再来一局
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("返回掷骰子比大小再来一局")]
	[MessageTypeHandler(((420) << 16) + 15)]
	public sealed class RespRestartDiceBattle : MessageObject, IResponseMessage
	{
		/// <summary>
		/// 游戏信息
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("游戏信息")]
		public DiceBattleGameInfo GameInfo { get; set; }

		/// <summary>
		/// 返回的错误码
		/// </summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("返回的错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			GameInfo = default;
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 通知掷骰子比大小游戏变化
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("通知掷骰子比大小游戏变化")]
	[MessageTypeHandler(((420) << 16) + 16)]
	public sealed class NotifyDiceBattleGameChanged : MessageObject, INotifyMessage
	{
		/// <summary>
		/// 游戏信息
		/// </summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("游戏信息")]
		public DiceBattleGameInfo GameInfo { get; set; }

		public override void Clear()
		{
			GameInfo = default;
		}
	}

}
