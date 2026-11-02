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
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Localization;
using GameFrameX.Core.Components;

namespace GameFrameX.Launcher.Composition;

/// <summary>
/// 旧栈早段宿主模块（<see cref="CapabilityHostModulePhase.BeforeCapabilities"/>）：承载启动模板固定序列中
/// 先于能力族激活的部分——控制库初始化 → 发现层激活 → 业务库初始化 → 组件注册 → 热更模块装载。
/// </summary>
/// <remarks>
/// The early-phase legacy host module (<see cref="CapabilityHostModulePhase.BeforeCapabilities"/>): carries
/// the part of the startup template's fixed sequence that precedes capability activation — control-database
/// initialization, discovery activation, business-database initialization, component registration and the
/// hotfix module load. 启停顺序语义：早段必须先于全部能力模块激活，保证能力侧声明读取与桥挂载时
/// 数据库、发现层与热更注册表已就绪（等价旧模板序列「热更 → 能力 → 监听」的热更半段）；机制经相位
/// 合成定序边固化该先后关系，本模块自身不声明任何 <c>Requires</c>。
/// Order semantics: the early phase activates before every capability module so that databases, the
/// discovery layer and the hotfix registry are ready by the time capabilities read declarations and mount
/// bridges (equivalent to the hotfix half of the legacy "hotfix → capability → listen" sequence); the
/// mechanism pins this ordering through phase-synthesized edges and this module itself declares no
/// <c>Requires</c>. 停用语义：热更模块的业务清理不承载于本模块停用——单进程多 Role 形态下早段模块随
/// 每个 Role 的停机各自停用，若在此触发热更 teardown 会在其余 Role 仍在服务时提前拆除热更业务；
/// 热更停止保持由首个 Role 的退出尾部执行一次（进程最后停机者），生命周期归一由后续服务生命周期
/// 收敛阶段重构。
/// Deactivation semantics: the hotfix business cleanup is not carried here — in the single-process
/// multi-role shape this module deactivates once per role, and tearing the hotfix stack down here would
/// dismantle hotfix business while other roles still serve; the hotfix stop stays a one-shot tail of the
/// first role's exit (the process's last stopper), to be restructured by the later server-lifecycle
/// convergence stage.
/// </remarks>
internal sealed class LegacyRuntimeHostModule : ICapabilityModule
{
    /// <summary>
    /// 早段固定序列承载步骤（等价迁移自启动模板）：控制库 → 发现层 → 业务库 → 组件注册 → 热更装载。
    /// </summary>
    private readonly ILegacyRuntimeHostOwner _owner;

    /// <summary>
    /// 初始化 <see cref="LegacyRuntimeHostModule"/>。
    /// </summary>
    /// <param name="owner">宿主回调面（启动模板实例）。</param>
    public LegacyRuntimeHostModule(ILegacyRuntimeHostOwner owner)
    {
        _owner = owner ?? throw new ArgumentNullException(nameof(owner));
    }

    /// <summary>
    /// 获取能力静态清单（宿主基础设施模块：无参数、无依赖、无 admin action）。
    /// </summary>
    public CapabilityManifest Manifest { get; } = new CapabilityManifest("host-legacy-runtime", "1.0.0");

    /// <summary>
    /// 装配期注册（早段为纯序列步骤，无服务 / job / 事件订阅可注册）。
    /// </summary>
    /// <param name="context">注册上下文（不使用）。</param>
    public void Register(CapabilityRegistrationContext context)
    {
    }

    /// <summary>
    /// 激活（早段固定序列）：控制库初始化 → 发现层激活 → 业务库初始化 → 组件注册 → 热更模块装载。
    /// </summary>
    /// <param name="context">激活上下文（不使用）。</param>
    /// <param name="cancellationToken">取消令牌（序列步骤为模板固有流程，不响应取消）。</param>
    /// <returns>异步任务。</returns>
    public async Task ActivateAsync(CapabilityActivationContext context, CancellationToken cancellationToken)
    {
        await _owner.InitializeControlDatabaseAsync();
        await _owner.ActivateDiscoveryLayerAsync();
        await _owner.InitializeBusinessDatabaseAsync();
        LogHelper.DebugConsole(LocalizationService.GetString(Localization.Keys.Launcher.ComponentRegisterBegin));
        await ComponentRegister.InitAsync(typeof(AppsHandler).Assembly);
        LogHelper.DebugConsole(LocalizationService.GetString(Localization.Keys.Launcher.ComponentRegisterEnd));
        await _owner.LoadHotfixModuleAsync();
    }

    /// <summary>
    /// 停用（有意空实现）：热更业务清理不随本模块停用——多 Role 进程逐 Role 停用会提前 teardown 热更
    ///（现状为仅首个 Role 退出尾部停一次），保持不动交服务生命周期收敛阶段重构。
    /// </summary>
    /// <param name="cancellationToken">取消令牌（不使用）。</param>
    /// <returns>已完成的任务。</returns>
    public Task DeactivateAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
