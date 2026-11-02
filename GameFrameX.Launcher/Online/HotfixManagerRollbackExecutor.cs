// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source code.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legal rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any disputes or liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository: https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using GameFrameX.Core.Hotfix;
using GameFrameX.Capability.Online.Operations.HotfixRollback;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Launcher.Online;

/// <summary>
/// Hotfix 版本切换执行器（生产实现）：
/// 组合既有公共原语完成方向无关的本进程热更版本切换，供组合根经
/// <c>Override&lt;IOnlineHotfixRollbackExecutor&gt;</c> 注入（Operations 侧 <c>RequireHostService</c> 即得非 null，消除恒缺席 8002）。
/// <para>
/// 切换时序不变量（红线）：
/// ① <b>单次装载入口</b>——切换 = 单次 <see cref="HotfixManager.LoadHotfixModuleAsync(AppSetting, HotfixLoadOptions)"/>：
/// 入口内部完成 同路径幂等守卫 → 目录兜底 → <c>Init(reload)</c> → 旧模块 <c>Retire</c>（10 分钟退役宽限，
/// 宽限期内 in-flight 请求按程序集身份路由到原模块，宽限排空后由退役链 Unload + GC 验证）→
/// 当前指针同步切换 → 桥接 <c>OnLoadSuccess</c>。本执行器<b>不直接调用 <c>Unload()</c></b>——
/// 立即卸载会剥夺退役宽限期的 in-flight 路由。
/// ② <b>切换时刻零下载</b>——执行器路径只做在盘预检 + 装载 + 装载路径校验，静态无网络 IO；
/// 制品获取只发生在登记期（<c>OnlineHotfixArtifactFetcher</c>）。
/// ③ <b>失败恢复</b>——目标装载失败（Init 失败指针未动 / 指针已切但桥接拒绝）按切换前装载路径重载原版本
/// （同路径时入口幂等守卫短路）：恢复成功则上抛切换失败（8002 语义为真失败而非恒定）；
/// 恢复也失败则合并双方信息上抛（进程热更面不一致，须人工介入）。
/// ④ <b>进程内互斥</b>——<see cref="SemaphoreSlim"/>(1,1) 串行化切换命令（互斥范围 = 预检 + 装载 + 校验 + 恢复）；
/// 多实例逐实例编排归运维发布流程（单进程语义既有口径）。
/// </para>
/// </summary>
public sealed class HotfixManagerRollbackExecutor : IOnlineHotfixRollbackExecutor
{
    /// <summary>
    /// 切换互斥门（进程内同一时刻至多一个 in-flight 切换）。
    /// </summary>
    private readonly SemaphoreSlim _switchGate = new SemaphoreSlim(1, 1);

    /// <summary>
    /// 应用配置（切换与失败恢复统一传本配置；null = 沿用 <see cref="HotfixManager"/> 启动期落定的进程级配置）。
    /// </summary>
    private readonly AppSetting _setting;

    /// <summary>
    /// 初始化 <see cref="HotfixManagerRollbackExecutor"/>。
    /// </summary>
    /// <param name="setting">应用配置（null = 沿用启动期落定的进程级配置）。</param>
    public HotfixManagerRollbackExecutor(AppSetting setting = null)
    {
        _setting = setting;
    }

    /// <summary>
    /// 将热更程序集切换到已落盘的目标版本：校验版本号并解析制品路径，在互斥门内经 <see cref="HotfixManager"/> 装载并确认实际装载路径；装载失败时尝试恢复切换前版本后抛出。
    /// </summary>
    /// <remarks>
    /// Switches the hotfix assembly to an already-downloaded target version. The version is validated and resolved to an artifact path,
    /// then under a single-flight gate the artifact must exist on disk (no download at switch time), is loaded via <see cref="HotfixManager"/>,
    /// and the loaded assembly location is verified. On failure the previously loaded version is reloaded before throwing.
    /// </remarks>
    /// <param name="targetVersion">目标热更版本号 / Target hotfix version</param>
    /// <param name="cancellationToken">取消令牌，仅用于等待切换互斥门 / Cancellation token, used only while waiting for the switch gate</param>
    /// <returns>表示异步切换操作的任务 / Task representing the asynchronous switch</returns>
    /// <exception cref="ArgumentException">版本号为空白或含非法路径段 / Version is blank or contains illegal path segments</exception>
    /// <exception cref="FileNotFoundException">目标版本制品未落盘 / Target version artifact is not on disk</exception>
    /// <exception cref="InvalidOperationException">目标版本装载失败（已恢复或恢复也失败） / Target load failed (original restored, or restoration also failed)</exception>
    public async Task RollbackAsync(string targetVersion, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(targetVersion))
        {
            throw new ArgumentException("目标版本号必填", nameof(targetVersion));
        }

        // 版本号经装载入口同源注入防线（拒 / \ : 与 .. 段），并在此解析目标路径（抛 ArgumentException = 拒绝切换）。
        var targetPath = new HotfixLoadOptions { Version = targetVersion, }.ResolveAssemblyFilePath();

        await _switchGate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // 切换时刻零下载红线：只消费已落盘制品；未落盘即拒，绝不回退到网络获取。
            if (!File.Exists(targetPath))
            {
                throw new FileNotFoundException("目标版本制品未落盘（制品须在登记期获取归档，切换时刻零下载）：" + targetPath, targetPath);
            }

            var originalAssemblyPath = HotfixManager.HotfixAssembly?.Location;
            var loaded = await HotfixManager.LoadHotfixModuleAsync(_setting, new HotfixLoadOptions { Version = targetVersion, }).ConfigureAwait(false);
            if (!loaded || !IsTargetLoaded(targetPath))
            {
                var switchFailure = loaded ? "装载路径校验不符" : "装载入口返回失败";
                try
                {
                    await RecoverOriginalAsync(originalAssemblyPath).ConfigureAwait(false);
                }
                catch (Exception recoveryFailure)
                {
                    throw new InvalidOperationException("目标版本装载失败（" + switchFailure + "）：" + targetVersion
                                                        + "；且原版本恢复失败（" + recoveryFailure.Message + "）——进程热更面不一致，须人工介入");
                }

                throw new InvalidOperationException("目标版本装载失败（" + switchFailure + "，已恢复原版本装载，切换未生效）：" + targetVersion);
            }
        }
        finally
        {
            _switchGate.Release();
        }
    }

    /// <summary>
    /// 装载健康校验：当前装载程序集路径与目标路径一致（指针切换与实际装载的双确认）。
    /// </summary>
    /// <param name="targetPath">目标制品完整路径。</param>
    /// <returns>目标已实际装载返回 <c>true</c>。</returns>
    private static bool IsTargetLoaded(string targetPath)
    {
        return string.Equals(HotfixManager.HotfixAssembly?.Location, targetPath, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 失败恢复：按切换前装载路径重载原版本（同路径时入口幂等守卫短路成功）；
    /// 无原版本（首次装载前失败）无可恢复；重载未被确认即抛（信息合并由调用方承担）。
    /// 恢复装载与主路径同口径传 <see cref="_setting"/>（统一传参，不传 null）。
    /// </summary>
    /// <param name="originalAssemblyPath">切换前的实际装载程序集路径。</param>
    private async Task RecoverOriginalAsync(string originalAssemblyPath)
    {
        if (originalAssemblyPath == null)
        {
            return;
        }

        var recovered = await HotfixManager.LoadHotfixModuleAsync(_setting, BuildOptionsFromLoadedPath(originalAssemblyPath)).ConfigureAwait(false);
        if (!recovered || !string.Equals(HotfixManager.HotfixAssembly?.Location, originalAssemblyPath, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("原版本重载未确认（装载路径不符）：" + originalAssemblyPath);
        }
    }

    /// <summary>
    /// 由实际装载路径反推装载参数（版本目录段 = 路径目录名；目录名为缺省目录 <c>hotfix</c> 时回空版本；
    /// 文件名取实际装载文件名）。
    /// </summary>
    /// <param name="assemblyPath">装载程序集完整路径。</param>
    /// <returns>等价装载参数。</returns>
    private static HotfixLoadOptions BuildOptionsFromLoadedPath(string assemblyPath)
    {
        var directoryName = Path.GetFileName(Path.GetDirectoryName(assemblyPath));
        return new HotfixLoadOptions
        {
            Version = string.Equals(directoryName, "hotfix", StringComparison.Ordinal) ? string.Empty : directoryName,
            AssemblyFileName = Path.GetFileName(assemblyPath),
        };
    }
}
