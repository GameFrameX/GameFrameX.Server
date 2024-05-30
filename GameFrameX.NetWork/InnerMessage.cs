using GameFrameX.NetWork.Messages;
using GameFrameX.Serialize.Serialize;
using GameFrameX.Setting;
using GameFrameX.Utility;

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

    public string ToMessageString()
    {
        return $"消息ID:[{MessageId}=MainId: {MessageManager.GetMainId(MessageId)} + SubId: {MessageManager.GetSubId(MessageId)},{GetType().Name}] 消息内容:{JsonHelper.Serialize(this)}";
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
    public byte[] MessageData { get; private set; }

    /// <summary>
    /// 消息类型
    /// </summary>
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
    }

    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }
}