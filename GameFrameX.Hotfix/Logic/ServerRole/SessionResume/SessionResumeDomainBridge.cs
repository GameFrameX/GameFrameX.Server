// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//
//  使用本项目须严格遵守相应法律法规与开源许可证之规定。
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please see the LICENSE file at the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  侵犯公民合法权益等法律法规所禁止的行为！
//  Any disputes and liabilities arising from secondary development of this project
//  shall be borne by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository:  https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
// ==========================================================================================

using System.Threading.Tasks;
using GameFrameX.Apps.Common.EventData;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Core.Events;
using GameFrameX.Core.Session;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Hotfix.Logic.Game.Room;
using GameFrameX.Hotfix.Logic.Player.Attribute;
using GameFrameX.Hotfix.Logic.Player.Mail;
using GameFrameX.Network.Abstractions;
using GameFrameX.Proto.Proto;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.Logic.ServerRole.SessionResume;

/// <summary>
/// 会话恢复域桥薄适配器（220-18 协议入口的唯一编排点）：Core 物理连接会话与 Online
/// 逻辑会话 / 对局恢复的协同转发——登录签发凭据、新连接恢复（验证 + Core 绑定 + 顶号编排 + 对局恢复）、
/// 断线两层协同通知。
/// <para>
/// 维护约束（红线）：
/// ① 桥独占——会话域桥 / 匹配域桥不可用（宿主未启 / 已停止 / 租约撤销）时：登录签发静默降级
///（凭据为空，登录主流程不受影响）、恢复请求显式失败（-(int)OnlineErrorCode 段，≤ -1001），
/// 绝不静默回落；② 可信身份：作用域三元组只取租约授权值，恢复主体只取 Token 解析结果
///（请求体字段一律不可影响作用域与主体）；③ 无缓存：逐请求读取 holder 当前租约；
/// ④ 恢复序：身份（Token）→ Core 绑定（UpdateSession 顶号编排复用）→ 业务在线簿记还原 →
/// 对局恢复（成员表核对 + 快照 / 增量）——身份失败即整体失败，对局失败只降级 Recovered 并回显错误码。
/// </para>
/// </summary>
internal static class SessionResumeDomainBridge
{
    /// <summary>
    /// 断线重连窗口（秒）。与 <c>OnlineMatchRuntimeOptions.ReconnectWindowSeconds</c> 缺省值对齐——
    /// 会话窗与对局窗同长，收敛口径单一；对局侧窗口仍由对局运行时自行裁决。
    /// </summary>
    internal const int ReconnectWindowSeconds = 30;

    /// <summary>
    /// 登录签发会话恢复凭据（断线前凭据交付）。
    /// </summary>
    /// <param name="playerId">会话绑定玩家标识。</param>
    /// <param name="connectionId">当前物理连接标识（Core SessionId）。</param>
    /// <returns>Token 明文；桥不可用或签发失败返回 null（登录主流程不受影响，日志披露）。</returns>
    public static async Task<string> IssueLoginCredentialAsync(long playerId, string connectionId)
    {
        var lease = RequireSessionContract(playerId);
        if (lease == null)
        {
            return null;
        }

        // 授权作用域不可得 = 依赖不可用：与会话域桥缺席同形态返回空凭据并披露，不以空作用域调用契约。
        var issueScope = BuildScope(playerId);
        if (issueScope == null)
        {
            // Localization: Hotfix.SessionResume.BridgeUnavailable - 会话域桥不可用（未发布或已撤销）, PlayerId: {0}, Code: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.BridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
            return null;
        }

        var issued = await lease.IssueForLoginAsync(issueScope, connectionId);
        if (!issued.IsSuccess)
        {
            // Localization: Hotfix.SessionResume.IssueFailed - 登录签发会话恢复凭据失败, Code: {0}, PlayerId: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.IssueFailed, issued.Code, playerId));
            return null;
        }

        return issued.Data.Token;
    }

    /// <summary>
    /// 新连接会话恢复编排：Token 验证与轮换 → Core 会话绑定（顶号编排复用）→ 业务在线簿记还原 →
    /// 对局归属核对与恢复。
    /// </summary>
    /// <param name="networkChannel">新连接的网络通道。</param>
    /// <param name="token">登录 / 上次恢复下发的会话凭据明文。</param>
    /// <param name="lastAcknowledgedSequence">客户端最后确认的服务器序号（对局增量下界）。</param>
    /// <returns>恢复结果（错误码 0 = 身份恢复成功；对局失败只降级 Recovered 并回显对局错误码）。</returns>
    public static async Task<SessionResumeOutcome> ResumeAsync(INetworkChannel networkChannel, string token, long lastAcknowledgedSequence)
    {
        var sessionId = networkChannel.GameAppSession?.SessionId;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return SessionResumeOutcome.Fail(OnlineErrorCode.ParameterInvalid, "连接会话标识缺失");
        }

        var lease = RequireSessionContract(0);
        if (lease == null)
        {
            return SessionResumeOutcome.Fail(OnlineErrorCode.DependencyUnavailable, "会话域桥不可用（宿主未运行或已停止）");
        }

        // 恢复主体由 Token 解析得出（玩家位 0 = 待解析；请求体字段不可影响作用域）。
        var resumeScope = BuildScope(0);
        if (resumeScope == null)
        {
            // 授权作用域不可得 = 依赖不可用：身份恢复显式失败，不以空作用域调用契约、不降级。
            // Localization: Hotfix.SessionResume.BridgeUnavailable - 会话域桥不可用（未发布或已撤销）, PlayerId: {0}, Code: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.BridgeUnavailable, 0, OnlineErrorCode.DependencyUnavailable));
            return SessionResumeOutcome.Fail(OnlineErrorCode.DependencyUnavailable, "授权作用域不可得（作用域提供者桥未发布或已撤销）");
        }

        var resumed = await lease.ResumeAsync(resumeScope, token, sessionId);
        if (!resumed.IsSuccess)
        {
            return SessionResumeOutcome.Fail(resumed.Code, resumed.Message);
        }

        var playerId = resumed.Data.PlayerId;

        // Core 绑定：通道 ActorId + 会话玩家位（含旧僵尸连接的顶号编排：通知 → Close → Remove → 绑定）。
        networkChannel.SetData(GlobalConst.ActorIdKey, playerId);
        await PlayerSessionManager.Instance.UpdateSessionAsync(sessionId, playerId, playerId.ToString(), NotifyDuplicateLoginAsync);

        // 业务在线还原（镜像登录尾段：OnRoleOnline + Room 重连标记）。
        EventDispatcher.Dispatch(playerId, new OnRoleOnlineEventArgs());
        var roomComp = await ActorManager.GetComponentAgentAsync<RoomComponentAgent>();
        await roomComp.MarkPlayerReconnectedAsync(playerId);

        // 属性快照 + 邮件懒同步（客户端重连后状态重建，与登录同口径）。
        var attributeComponentAgent = await ActorManager.GetComponentAgentAsync<PlayerAttributeComponentAgent>(playerId);
        await networkChannel.WriteAsync(attributeComponentAgent.BuildSyncSnapshot());
        var mailAgent = await ActorManager.GetComponentAgentAsync<MailComponentAgent>(playerId);
        await mailAgent.SyncAsync();

        // 对局归属核对与恢复（无对局 = Resumed=false 空回执，非失败；失败只降级 Recovered）。
        OnlineMatchDomainResume matchResume = null;
        var matchFailureCode = OnlineErrorCode.None;
        var matchFailureMessage = string.Empty;
        var matchContract = TryGetMatchContract();
        if (matchContract == null)
        {
            // 匹配域桥缺席（online-match 被排除的部署形态）：身份恢复仍成立，对局恢复面缺位披露。
            matchFailureMessage = "匹配域桥不可用（宿主未运行或已停止）";
            LogBridgeFailed(OnlineErrorCode.DependencyUnavailable, matchFailureMessage, playerId);
        }
        else
        {
            var matchScope = BuildScope(playerId);
            if (matchScope == null)
            {
                // 授权作用域不可得 = 依赖不可用：对局恢复按失败回显（身份恢复仍成立），不以空作用域调用契约。
                matchFailureCode = OnlineErrorCode.DependencyUnavailable;
                matchFailureMessage = "授权作用域不可得（作用域提供者桥未发布或已撤销）";
                LogBridgeFailed(matchFailureCode, matchFailureMessage, playerId);
            }
            else
            {
                var outcome = await matchContract.TryResumeForPlayerAsync(matchScope, lastAcknowledgedSequence);
                if (outcome.IsSuccess)
                {
                    matchResume = outcome.Data;
                }
                else
                {
                    matchFailureCode = outcome.Code;
                    matchFailureMessage = outcome.Message;
                    LogBridgeFailed(outcome.Code, outcome.Message, playerId);
                }
            }
        }

        return SessionResumeOutcome.Ok(playerId, resumed.Data.NewToken, matchResume, matchFailureCode, matchFailureMessage);
    }

    /// <summary>
    /// 断线两层协同：物理断开后通知 Online 会话域开重连窗、对局域标记断线成员。
    /// 旧回调迟到守卫——玩家已持新会话（恢复先于旧回调到达）时跳过，不误开窗 / 不误标记重连成员。
    /// </summary>
    /// <param name="playerId">断线玩家标识。</param>
    /// <param name="disconnectedSessionId">断开的物理连接会话标识。</param>
    /// <returns>异步任务。</returns>
    public static async Task NotifyPlayerDisconnectedAsync(long playerId, string disconnectedSessionId)
    {
        var currentSession = PlayerSessionManager.Instance.GetByPlayerId(playerId);
        if (currentSession != null && currentSession.SessionId != disconnectedSessionId)
        {
            // Localization: Hotfix.SessionResume.DisconnectStaleSkipped - 断线跳过协同（旧回调迟到，玩家已有新会话）, PlayerId: {0}
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.DisconnectStaleSkipped, playerId));
            return;
        }

        var sessionOutcome = await NotifySessionDomainDisconnectedAsync(playerId);
        var matchOutcome = await NotifyMatchDomainDisconnectedAsync(playerId);

        // Localization: Hotfix.SessionResume.DisconnectCoordination - 断线两层协同, PlayerId: {0}, 会话域结果: {1}, 对局域结果: {2}
        LogHelper.Info(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.DisconnectCoordination, playerId, sessionOutcome, matchOutcome));
    }

    /// <summary>
    /// 断线协同会话域段：桥在位时通知 Online 会话域开重连窗，产出结果描述（skipped / ok / failed:&lt;Code&gt;）。
    /// </summary>
    /// <param name="playerId">断线玩家标识。</param>
    /// <returns>会话域结果描述（桥缺席为 skipped）。</returns>
    private static async Task<string> NotifySessionDomainDisconnectedAsync(long playerId)
    {
        var sessionOutcome = "skipped";
        var sessionContract = TryGetSessionContract();
        if (sessionContract != null)
        {
            var sessionScope = BuildScope(playerId);
            if (sessionScope == null)
            {
                // 授权作用域不可得 = 依赖不可用：显式记失败并跳过开窗，不以空作用域调用契约。
                sessionOutcome = "failed:" + OnlineErrorCode.DependencyUnavailable;
                // Localization: Hotfix.SessionResume.BridgeUnavailable - 会话域桥不可用（未发布或已撤销）, PlayerId: {0}, Code: {1}
                LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.BridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
            }
            else
            {
                var notified = await sessionContract.NotifyPlayerDisconnectedAsync(sessionScope, ReconnectWindowSeconds);
                sessionOutcome = notified.IsSuccess ? "ok" : "failed:" + notified.Code;
            }
        }

        return sessionOutcome;
    }

    /// <summary>
    /// 断线协同对局域段：桥在位时通知对局域标记断线成员，产出结果描述（skipped / marked:&lt;MatchId&gt; / no-match / failed:&lt;Code&gt;）。
    /// </summary>
    /// <param name="playerId">断线玩家标识。</param>
    /// <returns>对局域结果描述（桥缺席为 skipped）。</returns>
    private static async Task<string> NotifyMatchDomainDisconnectedAsync(long playerId)
    {
        var matchOutcome = "skipped";
        var matchContract = TryGetMatchContract();
        if (matchContract != null)
        {
            var matchScope = BuildScope(playerId);
            if (matchScope == null)
            {
                // 授权作用域不可得 = 依赖不可用：显式记失败并跳过断线标记，不以空作用域调用契约。
                matchOutcome = "failed:" + OnlineErrorCode.DependencyUnavailable;
                LogBridgeFailed(OnlineErrorCode.DependencyUnavailable, "授权作用域不可得（作用域提供者桥未发布或已撤销）", playerId);
            }
            else
            {
                var notified = await matchContract.NotifyPlayerDisconnectedAsync(matchScope);
                matchOutcome = notified.IsSuccess
                    ? notified.Data != null && notified.Data.Marked ? "marked:" + notified.Data.MatchId : "no-match"
                    : "failed:" + notified.Code;
            }
        }

        return matchOutcome;
    }

    /// <summary>
    /// 顶号通知器：向被顶掉的旧会话发送"账号已在其他设备登录"提示（与登录路径同文案同形态；
    /// 顶号编排封装在 PlayerSessionManager 内，通知器须在 Close 前内联 await）。
    /// </summary>
    /// <param name="oldSession">被顶掉的旧会话。</param>
    private static Task NotifyDuplicateLoginAsync(IPlayerSession oldSession)
    {
        var msg = new RespPrompt
        {
            Type = 5,
            Content = LocalizationService.GetString(Localization.Keys.Apps.SessionManager.AccountAlreadyLoggedIn),
        };
        return oldSession.WriteAsync(msg);
    }

    /// <summary>
    /// 读取会话域桥契约（不可用即显式失败记录；逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/>，不缓存）。
    /// </summary>
    /// <param name="playerId">请求主体（日志披露用）。</param>
    /// <returns>契约面；桥不可用返回 null。</returns>
    private static IOnlineSessionReconnectContract RequireSessionContract(long playerId)
    {
        var contract = TryGetSessionContract();
        if (contract == null)
        {
            // Localization: Hotfix.SessionResume.BridgeUnavailable - 会话域桥不可用（未发布或已撤销）, PlayerId: {0}, Code: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.SessionResume.BridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
        }

        return contract;
    }

    /// <summary>
    /// 读取会话域桥契约（缺席 / 撤销竞态返回 null，不打日志——缺席语义由调用方决定）。
    /// </summary>
    /// <returns>契约面；桥不可用返回 null。</returns>
    private static IOnlineSessionReconnectContract TryGetSessionContract()
    {
        if (CapabilityBridge<IOnlineSessionReconnectContract>.TryAcquire(out var lease))
        {
            try
            {
                return lease.Contract;
            }
            catch (CapabilityBridgeUnavailableException)
            {
                // 获取与读取之间的撤销竞态：与缺席同语义，不降级。
            }
        }

        return null;
    }

    /// <summary>
    /// 读取匹配域桥契约（缺席 / 撤销竞态返回 null——online-match 为可选能力，缺席为合法部署形态）。
    /// </summary>
    /// <returns>契约面；桥不可用返回 null。</returns>
    private static IOnlineMatchDomain TryGetMatchContract()
    {
        if (CapabilityBridge<IOnlineMatchDomain>.TryAcquire(out var lease))
        {
            try
            {
                return lease.Contract;
            }
            catch (CapabilityBridgeUnavailableException)
            {
                // 获取与读取之间的撤销竞态：与缺席同语义，不降级。
            }
        }

        return null;
    }

    /// <summary>
    /// 以授权三元组 + 玩家位构造作用域（三元组经 <see cref="OnlineAuthorizedScopeReader"/> 读取；
    /// 提供者桥缺席 / 撤销时返回 null，调用方须按依赖不可用显式失败，不得把 null 传入契约）。
    /// </summary>
    /// <param name="playerId">玩家标识（恢复前为 0 = 待 Token 解析）。</param>
    /// <returns>生效作用域；授权作用域不可得时为 null。</returns>
    private static OnlineScope BuildScope(long playerId)
    {
        return OnlineAuthorizedScopeReader.BuildRequestScope(playerId);
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

    /// <summary>
    /// 会话恢复编排结果（220-19 响应的填充源）。
    /// </summary>
    internal sealed class SessionResumeOutcome
    {
        /// <summary>是否身份恢复成功（失败码见 <see cref="FailureCode"/>）。</summary>
        public bool Succeeded { get; private set; }

        /// <summary>身份失败码（成功时为 None；对局失败码见 <see cref="MatchFailureCode"/>）。</summary>
        public OnlineErrorCode FailureCode { get; private set; }

        /// <summary>身份失败描述。</summary>
        public string FailureMessage { get; private set; }

        /// <summary>恢复的玩家标识。</summary>
        public long PlayerId { get; private set; }

        /// <summary>轮换后的新会话凭据。</summary>
        public string NewToken { get; private set; }

        /// <summary>对局恢复回执（无对局时为 null 且 <see cref="MatchFailureCode"/> = None）。</summary>
        public OnlineMatchDomainResume MatchResume { get; private set; }

        /// <summary>对局恢复失败码（身份成功但窗口超时 / 已结束等；无失败为 None）。</summary>
        public OnlineErrorCode MatchFailureCode { get; private set; }

        /// <summary>对局恢复失败描述。</summary>
        public string MatchFailureMessage { get; private set; }

        /// <summary>
        /// 构造失败结果。
        /// </summary>
        /// <param name="code">失败码。</param>
        /// <param name="message">失败描述。</param>
        /// <returns>失败结果。</returns>
        public static SessionResumeOutcome Fail(OnlineErrorCode code, string message)
        {
            return new SessionResumeOutcome { Succeeded = false, FailureCode = code, FailureMessage = message, MatchFailureCode = OnlineErrorCode.None };
        }

        /// <summary>
        /// 构造成功结果。
        /// </summary>
        /// <param name="playerId">恢复的玩家标识。</param>
        /// <param name="newToken">轮换后的新凭据。</param>
        /// <param name="matchResume">对局恢复回执（可空）。</param>
        /// <param name="matchFailureCode">对局恢复失败码。</param>
        /// <param name="matchFailureMessage">对局恢复失败描述。</param>
        /// <returns>成功结果。</returns>
        public static SessionResumeOutcome Ok(long playerId, string newToken, OnlineMatchDomainResume matchResume, OnlineErrorCode matchFailureCode, string matchFailureMessage)
        {
            return new SessionResumeOutcome
            {
                Succeeded = true,
                FailureCode = OnlineErrorCode.None,
                PlayerId = playerId,
                NewToken = newToken,
                MatchResume = matchResume,
                MatchFailureCode = matchFailureCode,
                MatchFailureMessage = matchFailureMessage ?? string.Empty,
            };
        }
    }
}
