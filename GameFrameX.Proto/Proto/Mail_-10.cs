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
using System.Collections.Generic;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using ProtoBuf;

namespace GameFrameX.Proto.Proto
{
	/// <summary>
	/// 邮件附件视图（含领取状态）。用于邮件列表与读信响应。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("邮件附件视图（含领取状态）")]
	public sealed class MailAttachmentInfo
	{
		/// <summary>附件槽位 ID（邮件内唯一）</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("附件槽位 ID")]
		public int SlotId { get; set; }

		/// <summary>奖励类型（RewardType：0=普通道具，其余为预留路由位）</summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("奖励类型")]
		public int RewardType { get; set; }

		/// <summary>物品 ID</summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("物品 ID")]
		public int ItemId { get; set; }

		/// <summary>数量</summary>
		[ProtoMember(4)]
		[System.ComponentModel.Description("数量")]
		public long Count { get; set; }

		/// <summary>领取状态（ClaimStatus：0=可领，1=已领，2=作废）</summary>
		[ProtoMember(5)]
		[System.ComponentModel.Description("领取状态")]
		public int ClaimStatus { get; set; }
	}

	/// <summary>
	/// 邮件摘要。用于列表展示。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("邮件摘要")]
	public sealed class MailInfo
	{
		/// <summary>玩家邮件实例 ID</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("邮件实例 ID")]
		public long MailId { get; set; }

		/// <summary>来源运营邮件活动 ID</summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("活动 ID")]
		public long CampaignId { get; set; }

		/// <summary>来源发布版本号</summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("发布版本号")]
		public long CampaignVersion { get; set; }

		/// <summary>邮件业务类型（MailType）</summary>
		[ProtoMember(4)]
		[System.ComponentModel.Description("邮件业务类型")]
		public int MailType { get; set; }

		/// <summary>标题</summary>
		[ProtoMember(5)]
		[System.ComponentModel.Description("标题")]
		public string Title { get; set; }

		/// <summary>读信状态（ReadStatus：0=未读，1=已读）</summary>
		[ProtoMember(6)]
		[System.ComponentModel.Description("读信状态")]
		public int ReadStatus { get; set; }

		/// <summary>附件整体状态（AttachmentStatus：0=无附件，1=不可领，2=部分已领，3=全部已领）</summary>
		[ProtoMember(7)]
		[System.ComponentModel.Description("附件整体状态")]
		public int AttachmentStatus { get; set; }

		/// <summary>邮件生命周期状态（MailStatus）</summary>
		[ProtoMember(8)]
		[System.ComponentModel.Description("邮件生命周期状态")]
		public int MailStatus { get; set; }

		/// <summary>实例化时间（unix 秒）</summary>
		[ProtoMember(9)]
		[System.ComponentModel.Description("实例化时间")]
		public long CreateTime { get; set; }

		/// <summary>过期时间（unix 秒）</summary>
		[ProtoMember(10)]
		[System.ComponentModel.Description("过期时间")]
		public long ExpireTime { get; set; }

		/// <summary>是否存在附件</summary>
		[ProtoMember(11)]
		[System.ComponentModel.Description("是否存在附件")]
		public bool HasAttachment { get; set; }
	}

	/// <summary>
	/// 拉取邮件列表（触发懒同步）。客户端分页游标请求。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("拉取邮件列表请求")]
	[MessageTypeHandler(((-10) << 16) + 121)]
	public sealed class ReqMailList : MessageObject, IRequestMessage
	{
		/// <summary>分页游标（已拉取条数偏移）</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("分页游标")]
		public int Cursor { get; set; }

		/// <summary>每页大小</summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("每页大小")]
		public int PageSize { get; set; }

		public override void Clear()
		{
			Cursor = default;
			PageSize = default;
		}
	}

	/// <summary>
	/// 邮件列表响应。服务端先触发懒同步，再返回分页邮件摘要。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("邮件列表响应")]
	[MessageTypeHandler(((-10) << 16) + 122)]
	public sealed class RespMailList : MessageObject, IResponseMessage
	{
		/// <summary>邮件摘要列表</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("邮件摘要列表")]
		public List<MailInfo> Mails { get; set; } = new List<MailInfo>();

		/// <summary>未读计数</summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("未读计数")]
		public int UnreadCount { get; set; }

		/// <summary>是否还有更多</summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("是否还有更多")]
		public bool HasMore { get; set; }

		/// <summary>错误码（OperationStatusCode）</summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			Mails.Clear();
			UnreadCount = default;
			HasMore = default;
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 读信请求（标记已读，读信幂等）。请求体：邮件实例 ID。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("读信请求")]
	[MessageTypeHandler(((-10) << 16) + 123)]
	public sealed class ReqMailRead : MessageObject, IRequestMessage
	{
		/// <summary>邮件实例 ID</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("邮件实例 ID")]
		public long MailId { get; set; }

		public override void Clear()
		{
			MailId = default;
		}
	}

	/// <summary>
	/// 读信响应。返回完整文案、附件列表与领取状态。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("读信响应")]
	[MessageTypeHandler(((-10) << 16) + 124)]
	public sealed class RespMailRead : MessageObject, IResponseMessage
	{
		/// <summary>邮件实例 ID</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("邮件实例 ID")]
		public long MailId { get; set; }

		/// <summary>标题</summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("标题")]
		public string Title { get; set; }

		/// <summary>正文</summary>
		[ProtoMember(3)]
		[System.ComponentModel.Description("正文")]
		public string Content { get; set; }

		/// <summary>模板 ID（审计用）</summary>
		[ProtoMember(4)]
		[System.ComponentModel.Description("模板 ID")]
		public int TemplateId { get; set; }

		/// <summary>模板版本（审计用）</summary>
		[ProtoMember(5)]
		[System.ComponentModel.Description("模板版本")]
		public long TemplateVersion { get; set; }

		/// <summary>读信状态（ReadStatus）</summary>
		[ProtoMember(6)]
		[System.ComponentModel.Description("读信状态")]
		public int ReadStatus { get; set; }

		/// <summary>邮件生命周期状态（MailStatus）</summary>
		[ProtoMember(7)]
		[System.ComponentModel.Description("邮件生命周期状态")]
		public int MailStatus { get; set; }

		/// <summary>附件视图列表（含领取状态）</summary>
		[ProtoMember(8)]
		[System.ComponentModel.Description("附件视图列表")]
		public List<MailAttachmentInfo> Attachments { get; set; } = new List<MailAttachmentInfo>();

		/// <summary>错误码（OperationStatusCode）</summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			MailId = default;
			Title = default;
			Content = default;
			TemplateId = default;
			TemplateVersion = default;
			ReadStatus = default;
			MailStatus = default;
			Attachments.Clear();
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 删除邮件请求。未领取附件邮件会被拒绝（不可逆边界 B2）。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("删除邮件请求")]
	[MessageTypeHandler(((-10) << 16) + 125)]
	public sealed class ReqMailDelete : MessageObject, IRequestMessage
	{
		/// <summary>邮件实例 ID</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("邮件实例 ID")]
		public long MailId { get; set; }

		public override void Clear()
		{
			MailId = default;
		}
	}

	/// <summary>
	/// 删除邮件响应。失败原因走统一错误码（如 <c>UnclaimedAttachment</c> / <c>MailNotFound</c> / <c>MailAlreadyDeleted</c>）。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("删除邮件响应")]
	[MessageTypeHandler(((-10) << 16) + 126)]
	public sealed class RespMailDelete : MessageObject, IResponseMessage
	{
		/// <summary>邮件实例 ID</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("邮件实例 ID")]
		public long MailId { get; set; }

		/// <summary>错误码（OperationStatusCode）</summary>
		[ProtoMember(2047)]
		[System.ComponentModel.Description("错误码")]
		public int ErrorCode { get; set; }

		public override void Clear()
		{
			MailId = default;
			ErrorCode = default;
		}
	}

	/// <summary>
	/// 邮件变更通知（服务端推送）。客户端据此刷新列表。
	/// </summary>
	[ProtoContract]
	[System.ComponentModel.Description("邮件变更通知")]
	[MessageTypeHandler(((-10) << 16) + 127)]
	public sealed class NotifyMailChanged : MessageObject, INotifyMessage
	{
		/// <summary>发生变更的邮件实例 ID 列表（新建 / 状态变更）</summary>
		[ProtoMember(1)]
		[System.ComponentModel.Description("变更邮件 ID 列表")]
		public List<long> ChangedMailIds { get; set; } = new List<long>();

		/// <summary>当前未读计数</summary>
		[ProtoMember(2)]
		[System.ComponentModel.Description("未读计数")]
		public int UnreadCount { get; set; }

		public override void Clear()
		{
			ChangedMailIds.Clear();
			UnreadCount = default;
		}
	}

}
