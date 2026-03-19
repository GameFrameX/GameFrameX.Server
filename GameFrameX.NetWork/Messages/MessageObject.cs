// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================


using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象基类。
/// </summary>
/// <remarks>
/// Abstract base class for all network messages, providing common properties and methods.
/// </remarks>
[ProtoContract]
public abstract class MessageObject : INetworkMessage
{
    /// <summary>
    /// 初始化消息对象。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the message object and generates a unique ID.
    /// </remarks>
    protected MessageObject()
    {
        UpdateUniqueId();
    }

    /// <summary>
    /// 获取消息ID。
    /// </summary>
    /// <remarks>
    /// Gets the message identifier.
    /// </remarks>
    /// <value>消息ID / The message identifier</value>
    [JsonIgnore]
    public int MessageId { get; private set; }

    /// <summary>
    /// 获取消息业务类型。
    /// </summary>
    /// <remarks>
    /// Gets the operation type of the message.
    /// </remarks>
    /// <value>消息业务类型 / The operation type</value>
    [JsonIgnore]
    public byte OperationType { get; private set; }

    /// <summary>
    /// 设置消息ID。
    /// </summary>
    /// <remarks>
    /// Sets the message identifier.
    /// </remarks>
    /// <param name="messageId">消息ID / The message identifier</param>
    public void SetMessageId(int messageId)
    {
        MessageId = messageId;
    }

    /// <summary>
    /// 获取或设置消息的唯一ID。
    /// </summary>
    /// <remarks>
    /// Gets or sets the unique identifier for this message instance.
    /// </remarks>
    /// <value>消息的唯一ID / The unique identifier for the message</value>
    [JsonIgnore]
    public int UniqueId { get; set; }

    /// <summary>
    /// 更新唯一消息ID。
    /// </summary>
    /// <remarks>
    /// Generates and assigns a new unique identifier for this message.
    /// </remarks>
    public void UpdateUniqueId()
    {
        UniqueId = IdGenerator.GetNextUniqueIntId();
    }

    /// <summary>
    /// 设置唯一消息ID。
    /// </summary>
    /// <remarks>
    /// Sets the unique identifier for this message.
    /// </remarks>
    /// <param name="uniqueId">唯一消息ID / The unique identifier to set</param>
    public void SetUniqueId(int uniqueId)
    {
        UniqueId = uniqueId;
    }

    /// <summary>
    /// 获取格式化后的消息字符串。
    /// </summary>
    /// <remarks>
    /// Gets a formatted string representation of the message for logging purposes.
    /// </remarks>
    /// <param name="actorId">ActorId，用于标识消息所属的角色 / The actor ID associated with the message</param>
    /// <returns>格式化后的消息字符串 / The formatted message string</returns>
    public string ToFormatMessageString(long actorId = default)
    {
        return MessageObjectLoggerHelper.FormatMessage(MessageId, OperationType, UniqueId, this, actorId);
    }

    /// <summary>
    /// 获取JSON格式化后的消息字符串。
    /// </summary>
    /// <remarks>
    /// Serializes the message to a formatted JSON string.
    /// </remarks>
    /// <returns>JSON格式化后的消息字符串 / The formatted JSON string</returns>
    public string ToJsonString()
    {
        return JsonHelper.SerializeFormat(this);
    }

    /// <summary>
    /// 设置消息业务类型。
    /// </summary>
    /// <remarks>
    /// Sets the operation type for this message.
    /// </remarks>
    /// <param name="messageOperationType">消息业务类型 / The operation type to set</param>
    public void SetOperationType(byte messageOperationType)
    {
        OperationType = messageOperationType;
    }

    /// <summary>
    /// 转换为字符串。
    /// </summary>
    /// <remarks>
    /// Returns a JSON string representation of the message.
    /// </remarks>
    /// <returns>JSON字符串 / The JSON string representation</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    /// <summary>
    /// 清除消息内容。
    /// </summary>
    /// <remarks>
    /// Clears all message content and resets properties to default values.
    /// </remarks>
    public abstract void Clear();
}