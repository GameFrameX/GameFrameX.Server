// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏生态环境、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 统一消息发送器全局持有者。提供全局静态访问入口，供业务层使用。
/// 在服务启动时通过 <see cref="Initialize"/> 注入依赖组件，后续通过 <see cref="Sender" /> 属性访问。
/// </summary>
/// <remarks>
/// Unified message sender global holder. Provides a global static access point for the business layer.
/// Dependencies are injected via <see cref="Initialize"/> at service startup, then accessed via the <see cref="Sender" /> property.
/// </remarks>
public static class UnifiedMessageSenderHolder
{
    private static IUnifiedMessageSender _sender;
    private static MessageSendMetrics _metrics;
    private static readonly object _lock = new();

    /// <summary>
    /// 获取全局统一消息发送器实例。必须在调用 <see cref="Initialize"/> 之后使用。
    /// </summary>
    /// <remarks>
    /// Gets the global unified message sender instance. Must be used after calling <see cref="Initialize"/>.
    /// </remarks>
    /// <exception cref="InvalidOperationException">未初始化时访问 / Thrown when accessed before initialization</exception>
    public static IUnifiedMessageSender Sender
    {
        get
        {
            if (_sender == null)
            {
                throw new InvalidOperationException(
                    "UnifiedMessageSenderHolder has not been initialized. Call Initialize() first during startup.");
            }

            return _sender;
        }
    }

    /// <summary>
    /// 是否已初始化
    /// </summary>
    /// <remarks>
    /// Whether the holder has been initialized.
    /// </remarks>
    public static bool IsInitialized
    {
        get { return _sender != null; }
    }

    /// <summary>
    /// 获取统一消息发送指标聚合器。
    /// </summary>
    /// <exception cref="InvalidOperationException">未初始化或发送器未提供指标实例时访问 / Thrown when metrics are unavailable</exception>
    public static MessageSendMetrics Metrics
    {
        get
        {
            if (_metrics == null)
            {
                throw new InvalidOperationException(
                    "UnifiedMessageSender metrics are unavailable. Initialize() with UnifiedMessageSender first.");
            }

            return _metrics;
        }
    }

    /// <summary>
    /// 初始化统一消息发送器。在服务启动时调用一次。
    /// </summary>
    /// <remarks>
    /// Initializes the unified message sender. Called once during service startup.
    /// </remarks>
    /// <param name="sender">统一消息发送器实例 / The unified message sender instance</param>
    public static void Initialize(IUnifiedMessageSender sender)
    {
        if (sender == null)
        {
            throw new ArgumentNullException(nameof(sender));
        }

        lock (_lock)
        {
            _sender = sender;
            _metrics = (sender as UnifiedMessageSender)?.Metrics;
        }
    }

    /// <summary>
    /// 使用默认组件初始化。适用于标准启动流程。
    /// </summary>
    /// <remarks>
    /// Initializes with default components. Suitable for standard startup flow.
    /// </remarks>
    /// <param name="routeResolver">玩家路由解析器 / Player route resolver</param>
    /// <param name="localSender">本服玩家发送器 / Local player sender</param>
    /// <param name="remoteClient">远程消息客户端 / Remote message client</param>
    public static void InitializeWithDefaults(
        IPlayerRouteResolver routeResolver,
        IPlayerLocalSender localSender,
        IRemoteMessageClient remoteClient)
    {
        if (remoteClient == null)
        {
            throw new ArgumentNullException(nameof(remoteClient));
        }

        var instanceSelector = new ConsistentHashServerInstanceSelector();
        var metrics = new MessageSendMetrics();
        var sender = new UnifiedMessageSender(remoteClient, routeResolver, localSender, instanceSelector, metrics);
        Initialize(sender);
    }

}
