using System;
using ProtoBuf;
using System.Collections.Generic;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Proto.Proto
{
    /// <summary>
    /// 
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("")]
    public sealed class BagItem
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }

        /// <summary>
        /// 道具数量
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("道具数量")]
        public long Count { get; set; }
    }

    /// <summary>
    /// 请求背包数据
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求背包数据")]
    [MessageTypeHandler(6553610)]
    public sealed class ReqBagInfo : MessageObject, IRequestMessage
    {
    }

    /// <summary>
    /// 返回背包数据
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("返回背包数据")]
    [MessageTypeHandler(6553611)]
    public sealed class RespBagInfo : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();

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
    [MessageTypeHandler(6553612)]
    public sealed class NotifyBagItem : MessageObject, INotifyMessage
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }

        /// <summary>
        /// 最终道具数量
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("最终道具数量")]
        public long Count { get; set; }

        /// <summary>
        /// 变化的值
        /// </summary>
        [ProtoMember(3)]
        [System.ComponentModel.Description("变化的值")]
        public long Value { get; set; }
    }

    /// <summary>
    /// 通知背包数据变化
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("通知背包数据变化")]
    [MessageTypeHandler(6553613)]
    public sealed class NotifyBagInfoChanged : MessageObject, INotifyMessage
    {
        /// <summary>
        /// 变化的道具，key:道具id，value:数量
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("变化的道具，key:道具id，value:数量")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<int, NotifyBagItem> ItemDic { get; set; } = new Dictionary<int, NotifyBagItem>();
    }

    /// <summary>
    /// 请求合成宠物
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求合成宠物")]
    [MessageTypeHandler(6553614)]
    public sealed class ReqComposePet : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 碎片id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("碎片id")]
        public int FragmentId { get; set; }
    }

    /// <summary>
    /// 返回合成宠物
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("返回合成宠物")]
    [MessageTypeHandler(6553615)]
    public sealed class RespComposePet : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 合成宠物的Id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("合成宠物的Id")]
        public int PetId { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 请求使用道具
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求使用道具")]
    [MessageTypeHandler(6553616)]
    public sealed class ReqUseItem : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }

        /// <summary>
        /// 道具数量
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("道具数量")]
        public long Count { get; set; }
    }

    /// <summary>
    /// 请求使用道具
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("请求使用道具")]
    [MessageTypeHandler(6553617)]
    public sealed class RespUseItem : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }

        /// <summary>
        /// 道具数量
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("道具数量")]
        public long Count { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 丢弃物品请求
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("丢弃物品请求")]
    [MessageTypeHandler(6553618)]
    public sealed class ReqDiscardItem : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }

        /// <summary>
        /// 道具数量
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("道具数量")]
        public long Count { get; set; }
    }

    /// <summary>
    /// 丢弃物品返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("丢弃物品返回")]
    [MessageTypeHandler(6553619)]
    public sealed class RespDiscardItem : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }

        /// <summary>
        /// 道具数量
        /// </summary>
        [ProtoMember(2)]
        [System.ComponentModel.Description("道具数量")]
        public long Count { get; set; }

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 出售道具
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("出售道具")]
    [MessageTypeHandler(6553620)]
    public sealed class ReqSellItem : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 道具id
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("道具id")]
        public int ItemId { get; set; }
    }

    /// <summary>
    /// 出售道具
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("出售道具")]
    [MessageTypeHandler(6553621)]
    public sealed class RespItemChange : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 变化的道具
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("变化的道具")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<long, long> ItemDic { get; set; } = new Dictionary<long, long>();

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 增加道具
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("增加道具")]
    [MessageTypeHandler(6553622)]
    public sealed class ReqAddItem : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();
    }

    /// <summary>
    /// 增加道具返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("增加道具返回")]
    [MessageTypeHandler(6553623)]
    public sealed class RespAddItem : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 变化的道具
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("变化的道具")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

    /// <summary>
    /// 减少道具
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("减少道具")]
    [MessageTypeHandler(6553624)]
    public sealed class ReqRemoveItem : MessageObject, IRequestMessage
    {
        /// <summary>
        /// 
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();
    }

    /// <summary>
    /// 减少道具返回
    /// </summary>
    [ProtoContract]
    [System.ComponentModel.Description("减少道具返回")]
    [MessageTypeHandler(6553625)]
    public sealed class RespRemoveItem : MessageObject, IResponseMessage
    {
        /// <summary>
        /// 变化的道具
        /// </summary>
        [ProtoMember(1)]
        [System.ComponentModel.Description("变化的道具")]
        [ProtoMap(DisableMap = true)]
        public Dictionary<int, long> ItemDic { get; set; } = new Dictionary<int, long>();

        /// <summary>
        /// 返回的错误码
        /// </summary>
        [ProtoMember(888)]
        [System.ComponentModel.Description("返回的错误码")]
        public int ErrorCode { get; set; }
    }

}
