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
using ProtoBuf;

namespace GameFrameX.NetWork.Messages;

/// <summary>
/// 消息对象头。
/// </summary>
/// <remarks>
/// Represents the header of a network message, containing metadata such as message ID and operation type.
/// </remarks>
[ProtoContract]
public class MessageObjectHeader : INetworkMessageHeader
{
    /// <summary>
    /// 获取或设置消息ID。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message identifier.
    /// </remarks>
    /// <value>消息ID / The message identifier</value>
    [ProtoMember(1)]
    public int MessageId { get; set; }

    /// <summary>
    /// 获取或设置操作类型。
    /// </summary>
    /// <remarks>
    /// Gets or sets the operation type of the message.
    /// </remarks>
    /// <value>操作类型 / The operation type</value>
    [ProtoMember(2)]
    public byte OperationType { get; set; }

    /// <summary>
    /// 获取或设置压缩标记。
    /// </summary>
    /// <remarks>
    /// Gets or sets the compression flag indicating whether the message body is compressed.
    /// </remarks>
    /// <value>压缩标记 / The compression flag</value>
    [ProtoMember(3)]
    public byte ZipFlag { get; set; }

    /// <summary>
    /// 获取或设置唯一消息序列ID。
    /// </summary>
    /// <remarks>
    /// Gets or sets the unique sequence identifier for this message.
    /// </remarks>
    /// <value>唯一消息序列ID / The unique sequence identifier</value>
    [ProtoMember(4)]
    public int UniqueId { get; set; }

    /// <summary>
    /// 清除消息内容。
    /// </summary>
    /// <remarks>
    /// Resets all header properties to their default values.
    /// </remarks>
    public void Clear()
    {
        MessageId = default;
        OperationType = default;
        ZipFlag = default;
        UniqueId = default;
    }
}