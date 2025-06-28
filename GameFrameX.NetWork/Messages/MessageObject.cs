// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;
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
    /// <param name="actorId">ActorId</param>
    /// <returns>格式化后的消息字符串</returns>
    public string ToFormatMessageString(long actorId = default)
    {
        return MessageObjectLoggerHelper.FormatMessage(MessageId, OperationType, UniqueId, this, actorId);
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