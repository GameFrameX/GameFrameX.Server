// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Collections.Concurrent;
using System.Text.Json.Serialization;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;

namespace GameFrameX.NetWork;

/// <summary>
/// 内部消息
/// </summary>
public sealed class InnerNetworkMessage : IInnerNetworkMessage
{
    private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

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
        var message = (INetworkMessage)ProtoBufSerializerHelper.Deserialize(MessageData, MessageType);
        message.SetUniqueId(Header.UniqueId);
        MessageProtoHelper.SetMessageId(message);
        if (message is MessageObject messageObject)
        {
            messageObject.SetOperationType(Header.OperationType);
        }

        return message;
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
    /// <param name="actorId">ActorId</param>
    /// <returns>格式化后的消息字符串</returns>
    public string ToFormatMessageString(long actorId = default)
    {
        return MessageObjectLoggerHelper.FormatMessage(Header.MessageId, Header.OperationType, Header.UniqueId, DeserializeMessageObject(), actorId);
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
        try
        {
            var buffer = ProtoBufSerializerHelper.Serialize(message);
            networkMessage.SetMessageData(buffer);
            networkMessage.SetMessageHeader(messageObjectHeader);
            return networkMessage;
        }
        catch (Exception e)
        {
            LogHelper.Error("消息对象编码异常,请检查错误日志");
            LogHelper.Error(e);
            throw;
        }
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