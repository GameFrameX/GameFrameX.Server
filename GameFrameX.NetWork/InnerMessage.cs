using System.Collections.Concurrent;
using System.Text;
using GameFrameX.Extension;
using GameFrameX.NetWork.Messages;
using GameFrameX.Serialize.Serialize;
using GameFrameX.Setting;
using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.NetWork;

public class InnerMessage : IInnerMessage
{
    /// <summary>
    /// 消息ID
    /// </summary>
    public int MessageId { get; private set; }

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
    public int UniqueId { get; private set; }

    /// <summary>
    /// 更新消息唯一ID
    /// </summary>
    public void UpdateUniqueId()
    {
        UniqueId = UtilityIdGenerator.GetNextUniqueIntId();
    }

    /// <summary>
    /// 设置消息唯一ID
    /// </summary>
    /// <param name="uniqueId"></param>
    public void SetUniqueId(int uniqueId)
    {
        UniqueId = uniqueId;
    }

    public string ToSendMessageString(ServerType srcServerType, ServerType destServerType)
    {
        return $"---发送[{srcServerType} To {destServerType}] {ToMessageString()}";
    }

    public string ToReceiveMessageString(ServerType srcServerType, ServerType destServerType)
    {
        return $"---收到[{srcServerType} To {destServerType}] {ToMessageString()}";
    }


    private readonly StringBuilder _stringBuilder = new StringBuilder();

    public string ToMessageString()
    {
        _stringBuilder.Clear();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine($"{'\u2193'.RepeatChar(100)}");
        _stringBuilder.AppendLine(
            $"\u2193---MessageType:[{MessageType?.Name.CenterAlignedText(20)}]---MessageId:[{MessageId.ToString().CenterAlignedText(10)}]---MainId:[{MessageManager.GetMainId(MessageId).ToString().CenterAlignedText(5)}]---SubId:[{MessageManager.GetSubId(MessageId).ToString().CenterAlignedText(5)}]---\u2193");
        _stringBuilder.AppendLine($"{ToString().WordWrap(100),-100}");
        _stringBuilder.AppendLine($"{'\u2191'.RepeatChar(100)}");
        _stringBuilder.AppendLine();
        return _stringBuilder.ToString();
    }

    /// <summary>
    /// 消息操作业务类型
    /// </summary>
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
    /// 消息数据长度
    /// </summary>
    public ushort MessageDataLength { get; private set; }

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
    public MessageObject DeserializeMessageObject()
    {
        var value = SerializerHelper.Deserialize(MessageData, MessageType);
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

    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    public static InnerMessage Create(IOuterMessage message, MessageOperationType operationType)
    {
        var innerMessage = new InnerMessage();
        innerMessage.SetOperationType(operationType);
        innerMessage.SetMessageType(message.MessageType);
        innerMessage.SetMessageData(message.MessageData);
        innerMessage.SetMessageId(message.MessageId);
        innerMessage.SetData(GlobalConst.UniqueIdIdKey, message.UniqueId);
        return innerMessage;
    }

    public static InnerMessage Create(IMessage message, MessageOperationType operationType)
    {
        var innerMessage = new InnerMessage();
        innerMessage.SetOperationType(operationType);
        innerMessage.SetMessageType(message.GetType());
        innerMessage.SetUniqueId(message.UniqueId);
        var buffer = SerializerHelper.Serialize(message);
        innerMessage.SetMessageData(buffer);
        innerMessage.SetMessageId(message.MessageId);
        innerMessage.SetData(GlobalConst.UniqueIdIdKey, message.UniqueId);
        return innerMessage;
    }

    private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();

    public void SetData(string key, object value)
    {
        _data[key] = value;
    }

    public object GetData(string key)
    {
        _data.TryGetValue(key, out var value);
        return value;
    }

    public void ClearData()
    {
        _data.Clear();
    }
}