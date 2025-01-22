using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.NetWork;

/// <summary>
/// 内部消息
/// </summary>
public sealed class InnerNetworkMessage : IInnerNetworkMessage
{
    private readonly ConcurrentDictionary<string, object> _data = new();

    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonIgnore]
    public Type MessageType { get; private set; }

    /// <summary>
    /// 消息数据
    /// </summary>
    public byte[] MessageData { get; private set; }

    /// <summary>
    /// 消息头对象
    /// </summary>
    public INetworkMessageHeader Header { get; private set; }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
    public INetworkMessage DeserializeMessageObject()
    {
        return (INetworkMessage)ProtoBufSerializerHelper.Deserialize(MessageData, MessageType);
    }

    /// <summary>
    /// 设置消息数据
    /// </summary>
    /// <param name="messageData"></param>
    public void SetMessageData(byte[] messageData)
    {
        MessageData = messageData;
    }


    /// <summary>
    /// 获取格式化后的消息字符串
    /// </summary>
    /// <returns></returns>
    public string ToFormatMessageString()
    {
        var stringBuilder = StringBuilderCache.Acquire();
        stringBuilder.Clear();
        stringBuilder.AppendLine();
        // 向下的箭头
        stringBuilder.AppendLine($"{'\u2193'.RepeatChar(140)}");
        // 消息的头部信息
        // 消息类型
        stringBuilder.Append($"---MessageType:[{GetType().Name.CenterAlignedText(30)}]");
        // 消息ID
        stringBuilder.Append($"--MsgId:[{Header.MessageId.ToString().CenterAlignedText(11)}]({MessageIdUtility.GetMainId(Header.MessageId).ToString().CenterAlignedText(3)},{MessageIdUtility.GetSubId(Header.MessageId).ToString().CenterAlignedText(3)})");
        // 操作类型
        stringBuilder.Append($"--OpType:[{Header.OperationType.ToString().CenterAlignedText(20)}]");
        // 唯一ID
        stringBuilder.Append($"--UniqueId:[{Header.UniqueId.ToString().CenterAlignedText(13)}]---");
        // 消息的内容 分割
        stringBuilder.AppendLine();
        // 消息内容
        stringBuilder.AppendLine($"{DeserializeMessageObject().ToJsonString()}");
        // 向上的箭头
        stringBuilder.AppendLine($"{'\u2191'.RepeatChar(140)}");
        stringBuilder.AppendLine();
        return StringBuilderCache.GetStringAndRelease(stringBuilder);
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
    /// 清除自定义数据
    /// </summary>
    public void ClearData()
    {
        _data.Clear();
    }

    /// <summary>
    /// 设置消息头
    /// </summary>
    /// <param name="header"></param>
    public void SetMessageHeader(INetworkMessageHeader header)
    {
        Header = header;
    }

    /// <summary>
    /// 设置消息类型
    /// </summary>
    /// <param name="messageType"></param>
    public void SetMessageType(Type messageType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// 创建内部消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageObjectHeader"></param>
    /// <returns></returns>
    public static IInnerNetworkMessage Create(INetworkMessage message, INetworkMessageHeader messageObjectHeader)
    {
        var networkMessage = new InnerNetworkMessage();
        messageObjectHeader.OperationType = MessageProtoHelper.GetMessageOperationType(message);
        messageObjectHeader.MessageId = MessageProtoHelper.GetMessageIdByType(message);
        messageObjectHeader.UniqueId = message.UniqueId;

        networkMessage.SetMessageType(message.GetType());
        var buffer = ProtoBufSerializerHelper.Serialize(message);
        networkMessage.SetMessageData(buffer);
        networkMessage.SetMessageHeader(messageObjectHeader);
        return networkMessage;
    }

    /// <summary>
    /// 获取自定义数据
    /// </summary>
    /// <returns></returns>
    public Dictionary<string, object> GetData()
    {
        return _data.ToDictionary();
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
    /// 创建内部消息
    /// </summary>
    /// <param name="messageObjectHeader">消息头</param>
    /// <param name="messageData">消息体</param>
    /// <param name="messageType">消息体的类型</param>
    /// <returns></returns>
    public static InnerNetworkMessage Create(INetworkMessageHeader messageObjectHeader, byte[] messageData, Type messageType)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetMessageHeader(messageObjectHeader);
        innerMessage.SetMessageData(messageData);
        innerMessage.SetMessageType(messageType);
        return innerMessage;
    }
}