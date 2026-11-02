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

using GameFrameX.Capability.Online.Abstractions;

namespace GameFrameX.Launcher.Online;

/// <summary>
/// Online 存储模式策略（纯谓词，自退役的静态组合根迁入）：族被启用（双闸任一开）但
/// <c>Online.UseGameDbStores</c> 未在配置中显式声明（null）时须发「存储模式未声明」启动期告警
///（无声降级防护）。
/// </summary>
/// <remarks>
/// The Online storage-mode policy (a pure predicate, migrated from the retired static composition root):
/// the "storage mode undeclared" startup warning (the silent-degradation guard) must fire when the family
/// is enabled (either gate on) yet <c>Online.UseGameDbStores</c> was never explicitly declared in
/// configuration (null). 三态口径收敛为纯谓词供单测直接断言；显式 false（有意选择 InMemory，如本地
/// 无库开发）与双闸全关（族未启用，零装配零日志）均不告警。
/// The tri-state contract is collapsed into this pure predicate for direct unit-test assertion; an
/// explicit false (a deliberate in-memory choice) and both gates off (family disabled) never warn.
/// </remarks>
public static class OnlineCapabilityStoreModePolicy
{
    /// <summary>
    /// 判定是否需要发出「存储模式未声明」启动期告警：族已启用但 <c>Online.UseGameDbStores</c> 未显式声明时为 true。
    /// </summary>
    /// <param name="online">当前 Role 的 Online 配置段。</param>
    /// <returns>未声明存储模式且族已启用时为 true；否则为 false。</returns>
    public static bool ShouldWarnStoreModeUndeclared(OnlineCapabilityOptions online)
    {
        return online != null
            && (online.EnableRuntime || online.EnableAdminApi)
            && online.UseGameDbStores == null;
    }
}
