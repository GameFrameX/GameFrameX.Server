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
/// 协议版本协商器。在客户端与服务端建立连接时协商协议版本，确保兼容性。
/// </summary>
/// <remarks>
/// Protocol version negotiator. Negotiates protocol versions when a client establishes a connection with a server to ensure compatibility.
/// </remarks>
public interface IProtocolVersionNegotiator
{
    /// <summary>
    /// 检查指定消息类型的本地版本是否与远端兼容。
    /// </summary>
    /// <remarks>
    /// Checks whether the local version of the specified message type is compatible with the remote side.
    /// </remarks>
    /// <param name="messageType">消息类型 / The message type to check</param>
    /// <returns>true 兼容；false 不兼容 / true if compatible; false if incompatible</returns>
    bool IsCompatible(Type messageType);

    /// <summary>
    /// 获取指定消息类型的协议版本信息。
    /// </summary>
    /// <remarks>
    /// Gets the protocol version information for the specified message type.
    /// </remarks>
    /// <param name="messageType">消息类型 / The message type to query</param>
    /// <returns>版本字符串；未标注特性时返回 "0.0" / The version string; returns "0.0" when no attribute is present</returns>
    string GetVersion(Type messageType);
}