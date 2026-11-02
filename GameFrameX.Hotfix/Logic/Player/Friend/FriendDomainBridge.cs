// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations.
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本组织及其贡献者不承担任何责任，开发者应自行承担。
//   The organization and contributors assume no responsibility; developers shall bear the responsibility themselves.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:    https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Collections.Generic;
using System.Threading.Tasks;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Hotfix.Logic.Player.Friend;

/// <summary>
/// 好友域桥薄适配器（C244，好友域 120 / 660 两族协议入口的唯一转发点）：经
/// <see cref="CapabilityBridge{TContract}"/>（契约 <see cref="IOnlineFriendDomain"/>）进程内租约把
/// 全部好友协议行为转发到 Online 好友域契约面，并按 T2 裁定完成两族错误码映射。
/// <para>
/// 维护约束（红线）：
/// ① 域桥独占——桥不可用（宿主未启 / 已停止 / 租约撤销）与服务失败一律映射为
/// <c>-(int)OnlineErrorCode</c> 段的显式错误码（≤ -1001，与旧 120 链 -1..-104 段、660 段 660001..660005 不重叠），
/// 绝不静默回落、绝不返回成功空列表掩盖错库 / 错进程（旧 120/660 链已随本切换退役，无回落目标）；
/// ② 可信身份：作用域三元组只取能力侧授权作用域提供者发布的组合根授权值，玩家位只取会话绑定 playerId
///（<c>ServerRoleHandlerHelper.GetCurrentPlayerId</c>），请求体字段一律不可影响作用域；
/// ③ 无缓存：逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/> 取契约租约，热更 / 宿主重启后旧引用不存活；
/// ④ 错误码映射表——120 族保留 -100/-101/-103/-104（-102 目标不存在已废弃披露），
/// 660 族全保留 660001..660005（660004 同时覆盖「对方已先申请」的单记录收敛，裁定 R6）；
/// 其余（含好友上限 StateOperationForbidden）走 Online 显式段。
/// </para>
/// </summary>
/// <remarks>
/// The friend-domain thin bridge adapter (C244, the single forwarding point for both the domain-120
/// and domain-660 friend protocol entries): forwards every friend protocol behaviour in-process to the
/// Online friend-domain contract face through <see cref="CapabilityBridge{TContract}"/> (contract
/// <see cref="IOnlineFriendDomain"/>), mapping error codes per the T2 rulings. The bridge path is
/// exclusive — unavailability (host not started / stopped / lease revoked) and service failures map to
/// explicit <c>-(int)OnlineErrorCode</c> codes (≤ -1001, disjoint from the legacy -1..-104 and
/// 660001..660005 bands) with no silent fallback and no successful empty lists masking a wrong
/// store or wrong process (the legacy chains retired with this cutover — there is no fallback
/// target). The scope triple comes only from the capability-published authorized-scope provider and
/// the player position only from the session-bound player id — request-body fields can never influence
/// the scope; nothing is cached — each request re-acquires the contract lease so no stale reference
/// survives a hotfix swap or host restart. The mapping tables: the 120 family keeps
/// -100/-101/-103/-104 (-102 "target absent" is dropped and disclosed); the 660 family keeps all of
/// 660001..660005 (660004 also covers the single-record convergence of "the other side requested
/// first", ruling R6); everything else (including the friend-limit StateOperationForbidden) uses the
/// explicit Online band.
/// </remarks>
public static class FriendDomainBridge
{
    /// <summary>120 族旧错误码：参数非法。</summary>
    private const int LegacyInvalidParameter = -100;

    /// <summary>120 族旧错误码：不能添加自己为好友。</summary>
    private const int LegacyCannotSelf = -101;

    /// <summary>120 族旧错误码：双方已是好友。</summary>
    private const int LegacyAlreadyFriends = -103;

    /// <summary>120 族旧错误码：好友关系不存在。</summary>
    private const int LegacyRelationNotFound = -104;

    /// <summary>120 族旧异常码：调用异常（与旧链 -2 一致）。</summary>
    private const int LegacyException = -2;

    /// <summary>
    /// 好友列表查询（120 族 ReqFriendList）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识（作用域玩家位唯一来源）。</param>
    /// <param name="response">协议响应。</param>
    public static async Task QueryFriendListAsync(long playerId, RespFriendList response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.Friends = new List<FriendInfo>();
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.ListFriendsAsync(scope);
            if (!result.IsSuccess)
            {
                response.Friends = new List<FriendInfo>();
                response.ErrorCode = -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            response.Friends = MapFriendInfos(result.Data);
            response.ErrorCode = 0;
        }
        catch (System.Exception exception)
        {
            response.Friends = new List<FriendInfo>();
            response.ErrorCode = LegacyException;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Friend.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 直接添加好友（120 族 ReqFriendByAdd；T2 裁定 R3：映射为域面 Establish = 申请 + 立即接受）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="targetPlayerId">目标玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task AddFriendDirectAsync(long playerId, long targetPlayerId, RespFriendByAdd response)
    {
        if (targetPlayerId <= 0)
        {
            response.Success = false;
            response.ErrorCode = LegacyInvalidParameter;
            return;
        }

        if (targetPlayerId == playerId)
        {
            response.Success = false;
            response.ErrorCode = LegacyCannotSelf;
            return;
        }

        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.Success = false;
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.EstablishAsync(scope, targetPlayerId);
            if (!result.IsSuccess)
            {
                response.Success = false;
                response.ErrorCode = result.Code == OnlineErrorCode.ParameterInvalid ? LegacyInvalidParameter : -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            if (result.Data.Before == OnlineFriendRelationKind.Friend)
            {
                // 旧链语义：重复添加已是好友 → -103（Success=false）。
                response.Success = false;
                response.ErrorCode = LegacyAlreadyFriends;
                return;
            }

            response.Success = result.Data.After == OnlineFriendRelationKind.Friend;
            response.ErrorCode = response.Success ? 0 : -(int)OnlineErrorCode.InternalError;
        }
        catch (System.Exception exception)
        {
            response.Success = false;
            response.ErrorCode = LegacyException;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Friend.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 删除好友（120 族 ReqDeleteFriend）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="targetPlayerId">对方玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task DeleteFriendAsync(long playerId, long targetPlayerId, RespDeleteFriend response)
    {
        if (targetPlayerId <= 0)
        {
            response.Success = false;
            response.ErrorCode = LegacyInvalidParameter;
            return;
        }

        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.Success = false;
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.RemoveAsync(scope, targetPlayerId);
            if (!result.IsSuccess)
            {
                response.Success = false;
                response.ErrorCode = result.Code == OnlineErrorCode.ResourceNotFound ? LegacyRelationNotFound : -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            response.Success = true;
            response.ErrorCode = 0;
        }
        catch (System.Exception exception)
        {
            response.Success = false;
            response.ErrorCode = LegacyException;
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Friend.DomainBridgeException, playerId));
        }
    }

    /// <summary>
    /// 发送好友申请（660 族 ReqFriendSendRequest；错误码映射 660002/660003/660004，裁定 R5/R6）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="toPlayerId">目标玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task SendRequestAsync(long playerId, long toPlayerId, RespFriendSendRequest response)
    {
        if (toPlayerId == playerId)
        {
            response.ErrorCode = (int)FriendErrorCode.CannotSelf;
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
            var result = await lease.RequestAsync(scope, toPlayerId);
            if (!result.IsSuccess)
            {
                response.ErrorCode = MapFamilyCodeFailure(result.Code);
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            switch (result.Data.Before)
            {
                case OnlineFriendRelationKind.Friend:
                    response.ErrorCode = (int)FriendErrorCode.AlreadyFriends;
                    return;
                case OnlineFriendRelationKind.IncomingRequest:
                case OnlineFriendRelationKind.OutgoingRequest:
                    // 裁定 R6：单记录模型下「对方已先申请」与「自己重复申请」同报申请等待中。
                    response.ErrorCode = (int)FriendErrorCode.RequestPending;
                    return;
                default:
                    response.ErrorCode = 0;
                    return;
            }
        }
        catch (System.Exception exception)
        {
            LogHelper.Fatal(exception);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }

    /// <summary>
    /// 接受好友申请（660 族 ReqFriendAccept；前置不满足映射 660005，裁定 R7）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="fromPlayerId">申请来源玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task AcceptAsync(long playerId, long fromPlayerId, RespFriendAccept response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.AcceptAsync(scope, fromPlayerId);
            response.ErrorCode = result.IsSuccess ? 0 : MapFamilyCodeFailure(result.Code);
            if (!result.IsSuccess)
            {
                LogBridgeFailed(result.Code, result.Message, playerId);
            }
        }
        catch (System.Exception exception)
        {
            LogHelper.Fatal(exception);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }

    /// <summary>
    /// 拒绝好友申请（660 族 ReqFriendReject；前置不满足映射 660005）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="fromPlayerId">申请来源玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task RejectAsync(long playerId, long fromPlayerId, RespFriendReject response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.RejectAsync(scope, fromPlayerId);
            response.ErrorCode = result.IsSuccess ? 0 : MapFamilyCodeFailure(result.Code);
            if (!result.IsSuccess)
            {
                LogBridgeFailed(result.Code, result.Message, playerId);
            }
        }
        catch (System.Exception exception)
        {
            LogHelper.Fatal(exception);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }

    /// <summary>
    /// 解除好友关系（660 族 ReqFriendRemove；非好友映射 660005，自解除映射 660002，裁定 R7）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="friendPlayerId">对方玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task RemoveAsync(long playerId, long friendPlayerId, RespFriendRemove response)
    {
        if (friendPlayerId == playerId)
        {
            response.ErrorCode = (int)FriendErrorCode.CannotSelf;
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
            var result = await lease.RemoveAsync(scope, friendPlayerId);
            response.ErrorCode = result.IsSuccess ? 0 : MapFamilyCodeFailure(result.Code);
            if (!result.IsSuccess)
            {
                LogBridgeFailed(result.Code, result.Message, playerId);
            }
        }
        catch (System.Exception exception)
        {
            LogHelper.Fatal(exception);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
        }
    }

    /// <summary>
    /// 好友关系全量列表（660 族 ReqFriendRelationList：好友 / 收到申请 / 发出申请）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="response">协议响应。</param>
    public static async Task ListRelationsAsync(long playerId, RespFriendRelationList response)
    {
        var lease = RequireLease(playerId, out var scope);
        if (lease == null)
        {
            response.ErrorCode = -(int)OnlineErrorCode.DependencyUnavailable;
            return;
        }

        try
        {
            var result = await lease.ListRelationsAsync(scope);
            if (!result.IsSuccess)
            {
                response.ErrorCode = -(int)result.Code;
                LogBridgeFailed(result.Code, result.Message, playerId);
                return;
            }

            foreach (var relation in result.Data)
            {
                switch (relation.Kind)
                {
                    case OnlineFriendRelationKind.Friend:
                        response.Friends.Add(relation.OtherPlayerId);
                        break;
                    case OnlineFriendRelationKind.IncomingRequest:
                        response.IncomingRequests.Add(relation.OtherPlayerId);
                        break;
                    case OnlineFriendRelationKind.OutgoingRequest:
                        response.OutgoingRequests.Add(relation.OtherPlayerId);
                        break;
                    default:
                        // 静止态不进任何列表（与旧档案三列表语义一致）。
                        break;
                }
            }

            response.ErrorCode = 0;
        }
        catch (System.Exception exception)
        {
            LogHelper.Fatal(exception);
            response.ErrorCode = (int)OperationStatusCode.InternalServerError;
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
    private static IOnlineFriendDomain RequireLease(long playerId, out OnlineScope scope)
    {
        scope = null;
        IOnlineFriendDomain contract = null;
        if (CapabilityBridge<IOnlineFriendDomain>.TryAcquire(out var lease))
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

        // Localization: Hotfix.Friend.DomainBridgeUnavailable - 好友域桥不可用（未发布或已撤销）, PlayerId: {0}, Code: {1}
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Friend.DomainBridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
        return null;
    }

    /// <summary>
    /// 660 族失败映射：资源不存在（申请 / 关系前置不满足）→ 660005；其余 → Online 显式段。
    /// </summary>
    /// <param name="code">域桥失败码。</param>
    /// <returns>660 族协议错误码。</returns>
    private static int MapFamilyCodeFailure(OnlineErrorCode code)
    {
        return code == OnlineErrorCode.ResourceNotFound ? (int)FriendErrorCode.RequestNotFound : -(int)code;
    }

    /// <summary>
    /// 域桥失败日志（Error 级；错误码已映射进响应，本日志供服务端归因）。
    /// </summary>
    /// <param name="code">失败码。</param>
    /// <param name="message">失败描述。</param>
    /// <param name="playerId">请求主体。</param>
    private static void LogBridgeFailed(OnlineErrorCode code, string message, long playerId)
    {
        // Localization: Hotfix.Friend.DomainBridgeFailed - 好友域桥返回失败, Code: {0}, Message: {1}, PlayerId: {2}
        LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Friend.DomainBridgeFailed, code, message, playerId));
    }

    /// <summary>
    /// 契约条目映射为协议条目（字段级转发，无语义衍生）。
    /// </summary>
    /// <param name="entries">契约条目。</param>
    /// <returns>协议好友条目。</returns>
    private static List<FriendInfo> MapFriendInfos(IReadOnlyList<OnlineFriendEntry> entries)
    {
        var friends = new List<FriendInfo>(entries.Count);
        foreach (var entry in entries)
        {
            friends.Add(new FriendInfo
            {
                PlayerId = entry.PlayerId,
                PlayerName = entry.Name,
            });
        }

        return friends;
    }
}
