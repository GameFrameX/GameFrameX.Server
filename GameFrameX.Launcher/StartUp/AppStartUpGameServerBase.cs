// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository:  https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:    https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:      https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using GameFrameX.Apps;
using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Hotfix;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.Discovery;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Utility;
using GameFrameX.Launcher.Composition;
using GameFrameX.Localization;
using GameFrameX.Network.Abstractions;
using GameFrameX.Network.HTTP;
using GameFrameX.Network.Message;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Launcher.StartUp;

/// <summary>
/// 游戏服务器启动模板：固化「启动日志 → Role 预备 → ActorLimit → 统一组合启动」共享启动序列与退出兜底；
/// 旧栈固定装配步骤（DB 初始化 → 发现层激活 → 组件注册 → 热更装载 → 网络监听 → ServiceOnline → 就绪屏障 →
/// 心跳 Active）与能力族激活统一经组合根（<see cref="RuntimeCompositionRoot"/> → CapabilityComposition）
/// 装配为早晚宿主模块 + 能力模块的同一运行时，由机制拓扑序定序（等价旧序列「热更 → 能力 → 监听」）；
/// 停机所有权统一到 <c>CapabilityRuntime.StopAsync</c>（晚段模块停用 = ServiceOffline + 网络停机与退出令牌置位，
/// 随后能力族逆序停用）。差异点全部经 hook 下放（<see cref="PrepareAsync"/> / <see cref="OnStartupCompletedAsync"/> /
/// <see cref="OnStoppingAsync(string)"/> / <see cref="ShutdownAsync(string)"/>）。
/// </summary>
/// <remarks>
/// The game-server startup template: freezes the shared startup sequence (start logging → role
/// preparation → ActorLimit → the unified composition start) together with the exit catch-all; the
/// legacy fixed assembly steps (database initialization → discovery activation → component registration
/// → hotfix loading → network listening → ServiceOnline → the ready barrier → the Active heartbeat) and
/// the capability-family activation are composed — through the composition root
/// (<see cref="RuntimeCompositionRoot"/> → CapabilityComposition) — into one runtime of early/late host
/// modules plus capability modules, ordered by the mechanism's topological order (equivalent to the
/// legacy "hotfix → capability → listen" sequence); the stop ownership unifies on
/// <c>CapabilityRuntime.StopAsync</c> (the late module's deactivation = ServiceOffline plus the network
/// shutdown and exit-token settlement, followed by the capability family's reverse-order teardown).
/// Every role difference rides hooks. 序列方法 sealed 是刻意选择：防止装配序列在派生类再分叉；自拟序列的
/// 异形 Role（无库网关、自拟装配顺序）直承 <see cref="AppStartUpBase"/> 自行编排启动序列——护栏、缺省配置
/// 收口与装配件届时按需重新抽取（机械操作，非单向门）。
/// Sealing the sequence is deliberate — it prevents the assembly sequence from forking again in derived
/// classes; free-form roles derive <see cref="AppStartUpBase"/> directly and orchestrate their own
/// startup, re-extracting the guard, the funnel and the wiring parts on demand. 单进程多 Role
///（--AllInOne / 复数 --ServerType）与多进程单 Role 两形态兼容：共享装配步骤幂等（控制库/业务库 Contains
/// 守卫、发现层 Activate 一次性守卫、默认库 set-once、热更装载同路径幂等守卫），
/// <see cref="AppStartUpBase.MarkStartUpReady"/> 屏障保持在各 Role 的全就绪之后（优先级链启动语义不变）；
/// 能力族同族进程内单例（首个命中 Role 携带族模块组合，其余命中 Role 只装配宿主模块——族停止随组合者
/// Role 的运行时停机，逆序停机下组合者最后停）。
/// Compatible with both the single-process multi-role shape and the multi-process single-role shape:
/// the shared wiring steps stay idempotent, the startup-ready barrier is still set only after each
/// role is fully up, and the capability family composes once per process (the first named role carries
/// the family modules; the family stops with the composer role's runtime, which stops last under the
/// reverse-order shutdown).
/// </remarks>
internal abstract class AppStartUpGameServerBase : AppStartUpBase, ILegacyRuntimeHostOwner
{
    /// <summary>
    /// 进程级组合根协调器锚点（族互斥闸与组合者登记的进程共享载体；实例状态全部在协调器实例内，
    /// 锚点仅是进程作用域引用——组合根唯一红线由 GFX 分析器在静态类层面固化）。
    /// </summary>
    /// <remarks>
    /// The process-level composition-root coordinator anchor (the process-shared carrier of the family
    /// gate and the composer registration; all mutable state lives on the coordinator instance — the
    /// anchor is merely a process-scoped reference, and the single-composition-root red line is pinned
    /// by the GFX analyzer at the static-class level).
    /// </remarks>
    private static readonly RuntimeCompositionRoot ProcessCompositionRoot = new RuntimeCompositionRoot();

    /// <summary>
    /// 本 Role 的组合产物（宿主实例字段——迁移自退役静态根的 <c>_runtime</c>；停机所有权所在）。
    /// </summary>
    private Capability.Hosting.Core.CapabilityRuntime _composedRuntime;

    /// <summary>
    /// 本次停机原因（停机护栏内置位；晚段模块停用序列经 <see cref="CurrentStopMessage"/> 回读）。
    /// </summary>
    private string _stopMessage;

    /// <summary>
    /// 启动序列完成标志（就绪屏障放行时置位；未置位时的停机走启动失败补齐路径）。
    /// </summary>
    private volatile bool _startupSequenceCompleted;

    /// <summary>
    /// 启动游戏服务器（sealed 模板序列）：启动日志 → Role 预备 → ActorLimit → 统一组合启动
    ///（早段宿主模块：控制库 → 发现层 → 业务库 → 组件注册 → 热更装载；能力模块：拓扑序激活；
    /// 晚段宿主模块：网络监听 → KCP 发布 → ServiceOnline → 就绪屏障 → 心跳 Active）→
    /// 运行至退出令牌；任何一步失败落入统一兜底（异常日志 + Fatal），随后退出清理 hook 收尾。
    /// </summary>
    /// <remarks>
    /// Starts the game server (the sealed template sequence): start logging → role preparation →
    /// ActorLimit → the unified composition start (the early host module: control database → discovery →
    /// business database → component registration → hotfix load; the capability modules activate in
    /// topological order; the late host module: network listening → kcp publish → ServiceOnline → the
    /// ready barrier → the Active heartbeat) → run until the exit token; any failure falls into the
    /// unified catch-all (exception log + Fatal) and the exit-cleanup hook closes the run.
    /// </remarks>
    /// <returns>表示异步启动操作的任务（运行至退出后完成）/ A task that completes after the run-until-exit cycle</returns>
    public sealed override async Task StartAsync()
    {
        string exitMessage = null;
        try
        {
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.Launcher.ServerStartBegin, Setting.ServerType));
            await PrepareAsync();

            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Launcher.ActorLimitConfigBegin));
            // 规则改由配置驱动：一经配置即生效（生产可用），默认 None 行为与历史等价
            if (!Enum.TryParse<ActorLimit.RuleType>(Setting.ActorLimitRule, true, out var actorLimitRule))
            {
                // Localization: Core.ActorLimit.UnsupportedRuleType - 不支持的rule类型:{0}
                LogHelper.Error(LocalizationService.GetString(Localization.Keys.Core.ActorLimit.UnsupportedRuleType, Setting.ActorLimitRule));
                actorLimitRule = ActorLimit.RuleType.None;
            }

            ActorLimit.Init(actorLimitRule);
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Launcher.ActorLimitConfigEnd));

            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Launcher.DatabaseServiceStartBegin));
            // 旧栈固定序列与能力族统一经组合根装配并启动（早段模块含 DB 三步与热更装载；晚段模块含监听与收尾）。
            _composedRuntime = await ProcessCompositionRoot.ComposeAndStartAsync(this);
            LogHelper.DebugConsole(LocalizationService.GetString(Localization.Keys.Launcher.DatabaseServiceStartEnd));

            exitMessage = await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.Launcher.ServerExecutionException, e));
            LogHelper.Fatal(e);
        }

        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Launcher.ServerExitBegin));
        await ShutdownAsync(exitMessage);
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Launcher.ServerExitSuccess));
    }

    /// <summary>
    /// Role 专属预备 hook：标准序列最前段（启动日志之后、ActorLimit 与一切装配之前）执行，默认空实现。
    /// </summary>
    /// <remarks>
    /// The role-specific preparation hook at the very front of the standard sequence (after the
    /// start log, before ActorLimit and any wiring); the default is a no-op. 只放纯 Role 专属的
    /// 前置动作；进程级职责（如热更目录确保）不下放 hook，由对应组件自身兜底。
    /// Only genuinely role-specific preparation belongs here; process-level concerns (such as
    /// ensuring the hotfix directory) are not hooks — the owning component covers them itself.
    /// </remarks>
    /// <returns>表示 hook 操作的异步任务 / A task representing the hook operation</returns>
    protected virtual Task PrepareAsync()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 启动完成 hook（缺省 = 统一形态，全 Role 生效）：网络监听就绪后分发 <c>ServiceOnline</c> 事件
    ///（唯一分发点——热更侧不分发）。
    /// </summary>
    /// <remarks>
    /// The startup-completed hook (default = the unified shape, effective for every role): dispatches the
    /// <c>ServiceOnline</c> event once the network listeners are up — the single dispatch point (the
    /// hotfix side does not dispatch it).
    /// </remarks>
    /// <returns>表示 hook 操作的异步任务 / A task representing the hook operation</returns>
    protected virtual Task OnStartupCompletedAsync()
    {
        GameFrameX.Core.Events.GameEventDispatcher.ProcessDefault.Dispatch(0, new ServiceOnlineEventArgs(Setting.ServerType, Setting.ServerInstanceId, DateTime.UtcNow));
        return Task.CompletedTask;
    }

    /// <summary>
    /// 停机幂等护栏标志位（0 = 未停机，1 = 已进入停机）。
    /// </summary>
    /// <remarks>
    /// The idempotent stop guard flag (0 = running, 1 = stopping/stopped).
    /// </remarks>
    private int _stopped;

    /// <summary>
    /// 停止 Role：幂等护栏保证首个调用生效——停机所有权统一到 <c>CapabilityRuntime.StopAsync</c>：
    /// admin-only 族运行时（如为本 Role 组合）先停，随后本 Role 组合产物停机（机制逆序驱动：
    /// 晚段模块停用 = ServiceOffline + 网络停机与退出令牌置位 → 能力族逆序停用 → 早段模块停用）；
    /// 启动未达监听段的失败路径补齐旧停机尾部（ServiceOffline + 基类停机流程）保持退出令牌语义。
    /// </summary>
    /// <remarks>
    /// Stops the role behind an idempotent guard: the stop ownership unifies on
    /// <c>CapabilityRuntime.StopAsync</c> — the admin-only family runtime (when composed by this role)
    /// stops first, then this role's composed runtime (mechanism-driven reverse order: the late module's
    /// deactivation = ServiceOffline plus the network shutdown and exit-token settlement → the capability
    /// family's reverse teardown → the early module); on the startup-failure path (never reaching the
    /// listener phase) the legacy stop tail is appended (ServiceOffline plus the base stop flow) to keep
    /// the exit-token semantics. 双停机路径竞争是单进程多 Role 形态的固有现象
    ///（<see cref="AppEnter"/> 逆序停机与 Role 自停并发进入），护栏把两条路径收敛为恰好一次。
    /// The dual-path stop race is inherent to the single-process multi-role shape; the guard
    /// collapses both paths into exactly one stop.
    /// </remarks>
    /// <param name="message">终止原因 / Termination reason</param>
    /// <returns>表示异步停止操作的任务 / A task representing the asynchronous stop operation</returns>
    public sealed override async Task StopAsync(string message = "")
    {
        if (Interlocked.Exchange(ref _stopped, 1) != 0)
        {
            return;
        }

        _stopMessage = message;

        // admin-only 族运行时（仅管理面）先停（等价旧序 admin-first）；族随组合运行时停机的形态不经过这里。
        var adminOnlyFamilyRuntime = ProcessCompositionRoot.DetachAdminOnlyFamilyRuntime(Setting.ServerType);
        if (adminOnlyFamilyRuntime != null)
        {
            await adminOnlyFamilyRuntime.StopAsync();
        }

        var runtime = _composedRuntime;
        if (runtime != null)
        {
            _composedRuntime = null;
            await runtime.StopAsync();
        }

        if (!_startupSequenceCompleted)
        {
            // 启动未达监听段（晚段模块从未激活，其停用序列不会执行）：补齐旧停机尾部——
            // ServiceOffline 分发与基类停机流程（含退出令牌置位），与既有失败路径行为一致。
            await OnStoppingAsync(message);
            await base.StopAsync(message);
        }
    }

    /// <summary>
    /// 停机钩子：护栏放行后回调恰好一次，缺省 = <c>ServiceOffline</c> 事件分发（唯一分发点——热更侧不分发）；
    /// 统一形态下由晚段宿主模块的停用序列调用（先于网络停机）。
    /// </summary>
    /// <remarks>
    /// The stopping hook invoked exactly once after the guard admits the first stop; the default
    /// dispatches the <c>ServiceOffline</c> event — the single dispatch point (the hotfix side does not
    /// dispatch it); in the unified shape it is invoked by the late host module's deactivation sequence
    /// (before the network shutdown).
    /// </remarks>
    /// <param name="message">终止原因（首个 <see cref="StopAsync(string)"/> 调用透传）/ The termination reason passed through by the first stop call</param>
    /// <returns>表示钩子操作的异步任务 / A task representing the hook operation</returns>
    protected virtual Task OnStoppingAsync(string message)
    {
        GameFrameX.Core.Events.GameEventDispatcher.ProcessDefault.Dispatch(0, new ServiceOfflineEventArgs(Setting.ServerType, Setting.ServerInstanceId, "Stopped", DateTime.UtcNow));
        return Task.CompletedTask;
    }

    /// <summary>
    /// 退出清理 hook（缺省 = 统一形态）：自停——护栏内的 <see cref="StopAsync(string)"/>，
    /// 宿主逆序停机先到时本调用被护栏吸收，<c>ServiceOffline</c> 仍恰好一次。
    /// Game 型覆写为「宿主停机（护栏内含统一运行时停机）→ 热更模块业务清理」两段编排。
    /// </summary>
    /// <remarks>
    /// The exit-cleanup hook (default = the unified shape): self-stop through the guarded
    /// <see cref="StopAsync(string)"/> — when the host's reverse-order stop wins the race, the guard
    /// absorbs this call and <c>ServiceOffline</c> still fires exactly once. The Game shape overrides it
    /// with the two-phase orchestration: the host stop (with the unified-runtime stop inside the guard)
    /// then the hotfix module's business cleanup.
    /// </remarks>
    /// <param name="exitMessage">退出原因（可能为 null——启动失败路径未获得退出消息）/ The exit reason (may be null on the startup-failure path)</param>
    /// <returns>表示 hook 操作的异步任务 / A task representing the hook operation</returns>
    protected virtual Task ShutdownAsync(string exitMessage)
    {
        return StopAsync(exitMessage ?? "");
    }

    /// <summary>
    /// 初始化应用程序：未注入配置时回落到 <see cref="CreateDefaultSetting"/> 的 Role 级缺省配置。
    /// </summary>
    /// <remarks>
    /// Initializes the application: falls back to the role-level defaults from
    /// <see cref="CreateDefaultSetting"/> when no setting was injected (the launcher-options
    /// copy and the config-file section both count as injected settings).
    /// </remarks>
    protected override void Init()
    {
        Setting ??= CreateDefaultSetting();
        base.Init();
    }

    /// <summary>
    /// 创建该 Role 的缺省配置（类型/端口/数据库等因 Role 而异的全部缺省值）。
    /// </summary>
    /// <remarks>
    /// Creates the role-level default settings (every per-role default: server type, port,
    /// database wiring and debug switches). 具体 Role 只需实现本成员与
    /// <see cref="StartUpTagAttribute"/> 声明。
    /// Concrete roles implement only this member plus their
    /// <see cref="StartUpTagAttribute"/> declaration.
    /// </remarks>
    /// <returns>该 Role 的缺省配置 / The role-level default settings</returns>
    protected abstract AppSetting CreateDefaultSetting();

    /// <summary>
    /// 获取当前 Role 的应用配置（宿主回调面）。
    /// </summary>
    AppSetting ILegacyRuntimeHostOwner.Setting
    {
        get { return Setting; }
    }

    /// <summary>
    /// 获取本次停机原因（宿主回调面；停机护栏内置位）。
    /// </summary>
    string ILegacyRuntimeHostOwner.CurrentStopMessage
    {
        get { return _stopMessage; }
    }

    /// <summary>
    /// 装载热更模块（宿主回调面：经 <see cref="HotfixRuntime"/> 注入面；同路径幂等守卫在实例核内，
    /// --AllInOne 多 Role 的重复装载短路返回）。
    /// </summary>
    /// <returns>表示装载操作的异步任务。</returns>
    Task ILegacyRuntimeHostOwner.LoadHotfixModuleAsync()
    {
        return HotfixRuntime.ProcessDefault.LoadHotfixModuleAsync(Setting);
    }

    /// <summary>
    /// 初始化控制库（宿主回调面：桥接虚方法钩子，Role 覆写继续生效）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ILegacyRuntimeHostOwner.InitializeControlDatabaseAsync()
    {
        return InitializeControlDatabaseAsync();
    }

    /// <summary>
    /// 激活发现层（宿主回调面：桥接虚方法钩子，Role 覆写继续生效）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ILegacyRuntimeHostOwner.ActivateDiscoveryLayerAsync()
    {
        return ActivateDiscoveryLayerAsync();
    }

    /// <summary>
    /// 初始化业务库（宿主回调面：桥接虚方法钩子，Role 覆写继续生效）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ILegacyRuntimeHostOwner.InitializeBusinessDatabaseAsync()
    {
        return InitializeBusinessDatabaseAsync();
    }

    /// <summary>
    /// 初始化控制库（宿主回调面：<c>Contains</c> 判重护栏 + <c>IsDefault = false</c> 显式声明；
    /// 经 <see cref="GameFrameX.DataBase.GameDatabase"/> 注入面）。
    /// 控制库先行于业务库（D-Single 缺省回落：与业务库共用同一实例连接串），
    /// 经统一入口判重/命名；实现类型按 DbOptions.Provider 约定解析
    ///（PG 路径下 DbOptions.Name 仅作注册名，数据库由连接串 Database 决定）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    protected virtual async Task InitializeControlDatabaseAsync()
    {
        var gameDatabase = GameFrameX.DataBase.GameDatabase.ProcessDefault;
        // Contains 判重护栏：多 Role 进程（--AllInOne）下首个 Role 注册控制库，后续 Role 直接复用
        if (!gameDatabase.Contains(GameFrameX.DataBase.GameDatabase.ControlDatabaseName))
        {
            var controlDatabaseInitResult = await gameDatabase.InitAsync(new DbOptions { Provider = Setting.DatabaseProvider, ConnectionString = Setting.DataBaseUrl, Name = GameFrameX.DataBase.GameDatabase.ControlDatabaseName, IsDefault = false, IsUseTimeZone = Setting.IsUseTimeZone, });
            if (controlDatabaseInitResult == false)
            {
                throw new InvalidOperationException(LocalizationService.GetString(Localization.Keys.Launcher.DatabaseServiceStartFailed));
            }
        }
    }

    /// <summary>
    /// 控制库就绪后激活发现层——读侧 watcher + 写侧心跳（未配置广播端口时自动跳过）
    /// 并以真实 case 2/3 转发器重装跨 Role 路由缝。幂等：多 Role 进程首个调用生效。
    /// 再激活玩家路由层（建 player_route 索引 + 装 SyncTarget），Tier 1 fast-path 注入 SessionManager 适配器。
    /// 控制库句柄解析下沉到发现层内部，收敛为激活参数对象。
    /// Provider 装配经 DiscoveryRuntimeResolver 约定解析——按 DiscoveryActivationOptions.Provider
    /// 反射定位并激活目标 Runtime，组合根零分支；激活入口为异步方法，本方法全程 await，
    /// 带 SynchronizationContext 的宿主上启动线程不被装配期数据库 I/O 阻塞。
    /// 路由胶水装配自 Runtime 拆至组合侧 DiscoveryRoutingWire（发现层 Runtime 不再引用消息胶水程序集），
    /// 路由表经 ActiveDiscoveryRuntime.RouteTableProvider 槽位无分派回读。
    /// </summary>
    /// <remarks>
    /// Activates the discovery layer once the control database is ready — the read-side watcher
    /// plus the write-side heartbeat (skipped automatically when no broadcast port is configured)
    /// and the cross-role routing seam re-wired with the real case 2/3 forwarders. Idempotent: in
    /// a multi-role process the first call wins. It then activates the player-routing layer
    /// (creates the player_route index and installs the sync target) and injects the Tier 1
    /// fast-path adapter into the session manager. The provider dispatch lives in the
    /// <c>DiscoveryRuntimeResolver</c> convention so this composition surface holds no provider
    /// branch; the routing glue assembly lives in the composition-side <c>DiscoveryRoutingWire</c>,
    /// its route table read back branch-free through the <c>ActiveDiscoveryRuntime.RouteTableProvider</c> slot.
    /// </remarks>
    protected virtual async Task ActivateDiscoveryLayerAsync()
    {
        // 约定激活：Provider 元数据随 options 走，目标 Runtime 由 DiscoveryRuntimeResolver 反射定位（镜像 GameDb.Init 的 DbOptions.Provider）。
        await DiscoveryRuntimeResolver.ActivateAsync(new DiscoveryActivationOptions
        {
            Provider = Setting.DatabaseProvider,
            ConnectionName = GameFrameX.DataBase.GameDatabase.ControlDatabaseName,
            HostedRoleNames = RoleSet.Current,
            PlayerRouteFastPath = GameFrameX.Core.Session.PlayerSessionFastPathAdapter.Instance,
        });

        // 玩家路由 SyncTarget 与 Provider 无关（ResolverBootstrap 单例），激活后统一装配。
        GameFrameX.Core.Session.PlayerSessionManager.Instance.PlayerRouteSyncTarget = GameFrameX.Discovery.Routing.PlayerRouteResolverBootstrap.SyncTarget;

        // 本服发现层实例 Id 注入（与心跳自描述符同源）：PlayerSessionManager 的路由同步写真实实例 Id，
        // 跨服推送按 player_route.InstanceId 实例直达。仅观察形态（未配置广播端口）为 null——路由同步跳过。
        GameFrameX.Core.Session.PlayerSessionManager.Instance.LocalDiscoveryInstanceId = GameFrameX.Discovery.Routing.PlayerRouteResolverBootstrap.LocalInstanceId;
        // 路由胶水装配自 Runtime 拆至组合侧 DiscoveryRoutingWire（发现层 Runtime 不再引用消息胶水程序集）；
        // 路由表经公共槽位无分派回读，未激活时 null 触发 Initialize 的 ArgumentNullException fail-fast（既有语义）。
        GameFrameX.Network.RemoteMessaging.Routing.DiscoveryRoutingWire.Initialize(RoleSet.Current, ActiveDiscoveryRuntime.RouteTableProvider);
    }

    /// <summary>
    /// 业务库 Init（宿主回调面；经 <see cref="GameFrameX.DataBase.GameDatabase"/> 注入面），实现类型按
    /// DbOptions.Provider 约定解析；缺省 IsDefault = true——注册成功即声明式提名门面默认库，
    /// 全部业务读写必须落在业务库（控制库已显式 IsDefault = false，先注册不会抢占门面）；失败抛
    /// <see cref="InvalidOperationException"/>。同名判重护栏：多 Role 进程共享同名业务库时首个注册生效，
    /// 与控制库 <c>Contains</c> 护卫同一多 Role 语义。
    /// </summary>
    /// <remarks>
    /// Initializes the business database through the instance facade with the provider resolved by
    /// convention; <c>IsDefault = true</c> by default means a successful registration declaratively
    /// nominates the facade's default database, and a failure throws
    /// <see cref="InvalidOperationException"/>. The same-name guard makes the first registration win
    /// when roles of a multi-role process share one business database.
    /// </remarks>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected virtual async Task InitializeBusinessDatabaseAsync()
    {
        var gameDatabase = GameFrameX.DataBase.GameDatabase.ProcessDefault;
        if (!gameDatabase.Contains(Setting.DataBaseName))
        {
            var initResult = await gameDatabase.InitAsync(new DbOptions { Provider = Setting.DatabaseProvider, ConnectionString = Setting.DataBaseUrl, Name = Setting.DataBaseName, IsUseTimeZone = Setting.IsUseTimeZone, });
            if (initResult == false)
            {
                throw new InvalidOperationException(LocalizationService.GetString(Localization.Keys.Launcher.DatabaseServiceStartFailed));
            }
        }
    }

    /// <summary>
    /// 启动网络监听（宿主回调面：晚段宿主模块激活序列调用；消息解码/编码/压缩与热更 HTTP 处理器桥接在此装配）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ILegacyRuntimeHostOwner.StartNetworkListenersAsync()
    {
        var hotfixRuntime = HotfixRuntime.ProcessDefault;
        return StartServerAsync<DefaultMessageDecoderHandler, DefaultMessageEncoderHandler>(new DefaultMessageCompressHandler(), new DefaultMessageDecompressHandler(), hotfixRuntime.GetListHttpHandler(), hotfixRuntime.GetHttpHandler);
    }

    /// <summary>
    /// 启动完成钩子回调（宿主回调面：晚段宿主模块激活序列调用，虚 hook 语义保留）。
    /// </summary>
    /// <returns>表示钩子操作的异步任务。</returns>
    Task ILegacyRuntimeHostOwner.OnStartupCompletedAsync()
    {
        return OnStartupCompletedAsync();
    }

    /// <summary>
    /// 放行启动就绪屏障并标记启动序列完成（宿主回调面：晚段宿主模块激活序列尾部调用）。
    /// </summary>
    void ILegacyRuntimeHostOwner.MarkStartUpReadyBarrier()
    {
        _startupSequenceCompleted = true;
        MarkStartUpReady();
    }

    /// <summary>
    /// 心跳切 Active（宿主回调面：晚段宿主模块激活序列最后一步；未激活发现层或无广播身份时无害 no-op）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ILegacyRuntimeHostOwner.MarkDiscoveryActiveAsync()
    {
        return ActiveDiscoveryRuntime.MarkActiveAsync();
    }

    /// <summary>
    /// 获取是否应发布 KCP 附加端点（宿主回调面）。
    /// </summary>
    bool ILegacyRuntimeHostOwner.ShouldPublishInnerKcpEndpoint
    {
        get { return Setting.IsEnableInnerKcp && IsInnerKcpListenerConfigured; }
    }

    /// <summary>
    /// 发布 KCP 附加端点（宿主回调面：屏障前发布，Booting 实例不进路由视图）。
    /// </summary>
    void ILegacyRuntimeHostOwner.PublishInnerKcpAdvertiseEndpoint()
    {
        GameFrameX.Discovery.ActiveDiscoveryRuntime.PublishActiveAdvertiseEndpoints(Setting.ServerType, new[] { KeyValuePair.Create("kcp", Setting.InnerKcpPort) });
    }

    /// <summary>
    /// 停机钩子回调（宿主回调面：晚段宿主模块停用序列调用，虚 hook 语义保留）。
    /// </summary>
    /// <param name="message">终止原因。</param>
    /// <returns>表示钩子操作的异步任务。</returns>
    Task ILegacyRuntimeHostOwner.OnStoppingAsync(string message)
    {
        return OnStoppingAsync(message);
    }

    /// <summary>
    /// 基类停机流程（宿主回调面：晚段宿主模块停用序列收尾——停机日志 + 网络停机 + 运行时状态与退出令牌置位）。
    /// </summary>
    /// <param name="message">终止原因。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task ILegacyRuntimeHostOwner.StopNetworkAndMarkStoppedAsync(string message)
    {
        return base.StopAsync(message);
    }

    /// <summary>
    /// 消息处理器解析（统一形态，全 Role 生效）：接热更注册表按消息 ID 解析处理器（经 HotfixRuntime 注入面）。
    /// </summary>
    /// <remarks>
    /// The resolution hook of the base package funnel (the unified shape, effective for every role):
    /// bridges the hotfix message-handler registry. Messages without a registered handler return
    /// <c>null</c> and hit the funnel's "handler not found" error log.
    /// </remarks>
    /// <param name="messageId">消息ID / The message identifier</param>
    /// <returns>处理器实例；未注册时为 <c>null</c> / The handler instance, or <c>null</c> when unregistered</returns>
    protected override IMessageHandler ResolveMessageHandler(int messageId)
    {
        return HotfixRuntime.ProcessDefault.GetMessageHandler(messageId);
    }

    /// <summary>
    /// 回复心跳消息（统一形态，全 Role 生效）：回显请求 UniqueId 的池化 NotifyHeartBeat 保活回包。
    /// </summary>
    /// <remarks>
    /// The keepalive reply (the unified shape, effective for every role): echoes the request's
    /// UniqueId in a pooled <c>NotifyHeartBeat</c> carrying the server timestamp. The receive-time
    /// stamping happens in the base funnel before this hook.
    /// </remarks>
    /// <param name="networkChannel">会话网络通道 / The session network channel</param>
    /// <param name="messagePackage">心跳消息包 / The heartbeat message package</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    protected override async ValueTask ReplyHeartBeatAsync(INetworkChannel networkChannel, INetworkMessagePackage messagePackage)
    {
        if (messagePackage.DeserializeMessageObject() is ReqHeartBeat request)
        {
            var notifyHeartBeat = MessageObjectPoolHelper.Get<NotifyHeartBeat>();
            try
            {
                notifyHeartBeat.Timestamp = TimerHelper.UnixTimeMilliseconds();
                notifyHeartBeat.UniqueId = request.UniqueId;
                await networkChannel.WriteAsync(notifyHeartBeat);
            }
            finally
            {
                // 池化消息用毕归还（WriteAsync 在首个 await 前完成编码消费，归还安全）
                MessageObjectPoolHelper.Return(notifyHeartBeat);
            }
        }
    }
}
