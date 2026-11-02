// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and related international regulations.
//
//  使用本项目须严格遵守相应法律法规与开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录的 LICENSE 文件。
//  please refer to the LICENSE file at the source code root for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any disputes and liabilities arising from secondary development,
//  本组织不承担任何责任，开发者须自行承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository: https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Threading.Tasks;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Proto.Proto;
using GameFrameX.Utility;

namespace GameFrameX.Hotfix.Logic.ServerRole.Match;

/// <summary>
/// 匹配域桥薄适配器（C246，420 族协议入口的唯一转发点）：经 <see cref="CapabilityBridge{TContract}"/>
///（契约 <see cref="IOnlineMatchDomain"/>）进程内租约把入队 / 取消 / 状态 / 入场协议行为转发到
/// Online 匹配域契约面，并按 T2 裁定完成错误码映射。
/// <para>
/// 维护约束（红线）：
/// ① 域桥独占——桥不可用（宿主未启 / 已停止 / 租约撤销）与服务失败一律映射为
/// <c>-(int)OnlineErrorCode</c> 段的显式错误码（≤ -1001，与 420 族旧段 420001..420005 不重叠），
/// 绝不静默回落（旧 FIFO 内存池链已随本切换退役，无回落目标）；
/// ② 可信身份：作用域三元组只取 lease 携带的组合根授权值，玩家位只取会话绑定 playerId
///（<c>ServerRoleHandlerHelper.GetCurrentPlayerId</c>），请求体字段一律不可影响作用域；
/// ③ 无缓存：逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/> 取契约租约，热更 / 宿主重启后旧引用不存活；
/// ④ 错误码映射表——本地校验保留 420004（模式非法）/ 420005（评分非法），
/// 入队查重保留 420002（已在排队），取消终态回执保留 420003（未在排队）；
/// 其余（含限流 RateLimitExceeded）走 Online 显式段；入场为新面，失败一律走 Online 显式段。
/// </para>
/// </summary>
/// <remarks>
/// The match-domain thin bridge adapter (C246, the single forwarding point for the domain-420 protocol
/// entries): forwards enqueue / cancel / status / enter protocol behaviours in-process to the Online
/// match-domain contract face through <see cref="CapabilityBridge{TContract}"/> (contract
/// <see cref="IOnlineMatchDomain"/>), mapping error codes per the T2 rulings. The bridge path is
/// exclusive — unavailability and service failures map to explicit <c>-(int)OnlineErrorCode</c> codes
/// (≤ -1001, disjoint from the legacy 420001..420005 band)
/// with no silent fallback (the legacy in-memory FIFO pool chain retired with this cutover — there is
/// no fallback target). The scope triple comes only from the capability-published authorized-scope
/// provider and the player position only from the session-bound player id — request-body fields can
/// never influence the scope; nothing is cached — each request re-acquires the contract lease so no
/// stale reference survives a hotfix swap or host restart. The mapping tables: local validation keeps
/// 420004 (mode invalid) / 420005 (rating invalid), enqueue duplication keeps 420002 (already
/// queuing), the cancel terminal receipt keeps 420003 (not queuing); everything else (including the
/// rate limiter) uses the explicit Online band; entering is a new face whose failures always use the
/// Online band.
/// </remarks>
public static class MatchDomainBridge
{
    /// <summary>
    /// 加入匹配（420-10）：本地校验（模式 / 评分）后转发域面；成功回执的 MatchId 为 0，撮合结果由通知或状态查询提供。
    /// </summary>
    /// <remarks>
    /// Validates mode and rating before forwarding enqueue; success returns MatchId 0, with assignment supplied by notification or status query.
    /// </remarks>
    /// <param name="playerId">会话绑定玩家标识（作用域玩家位唯一来源） / Session-bound player ID, the sole source of the scope player identity.</param>
    /// <param name="request">协议请求 / Protocol request.</param>
    /// <param name="response">协议响应 / Protocol response.</param>
    public static async Task EnqueueAsync(long playerId, ReqMatchEnqueue request, RespMatchEnqueue response)
    {
        if (request.Mode != MatchMode.OneVsOne && request.Mode != MatchMode.TwoVsTwo)
        {
            response.ErrorCode = (int)MatchErrorCode.ModeInvalid;
            return;
        }

        if (request.Rating <= 0)
        {
            response.ErrorCode = (int)MatchErrorCode.RatingInvalid;
            return;
        }

        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.EnqueueAsync(scope, (int)request.Mode, request.Rating);
            if (!result.IsSuccess)
            {
                response.ErrorCode = MapEnqueueFailure(result.Code);
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            response.MatchId = result.Data == null ? 0 : result.Data.MatchId;
            response.ErrorCode = 0;
        }
        catch (System.Exception exception)
        {
            response.MatchId = 0;
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Match.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 取消排队（420-12）：排队票取消成功；终态回执（已撮合 / 已过期 / 无票）映射旧 420003。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task CancelAsync(long playerId, RespMatchCancel response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.CancelAsync(scope);
            if (!result.IsSuccess)
            {
                response.ErrorCode = -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            response.ErrorCode = result.Data != null && result.Data.Cancelled
                ? 0
                : (int)MatchErrorCode.NotQueuing;
        }
        catch (System.Exception exception)
        {
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Match.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 查询匹配状态（420-14）：排队中 / 已撮合（含对局标识）/ 其余终态。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task StatusAsync(long playerId, RespMatchStatus response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.GetStatusAsync(scope);
            if (!result.IsSuccess)
            {
                response.Queuing = false;
                response.MatchId = 0;
                response.ErrorCode = -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            response.Queuing = result.Data != null && result.Data.Queuing;
            response.MatchId = result.Data == null ? 0 : result.Data.MatchId;
            response.ErrorCode = 0;
        }
        catch (System.Exception exception)
        {
            response.Queuing = false;
            response.MatchId = 0;
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Match.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 按对局标识入场（420-16，C246 新面）：失败一律 Online 显式段。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="request">协议请求。</param>
    /// <param name="response">协议响应。</param>
    public static async Task EnterAsync(long playerId, ReqMatchEnter request, RespMatchEnter response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.EnterAsync(scope, request.MatchId);
            if (!result.IsSuccess)
            {
                response.ErrorCode = -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            response.MatchId = result.Data == null ? 0 : result.Data.MatchId;
            response.Started = result.Data != null && result.Data.Started;
            response.ErrorCode = 0;
        }
        catch (System.Exception exception)
        {
            response.MatchId = 0;
            response.Started = false;
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Match.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 读取当前域桥契约与生效作用域（逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/>，不缓存；
    /// 获取与读取之间的撤销竞态按缺席同语义显式失败）。授权作用域不可得（提供者桥缺席 / 撤销）同样视为依赖不可用：
    /// 返回 null 走调用方既有的桥不可用失败分支，契约面不被触达，绝不以空作用域降级调用。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识（作用域玩家位唯一来源，兼作日志披露）。</param>
    /// <param name="scope">输出：授权三元组 + 会话玩家位组成的生效作用域；返回 null 时为 null。</param>
    /// <returns>契约面；契约桥或授权作用域任一不可得返回 null。</returns>
    private static IOnlineMatchDomain RequireLease(long playerId, out OnlineScope scope)
    {
        scope = null;
        IOnlineMatchDomain contract = null;
        if (CapabilityBridge<IOnlineMatchDomain>.TryAcquire(out var lease))
        {
            try
            {
                contract = lease.Contract;
            }
            catch (CapabilityBridgeUnavailableException)
            {
                // 获取与读取之间的撤销竞态：与缺席同语义（显式失败），不降级。
            }
        }

        if (contract != null)
        {
            // 授权作用域不可得 = 依赖不可用：与契约缺席同口径显式失败，不降级。
            scope = OnlineAuthorizedScopeReader.BuildRequestScope(playerId);
            if (scope != null)
            {
                return contract;
            }
        }

        // Localization: Hotfix.Match.DomainBridgeUnavailable - 匹配域桥不可用（未发布或已撤销）, PlayerId: {0}, Code: {1}
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Match.DomainBridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
        return null;
    }

    /// <summary>
    /// 入队失败映射：重复排队 → 旧 420002；其余 → Online 显式段。
    /// </summary>
    /// <param name="code">域桥失败码。</param>
    /// <returns>420 族协议错误码。</returns>
    private static int MapEnqueueFailure(OnlineErrorCode code)
    {
        return code == OnlineErrorCode.DuplicateRequest ? (int)MatchErrorCode.AlreadyQueuing : -(int)code;
    }

    /// <summary>
    /// 域桥失败日志（Error 级；错误码已映射进响应，本日志供服务端归因）。
    /// </summary>
    /// <param name="code">失败码。</param>
    /// <param name="message">失败描述。</param>
    /// <param name="playerId">请求主体。</param>
    private static void LogBridgeFailed(OnlineErrorCode code, string message, long playerId)
    {
        // Localization: Hotfix.Match.DomainBridgeFailed - 匹配域桥返回失败, Code: {0}, Message: {1}, PlayerId: {2}
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Match.DomainBridgeFailed, code, message, playerId));
    }
}
