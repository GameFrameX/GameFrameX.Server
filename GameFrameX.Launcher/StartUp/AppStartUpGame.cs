using System.Reflection;
using GameFrameX.Core.Events;
using GameFrameX.Core.Session;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Utility;
using GameFrameX.Network.Abstractions;
using GameFrameX.Utility.Runtime;

namespace GameFrameX.Launcher.StartUp;

/// <summary>
/// 游戏服务器
/// </summary>
/// <remarks>
/// 组合根边界：本类所在启动模板序列（<see cref="AppStartUpGameServerBase"/>
/// 模板钩子 + 各 Role 差异覆写）负责基础设施装配——网络监听、GameDb 数据库、发现层、热更模块装载与退出编排；
/// 平台业务域服务（好友 / 资产 / 对局等）的组合经统一组合根（<see cref="GameFrameX.Launcher.Composition.RuntimeCompositionRoot"/>
/// → <c>CapabilityComposition.Compose</c> 单入口）承载：旧栈固定序列包装为早晚宿主模块与能力模块进入同一运行时
///（"Online" 配置段声明启用 Role）。两层边界经好友查询桥实证：Hotfix 协议面薄适配器经 Online 契约面
/// 进程内消费平台服务，不经本地注册表绕行。
/// Composition-root boundary: the startup template assembles the role differences through hooks while
/// since the dual-assembly convergence the composition of the legacy fixed sequence (wrapped as the
/// early/late host modules) and the platform domain services (friends, assets, matches, ...) both ride
/// the single composition root (<see cref="GameFrameX.Launcher.Composition.RuntimeCompositionRoot"/> →
/// <c>CapabilityComposition.Compose</c>, enabled roles declared in the "Online" configuration section)
/// into one runtime. The two-layer boundary is proven by the friend-query bridge: the Hotfix protocol
/// thin adapter consumes the platform services in-process through the Online contract face, never
/// through the local registry.
/// </remarks>
// 显式优先级修复与 Social 同为缺省 1000 的冲突——Game 为主服务先起（值越小优先级越高）
[StartUpTag(GameServerConst.Game.Name, 100)]
internal sealed class AppStartUpGame : AppStartUpGameServerBase
{
    /// <summary>
    /// Game 型退出清理（两段逆序编排）：宿主停机（护栏内含能力族停止：
    /// 停 admin → 停调度 → 逆序 Deactivate → 自动撤桥）→ 热更模块业务清理。
    /// </summary>
    /// <remarks>
    /// Game-shape exit cleanup (two reverse phases): the host stop (with the capability-family stop
    /// inside the guard: admin → scheduler → reverse Deactivate → automatic bridge retraction), then
    /// the hotfix module's business cleanup. C284 起监听所有权归宿主；C294 起 Online 编排并入
    /// 模板序列（<see cref="AppStartUpGameServerBase.ComposeCapabilitiesAsync"/>），本覆写只保留热更收尾。
    /// Since C284 listener ownership stays with the host; since C294 the Online orchestration lives in
    /// the template sequence, and this override only keeps the hotfix tail cleanup.
    /// </remarks>
    /// <param name="exitMessage">退出原因（可能为 null——启动失败路径未获得退出消息）/ The exit reason (may be null on the startup-failure path)</param>
    /// <returns>表示 hook 操作的异步任务 / A task representing the hook operation</returns>
    protected override async Task ShutdownAsync(string exitMessage)
    {
        await StopAsync(exitMessage ?? "");
        await GameFrameX.Core.Hotfix.HotfixRuntime.ProcessDefault.StopAsync(exitMessage);
    }


    /// <summary>
    /// 创建 Game 服务器的 KCP 会话首消息鉴权配置：放行登录流程消息，以角色登录为鉴权完成信号。
    /// </summary>
    /// <remarks>
    /// Creates the KCP session first-message authentication options for the game server.
    /// 白名单为现有 TCP 登录流全链路（账号登录 → 角色列表/角色创建 → 角色登录）；
    /// <see cref="ReqPlayerLogin"/> 是鉴权终态——业务侧 OnPlayerLogin 处理该消息并绑定 ActorId，此后会话消息全放行。
    /// C204 起 <see cref="ReqSessionResume"/>（会话恢复）与角色登录同为鉴权完成信号——恢复即绑定 ActorId。
    /// C284 起监听面归宿主，本覆写自热更侧 <c>AppStartUpHotfixGame</c> 上移，语句逐字保持
    ///（原名，C285 更名为 HotfixModuleLifecycleBridge）。
    /// Since C284 the listener surface lives with the host; this override moved up from the
    /// hotfix-side <c>AppStartUpHotfixGame</c> with the statements kept verbatim (renamed
    /// <c>HotfixModuleLifecycleBridge</c> in C285).
    /// </remarks>
    /// <returns>KCP 会话鉴权配置 / The KCP session authentication options</returns>
    protected override SessionAuthenticationOptions CreateKcpSessionAuthenticationOptions()
    {
        var options = base.CreateKcpSessionAuthenticationOptions();
        options.AllowedMessageIds.Add(GetLoginMessageId(typeof(ReqLogin)));
        options.AllowedMessageIds.Add(GetLoginMessageId(typeof(ReqPlayerList)));
        options.AllowedMessageIds.Add(GetLoginMessageId(typeof(ReqPlayerCreate)));
        options.AllowedMessageIds.Add(GetLoginMessageId(typeof(ReqPlayerLogin)));
        options.AllowedMessageIds.Add(GetLoginMessageId(typeof(ReqSessionResume)));
        options.AuthenticatedByMessageIds.Add(GetLoginMessageId(typeof(ReqPlayerLogin)));
        options.AuthenticatedByMessageIds.Add(GetLoginMessageId(typeof(ReqSessionResume)));
        return options;
    }

    /// <summary>
    /// 从消息类型的 <see cref="MessageTypeHandlerAttribute"/> 读取消息码（编译期固定值，不依赖运行时协议注册表初始化）。
    /// </summary>
    /// <param name="messageType">登录流程消息类型 / The login-flow message type</param>
    /// <returns>消息码 / The message id</returns>
    private static int GetLoginMessageId(Type messageType)
    {
        return messageType.GetCustomAttribute<MessageTypeHandlerAttribute>()?.MessageId
               // Localization: Hotfix.Startup.LoginMessageTypeMissingAttribute - 登录消息类型 {0} 缺少 {1}。
               ?? throw new InvalidOperationException(LocalizationService.GetString(Localization.Keys.Hotfix.LoginMessageTypeMissingAttribute, messageType.FullName, nameof(MessageTypeHandlerAttribute)));
    }

    /// <summary>
    /// Game 型会话建立钩子：达到最大在线数即拒绝并关闭，否则建立 <see cref="PlayerSession"/>
    ///（语句自热更侧 <c>AppStartUpHotfixGame.OnConnected</c> 上移，C284；原名，C285 更名为 HotfixModuleLifecycleBridge）。
    /// </summary>
    /// <remarks>
    /// The Game-shape session-established hook: rejects and closes the connection once the maximum
    /// client count is exceeded, otherwise registers the <see cref="PlayerSession"/> (moved up
    /// verbatim from the hotfix-side <c>AppStartUpHotfixGame.OnConnected</c>, C284; renamed
    /// <c>HotfixModuleLifecycleBridge</c> in C285).
    /// </remarks>
    /// <param name="appSession">新建立的会话对象 / Newly established session object</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected override async ValueTask OnConnectedAsync(IAppSession appSession)
    {
        // Localization: StartUp.Server.NewClientConnection - 新客户端连接 - 会话ID: {0}, 远程终端: {1}
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.Server.NewClientConnection, appSession.SessionId, appSession.RemoteEndPoint));
        var netChannel = new DefaultNetworkChannel(appSession, Setting);
        var count = PlayerSessionManager.Instance.Count();
        if (count > Setting.MaxClientCount)
        {
            // 达到最大在线人数限制
            await netChannel.WriteAsync(new NotifyServerFullyLoaded(), (int)OperationStatusCode.ServerFullyLoaded);
            netChannel.Close();
            return;
        }

        var session = new PlayerSession(appSession.SessionId, netChannel);
        PlayerSessionManager.Instance.Add(session);
    }

    /// <summary>
    /// Game 型会话断开钩子：摘除会话并对已绑定玩家分发 <see cref="PlayerDisconnectedEventArgs"/>
    ///——房间断线标记与会话恢复通知由热更侧全局监听器承接（C284 事件桥接契约，
    /// 与消息漏斗的热更注册表桥接同形态）。事件派发之后关闭该会话的全部通道实例：
    /// <see cref="IPlayerSession.WorkChannel"/>（连接钩子创建的工作通道，置于 if 块外——
    /// 未绑定玩家的会话同样释放）与消息漏斗的会话级缓存通道（经
    /// <see cref="AppStartUpBase.CloseSessionNetworkChannel"/>），正常断线不再依赖 GC 终结器
    /// 释放通道级关闭源（C350）。
    /// </summary>
    /// <remarks>
    /// The Game-shape session-disconnected hook: removes the session and, for a bound player,
    /// dispatches <see cref="PlayerDisconnectedEventArgs"/> — the hotfix-side global listener
    /// carries the room disconnected-marking and the session-resume notification (the C284
    /// event-bridging contract, mirroring the hotfix-registry bridging of the message funnel).
    /// The dispatch is fire-and-forget, the same shape as the <c>SessionRemovedEventArgs</c>
    /// logout cleanup dispatched inside <c>PlayerSessionManager.Remove</c>. After the dispatch,
    /// every channel instance of the session is closed: the <see cref="IPlayerSession.WorkChannel"/>
    /// created by the connect hook (outside the <c>if</c>, so sessions of unbound players are
    /// released too) and the funnel's session-scoped cache channel via
    /// <see cref="AppStartUpBase.CloseSessionNetworkChannel"/> — a normal disconnect no longer
    /// relies on the garbage collector to release the channel-level cancellation source (C350).
    /// </remarks>
    /// <param name="appSession">断开连接的会话对象 / Session object that disconnected</param>
    /// <param name="disconnectEventArgs">断开连接的相关参数 / Parameters related to disconnection</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected override ValueTask OnDisconnectedAsync(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        // Localization: Hotfix.StartUp.ClientDisconnected - 客户端断开连接 - 会话ID: {0}, 断开原因: {1}
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Hotfix.StartUpModule.ClientDisconnected, appSession.SessionId, disconnectEventArgs.Reason));
        var session = PlayerSessionManager.Instance.Remove(appSession.SessionId);
        if (session != null && session.PlayerId > 0)
        {
            // 房间断线标记 + 会话恢复通知（C204 两层协同）经事件桥接由热更侧监听器承接
            EventDispatcher.Dispatch(0, new PlayerDisconnectedEventArgs(session.PlayerId, appSession.SessionId));
        }

        // 先派发再关闭（监听器可能读取通道内会话数据）；置于 if 外确保未绑定玩家的会话通道同样释放
        session?.WorkChannel?.Close();
        CloseSessionNetworkChannel(appSession);
        return ValueTask.CompletedTask;
    }


    /// <summary>
    /// 创建 Game 服务器的缺省配置。
    /// </summary>
    /// <remarks>
    /// Creates the Game role-level default settings.
    /// 数据库连接地址优先从环境变量读取，避免在源码中硬编码凭证（消除 Sonar csharpsquid:S2068）。
    /// The database connection URL is read from the environment variable first to avoid
    /// hard-coded credentials in source (Sonar csharpsquid:S2068).
    /// </remarks>
    /// <returns>Game 缺省配置 / The Game default settings</returns>
    protected override AppSetting CreateDefaultSetting()
    {
        return new AppSetting
        {
            ServerId = GameServerConst.Game.Id,
            ServerType = GameServerConst.Game.Name,
            InnerPort = 29100,
            MetricsPort = 29090,
            HttpPort = 28080,
            WsPort = 29110,
            MinModuleId = 10,
            HttpIsDevelopment = true,
            MaxModuleId = 9999,
            TagName = "GameFrameX",
            // 数据库连接地址优先从环境变量读取，避免在源码中硬编码凭证（消除 Sonar csharpsquid:S2068）
            DataBaseUrl = Environment.GetEnvironmentVariable("GAMEFRAMEX_GAME_DB_URL") ?? "mongodb://127.0.0.1:27017/?authSource=admin",
            DataBaseName = "gameframex",
        };
    }
}
