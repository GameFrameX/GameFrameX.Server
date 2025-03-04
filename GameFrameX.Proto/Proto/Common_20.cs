using System;
using ProtoBuf;
using System.Collections.Generic;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto.Proto
{
    /// <summary>
    /// 返回码
    /// </summary>
    [System.ComponentModel.Description("返回码")]
    public enum ResultCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [System.ComponentModel.Description("成功")]
        Success = 0,

        /// <summary>
        /// 失败
        /// </summary>
        [System.ComponentModel.Description("失败")]
        Failed = 1,
    }

    /// <summary>
    /// 
    /// </summary>
    [System.ComponentModel.Description("")]
    public enum PhoneType
    {
        /// <summary>
        /// 手机
        /// </summary>
        [System.ComponentModel.Description("手机")]
        Mobile = 0,

        /// <summary>
        /// 
        /// </summary>
        [System.ComponentModel.Description("")]
        Home = 1,

        /// <summary>
        /// 工作号码
        /// </summary>
        [System.ComponentModel.Description("工作号码")]
        Work = 2,
    }

    /// <summary>
    /// 操作错误代码
    /// </summary>
    [System.ComponentModel.Description("操作错误代码")]
    public enum OperationStatusCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [System.ComponentModel.Description("成功")]
        Ok = 0,

        /// <summary>
        /// 配置表错误
        /// </summary>
        [System.ComponentModel.Description("配置表错误")]
        ConfigErr = 1,

        /// <summary>
        /// 客户端传递参数错误
        /// </summary>
        [System.ComponentModel.Description("客户端传递参数错误")]
        ParamErr = 2,

        /// <summary>
        /// 消耗不足
        /// </summary>
        [System.ComponentModel.Description("消耗不足")]
        CostNotEnough = 3,

        /// <summary>
        /// 未开通服务
        /// </summary>
        [System.ComponentModel.Description("未开通服务")]
        Forbidden = 4,

        /// <summary>
        /// 不存在
        /// </summary>
        [System.ComponentModel.Description("不存在")]
        NotFound = 5,

        /// <summary>
        /// 已经存在
        /// </summary>
        [System.ComponentModel.Description("已经存在")]
        HasExist = 6,

        /// <summary>
        /// 账号不存在或为空
        /// </summary>
        [System.ComponentModel.Description("账号不存在或为空")]
        AccountCannotBeNull = 7,

        /// <summary>
        /// 无法执行数据库修改
        /// </summary>
        [System.ComponentModel.Description("无法执行数据库修改")]
        Unprocessable = 8,

        /// <summary>
        /// 未知平台
        /// </summary>
        [System.ComponentModel.Description("未知平台")]
        UnknownPlatform = 9,

        /// <summary>
        /// 正常通知
        /// </summary>
        [System.ComponentModel.Description("正常通知")]
        Notice = 10,

        /// <summary>
        /// 功能未开启，主消息屏蔽
        /// </summary>
        [System.ComponentModel.Description("功能未开启，主消息屏蔽")]
        FuncNotOpen = 11,

        /// <summary>
        /// 其他
        /// </summary>
        [System.ComponentModel.Description("其他")]
        Other = 12,

        /// <summary>
        /// 内部服务错误
        /// </summary>
        [System.ComponentModel.Description("内部服务错误")]
        InternalServerError = 13,

        /// <summary>
        /// 通知客户端服务器人数已达上限
        /// </summary>
        [System.ComponentModel.Description("通知客户端服务器人数已达上限")]
        ServerFullyLoaded = 14,
    }

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("")]
    public sealed class PhoneNumber
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        public string Number { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("")]
        public PhoneType Type { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("")]
    public sealed class Person
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        public string Name { get; set; }

        /// <summary>
        /// Unique ID number for this person.
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("Unique ID number for this person.")]
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(3)]
        [System.ComponentModel.Description("")]
        public string Email { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(4)]
        [System.ComponentModel.Description("")]
        public List<PhoneNumber> Phones { get; set; } = new List<PhoneNumber>();
    }

    /// <summary>
    /// Our address book file is just one of these.
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("Our address book file is just one of these.")]
    public sealed class AddressBook
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        public List<Person> People { get; set; } = new List<Person>();
    }

}
