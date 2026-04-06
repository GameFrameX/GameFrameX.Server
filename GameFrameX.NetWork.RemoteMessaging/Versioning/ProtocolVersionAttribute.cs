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

namespace GameFrameX.NetWork.RemoteMessaging.Versioning;

/// <summary>
/// 协议版本特性。标注在消息类型上，用于协议演进时的版本协商和兼容性检查。
/// 演进规则：
/// 1. 新增字段：使用新的 ProtoMember 编号，不修改已有编号。
/// 2. 废弃字段：保留编号但不使用，不得删除或复用编号。
/// 3. 版本升级：Minor 版本递增表示向后兼容，Major 版本递增表示不兼容变更。
/// </summary>
/// <remarks>
/// Protocol version attribute. Annotate on message types for version negotiation and compatibility checking during protocol evolution.
/// Evolution rules:
/// 1. Adding fields: Use new ProtoMember numbers; do not modify existing numbers.
/// 2. Deprecating fields: Keep the number but stop using it; never delete or reuse numbers.
/// 3. Version upgrades: Increment Minor for backward-compatible changes; increment Major for breaking changes.
/// </remarks>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ProtocolVersionAttribute : Attribute
{
    /// <summary>
    /// 初始化协议版本特性。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the protocol version attribute.
    /// </remarks>
    /// <param name="major">主版本号 / The major version number</param>
    /// <param name="minor">次版本号 / The minor version number</param>
    public ProtocolVersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }

    /// <summary>
    /// 主版本号。不兼容变更时递增。
    /// </summary>
    /// <value>主版本号 / The major version number</value>
    public int Major { get; }

    /// <summary>
    /// 次版本号。向后兼容新增时递增。
    /// </summary>
    /// <value>次版本号 / The minor version number</value>
    public int Minor { get; }

    /// <summary>
    /// 协议描述。
    /// </summary>
    /// <value>协议描述 / The protocol description</value>
    public string Description { get; set; }

    /// <summary>
    /// 版本字符串。
    /// </summary>
    /// <value>版本字符串（格式 "Major.Minor"） / The version string (format "Major.Minor")</value>
    public string VersionString
    {
        get { return $"{Major}.{Minor}"; }
    }
}