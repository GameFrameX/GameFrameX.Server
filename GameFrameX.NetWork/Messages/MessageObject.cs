using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象
/// </summary>
[ProtoContract]
public abstract class MessageObject : INetworkMessage
{
    /// <summary>
    /// </summary>
    protected MessageObject()
    {
        UpdateUniqueId();
    }
    /*/// <summary>
    /// 单位id
    /// </summary>
    [JsonIgnore]
    public int UniId { get; set; }*/

    /// <summary>
    /// 消息ID
    /// </summary>
    [JsonIgnore]
    public int MessageId { get; private set; }

    /// <summary>
    /// 消息业务类型
    /// </summary>
    [JsonIgnore]
    public MessageOperationType OperationType { get; private set; }

    /// <summary>
    /// 设置消息ID
    /// </summary>
    /// <param name="messageId"></param>
    public void SetMessageId(int messageId)
    {
        MessageId = messageId;
    }

    /// <summary>
    /// 消息的唯一ID
    /// </summary>
    [JsonIgnore]
    public int UniqueId { get; set; }


    /// <summary>
    /// 更新唯一消息ID
    /// </summary>
    public void UpdateUniqueId()
    {
        UniqueId = IdGenerator.GetNextUniqueIntId();
    }

    /// <summary>
    /// 设置唯一消息ID
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
        return MessageObjectLoggerHelper.FormatMessage(MessageId, OperationType, UniqueId, this);
    }

    /// <summary>
    /// 获取JSON格式化后的消息字符串
    /// </summary>
    /// <returns></returns>
    public string ToJsonString()
    {
        return JsonHelper.SerializeFormat(this);
    }

    /// <summary>
    /// 设置消息业务类型
    /// </summary>
    /// <param name="messageOperationType">消息业务类型 </param>
    public void SetOperationType(MessageOperationType messageOperationType)
    {
        OperationType = messageOperationType;
    }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }
}