// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


﻿// ==========================================================================================
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


using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象日志。
/// </summary>
/// <remarks>
/// Represents a log object for serializing network message information.
/// </remarks>
public sealed class MessageObjectLogObject
{
    /// <summary>
    /// 初始化消息日志对象。
    /// </summary>
    /// <remarks>
    /// Creates a new message log object with the specified parameters.
    /// </remarks>
    /// <param name="name">消息类型名称 / The message type name</param>
    /// <param name="messageId">消息ID / The message identifier</param>
    /// <param name="operationType">操作类型 / The operation type</param>
    /// <param name="uniqueId">唯一ID / The unique identifier</param>
    /// <param name="messageObject">消息对象 / The message object</param>
    /// <param name="actorId">ActorId / The actor identifier</param>
    public MessageObjectLogObject(string name, int messageId, int operationType, int uniqueId, INetworkMessage messageObject, long actorId)
    {
        MessageType = name;
        MessageId = messageId;
        OpType = operationType;
        UniqueId = uniqueId;
        Data = messageObject;
        ActorId = actorId;
    }

    /// <summary>
    /// 获取或设置消息类型。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message type name.
    /// </remarks>
    /// <value>消息类型 / The message type name</value>
    public string MessageType { get; set; }

    /// <summary>
    /// 获取或设置ActorId。
    /// </summary>
    /// <remarks>
    /// Gets or sets the actor identifier.
    /// </remarks>
    /// <value>ActorId / The actor identifier</value>
    public long ActorId { get; set; }

    /// <summary>
    /// 获取或设置消息ID。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message identifier.
    /// </remarks>
    /// <value>消息ID / The message identifier</value>
    public int MessageId { get; set; }

    /// <summary>
    /// 获取或设置操作类型。
    /// </summary>
    /// <remarks>
    /// Gets or sets the operation type.
    /// </remarks>
    /// <value>操作类型 / The operation type</value>
    public int OpType { get; set; }

    /// <summary>
    /// 获取或设置唯一ID。
    /// </summary>
    /// <remarks>
    /// Gets or sets the unique identifier.
    /// </remarks>
    /// <value>唯一ID / The unique identifier</value>
    public int UniqueId { get; set; }

    /// <summary>
    /// 获取或设置消息对象。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message data object.
    /// </remarks>
    /// <value>消息对象 / The message data object</value>
    public object Data { get; set; }
}