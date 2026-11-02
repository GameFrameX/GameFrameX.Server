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

using System.Text.Json;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Hosting.AdminApi;
using GameFrameX.Capability.Hosting.Core;
using GameFrameX.Capability.Online.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Assets;
using GameFrameX.Capability.Online.Foundation.Identity;
using GameFrameX.Capability.Online.Foundation.Storage;
using GameFrameX.Capability.Online.Operations.HotfixRollback;
using GameFrameX.Capability.Online.Persistence.Assets;
using GameFrameX.Capability.Online.Persistence.Identity;
using GameFrameX.Capability.Online.Persistence.Social;
using GameFrameX.Capability.Online.Persistence.Storage;
using GameFrameX.Capability.Online.Match;
using GameFrameX.Capability.Online.Persistence.Match;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Logger;
using GameFrameX.Launcher.Online;
using GameFrameX.Localization;
using GameFrameX.Capability.Online.Social;

namespace GameFrameX.Launcher.Composition;

/// <summary>
/// 运行时组合根协调器（实例形态，取代退役的静态 <c>OnlineCapabilityCompositionRoot</c>）：
/// 为每个 Role 经 <see cref="CapabilityComposition"/> 组装「早晚宿主模块 + （启用时）online 能力族」的统一运行时，
/// 并承载进程级族互斥闸与组合者登记（原静态字段迁实例）。
/// </summary>
/// <remarks>
/// The runtime composition-root coordinator (an instance shape, replacing the retired static
/// <c>OnlineCapabilityCompositionRoot</c>): composes, per role and through <see cref="CapabilityComposition"/>,
/// the unified runtime of "early/late host modules + (when enabled) the online capability family", and
/// carries the process-level family mutual-exclusion gate and the composer registration (the former
/// static fields migrated onto this instance). 维护约束（红线）：
/// ① 组合根唯一——一切装配经 <see cref="CapabilityComposition"/>（机制内 <c>CapabilityHostBuilder</c> 定序），
/// 本类只做族判定、Options 组装与宿主模块声明，不自带启停定序；
/// ② 同族进程内单例——多 Role 进程首个 <c>EnableRoles</c> 命中的 Role 携带族模块组合，其余命中 Role
/// 只装配宿主模块（互斥闸），族的停止随组合者 Role 的运行时停机（逆序停机下组合者最后停）；
/// ③ 双闸全关 / 配置段缺席 / Role 未命中时走无族组合（宿主模块 only，零族装配零族日志——现状等价）；
/// ④ admin-only 形态（<c>EnableRuntime=false</c> 且 <c>EnableAdminApi=true</c>）经独立的族运行时承载
/// （仅管理面），其启动点在宿主运行时启动之后（管理面模块注册面不依赖旧栈），停止先于宿主运行时。
/// Red lines: a single composition root — every assembly goes through <see cref="CapabilityComposition"/>
/// (ordered by the mechanism's <c>CapabilityHostBuilder</c>), this class only decides family inclusion,
/// builds options and declares host modules with no ordering of its own; one family composition per
/// process; the family-less path for disabled/unmatched roles keeps zero family assembly and zero family
/// logging; the admin-only shape rides a separate family runtime started after the host runtime and
/// stopped before it.
/// </remarks>
internal sealed class RuntimeCompositionRoot
{
    /// <summary>
    /// 族组合互斥闸（0 = 未组合 / 组合失败已复位，1 = 有 Role 正在组合或已完成组合）。
    /// </summary>
    private int _familyComposed;

    /// <summary>
    /// 族组合者 Role 名（ServerType；未组合为 null）。族的停止只归组合者 Role。
    /// </summary>
    private string _composerServerType;

    /// <summary>
    /// admin-only 形态的族运行时（仅管理面；未组合为 null）。
    /// </summary>
    private CapabilityRuntime _adminOnlyFamilyRuntime;

    /// <summary>
    /// 组装并启动一个 Role 的统一运行时：判定族启用形态 → 组装（宿主模块恒在）→ 启动。
    /// </summary>
    /// <param name="owner">宿主回调面（启动模板实例）。</param>
    /// <returns>本 Role 的组合产物（停机由调用方持有并驱动）。</returns>
    public async Task<CapabilityRuntime> ComposeAndStartAsync(ILegacyRuntimeHostOwner owner)
    {
        var setting = owner.Setting;
        var online = setting.Online;
        var familyRequested = online != null && IsRoleEnabled(online, setting.ServerType) && (online.EnableRuntime || online.EnableAdminApi);

        if (familyRequested && online.EnableRuntime)
        {
            // 同族进程内单例：首个命中 Role 携带族模块组合，后续命中 Role 只装配宿主模块。
            if (System.Threading.Interlocked.CompareExchange(ref _familyComposed, 1, 0) != 0)
            {
                return await ComposeAndStartHostRuntimeAsync(setting, owner);
            }

            CapabilityRuntime runtime = null;
            try
            {
                var options = BuildFamilyCompositionOptions(setting, enableRuntime: true);
                AddHostModules(options, owner);
                runtime = CapabilityComposition.Compose(new OnlineCapabilityFamily(), options);
                _composerServerType = setting.ServerType;
                await runtime.StartAsync();
                return runtime;
            }
            catch
            {
                await StopFailedRuntimeAsync(runtime);
                _composerServerType = null;
                System.Threading.Volatile.Write(ref _familyComposed, 0);
                throw;
            }
        }

        if (familyRequested)
        {
            // admin-only 形态：宿主运行时承载旧栈；族运行时仅承载管理面（模块注册即可用，不激活），
            // 启动于宿主运行时之后——管理面查询到达时旧栈已就绪。
            var hostRuntime = await ComposeAndStartHostRuntimeAsync(setting, owner);
            if (System.Threading.Interlocked.CompareExchange(ref _familyComposed, 1, 0) != 0)
            {
                return hostRuntime;
            }

            // admin-only 族运行时（仅管理面）。启动失败 = 组合失败：尽力停止已启动部分、复位族闸后重抛
            //（宿主运行时一并停机，不留无法回收的监听/调度残骸——调用方模板的失败补齐路径完成收尾）。
            _composerServerType = setting.ServerType;
            try
            {
                var adminOptions = BuildFamilyCompositionOptions(setting, enableRuntime: false);
                _adminOnlyFamilyRuntime = CapabilityComposition.Compose(new OnlineCapabilityFamily(), adminOptions);
                await _adminOnlyFamilyRuntime.StartAsync();
                return hostRuntime;
            }
            catch
            {
                await StopFailedRuntimeAsync(_adminOnlyFamilyRuntime);
                _adminOnlyFamilyRuntime = null;
                _composerServerType = null;
                System.Threading.Volatile.Write(ref _familyComposed, 0);
                await StopFailedRuntimeAsync(hostRuntime);
                throw;
            }
        }

        // 族未启用（配置段缺席 / Role 未命中 / 双闸全关）：无族组合承载旧栈，零族装配零族日志（现状等价）。
        return await ComposeAndStartHostRuntimeAsync(setting, owner);
    }

    /// <summary>
    /// 摘取 admin-only 族运行时（仅组合者 Role 匹配；摘取后由调用方停机——先于宿主运行时停机）。
    /// </summary>
    /// <param name="serverType">调用方 Role 名（ServerType）。</param>
    /// <returns>admin-only 族运行时；非组合者 / 未组合 / 已摘取时为 null。</returns>
    public CapabilityRuntime DetachAdminOnlyFamilyRuntime(string serverType)
    {
        if (_adminOnlyFamilyRuntime == null || !string.Equals(_composerServerType, serverType, System.StringComparison.Ordinal))
        {
            return null;
        }

        var runtime = _adminOnlyFamilyRuntime;
        _adminOnlyFamilyRuntime = null;
        return runtime;
    }

    /// <summary>
    /// 组装并启动无族宿主运行时（早晚宿主模块 only；无族组合不渲染装配报告——旧栈步骤自身有日志，现状等价）。
    /// </summary>
    /// <param name="setting">当前 Role 的应用配置。</param>
    /// <param name="owner">宿主回调面。</param>
    /// <returns>宿主组合产物。</returns>
    private static async Task<CapabilityRuntime> ComposeAndStartHostRuntimeAsync(AppSetting setting, ILegacyRuntimeHostOwner owner)
    {
        var options = new CapabilityCompositionOptions(default(JsonElement), new LogHelperCapabilityAssemblyLog())
        {
            EnableRuntime = true,
            // 无族形态无能力模块消费作用域三元组；机制要求合法值（>0），未配置段回落 1 占位。
            HostOptions = BuildHostOptions(setting),
        };
        AddHostModules(options, owner);

        CapabilityRuntime runtime = null;
        try
        {
            runtime = CapabilityComposition.Compose(options);
            await runtime.StartAsync();
            return runtime;
        }
        catch
        {
            await StopFailedRuntimeAsync(runtime);
            throw;
        }
    }

    /// <summary>
    /// 构造宿主选项（作用域单键化 D4 后仅 ServerId，未配置（&lt;=0）回落 1 占位——宿主模块不消费作用域，仅满足机制合法性校验）。
    /// </summary>
    /// <param name="setting">当前 Role 的应用配置。</param>
    /// <returns>宿主选项。</returns>
    private static CapabilityHostOptions BuildHostOptions(AppSetting setting)
    {
        return new CapabilityHostOptions
        {
            ServerId = setting.ServerId > 0 ? setting.ServerId : 1,
        };
    }

    /// <summary>
    /// 声明早晚宿主模块（早段承载 DB/发现/组件/热更；晚段承载监听/ServiceOnline/屏障/Active）。
    /// </summary>
    /// <param name="options">组合选项。</param>
    /// <param name="owner">宿主回调面。</param>
    private static void AddHostModules(CapabilityCompositionOptions options, ILegacyRuntimeHostOwner owner)
    {
        options.AddHostModule(new LegacyRuntimeHostModule(owner), CapabilityHostModulePhase.BeforeCapabilities);
        options.AddHostModule(new LegacyNetworkListenerHostModule(owner), CapabilityHostModulePhase.AfterCapabilities);
    }

    /// <summary>
    /// 组合失败路径的尽力停止：撤回已启动部分，停止自身的异常只记日志，不得覆盖触发回滚的原异常。
    /// </summary>
    /// <param name="runtime">已创建的运行时（可能为 null）。</param>
    private static async Task StopFailedRuntimeAsync(CapabilityRuntime runtime)
    {
        if (runtime == null)
        {
            return;
        }

        try
        {
            await runtime.StopAsync();
        }
        catch (System.Exception stopException)
        {
            LogHelper.Error(stopException.ToString());
        }
    }

    /// <summary>
    /// 构造 Online 族组合选项（族配置段 JSON + 双闸 + 作用域三元组 + 能力过滤器 + 管理面 + 宿主 Override 面）——
    /// 等价迁移自退役的静态组合根。
    /// </summary>
    /// <param name="setting">当前 Role 的应用配置。</param>
    /// <param name="enableRuntime">业务运行面闸（admin-only 形态传 false）。</param>
    /// <returns>族组合选项。</returns>
    private static CapabilityCompositionOptions BuildFamilyCompositionOptions(AppSetting setting, bool enableRuntime)
    {
        var online = setting.Online;
        var options = new CapabilityCompositionOptions(BuildFamilyConfigurationSection(setting), new LogHelperCapabilityAssemblyLog())
        {
            EnableRuntime = enableRuntime,
            HostOptions = new CapabilityHostOptions
            {
                ServerId = setting.ServerId,
            },
            Filter = new CapabilityAssemblyFilter(online.Capabilities, online.Disable),
        };

        if (online.EnableAdminApi)
        {
            options.UseAdminApi(adminOptions =>
            {
                adminOptions.AdminPort = setting.OnlineAdminPort;
                adminOptions.AdminApiPrefix = setting.OnlineAdminApiPrefix;
            });
        }

        // 宿主扩展点（Override 注入，来源 = 宿主组合根）：Hotfix 版本切换执行器、玩家目录与回滚状态日志。
        // 类型实参必须显式写成契约接口：描述符表按类型键索引，省略时推断为具型，模块的 RequireHostService<契约> 找不到覆盖。
        options.Override<IOnlineHotfixRollbackExecutor>(new HotfixManagerRollbackExecutor(setting));
        options.Override<IOnlinePlayerDirectory>(new HostPlayerDirectory());
        options.Override<IOnlineHotfixRollbackLog>(new LogHelperHotfixRollbackLog());
        options.Override<IOnlineMatchAssignmentNotifier>(new OnlineMatchAssignmentSessionNotifier());

        // 玩法注入：RPS 验收夹具游戏（对局能力零具体玩法；宿主组合根注入演示黄金路径用的样例玩法）。
        options.Configure<OnlineMatchOptions>(match => match.AddGame(new OnlineRockPaperScissorsGame()));

        // GameDb 一级 store 覆盖（Online.UseGameDbStores 三态）：true 预置五个持久化适配器（业务库已就绪由宿主模块早段保证）；
        // 未声明（null）= 无声降级防护位——启用族但未声明存储模式时告警（不 fail-fast，保护既有无键部署），随后保持 InMemory 默认。
        if (online.UseGameDbStores == true)
        {
            options.Override<IOnlineIdentityStore>(new GameDbOnlineIdentityStore());
            options.Override<IOnlinePlayerStorageStore>(new GameDbOnlinePlayerStorageStore());
            options.Override<IOnlineAssetStore>(new GameDbOnlineAssetStore());
            options.Override<IOnlineAssetTransactionStore>(new GameDbOnlineAssetTransactionStore());
            options.Override<IOnlineFriendStore>(new GameDbOnlineFriendStore());
            var matchSettlementStore = new GameDbOnlineMatchSettlementStore();
            options.Override<IOnlineMatchResultStore>(matchSettlementStore);
            options.Override<IOnlineMatchSettlementStore>(matchSettlementStore);
        }
        else if (OnlineCapabilityStoreModePolicy.ShouldWarnStoreModeUndeclared(online))
        {
            LogHelper.Warning(LocalizationService.GetString(Localization.Keys.Capability.OnlineModule.UseGameDbStoresUndeclared, setting.ServerType));
        }

        return options;
    }

    /// <summary>
    /// 判定 <c>EnableRoles</c>（缺省 ["Game"]）是否命中本 Role。
    /// </summary>
    /// <param name="online">Online 配置段。</param>
    /// <param name="serverType">本 Role 名（ServerType）。</param>
    /// <returns>命中返回 true。</returns>
    private static bool IsRoleEnabled(OnlineCapabilityOptions online, string serverType)
    {
        var enableRoles = online.GetEffectiveEnableRoles();
        for (var index = 0; index < enableRoles.Count; index++)
        {
            if (string.Equals(enableRoles[index], serverType, System.StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 重建族配置段原始 JSON（族级键 + Settings 节点）：族脊柱包读取 ServerId 落授权作用域（单键化 D4），
    /// 机制 Options 绑定器读取 Settings.&lt;能力Id&gt; 做能力级参数覆盖。
    /// </summary>
    /// <param name="setting">当前 Role 的应用配置。</param>
    /// <returns>族配置段 JSON 节点。</returns>
    private static JsonElement BuildFamilyConfigurationSection(AppSetting setting)
    {
        var online = setting.Online;
        var section = new Dictionary<string, object>
        {
            ["ServerId"] = setting.ServerId,
            ["EnableRuntime"] = online.EnableRuntime,
            ["EnableAdminApi"] = online.EnableAdminApi,
            ["UseGameDbStores"] = online.UseGameDbStores,
            ["Settings"] = online.Settings.ValueKind == JsonValueKind.Undefined || online.Settings.ValueKind == JsonValueKind.Null ? new Dictionary<string, object>() : online.Settings,
        };
        return JsonSerializer.SerializeToElement(section);
    }
}
