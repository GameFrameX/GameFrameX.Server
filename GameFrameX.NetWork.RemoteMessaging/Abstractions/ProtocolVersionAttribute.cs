// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.NetWork.RemoteMessaging.Abstractions;

/// <summary>
/// 协议版本特性。标注在消息类型上，用于协议演进时的版本协商和兼容性检查。
///
/// 演进规则：
/// 1. 新增字段：使用新的 ProtoMember 编号，不修改已有编号。
/// 2. 废弃字段：保留编号但不使用，不得删除或复用编号。
/// 3. 版本升级：Minor 版本递增表示向后兼容，Major 版本递增表示不兼容变更。
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ProtocolVersionAttribute : Attribute
{
    /// <summary>
    /// 主版本号。不兼容变更时递增。
    /// </summary>
    public int Major { get; }

    /// <summary>
    /// 次版本号。向后兼容新增时递增。
    /// </summary>
    public int Minor { get; }

    /// <summary>
    /// 协议描述。
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 初始化协议版本特性。
    /// </summary>
    /// <param name="major">主版本号</param>
    /// <param name="minor">次版本号</param>
    public ProtocolVersionAttribute(int major, int minor)
    {
        Major = major;
        Minor = minor;
    }

    /// <summary>
    /// 版本字符串。
    /// </summary>
    public string VersionString => $"{Major}.{Minor}";
}
