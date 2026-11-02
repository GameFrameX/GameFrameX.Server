// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
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
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository: https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Hosting.AdminApi;
using GameFrameX.Capability.Hosting.Core;
using GameFrameX.Capability.Online.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Idempotency;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Capability.Online.Match;
using GameFrameX.Capability.Online.Operations.HotfixRollback;
using GameFrameX.Capability.Online.Social;
using Xunit;

namespace GameFrameX.Hotfix.Tests.Online;


/// <summary>
/// Online 能力族测试组合挂具（C294：测试与宿主共用同一装配面
/// <see cref="CapabilityComposition.Compose"/>——族配置段 JSON、宿主 Override、过滤器与
/// 管理面适配器全部经 <see cref="CapabilityCompositionOptions"/> 注入；admin 调度器按
/// <c>AdminApiComposition.CreateAdminApiPlane</c> 同口径组装但直接暴露
/// <see cref="AdminApiDispatcher"/>，供行为用例内网调用）。
/// </summary>
public static class OnlineCapabilityTestHarness
{
    /// <summary>测试用区服标识（与族配置段一致）。</summary>
    public const long ServerId = 1001;

    /// <summary>
    /// 构造族配置段 JSON（区服键 + 可选 Settings 子对象；缺失键由族 fail-fast，因此恒写入）。
    /// </summary>
    /// <param name="settingsJson">Settings 子对象原文（null = 不写 Settings 键）。</param>
    /// <returns>族配置段节点。</returns>
    public static JsonElement FamilySection(string settingsJson = null)
    {
        var settings = string.IsNullOrEmpty(settingsJson) ? string.Empty : ",\"Settings\":" + settingsJson;
        return JsonDocument.Parse("{\"ServerId\":1001" + settings + "}").RootElement.Clone();
    }

    /// <summary>
    /// 构造组合选项（收集器日志 + 合法宿主选项 + 两个宿主扩展点 Override）。
    /// <para>
    /// 管理面适配器（错误码表 / 作用域守卫 / 幂等包裹）由族基建 <see cref="OnlineCapabilityFamily"/> 注册，
    /// 测试不得 Override 绕行——<c>UseAdminApi</c> 形态必须走真实族基建解析路径。
    /// </para>
    /// </summary>
    /// <param name="settingsJson">Settings 子对象原文（null = 不写 Settings 键）。</param>
    /// <returns>组合选项。</returns>
    public static CapabilityCompositionOptions CreateOptions(string settingsJson = null)
    {
        var options = new CapabilityCompositionOptions(FamilySection(settingsJson), new CollectingCapabilityAssemblyLog())
        {
            HostOptions = new CapabilityHostOptions { ServerId = ServerId, },
        };

        // 宿主扩展点：Social 的玩家目录与 Operations 的热更回滚执行器（两者均 RequireHostService，缺失即装配 fail-fast）。
        options.Override<IOnlinePlayerDirectory>(new EmptyPlayerDirectory());
        options.Override<IOnlineHotfixRollbackExecutor>(new NoopHotfixRollbackExecutor());

        // 玩法注入：RPS 验收夹具游戏（原包内自动注册的对局玩法；生产装配由各宿主组合根自行注入）。
        options.Configure<OnlineMatchOptions>(match => match.AddGame(new OnlineConnectFourGame()));
        return options;
    }

    /// <summary>
    /// 组合 Online 能力族（全装配；过滤器 / Settings / Configure 由用例经 <paramref name="configure"/> 追加）。
    /// </summary>
    /// <param name="settingsJson">Settings 子对象原文（null = 不写 Settings 键）。</param>
    /// <param name="configure">用例追加的编排动作。</param>
    /// <returns>组合产物。</returns>
    public static CapabilityRuntime Compose(string settingsJson = null, Action<CapabilityCompositionOptions> configure = null)
    {
        var options = CreateOptions(settingsJson);
        configure?.Invoke(options);
        return CapabilityComposition.Compose(new OnlineCapabilityFamily(), options);
    }

    /// <summary>
    /// 按机制同口径组装 admin 调度器（遍历模块贡献 action；错误码表 / 作用域守卫 / 幂等包裹一律从
    /// 族基建注册的宿主共享服务面解析——与 <c>AdminApiComposition.CreateAdminApiPlane</c> 同源，测试不另造适配器）。
    /// </summary>
    /// <param name="host">已装配能力宿主。</param>
    /// <returns>admin 调度器。</returns>
    public static AdminApiDispatcher CreateDispatcher(CapabilityHost host)
    {
        var registry = new AdminActionRegistry();
        foreach (var module in host.Modules)
        {
            if (module is IAdminActionContributor contributor)
            {
                contributor.RegisterAdminActions(registry);
            }
        }

        return new AdminApiDispatcher(
            registry,
            host.Services.GetService<AdminApiErrorCodes>(),
            host.Services.GetService<IAdminApiScopeAdapter>(),
            new AdminApiClaimedScope(host.ServerId),
            host.Services.GetOptionalService<IAdminApiIdempotencyAdapter>(),
            host.Report,
            host.Services);
    }

    /// <summary>
    /// 组合并启动业务面（Activate 桥发布完成后返回；用例须在结束时 <c>StopAsync</c> 以撤桥）。
    /// </summary>
    /// <param name="settingsJson">Settings 子对象原文。</param>
    /// <param name="configure">用例追加的编排动作。</param>
    /// <returns>已启动的组合产物。</returns>
    public static async Task<CapabilityRuntime> ComposeStartedAsync(string settingsJson = null, Action<CapabilityCompositionOptions> configure = null)
    {
        var runtime = Compose(settingsJson, configure);
        await runtime.Host.StartAsync();
        return runtime;
    }

    /// <summary>
    /// 组合并在 <paramref name="body"/> 结束后恒停止（finally StopAsync：撤桥 + 释放钩子声明——
    /// 同进程下一次组合才能重新声明 <c>IOnlineChatModerationHook</c>）。
    /// </summary>
    /// <param name="body">用例主体（入参为已组合、未启动的产物）。</param>
    /// <param name="settingsJson">Settings 子对象原文。</param>
    /// <param name="configure">用例追加的编排动作。</param>
    /// <returns>异步任务。</returns>
    public static async Task WithRuntimeAsync(Func<CapabilityRuntime, Task> body, string settingsJson = null, Action<CapabilityCompositionOptions> configure = null)
    {
        var runtime = Compose(settingsJson, configure);
        try
        {
            await body(runtime);
        }
        finally
        {
            await runtime.StopAsync();
        }
    }

    /// <summary>
    /// 空玩家目录桩（Social 的 <c>RequireHostService&lt;IOnlinePlayerDirectory&gt;</c> 最小满足；
    /// 搜索 / 解析均返回空集合 = 部署未接目录的披露态，不是伪数据）。
    /// </summary>
    public sealed class EmptyPlayerDirectory : IOnlinePlayerDirectory
    {
        /// <summary>
        /// 按名称搜索玩家目录；本桩不接任何目录数据，固定返回空集合，表示部署未接入玩家目录。
        /// </summary>
        /// <remarks>
        /// Searches the player directory by name; this stub has no directory data and always returns an empty collection.
        /// </remarks>
        /// <param name="keyword">搜索关键字（本桩忽略） / Search keyword (ignored by this stub)</param>
        /// <param name="limit">返回条数上限（本桩忽略） / Maximum number of results (ignored by this stub)</param>
        /// <param name="cancellationToken">取消令牌（本桩忽略） / Cancellation token (ignored by this stub)</param>
        /// <returns>固定为空的目录条目集合 / An always-empty collection of directory entries</returns>
        public Task<IReadOnlyList<OnlinePlayerDirectoryEntry>> SearchByNameAsync(string keyword, int limit, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<OnlinePlayerDirectoryEntry>>(Array.Empty<OnlinePlayerDirectoryEntry>());
        }

        /// <summary>
        /// 按玩家标识批量解析目录条目；本桩不接任何目录数据，固定返回空集合。
        /// </summary>
        /// <remarks>
        /// Resolves directory entries by player identifiers; this stub has no directory data and always returns an empty collection.
        /// </remarks>
        /// <param name="playerIds">待解析的玩家标识集合（本桩忽略） / Player identifiers to resolve (ignored by this stub)</param>
        /// <param name="cancellationToken">取消令牌（本桩忽略） / Cancellation token (ignored by this stub)</param>
        /// <returns>固定为空的目录条目集合 / An always-empty collection of directory entries</returns>
        public Task<IReadOnlyList<OnlinePlayerDirectoryEntry>> FindAsync(IReadOnlyList<long> playerIds, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<OnlinePlayerDirectoryEntry>>(Array.Empty<OnlinePlayerDirectoryEntry>());
        }
    }

    /// <summary>
    /// 空操作热更回滚执行器桩（Operations 的 <c>RequireHostService&lt;IOnlineHotfixRollbackExecutor&gt;</c> 最小满足）。
    /// </summary>
    public sealed class NoopHotfixRollbackExecutor : IOnlineHotfixRollbackExecutor
    {
        /// <summary>
        /// 执行热更回滚；本桩仅用于满足宿主服务依赖，测试不验证回滚效果，因此不做任何操作直接完成。
        /// </summary>
        /// <remarks>
        /// Performs a hotfix rollback; this stub only satisfies the host service requirement and completes immediately without doing anything.
        /// </remarks>
        /// <param name="targetVersion">回滚目标版本（本桩忽略） / Target rollback version (ignored by this stub)</param>
        /// <param name="cancellationToken">取消令牌（本桩忽略） / Cancellation token (ignored by this stub)</param>
        /// <returns>已完成的任务 / An already completed task</returns>
        public Task RollbackAsync(string targetVersion, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}

/// <summary>
/// 好友域桥验证种子（C294 §4.6：生产配置键 <c>FriendBridgeSeedPlayerIds</c> 删除后，种子语义挪进测试夹具；
/// 语句级迁移自旧 <c>OnlineFriendDomainBridgeWiring.SeedFriendDomainBridgeVerificationAsync</c>——
/// 首元素为请求方、其余为好友，经真实 <see cref="OnlineFriendService"/> 写路径两两建立，幂等收敛）。
/// </summary>
