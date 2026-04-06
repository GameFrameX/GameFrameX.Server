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


using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Utility;
using ProtoBuf;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP 消息包装类，用于 ProtoBuf 序列化的 HTTP 消息传输。
/// </summary>
/// <remarks>
/// HTTP message wrapper class for ProtoBuf-serialized HTTP message transmission.
/// </remarks>
[ProtoContract]
public sealed class MessageHttpObject
{
    /// <summary>
    /// 获取或设置消息 ID。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message ID.
    /// </remarks>
    /// <value>消息 ID / Message ID</value>
    [ProtoMember(1)]
    public int Id { get; set; }

    /// <summary>
    /// 获取或设置消息序列号。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message unique ID (sequence number).
    /// </remarks>
    /// <value>消息序列号 / Message unique ID</value>
    [ProtoMember(2)]
    public int UniqueId { get; set; }

    /// <summary>
    /// 获取或设置消息体字节数组。
    /// </summary>
    /// <remarks>
    /// Gets or sets the message body as byte array.
    /// </remarks>
    /// <value>消息体字节数组 / Message body byte array</value>
    [JsonIgnore]
    [ProtoMember(3)]
    public byte[] Body { get; set; }

    /// <summary>
    /// 返回当前对象的 JSON 字符串表示。
    /// </summary>
    /// <remarks>
    /// Returns a JSON string representation of the current object.
    /// </remarks>
    /// <returns>当前对象的 JSON 字符串表示 / JSON string representation of the current object</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }
}