using System.Collections.Concurrent;
using System.Text;
using GameFrameX.Extension;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using Newtonsoft.Json;

namespace GameFrameX.NetWork;

/// <summary>
/// 内部消息
/// </summary>
public sealed class InnerNetworkMessage : IInnerNetworkMessage
{
    /// <summary>
    /// 消息数据
    /// </summary>
    public byte[] MessageData { get; private set; }

    /// <summary>
    /// 消息头对象
    /// </summary>
    public INetworkMessageHeader Header { get; private set; }

    /// <summary>
    /// 设置消息头
    /// </summary>
    /// <param name="header"></param>
    public void SetMessageHeader(INetworkMessageHeader header)
    {
        Header = header;
    }

    /// <summary>
    /// 消息唯一ID
    /// </summary>
    public string UniqueId { get; private set; }

    /// <summary>
    /// 设置唯一消息ID
    /// </summary>
    /// <param name="uniqueId"></param>
    public void SetUniqueId(string uniqueId)
    {
        UniqueId = uniqueId;
    }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
    public INetworkMessage DeserializeMessageObject()
    {
        return (INetworkMessage)ProtoBufSerializerHelper.Deserialize(MessageData, MessageType);
    }

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
        stringBuilder.AppendLine($"{'\u2193'.RepeatChar(120)}");
        // 消息的头部信息
        // 消息类型
        stringBuilder.Append($"---MessageType:[{GetType().Name.CenterAlignedText(20)}]");

        // 消息ID
        stringBuilder.Append($"--MsgId:[{Header.MessageId.ToString().CenterAlignedText(10)}]({MessageIdUtility.GetMainId(Header.MessageId).ToString().CenterAlignedText(5)},{MessageIdUtility.GetSubId(Header.MessageId).ToString().CenterAlignedText(5)})");
        // 操作类型
        stringBuilder.Append($"--OpType:[{((MessageOperationType)Header.OperationType).ToString().CenterAlignedText(12)}]");
        // 唯一ID
        stringBuilder.Append($"--UniqueId:[{Header.UniqueId.ToString().CenterAlignedText(12)}]---");
        stringBuilder.AppendLine();
        // 消息的内容 分割
        stringBuilder.AppendLine();
        // 消息内容
        stringBuilder.AppendLine($"{DeserializeMessageObject().ToJsonString()}");
        // 向上的箭头
        stringBuilder.AppendLine($"{'\u2191'.RepeatChar(120)}");
        stringBuilder.AppendLine();
        return StringBuilderCache.GetStringAndRelease(stringBuilder);
    }

    /// <summary>
    /// 创建内部消息
    /// </summary>
    /// <param name="message"></param>
    /// <param name="messageObjectHeader"></param>
    /// <returns></returns>
    public static IInnerNetworkMessage Create(INetworkMessage message, INetworkMessageHeader messageObjectHeader)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetMessageType(message.GetType());
        innerMessage.SetUniqueId(message.UniqueId.ToString());
        var buffer = ProtoBufSerializerHelper.Serialize(message);
        innerMessage.SetMessageData(buffer);
        messageObjectHeader.OperationType = (byte)message.OperationType;
        messageObjectHeader.MessageId = message.MessageId;
        innerMessage.SetMessageHeader(messageObjectHeader);
        return innerMessage;
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

    public static InnerNetworkMessage Create(MessageObjectHeader messageObjectHeader, byte[] messageData, Type messageType)
    {
        var innerMessage = new InnerNetworkMessage();
        innerMessage.SetMessageHeader(messageObjectHeader);
        innerMessage.SetMessageData(messageData);
        innerMessage.SetMessageType(messageType);
        return innerMessage;
    }
}