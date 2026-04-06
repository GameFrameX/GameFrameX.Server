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
/// 协议版本协商器。在客户端与服务端建立连接时协商协议版本，确保兼容性。
/// </summary>
public interface IProtocolVersionNegotiator
{
    /// <summary>
    /// 检查指定消息类型的本地版本是否与远端兼容。
    /// </summary>
    /// <param name="messageType">消息类型</param>
    /// <returns>true 兼容；false 不兼容</returns>
    bool IsCompatible(Type messageType);

    /// <summary>
    /// 获取指定消息类型的协议版本信息。
    /// </summary>
    /// <param name="messageType">消息类型</param>
    /// <returns>版本字符串；未标注特性时返回 "0.0"</returns>
    string GetVersion(Type messageType);
}
