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

namespace GameFrameX.NetWork.RemoteMessaging;

/// <summary>
/// 远程消息客户端持有者。提供全局静态访问入口，供业务层（如 ComponentAgent）使用。
/// 在启动时初始化一次，后续通过 <see cref="Client" /> 属性访问。
/// </summary>
/// <remarks>
/// Remote message client holder. Provides a global static access point for the business layer (e.g. ComponentAgent).
/// Initialized once at startup, then accessed via the <see cref="Client" /> property.
/// </remarks>
public static class RemoteMessageClientHolder
{
    private static readonly Lazy<IRemoteMessageClient> LazyClient = new(() =>
    {
        // 使用配置驱动构建，便于灰度和运维调整。
        return RemoteMessagingBuilder.BuildFromEnvironment();
    });

    /// <summary>
    /// 获取全局统一远程消息客户端实例。
    /// </summary>
    /// <remarks>
    /// Gets the global unified remote message client instance.
    /// </remarks>
    /// <value>全局远程消息客户端实例 / The global remote message client instance</value>
    public static IRemoteMessageClient Client
    {
        get { return LazyClient.Value; }
    }
}