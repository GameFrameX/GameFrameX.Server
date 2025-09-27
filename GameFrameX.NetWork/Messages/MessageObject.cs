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