// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source code for the full license text.
//  ==========================================================================================

using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Hotfix.Logic.Game.Room;

namespace GameFrameX.Hotfix.Logic.Game.ConnectFour;

/// <summary>
/// 四子棋对局桥薄适配器：把四子棋的房间制协议入口（开局 / 落子 / 查询 / 再来一局）经
/// <see cref="CapabilityBridge{TContract}"/>（契约 <see cref="IOnlineMatchGameBridge"/>）进程内转发到
/// Online 对局运行时——本仓旗舰玩法的最短闭环演示：平台裁决 → 结算（结果先落定）→
/// 结算 outbox 经统一资产入口发奖 → 对局结算投影写入默认榜。
/// <para>
/// 桥接语义（维护约束 / 红线）：
/// ① 桥在场时桥路径独占——桥不可用与服务失败一律映射为 <c>-(int)OnlineErrorCode</c> 段的显式
/// 错误码，绝不静默回落本地链（本地链仅在能力未装配、桥未发布时承载）；
/// ② 可信身份——作用域三元组只取能力侧授权作用域提供者发布的组合根授权值，
/// 玩家位只取请求主体 ActorId，落子请求体字段一律不可影响作用域；
/// ③ 无缓存——逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/> 取契约租约；
/// ④ 本适配器零玩法裁决——合法性（回合 / 列满 / 四连）全部由平台对局运行时权威裁决，
/// 协议层的本地棋盘只是回显镜像（视图投影，非权威），胜负与发奖只认平台结算事实。
/// </para>
/// </summary>
public static class ConnectFourMatchBridge
{
    /// <summary>
    /// 获取对局桥是否在场接管（能力过滤器是唯一开关：online-match 启用即发布对局桥）。
    /// </summary>
    public static bool IsBridgeEnabled
    {
        get { return TryGetContract() != null; }
    }

    /// <summary>
    /// 尝试以桥路径开局（幂等：确定性对局标识，同房间同局号复用既有对局）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识——作用域玩家位唯一来源。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">局号。</param>
    /// <param name="playerIds">本局玩家集合（房间成员表，首个为房主）。</param>
    /// <param name="response">协议响应（失败填充显式负值错误码；成功保持 0）。</param>
    /// <returns>桥接管返回 <c>true</c>；桥未发布返回 <c>false</c>（本地链承载）。</returns>
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
    /// 尝试以桥路径提交一次落子（服务端权威裁决：回合校验、重力落子、四连与平局判定；
    /// 列号经输入意图通道透传，任何请求体结果字段一律不参与裁决）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识——作用域玩家位唯一来源。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">局号。</param>
    /// <param name="column">目标列号（0 起）。</param>
    /// <returns>桥未发布返回 <c>(false, 0, null)</c>；接管返回 <c>(true, 错误码, 视图)</c>（错误码 0 = 成功且视图非空；失败时视图为 null，错误码为桥映射后的显式负值，由调用方经响应回填）。</returns>
    public static async Task<(bool Handled, int ErrorCode, OnlineMatchBridgeGameView View)> TrySubmitMoveAsync(long playerId, long roomId, int round, int column)
    {
        if (!IsBridgeEnabled)
        {
            return (false, 0, null);
        }

        var (errorCode, view) = await InvokeAsync(playerId, (lease, scope) => lease.SubmitGestureAsync(scope, roomId, round, column));
        return (true, errorCode, errorCode == 0 ? view : null);
    }

    /// <summary>
    /// 尝试以桥路径查询对局视图（生命周期与胜负权威投影；协议层据此驱动房间状态映射）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">局号。</param>
    /// <returns>桥未发布返回 <c>(false, 0, null)</c>；接管返回 <c>(true, 错误码, 视图)</c>（错误码 0 = 成功且视图非空；失败时视图为 null，错误码须显式回传客户端，不得静默吞掉）。</returns>
    public static async Task<(bool Handled, int ErrorCode, OnlineMatchBridgeGameView View)> TryGetGameInfoAsync(long playerId, long roomId, int round)
    {
        if (!IsBridgeEnabled)
        {
            return (false, 0, null);
        }

        var (errorCode, view) = await InvokeAsync(playerId, (lease, scope) => lease.GetGameInfoAsync(scope, roomId, round));
        return (true, errorCode, errorCode == 0 ? view : null);
    }

    /// <summary>
    /// 尝试以桥路径开启下一局（再来一局语义 = 以新局号创建新对局，幂等；旧局终态由平台 tick 释放）。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识。</param>
    /// <param name="roomId">房间标识。</param>
    /// <param name="round">新局号（房间当前局号 + 1）。</param>
    /// <param name="playerIds">本局玩家集合。</param>
    /// <returns>桥未发布返回 <c>(false, 0, null)</c>；接管返回 <c>(true, 错误码, 视图)</c>（错误码 0 = 成功且视图非空；失败时视图为 null，错误码须显式回传客户端）。</returns>
    public static async Task<(bool Handled, int ErrorCode, OnlineMatchBridgeGameView View)> TryStartNextRoundAsync(long playerId, long roomId, int round, IReadOnlyList<long> playerIds)
    {
        if (!IsBridgeEnabled)
        {
            return (false, 0, null);
        }

        var (errorCode, view) = await InvokeAsync(playerId, (lease, scope) => lease.StartRoomGameAsync(scope, roomId, round, playerIds));
        return (true, errorCode, errorCode == 0 ? view : null);
    }

    /// <summary>
    /// Room 状态映射决策（纯函数）：桥视图生命周期状态与房间当前状态的组合决定房间侧
    /// 唯一允许的状态推进动作（对局运行中 → 房间保持；桥进入结算 → 房间 Settling；桥终态 → 房间 Settled）。
    /// </summary>
    /// <param name="viewState">桥视图生命周期状态。</param>
    /// <param name="roomStatus">房间当前状态。</param>
    /// <returns>状态推进动作（0 = 不推进；1 = Playing → Settling；2 = Playing/Settling → Settled）。</returns>
    public static int ComputeRoomAction(OnlineMatchBridgeGameState viewState, RoomStatus roomStatus)
    {
        if (viewState == OnlineMatchBridgeGameState.Settling && roomStatus == RoomStatus.Playing)
        {
            return 1;
        }

        var isTerminal = viewState == OnlineMatchBridgeGameState.Completed ||
                         viewState == OnlineMatchBridgeGameState.SettlementFailed ||
                         viewState == OnlineMatchBridgeGameState.Terminated;
        if (isTerminal && (roomStatus == RoomStatus.Playing || roomStatus == RoomStatus.Settling))
        {
            return 2;
        }

        return 0;
    }

    /// <summary>
    /// 观察到桥视图后的 Room 状态映射执行（平台结算事实驱动房间推进；不满足推进条件时不触碰房间状态）。
    /// </summary>
    /// <param name="roomAgent">房间组件代理。</param>
    /// <param name="room">房间状态。</param>
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
            case 1:
                await roomAgent.EnterSettlingAsync(room.RoomId);
                break;
            case 2:
                await roomAgent.MarkSettledAsync(room.RoomId);
                break;
        }
    }

    /// <summary>
    /// 统一调用核心：lease 逐请求读取、作用域组装（三元组 = lease 授权值、玩家位 = 请求主体）、
    /// 契约失败与异常的显式错误码映射与日志。
    /// </summary>
    /// <param name="playerId">请求主体（玩家全局 actor）标识。</param>
    /// <param name="invoke">契约调用体。</param>
    /// <returns>(错误码, 视图)：错误码 0 = 成功。</returns>
    private static async Task<(int ErrorCode, OnlineMatchBridgeGameView View)> InvokeAsync(long playerId, Func<IOnlineMatchGameBridge, OnlineScope, Task<OnlineResult<OnlineMatchBridgeGameView>>> invoke)
    {
        var contract = TryGetContract();
        if (contract == null)
        {
            // Localization: Hotfix.Game.MatchBridgeUnavailable - 对局桥不可用（未发布或已撤销）, ActorId: {0}, Code: {1}
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.Hotfix.Game.MatchBridgeUnavailable, playerId, OnlineErrorCode.DependencyUnavailable));
            return (-(int)OnlineErrorCode.DependencyUnavailable, null);
        }

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
    /// 读取对局桥契约（逐请求 <see cref="CapabilityBridge{TContract}.TryAcquire"/>，不缓存）。
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
