using System.Collections.Concurrent;
using System.Text;
using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.NetWork;

/// <summary>
/// 外部消息
/// </summary>
public sealed class OuterNetworkMessage : IOuterNetworkMessage
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
    /// 消息唯一ID
    /// </summary>
    public string UniqueId { get; private set; }

    /// <summary>
    /// 转换消息数据为消息对象
    /// </summary>
    /// <returns></returns>
    public INetworkMessage DeserializeMessageObject()
    {
        var value = (INetworkMessage)ProtoBufSerializerHelper.Deserialize(MessageData, MessageType);
        value.SetUniqueId(Header.UniqueId);
        return value;
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
        return MessageObjectLoggerHelper.FormatMessage(Header.MessageId, Header.OperationType, Header.UniqueId, DeserializeMessageObject());
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
    /// 设置唯一消息ID
    /// </summary>
    /// <param name="uniqueId"></param>
    public void SetUniqueId(string uniqueId)
    {
        UniqueId = uniqueId;
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
    public static IOuterNetworkMessage Create(INetworkMessage message, INetworkMessageHeader messageObjectHeader)
    {
        var innerMessage = new OuterNetworkMessage();
        innerMessage.SetMessageType(message.GetType());
        innerMessage.SetUniqueId(message.UniqueId.ToString());
        try
        {
            var buffer = ProtoBufSerializerHelper.Serialize(message);
            innerMessage.SetMessageData(buffer);
        }
        catch (Exception e)
        {
            LogHelper.Error("消息对象编码异常,请检查错误日志");
            LogHelper.Error(e);
            throw;
        }

        messageObjectHeader.OperationType = MessageProtoHelper.GetMessageOperationType(message.GetType());
        messageObjectHeader.MessageId = MessageProtoHelper.GetMessageIdByType(message.GetType());
        messageObjectHeader.UniqueId = message.UniqueId;
        innerMessage.SetMessageHeader(messageObjectHeader);
        return innerMessage;
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
    public static OuterNetworkMessage Create(INetworkMessageHeader messageObjectHeader, byte[] messageData, Type messageType)
    {
        var message = new OuterNetworkMessage();
        message.SetMessageHeader(messageObjectHeader);
        message.SetMessageData(messageData);
        message.SetMessageType(messageType);
        return message;
    }
}