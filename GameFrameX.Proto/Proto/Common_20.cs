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
