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

using GameFrameX.Utility.Setting;

namespace GameFrameX.Launcher.Composition;

/// <summary>
/// 旧栈宿主回调面：<see cref="LegacyRuntimeHostModule"/>（早段）与
/// <see cref="LegacyNetworkListenerHostModule"/>（晚段）经本接口回调宿主启动模板的固定序列步骤
///（模板的虚方法钩子语义原样保留——Role 覆写继续生效）。
/// </summary>
/// <remarks>
/// The legacy-stack host callback surface: the early-phase <see cref="LegacyRuntimeHostModule"/> and the
/// late-phase <see cref="LegacyNetworkListenerHostModule"/> call back into the startup template's fixed
/// sequence steps through this interface (the template's virtual-hook semantics are preserved verbatim
/// — role overrides keep working).
/// </remarks>
internal interface ILegacyRuntimeHostOwner
{
    /// <summary>获取当前 Role 的应用配置。</summary>
    AppSetting Setting { get; }

    /// <summary>获取本次停机原因（晚段模块停用序列透传给 ServiceOffline 与基类停机流程）。</summary>
    string CurrentStopMessage { get; }

    /// <summary>
    /// 装载热更模块（经 <see cref="GameFrameX.Core.Hotfix.HotfixRuntime"/> 注入面；同路径幂等守卫在实例核内）。
    /// </summary>
    /// <returns>表示装载操作的异步任务。</returns>
    Task LoadHotfixModuleAsync();

    /// <summary>
    /// 初始化控制库（<c>Contains</c> 判重护栏 + <c>IsDefault = false</c> 显式声明；虚方法语义保留）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task InitializeControlDatabaseAsync();

    /// <summary>
    /// 激活发现层（读侧 watcher + 写侧心跳 + 跨 Role 路由缝重装 + 玩家路由层；虚方法语义保留）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ActivateDiscoveryLayerAsync();

    /// <summary>
    /// 初始化业务库（缺省 <c>IsDefault = true</c> 声明式提名门面默认库；虚方法语义保留）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task InitializeBusinessDatabaseAsync();

    /// <summary>
    /// 启动网络监听（消息解码/编码/压缩与热更 HTTP 处理器桥接由模板内部装配）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task StartNetworkListenersAsync();

    /// <summary>
    /// 启动完成钩子（缺省分发 <c>ServiceOnline</c> 事件——唯一分发点；虚方法语义保留）。
    /// </summary>
    /// <returns>表示钩子操作的异步任务。</returns>
    Task OnStartupCompletedAsync();

    /// <summary>
    /// 放行启动就绪屏障（优先级链启动语义）并标记启动序列完成。
    /// </summary>
    void MarkStartUpReadyBarrier();

    /// <summary>
    /// 心跳切 <c>Active</c>（未激活发现层或无广播身份时为无害 no-op）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task MarkDiscoveryActiveAsync();

    /// <summary>
    /// 获取是否应发布 KCP 附加端点（<c>IsEnableInnerKcp</c> 且已配置内网 KCP 监听）。
    /// </summary>
    bool ShouldPublishInnerKcpEndpoint { get; }

    /// <summary>
    /// 发布 KCP 附加端点到发现层广播。
    /// </summary>
    void PublishInnerKcpAdvertiseEndpoint();

    /// <summary>
    /// 停机钩子（缺省分发 <c>ServiceOffline</c> 事件——唯一分发点；虚方法语义保留）。
    /// </summary>
    /// <param name="message">终止原因。</param>
    /// <returns>表示钩子操作的异步任务。</returns>
    Task OnStoppingAsync(string message);

    /// <summary>
    /// 基类停机流程（停机日志 + 网络停机 + 运行时状态与退出令牌置位）——晚段模块停用序列的收尾步骤。
    /// </summary>
    /// <param name="message">终止原因。</param>
    /// <returns>表示异步操作的任务。</returns>
    Task StopNetworkAndMarkStoppedAsync(string message);
}
