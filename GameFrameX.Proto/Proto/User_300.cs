using System;
using ProtoBuf;
using System.Collections.Generic;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto.Proto
{
    /// <summary>
    /// 请求账号登录
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求账号登录")]
    [MessageTypeHandler(19660810)]
    public sealed class ReqLogin : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("")]
        public string Platform { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(3)]
        [System.ComponentModel.Description("")]
        public int SdkType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(4)]
        [System.ComponentModel.Description("")]
        public string SdkToken { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(5)]
        [System.ComponentModel.Description("")]
        public string Device { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [ProtoMember(6)]
        [System.ComponentModel.Description("密码")]
        public string Password { get; set; }
    }

    /// <summary>
    /// 请求账号登录返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求账号登录返回")]
    [MessageTypeHandler(19660811)]
    public sealed class RespLogin : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        public int Code { get; set; }

        /// <summary>
        /// 账号名
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("账号名")]
        public string RoleName { get; set; }

        /// <summary>
        /// 账号ID
        /// </summary>
        [ProtoMember(3)]
        [System.ComponentModel.Description("账号ID")]
        public long Id { get; set; }

        /// <summary>
        /// 账号等级
        /// </summary>
        [ProtoMember(4)]
        [System.ComponentModel.Description("账号等级")]
        public uint Level { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ProtoMember(5)]
        [System.ComponentModel.Description("创建时间")]
        public long CreateTime { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 请求角色创建
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求角色创建")]
    [MessageTypeHandler(19660812)]
    public sealed class ReqPlayerCreate : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("账号ID")]
        public long Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("角色名")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 请求角色创建返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求角色创建返回")]
    [MessageTypeHandler(19660813)]
    public sealed class RespPlayerCreate : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("角色信息")]
        public PlayerInfo PlayerInfo { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 请求角色列表
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求角色列表")]
    [MessageTypeHandler(19660814)]
    public sealed class ReqPlayerList : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 账号ID
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("账号ID")]
        public long Id { get; set; }
    }

    /// <summary>
    /// 请求角色列表返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求角色列表返回")]
    [MessageTypeHandler(19660815)]
    public sealed class RespPlayerList : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 角色列表
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("角色列表")]
        public List<PlayerInfo> PlayerList { get; set; } = new List<PlayerInfo>();

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("")]
    public sealed class PlayerInfo
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("角色ID")]
        public long Id { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("角色名")]
        public string Name { get; set; }

        /// <summary>
        /// 角色等级
        /// </summary>
        [ProtoMember(3)]
        [System.ComponentModel.Description("角色等级")]
        public uint Level { get; set; }

        /// <summary>
        /// 角色状态
        /// </summary>
        [ProtoMember(4)]
        [System.ComponentModel.Description("角色状态")]
        public int State { get; set; }

        /// <summary>
        /// 角色头像
        /// </summary>
        [ProtoMember(5)]
        [System.ComponentModel.Description("角色头像")]
        public uint Avatar { get; set; }

        /// <summary>
        /// 角色当前经验
        /// </summary>
        [ProtoMember(6)]
        [System.ComponentModel.Description("角色当前经验")]
        public ulong CurrentExp { get; set; }
    }

    /// <summary>
    /// 请求玩家登录
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求玩家登录")]
    [MessageTypeHandler(19660816)]
    public sealed class ReqPlayerLogin : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("角色ID")]
        public long Id { get; set; }
    }

    /// <summary>
    /// 请求玩家登录返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求玩家登录返回")]
    [MessageTypeHandler(19660817)]
    public sealed class RespPlayerLogin : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        public int Code { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("创建时间")]
        public long CreateTime { get; set; }

        /// <summary>
        /// 角色信息
        /// </summary>
        [ProtoMember(3)]
        [System.ComponentModel.Description("角色信息")]
        public PlayerInfo PlayerInfo { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 客户端每次请求都会回复错误码
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("客户端每次请求都会回复错误码")]
    [MessageTypeHandler(19660818)]
    public sealed class RespErrorCode : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 0:表示无错误
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("0:表示无错误")]
        public long ErrCode { get; set; }

        /// <summary>
        /// 错误描述（不为0时有效）
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("错误描述（不为0时有效）")]
        public string Desc { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("")]
    [MessageTypeHandler(19660819)]
    public sealed class RespPrompt : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 提示信息类型（1Tip提示，2跑马灯，3插队跑马灯，4弹窗，5弹窗回到登陆，6弹窗退出游戏）
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("提示信息类型（1Tip提示，2跑马灯，3插队跑马灯，4弹窗，5弹窗回到登陆，6弹窗退出游戏）")]
        public int Type { get; set; }

        /// <summary>
        /// 提示内容
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("提示内容")]
        public string Content { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

}
