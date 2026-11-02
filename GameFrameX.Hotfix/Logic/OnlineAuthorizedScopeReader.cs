// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   Any legal disputes and liabilities arising from secondary development based on this project
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool
//   CNB Repository: https://cnb.cool
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
/// <summary>
/// Online 授权作用域读取器（C294 桥消费侧共享件）：经 <see cref="CapabilityBridge{TContract}"/>
/// 进程内读取 online-foundation 发布的 <see cref="IOnlineAuthorizedScopeProvider"/> 租约，
/// 把宿主组合根授权的作用域三元组与请求玩家位组装为生效作用域。
/// <para>
/// 维护约束（红线）：① 可信身份——三元组只取能力侧发布的授权值（等价旧域桥租约的
/// <c>AuthorizedScope</c>），玩家位只由调用方按会话绑定身份填充，请求体字段一律不可影响作用域；
/// ② 无缓存——逐请求读取桥租约，不缓存 <see cref="OnlineScope"/> 实例，宿主停止 / 重启后旧引用不存活；
/// ③ 缺席 / 撤销不降级——提供者桥缺席或租约已撤销时返回 false，由各域薄适配器按
/// <see cref="OnlineErrorCode.DependencyUnavailable"/> 显式失败处理，绝不回落本地默认值。
/// </para>
/// </summary>
/// <remarks>
/// The Online authorized-scope reader (the C294 bridge-consumer shared part): reads the
/// online-foundation-published <see cref="IOnlineAuthorizedScopeProvider"/> lease in-process through
/// <see cref="CapabilityBridge{TContract}"/> and combines the composition-root-authorized scope triple
/// with the requesting player position. The triple comes only from the capability-published authorized
/// values (equivalent to the old domain-lease <c>AuthorizedScope</c>); nothing is cached — every request
/// re-reads the bridge lease; absence or revocation returns false so each domain thin adapter fails
/// explicitly with <see cref="OnlineErrorCode.DependencyUnavailable"/> and never falls back to defaults.
/// </remarks>
internal static class OnlineAuthorizedScopeReader
{
    /// <summary>
    /// 读取授权作用域区服键（玩家位为 0，由调用方按请求主体填充；单键化 D4 后无租户位/App 位）。
    /// </summary>
    /// <param name="authorizedScope">输出：授权作用域（提供者桥缺席 / 已撤销时为 null）。</param>
    /// <returns>提供者在场且可达返回 true；缺席 / 撤销返回 false。</returns>
    internal static bool TryGetAuthorizedScope(out OnlineScope authorizedScope)
    {
        if (CapabilityBridge<IOnlineAuthorizedScopeProvider>.TryAcquire(out var lease))
        {
            try
            {
                authorizedScope = lease.Contract.AuthorizedScope;
                if (authorizedScope != null)
                {
                    return true;
                }
            }
            catch (CapabilityBridgeUnavailableException)
            {
                // 获取与读取之间的撤销竞态：与缺席同语义（显式失败），不降级。
            }
        }

        authorizedScope = null;
        return false;
    }

    /// <summary>
    /// 组装生效作用域（授权区服键 + 请求玩家位）。
    /// </summary>
    /// <param name="playerId">请求主体玩家标识（会话绑定身份或 Token 解析身份）。</param>
    /// <returns>生效作用域；提供者桥缺席 / 已撤销时为 null。</returns>
    internal static OnlineScope BuildRequestScope(long playerId)
    {
        return TryGetAuthorizedScope(out var authorizedScope)
            ? new OnlineScope(authorizedScope.ServerId, playerId)
            : null;
    }
}
