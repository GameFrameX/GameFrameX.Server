using System.Collections.Concurrent;
using System.Text;
using GameFrameX.Extension;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.NetWork;

/// <summary>
/// 内部消息
/// </summary>
public class InnerNetworkMessage : IInnerNetworkMessage
{
    /// <summary>
    /// 消息ID
    /// </summary>
    [JsonIgnore]
    public int MessageId { get; private set; }

    /// <summary>
    /// 消息数据长度
    /// </summary>
    [JsonIgnore]
    public ushort MessageDataLength { get; private set; }

    /// <summary>
    /// 设置消息ID
    /// </summary>
    /// <param name="messageId"></param>
    public void SetMessageId(int messageId)
    {
        MessageId = messageId;
    }

    /// <summary>
    /// 消息唯一ID
    /// </summary>
    [JsonIgnore]
    public int UniqueId { get; private set; }

    /// <summary>
    /// 更新消息唯一ID
    /// </summary>
    public void UpdateUniqueId()
    {
        UniqueId = IdGenerator.GetNextUniqueIntId();
    }

    /// <summary>
    /// 设置消息唯一ID
    /// </summary>
    /// <param name="uniqueId"></param>
    public void SetUniqueId(int uniqueId)
    {
        UniqueId = uniqueId;
    }

    /// <summary>
    /// 获取格式化后的消息字符串
    /// </summary>
    /// <returns></returns>
    public string ToFormatMessageString()
    {
        StringBuilder stringBuilder = StringBuilderCache.Acquire();
        stringBuilder.Clear();
        stringBuilder.AppendLine();
        // 向下的箭头
        stringBuilder.AppendLine($"{'\u2193'.RepeatChar(120)}");
        // 消息的头部信息
        // 消息类型
        stringBuilder.Append($"---MessageType:[{GetType().Name.CenterAlignedText(20)}]");
        // 消息ID
        stringBuilder.Append($"--MsgId:[{MessageId.ToString().CenterAlignedText(10)}]({MessageIdUtility.GetMainId(MessageId).ToString().CenterAlignedText(5)},{MessageIdUtility.GetSubId(MessageId).ToString().CenterAlignedText(5)})");
        // 操作类型
        stringBuilder.Append($"--OpType:[{OperationType.ToString().CenterAlignedText(12)}]");
        // 唯一ID
        stringBuilder.Append($"--UniqueId:[{UniqueId.ToString().CenterAlignedText(12)}]---");
        stringBuilder.AppendLine();
        // 消息的内容 分割
        stringBuilder.AppendLine();
        // 消息内容
        stringBuilder.AppendLine($"{DeserializeMessageObject().ToString().WordWrap(120),-120}");
        // 向上的箭头
        stringBuilder.AppendLine($"{'\u2191'.RepeatChar(120)}");
        stringBuilder.AppendLine();
        return StringBuilderCache.GetStringAndRelease(stringBuilder);
    }

    /// <summary>
    /// 消息操作业务类型
    /// </summary>
    [JsonIgnore]
    public MessageOperationType OperationType { get; private set; }

    /// <summary>
    /// 设置消息操作业务类型
    /// </summary>
    /// <param name="operationType"></param>
    public void SetOperationType(MessageOperationType operationType)
    {
        OperationType = operationType;
    }

    /// <summary>
    /// 消息数据
    /// </summary>
    [JsonIgnore]
    public byte[] MessageData { get; private set; }


    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonIgnore]
    public Type MessageType { get; private set; }

    /// <summary>
    /// 设置消息类型
    /// </summary>
    /// <param name="messageType"></param>
    public void SetMessageType(Type messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// 反序列化消息内容
    /// </summary>
    /// <returns></returns>
    public INetworkMessage DeserializeMessageObject()
    {
        var value = ProtoBufSerializerHelper.Deserialize(MessageData, MessageType);
        return (MessageObject)value;
    }

    /// <summary>
    /// 设置消息内容
    /// </summary>
    /// <param name="messageData"></param>
    public void SetMessageData(byte[] messageData)
    {
        MessageData = messageData;
        MessageDataLength = (ushort)messageData.Length;
    }

    /// <summary>
    /// 消息转字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    /// <summary>
    /// 创建消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="operationType"></param>
    /// <returns></returns>
    public static InnerNetworkMessage Create(IOuterMessage message, MessageOperationType operationType)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetOperationType(operationType);
        innerMessage.SetMessageType(message.MessageType);
        innerMessage.SetMessageData(message.MessageData);
        innerMessage.SetMessageId(message.MessageId);
        innerMessage.SetData(GlobalConst.UniqueIdIdKey, message.UniqueId);
        return innerMessage;
    }

    /// <summary>
    /// 创建消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="header"></param>
    /// <param name="operationType"></param>
    /// <returns></returns>
    public static InnerNetworkMessage Create(INetworkMessage message, MessageObjectHeader header, MessageOperationType operationType)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetOperationType(operationType);
        innerMessage.SetMessageType(message.GetType());
        innerMessage.SetUniqueId(message.UniqueId);
        var buffer = ProtoBufSerializerHelper.Serialize(message);
        innerMessage.SetMessageData(buffer);

        header.OperationType = (byte)operationType;
        header.MessageId = message.MessageId;
        innerMessage.SetMessageHeader(header);
        innerMessage.SetMessageId(message.MessageId);
        innerMessage.SetData(GlobalConst.UniqueIdIdKey, message.UniqueId);
        return innerMessage;
    }

    /// <summary>
    /// 创建消息
    /// </summary>
    /// <param name="header"></param>
    /// <param name="messageData"></param>
    /// <param name="messageBodyType"></param>
    /// <returns></returns>
    public static InnerNetworkMessage Create(MessageObjectHeader header, byte[] messageData, Type messageBodyType)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetMessageHeader(header);
        innerMessage.SetMessageId(header.MessageId);
        innerMessage.SetMessageData(messageData);
        innerMessage.SetMessageType(messageBodyType);
        innerMessage.SetOperationType((MessageOperationType)header.OperationType);
        return innerMessage;
    }

    /// <summary>
    /// 消息头
    /// </summary>
    [JsonIgnore]
    public MessageObjectHeader Header { get; private set; }

    /// <summary>
    /// 设置消息头
    /// </summary>
    /// <param name="header"></param>
    public void SetMessageHeader(MessageObjectHeader header)
    {
        Header = header;
    }

    private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

    /// <summary>
    /// 获取自定义数据
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetData()
    {
        return _data.ToDictionary();
    }

    /// <summary>
    /// 设置自定义数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetData(string key, object value)
    {
        _data[key] = value;
    }

    /// <summary>
    /// 获取自定义数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public object GetData(string key)
    {
        _data.TryGetValue(key, out var value);
        return value;
    }

    /// <summary>
    /// 删除自定义数据
    /// </summary>
    /// <param name="key"></param>
    public bool RemoveData(string key)
    {
        return _data.Remove(key, out _);
    }

    /// <summary>
    /// 清除自定义数据
    /// </summary>
    public void ClearData()
    {
        _data.Clear();
    }
}