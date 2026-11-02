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

using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Localization;

namespace GameFrameX.Launcher.Composition;

/// <summary>
/// 旧栈晚段宿主模块（<see cref="CapabilityHostModulePhase.AfterCapabilities"/>）：承载启动模板固定序列中
/// 晚于能力族激活的部分——网络监听启动 → KCP 附加端点发布 → <c>ServiceOnline</c> 分发 →
/// 启动就绪屏障放行 → 心跳 <c>Active</c>；停用（激活逆序中最先执行）承载
/// <c>ServiceOffline</c> 分发与基类停机流程（网络停机 + 退出令牌置位）。
/// </summary>
/// <remarks>
/// The late-phase legacy host module (<see cref="CapabilityHostModulePhase.AfterCapabilities"/>): carries
/// the part of the startup template's fixed sequence that follows capability activation — the network
/// listener startup, the kcp additional-endpoint publish, the <c>ServiceOnline</c> dispatch, the
/// startup-ready barrier release and the <c>Active</c> heartbeat; its deactivation (first in the reverse
/// order) carries the <c>ServiceOffline</c> dispatch and the base stop flow (network shutdown plus the
/// exit-token settlement). 启停顺序语义：晚段必须晚于全部能力模块激活——监听开放时能力桥已发布、
/// 热更侧钩子已挂载（等价旧模板序列「热更 → 能力 → 监听」的监听半段）；停用逆序使网络先于能力族
/// teardown 关闭，消除了旧停机序中「监听仍开放而能力已拆」的窗口（有意改进，语义位移登记于所属 change）。
/// Order semantics: the late phase activates after every capability module — bridges are published and
/// hotfix-side hooks are mounted by the time listening opens (the listener half of the legacy
/// "hotfix → capability → listen" sequence); the reverse deactivation closes the network before the
/// capability teardown, removing the old stop-order window where listeners stayed open while capabilities
/// were already dismantled (a deliberate improvement registered in the owning change).
/// </remarks>
internal sealed class LegacyNetworkListenerHostModule : ICapabilityModule
{
    /// <summary>
    /// 晚段固定序列承载步骤（等价迁移自启动模板）：监听 → KCP 发布 → ServiceOnline → 就绪屏障 → Active。
    /// </summary>
    private readonly ILegacyRuntimeHostOwner _owner;

    /// <summary>
    /// 初始化 <see cref="LegacyNetworkListenerHostModule"/>。
    /// </summary>
    /// <param name="owner">宿主回调面（启动模板实例）。</param>
    public LegacyNetworkListenerHostModule(ILegacyRuntimeHostOwner owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    /// <summary>
    /// 获取能力静态清单（宿主基础设施模块：无参数、无依赖、无 admin action）。
    /// </summary>
    public CapabilityManifest Manifest { get; } = new CapabilityManifest("host-legacy-listener", "1.0.0");

    /// <summary>
    /// 装配期注册（晚段为纯序列步骤，无服务 / job / 事件订阅可注册）。
    /// </summary>
    /// <param name="context">注册上下文（不使用）。</param>
    public void Register(CapabilityRegistrationContext context)
    {
    }

    /// <summary>
    /// 激活（晚段固定序列）：网络监听启动 → KCP 附加端点发布（屏障前发布，Booting 实例不进路由视图）→
    /// <c>ServiceOnline</c> 分发 → 启动完成日志与就绪屏障放行 → 心跳 <c>Active</c>。
    /// </summary>
    /// <param name="context">激活上下文（不使用）。</param>
    /// <param name="cancellationToken">取消令牌（序列步骤为模板固有流程，不响应取消）。</param>
    /// <returns>异步任务。</returns>
    public async Task ActivateAsync(CapabilityActivationContext context, CancellationToken cancellationToken)
    {
        await _owner.StartNetworkListenersAsync();

        // KCP 附加端点发布：在监听器装配完成后、就绪屏障与心跳 Active 之前执行；屏障前发布是刻意的——
        // Role 按就绪屏障串行启动，屏障之后本 Role 的实例级闸门读数可能被同进程后续 Role 的装配污染；
        // Booting 实例不进任何路由视图，广播前置不会提前暴露端点。
        if (_owner.ShouldPublishInnerKcpEndpoint)
        {
            _owner.PublishInnerKcpAdvertiseEndpoint();
        }

        await _owner.OnStartupCompletedAsync();

        LogHelper.DebugConsole(LocalizationService.GetString(Localization.Keys.Launcher.EnterMainLoop));
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Launcher.ServerStartEnd, _owner.Setting.ServerType));

        // 启动阶段完成（DB/组件/Role 服务均已就绪），放行下一个 Role 的启动屏障
        _owner.MarkStartUpReadyBarrier();

        // 启动阶段真正完成后才把心跳从 Booting 切到 Active，避免其他进程在服务就绪前发现本实例并投递流量；
        // 未激活发现层或无广播身份时为无害 no-op。
        await _owner.MarkDiscoveryActiveAsync();
    }

    /// <summary>
    /// 停用（逆序中最先执行）：<c>ServiceOffline</c> 分发（先于网络关闭——消费方收到显式下线信号后网络才断）→
    /// 基类停机流程（停机日志 + 网络停机 + 运行时状态与退出令牌置位）。
    /// </summary>
    /// <param name="cancellationToken">取消令牌（不响应取消——停机收尾必须完成）。</param>
    /// <returns>异步任务。</returns>
    public async Task DeactivateAsync(CancellationToken cancellationToken)
    {
        var message = _owner.CurrentStopMessage ?? string.Empty;
        await _owner.OnStoppingAsync(message);
        await _owner.StopNetworkAndMarkStoppedAsync(message);
    }
}
