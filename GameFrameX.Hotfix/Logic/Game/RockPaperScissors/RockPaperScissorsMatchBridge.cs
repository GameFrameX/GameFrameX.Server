// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and applicable international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legal rights and interests of any legal persons as prohibited by law!
//   因基于本项目二次开发所产生的一切纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本组织不承担任何责任，开发者应自行承担。
//   The organization and contributors shall not be responsible; developers shall bear the responsibility themselves.
//  ==========================================================================================

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Hotfix.Logic.Game.Room;

namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;

/// <summary>
/// 对局样例桥薄适配器（C245，沿 C243 好友桥形态（该缝已随 C244 域桥演化））：
/// online-match 能力启用（对局桥由能力模块发布，C294 起能力过滤器是唯一开关）时把石头剪刀布的
/// 房间制协议入口（开局 / 出拳 / 查询 / 再来一局）经 <see cref="CapabilityBridge{TContract}"/>
///（契约 <see cref="IOnlineMatchGameBridge"/>）进程内转发到 Online 对局契约面；能力缺席（缺省部署）时不接管，
/// 旧链行为逐字节保持。
/// <para>
/// 维护约束（红线）：
/// ① 桥在场时桥路径独占——桥不可用（宿主未启 / 已停止 / 租约撤销）与服务失败一律映射为
/// <c>-(int)OnlineErrorCode</c> 段的显式错误码（≤ -1001，与旧链 -1..-99 段不重叠），
/// 绝不静默回落旧链；② 可信身份：作用域三元组只取能力侧授权作用域提供者发布的组合根授权值，
/// 玩家位只取请求主体 ActorId，请求体字段一律不可影响作用域；③ 无缓存：逐请求
/// <see cref="CapabilityBridge{TContract}.TryAcquire"/> 取契约租约，热更 / 宿主重启后旧引用不存活；
/// ④ 本适配器零玩法逻辑——只做参数转发、
/// 字段级视图映射与 Room 状态映射决策，裁决与结算全部归属平台对局运行时。
/// </para>
/// </summary>
/// <remarks>
/// The sample match thin bridge adapter (C245, following the C243 friend-bridge shape (since evolved by the C244 domain bridge)): when the
/// online-match capability is enabled (the bridge published by the capability module — since C294 the
/// capability filter is the only switch) it forwards the rock-paper-scissors room-shaped protocol
/// entries (start / submit / query / restart) in-process to the Online match contract face through
/// <see cref="CapabilityBridge{TContract}"/> (contract <see cref="IOnlineMatchGameBridge"/>); when the
/// capability is absent (default) it does not intercept and the legacy chain stays byte-identical.
/// With the bridge present the path is exclusive — unavailability and service failures map to explicit
/// -(int)OnlineErrorCode codes (≤ -1001, disjoint from the legacy -1..-99 band), never silently
/// falling back; the scope triple comes only from the capability-published authorized-scope provider
/// and the player position only from the requesting actor id; nothing is cached — each request
/// re-acquires the contract lease; and the adapter carries zero gameplay logic — it only forwards
/// parameters, maps views field-by-field and decides room-state transitions, while adjudication and
/// settlement belong to the platform match runtime.
/// </remarks>
public static class RockPaperScissorsMatchBridge
{
    /// <summary>
    /// 获取对局桥是否在场接管（C294 起能力过滤器是唯一开关：online-match 启用即发布对局桥；
    /// 旧 <c>Online.EnableMatchGameBridge</c> 配置键已删除）。
    /// </summary>
    public static bool IsBridgeEnabled
    {
        get { return TryGetContract() != null; }
    }

    /// <summary>
    /// 尝试以桥路径开局（幂等：确定性对局标识）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识——作用域玩家位唯一来源。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">局号。</param>
    /// <param name="playerIds">本局玩家集合（房间成员表，首个为房主）。</param>
    /// <param name="response">协议响应（失败填充显式负值错误码；成功保持 0，房间状态推进由调用方旧链编排）。</param>
    /// <returns>开关开启返回 <c>true</c>（本次请求已由桥路径裁决，含显式失败）；
    /// 开关关闭返回 <c>false</c>（未接管，响应未被触碰）。</returns>
    public static async Task<bool> TryStartRoomGameAsync(long playerId, long roomId, int round, IReadOnlyList<long> playerIds, RespStartRoomGame response)
    {
        if (!IsBridgeEnabled)
        {
            return false;
        }

        var (errorCode, _) = await InvokeAsync(playerId, (lease, scope) => lease.StartRoomGameAsync(scope, roomId, round, playerIds));
        if (errorCode != 0)
        {
            response.ErrorCode = errorCode;
        }

        return true;
    }

    /// <summary>
    /// 尝试以桥路径提交一次出拳意图（服务端裁决；保密投影视图回填）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识——作用域玩家位唯一来源。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">局号。</param>
    /// <param name="gesture">出拳值（1 = 石头，2 = 剪刀，3 = 布）。</param>
    /// <param name="response">协议响应（成功填充映射视图与 0；失败填充显式负值错误码且 GameInfo 置空）。</param>
    /// <returns>开关关闭返回 <c>Handled=false</c>（未接管，响应未被触碰）；
    /// 开启返回 <c>Handled=true</c>，成功时 <c>View</c> 为契约视图（供调用方做 Room 状态映射与全员推送），失败时为 <c>null</c>。</returns>
    public static async Task<(bool Handled, OnlineMatchBridgeGameView View)> TrySubmitGestureAsync(long playerId, long roomId, int round, int gesture, RespSubmitRockPaperScissorsGesture response)
    {
        if (!IsBridgeEnabled)
        {
            return (false, null);
        }

        var (errorCode, view) = await InvokeAsync(playerId, (lease, scope) => lease.SubmitGestureAsync(scope, roomId, round, gesture));
        if (errorCode != 0)
        {
            response.GameInfo = null;
            response.ErrorCode = errorCode;
            return (true, null);
        }

        response.GameInfo = MapGameInfo(view);
        response.ErrorCode = 0;
        return (true, view);
    }

    /// <summary>
    /// 尝试以桥路径查询对局视图（保密投影；客户端经本查询收敛 tick 驱动的变更）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识——作用域玩家位唯一来源。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">局号。</param>
    /// <param name="response">协议响应（成功填充映射视图与 0；失败填充显式负值错误码且 GameInfo 置空）。</param>
    /// <returns>开关关闭返回 <c>Handled=false</c>（未接管，响应未被触碰）；
    /// 开启返回 <c>Handled=true</c>，成功时 <c>View</c> 为契约视图（供调用方做 Room 状态映射），失败时为 <c>null</c>。</returns>
    public static async Task<(bool Handled, OnlineMatchBridgeGameView View)> TryGetGameInfoAsync(long playerId, long roomId, int round, RespRockPaperScissorsGameInfo response)
    {
        if (!IsBridgeEnabled)
        {
            return (false, null);
        }

        var (errorCode, view) = await InvokeAsync(playerId, (lease, scope) => lease.GetGameInfoAsync(scope, roomId, round));
        if (errorCode != 0)
        {
            response.GameInfo = null;
            response.ErrorCode = errorCode;
            return (true, null);
        }

        response.GameInfo = MapGameInfo(view);
        response.ErrorCode = 0;
        return (true, view);
    }

    /// <summary>
    /// 尝试以桥路径开启下一局（再来一局语义 = 以新局号创建新对局，幂等；旧局终态由平台 tick 释放）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识——作用域玩家位唯一来源。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">新局号（房间当前局号 + 1）。</param>
    /// <param name="playerIds">本局玩家集合。</param>
    /// <param name="response">协议响应（失败填充显式负值错误码且 GameInfo 置空；成功保持 0，GameInfo 由调用方在房间推进后回填）。</param>
    /// <returns>开关关闭返回 <c>Handled=false</c>（未接管，响应未被触碰）；
    /// 开启返回 <c>Handled=true</c>，成功时 <c>View</c> 为新对局视图，失败时为 <c>null</c>。</returns>
    public static async Task<(bool Handled, OnlineMatchBridgeGameView View)> TryStartNextRoundAsync(long playerId, long roomId, int round, IReadOnlyList<long> playerIds, RespRestartRockPaperScissorsGame response)
    {
        if (!IsBridgeEnabled)
        {
            return (false, null);
        }

        var (errorCode, view) = await InvokeAsync(playerId, (lease, scope) => lease.StartRoomGameAsync(scope, roomId, round, playerIds));
        if (errorCode != 0)
        {
            response.GameInfo = null;
            response.ErrorCode = errorCode;
            return (true, null);
        }

        response.ErrorCode = 0;
        return (true, view);
    }

    /// <summary>
    /// 契约视图映射为协议游戏信息（字段级转发，无语义衍生）。
    /// </summary>
    /// <param name="view">契约视图（保密投影由玩法插件产出）。</param>
    /// <returns>协议游戏信息；<paramref name="view"/> 为 null 时返回 null。</returns>
    /// <remarks>出拳保密：未结算局契约侧 Gesture 已为 0（None），映射原样转发；
    /// 视图携带的 Wins 计分不外发——现网协议无比分字段（样例能力边界，notes §1.4）。</remarks>
    public static RockPaperScissorsGameInfo MapGameInfo(OnlineMatchBridgeGameView view)
    {
        if (view == null)
        {
            return null;
        }

        var players = new List<RockPaperScissorsPlayerInfo>(view.Players.Count);
        foreach (var player in view.Players)
        {
            players.Add(new RockPaperScissorsPlayerInfo
            {
                PlayerId = player.PlayerId,
                HasGesture = player.HasGesture,
                Gesture = (RockPaperScissorsGesture)player.Gesture,
            });
        }

        return new RockPaperScissorsGameInfo
        {
            RoomId = view.RoomId,
            Round = view.Round,
            WinnerPlayerId = view.WinnerPlayerId,
            Players = players,
        };
    }

    /// <summary>
    /// Room 状态映射决策（纯函数，notes §1.3）：桥视图状态与房间当前状态的组合决定
    /// 房间侧唯一允许的状态推进动作。
    /// </summary>
    /// <param name="viewState">桥视图生命周期状态。</param>
    /// <param name="roomStatus">房间当前状态。</param>
    /// <returns>状态推进动作（None = 不推进；EnterSettling = Playing → Settling；MarkSettled = Playing/Settling → Settled）。</returns>
    public static RockPaperScissorsMatchRoomAction ComputeRoomAction(OnlineMatchBridgeGameState viewState, RoomStatus roomStatus)
    {
        if (viewState == OnlineMatchBridgeGameState.Settling && roomStatus == RoomStatus.Playing)
        {
            return RockPaperScissorsMatchRoomAction.EnterSettling;
        }

        var isTerminal = viewState == OnlineMatchBridgeGameState.Completed ||
                         viewState == OnlineMatchBridgeGameState.SettlementFailed ||
                         viewState == OnlineMatchBridgeGameState.Terminated;
        if (isTerminal && (roomStatus == RoomStatus.Playing || roomStatus == RoomStatus.Settling))
        {
            return RockPaperScissorsMatchRoomAction.MarkSettled;
        }

        return RockPaperScissorsMatchRoomAction.None;
    }

    /// <summary>
    /// 观察到桥视图后的 Room 状态映射执行（按 <see cref="ComputeRoomAction"/> 决策驱动房间代理推进；
    /// 不满足推进条件时不触碰房间状态）。
    /// </summary>
    /// <param name="roomAgent">房间组件代理。</param>
    /// <param name="room">房间状态（只读引用；本方法不改其字段，推进经房间代理服务方法）。</param>
    /// <param name="view">桥视图。</param>
    /// <returns>异步任务。</returns>
    public static async Task ApplyRoomStateAsync(RoomComponentAgent roomAgent, GameFrameX.Apps.Game.Room.Entity.RoomState room, OnlineMatchBridgeGameView view)
    {
        if (roomAgent == null || room == null || view == null)
        {
            return;
        }

        switch (ComputeRoomAction(view.State, room.Status))
        {
            case RockPaperScissorsMatchRoomAction.EnterSettling:
                await roomAgent.EnterSettlingAsync(room.RoomId);
                break;
            case RockPaperScissorsMatchRoomAction.MarkSettled:
                await roomAgent.MarkSettledAsync(room.RoomId);
                break;
        }
    }

    /// <summary>
    /// 统一调用核心：lease 逐请求读取与活跃位复查、作用域组装（三元组 = lease 授权值、玩家位 = 请求主体）、
    /// 契约失败与异常的显式错误码映射与日志（与好友桥同形态）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识。</param>
    /// <param name="invoke">契约调用体（接收 lease 与组装好的作用域）。</param>
    /// <returns>(错误码, 视图)：错误码 0 = 成功（视图非空）；非 0 为显式负值段错误码（视图为 null）。</returns>
    private static async Task<(int ErrorCode, OnlineMatchBridgeGameView View)> InvokeAsync(long playerId, Func<IOnlineMatchGameBridge, OnlineScope, Task<OnlineResult<OnlineMatchBridgeGameView>>> invoke)
    {
        var contract = TryGetContract();
        if (contract == null)
        {
            // Localization: Hotfix.Game.MatchBridgeUnavailable - 对局桥不可用（未发布或已撤销）, ActorId: {0}, Code: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Game.MatchBridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
            return (-(int)OnlineErrorCode.DependencyUnavailable, null);
        }

        // 授权作用域不可得 = 依赖不可用：与对局桥缺席同口径显式失败，不以空作用域调用契约、不降级。
        var scope = OnlineAuthorizedScopeReader.BuildRequestScope(playerId);
        if (scope == null)
        {
            // Localization: Hotfix.Game.MatchBridgeUnavailable - 对局桥不可用（未发布或已撤销）, ActorId: {0}, Code: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Game.MatchBridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
            return (-(int)OnlineErrorCode.DependencyUnavailable, null);
        }

        try
        {
            var result = await invoke(contract, scope);
            if (!result.IsSuccess)
            {
                // Localization: Hotfix.Game.MatchBridgeFailed - 对局桥返回失败, Code: {0}, Message: {1}, ActorId: {2}
                LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Game.MatchBridgeFailed, result.Code, result.Message, playerId));
                return (-(int)result.Code, null);
            }

            return (0, result.Data);
        }
        catch (System.Exception exception)
        {
            // Localization: Hotfix.Game.MatchBridgeException - 对局桥调用异常, ActorId: {0}
            LogHelper.Error(exception, LocalizationService.GetString(Localization.Keys.Hotfix.Game.MatchBridgeException, playerId));
            return (-2, null);
        }
    }

    /// <summary>
    /// 读取对局桥契约（逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/>，不缓存；
    /// 缺席 / 撤销竞态返回 null——缺席 = 能力未装配的合法部署形态，撤销按依赖不可用显式失败）。
    /// </summary>
    /// <returns>契约面；桥不可用返回 null。</returns>
    private static IOnlineMatchGameBridge TryGetContract()
    {
        if (CapabilityBridge<IOnlineMatchGameBridge>.TryAcquire(out var lease))
        {
            try
            {
                return lease.Contract;
            }
            catch (CapabilityBridgeUnavailableException)
            {
                // 获取与读取之间的撤销竞态：与缺席同语义（显式失败），不降级。
            }
        }

        return null;
    }
}

