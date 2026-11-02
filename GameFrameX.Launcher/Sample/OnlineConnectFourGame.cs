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

using System.Collections.Generic;
using System.Text.Json;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Assets;

namespace GameFrameX.Capability.Online.Match;

/// <summary>
/// 四子棋玩法插件（对局运行时权威裁决）：把四子棋的重力落子 / 回合制 / 四连与平局判定
/// 接入平台对局管线——结算走 <see cref="OnlineMatchSettlementService"/>（结果先落定）、
/// 发奖由结算 outbox 经 <see cref="OnlineGrantService"/> 统一提交、上榜由对局结算投影写入默认榜。
/// <para>
/// 维护约束（红线）：本类只做纯裁决——只依据服务端持有的棋盘与成员身份判定合法性，
/// 绝不读取客户端提交的任何「结果」；棋盘状态只存于 <see cref="OnlineMatch.GameState"/>；
/// 参与奖资格与石头剪刀布样例同口径：负方全程零落子（挂机）时抑制全部参与奖，防刷。
/// </para>
/// </summary>
public sealed class OnlineConnectFourGame : IOnlineMatchGame
{
    /// <summary>模式标识（与客户端协议模块 <c>_0470_ConnectFour</c> 对齐）。</summary>
    public const int MatchMode = 470;

    /// <summary>棋盘宽度（列数）。</summary>
    public const int Width = 7;

    /// <summary>棋盘高度（行数）。</summary>
    public const int Height = 6;

    /// <summary>样例奖励资产标识。</summary>
    public const string RewardAssetId = "sample-coin";

    /// <summary>胜方奖励数量。</summary>
    public const long WinnerRewardAmount = 100L;

    /// <summary>参与奖数量（负方与平局发放；负方零落子时整场抑制，防挂机刷奖）。</summary>
    public const long ParticipantRewardAmount = 10L;

    /// <summary>落子事件（载荷含落子方、列、行与落子后的棋盘快照）。</summary>
    private const string MovePlayedEventType = "ConnectFourMovePlayed";

    /// <summary>终局事件（载荷含胜者与是否平局）。</summary>
    private const string GameOverEventType = "ConnectFourGameOver";

    /// <summary>
    /// 获取模式标识。
    /// </summary>
    public int Mode
    {
        get { return MatchMode; }
    }

    /// <summary>
    /// 获取最小玩家数。
    /// </summary>
    public int MinPlayers
    {
        get { return 2; }
    }

    /// <summary>
    /// 获取最大玩家数。
    /// </summary>
    public int MaxPlayers
    {
        get { return 2; }
    }

    /// <summary>
    /// 构造初始玩法状态：清空棋盘，先手交给成员表第一位。
    /// </summary>
    /// <param name="match">对局。</param>
    /// <returns>序列化后的初始状态。</returns>
    public byte[] CreateInitialState(OnlineMatch match)
    {
        var state = new OnlineConnectFourState
        {
            CurrentPlayerId = 0,
            WinnerPlayerId = 0,
            IsDraw = false,
            MoveCount = 0,
            MoveCounts = new Dictionary<long, int>(),
            Board = new int[Width * Height],
        };

        if (match.Members != null)
        {
            foreach (var member in match.Members)
            {
                if (member == null || member.State == OnlineMatchMemberState.Left || member.State == OnlineMatchMemberState.Kicked)
                {
                    continue;
                }

                state.MoveCounts[member.PlayerId] = 0;

                if (state.CurrentPlayerId == 0)
                {
                    state.CurrentPlayerId = member.PlayerId;
                }
            }
        }

        return Serialize(state);
    }

    /// <summary>
    /// 裁决并应用一次落子：轮到该成员且目标列在界内未满时按重力落子，随后做四连 / 平局判定；
    /// 落子成功即发落子事件，分出胜负再追加终局事件。
    /// </summary>
    /// <param name="match">对局。</param>
    /// <param name="member">提交成员（身份已由 Actor 校验）。</param>
    /// <param name="input">输入意图（<see cref="OnlineMatchInput.ActionId"/> = 列号，0 起）。</param>
    /// <returns>裁决结果。</returns>
    public OnlineMatchGameStepResult ApplyInput(OnlineMatch match, OnlineMatchMember member, OnlineMatchInput input)
    {
        var state = Deserialize(match.GameState);
        if (state == null || state.CurrentPlayerId == 0)
        {
            return Reject(OnlineMatchInputRejection.IllegalAction, "对局状态不可用");
        }

        if (state.WinnerPlayerId != 0 || state.IsDraw)
        {
            return Reject(OnlineMatchInputRejection.IllegalAction, "对局已结束");
        }

        if (member.PlayerId != state.CurrentPlayerId)
        {
            return Reject(OnlineMatchInputRejection.IllegalAction, "未轮到该玩家落子");
        }

        var column = input.ActionId;
        if (column < 0 || column >= Width)
        {
            return Reject(OnlineMatchInputRejection.IllegalAction, "非法列号");
        }

        var row = FindDropRow(state.Board, column);
        if (row < 0)
        {
            return Reject(OnlineMatchInputRejection.IllegalAction, "该列已落满");
        }

        var seat = SeatOf(match, member.PlayerId);
        state.Board[row * Width + column] = seat;
        state.MoveCount += 1;

        int memberMoveCount;
        state.MoveCounts.TryGetValue(member.PlayerId, out memberMoveCount);
        state.MoveCounts[member.PlayerId] = memberMoveCount + 1;

        var emittedEvents = new List<OnlineMatchServerEvent>
        {
            BuildEvent(MovePlayedEventType, new MovePlayedPayload
            {
                PlayerId = member.PlayerId,
                Column = column,
                Row = row,
                Board = state.Board,
            }),
        };

        var opponent = FindOpponent(match, member.PlayerId);
        if (CheckWin(state.Board, row, column, seat))
        {
            state.WinnerPlayerId = member.PlayerId;
            state.CurrentPlayerId = 0;
            emittedEvents.Add(BuildEvent(GameOverEventType, new GameOverPayload
            {
                WinnerPlayerId = state.WinnerPlayerId,
                IsDraw = false,
            }));
        }
        else if (state.MoveCount >= Width * Height)
        {
            state.IsDraw = true;
            state.CurrentPlayerId = 0;
            emittedEvents.Add(BuildEvent(GameOverEventType, new GameOverPayload
            {
                WinnerPlayerId = 0,
                IsDraw = true,
            }));
        }
        else
        {
            state.CurrentPlayerId = opponent;
        }

        return new OnlineMatchGameStepResult
        {
            Accepted = true,
            Rejection = OnlineMatchInputRejection.None,
            GameState = Serialize(state),
            ServerEvents = emittedEvents,
            Message = "已接受",
        };
    }

    /// <summary>
    /// 时间推进：本样例不布防回合时钟（双人面对面样例无需超时判负），恒返回未改写；
    /// 对局生命周期由落子驱动，异常退出走平台统一终止路径。
    /// </summary>
    /// <param name="match">对局。</param>
    /// <param name="nowUnixMilliseconds">当前时刻（UTC 毫秒）。</param>
    /// <param name="emittedEvents">本次推进要追加的服务器事件。</param>
    /// <returns>恒为 <c>false</c>（未改写状态）。</returns>
    public bool Advance(OnlineMatch match, long nowUnixMilliseconds, List<OnlineMatchServerEvent> emittedEvents)
    {
        return false;
    }

    /// <summary>
    /// 判定是否已分出结果（四连或平局）。
    /// </summary>
    /// <param name="match">对局。</param>
    /// <returns>已分出结果返回 <c>true</c>。</returns>
    public bool IsCompleted(OnlineMatch match)
    {
        var state = Deserialize(match.GameState);
        return state != null && (state.WinnerPlayerId != 0 || state.IsDraw);
    }

    /// <summary>
    /// 产出权威结算结果：胜方得胜方奖励；负方与平局双方在负方有有效落子时得参与奖；
    /// 任意一方全程零落子（挂机）时抑制全部非胜方参与奖。
    /// </summary>
    /// <param name="match">对局。</param>
    /// <returns>结算结果；状态缺失返回 null。</returns>
    public OnlineMatchResult BuildResult(OnlineMatch match)
    {
        var state = Deserialize(match.GameState);
        if (state == null)
        {
            return null;
        }

        var result = new OnlineMatchResult
        {
            Mode = match.Mode,
            Region = match.Region,
            ParticipationRewardsSuppressed = HasMemberWithoutMove(match, state),
            Entries = new List<OnlineMatchResultEntry>(),
        };

        foreach (var member in match.Members)
        {
            if (member == null || member.State == OnlineMatchMemberState.Left || member.State == OnlineMatchMemberState.Kicked)
            {
                continue;
            }

            var isWinner = state.WinnerPlayerId != 0 && member.PlayerId == state.WinnerPlayerId;
            var rewards = new List<OnlineAssetChangeLine>();
            if (isWinner)
            {
                rewards.Add(new OnlineAssetChangeLine(OnlineAssetKind.Currency, RewardAssetId, WinnerRewardAmount));
            }
            else if (!result.ParticipationRewardsSuppressed)
            {
                rewards.Add(new OnlineAssetChangeLine(OnlineAssetKind.Currency, RewardAssetId, ParticipantRewardAmount));
            }

            result.Entries.Add(new OnlineMatchResultEntry
            {
                PlayerId = member.PlayerId,
                Rank = state.WinnerPlayerId == 0 || isWinner ? 1 : 2,
                IsWinner = isWinner,
                Score = isWinner ? 1 : 0,
                Rewards = rewards,
            });
        }

        return result;
    }

    /// <summary>
    /// 判定是否存在零落子成员（全程挂机——参与奖抑制条件）。
    /// 按每玩家落子计数判定（总落子数在可结算局恒 ≥ 4，以总量判定会使抑制永不生效）；
    /// 计数缺失按零落子处理（fail-safe：宁可少发奖，不可被挂机刷奖）。
    /// </summary>
    /// <param name="match">对局。</param>
    /// <param name="state">玩法状态。</param>
    /// <returns>存在零落子成员返回 <c>true</c>。</returns>
    private static bool HasMemberWithoutMove(OnlineMatch match, OnlineConnectFourState state)
    {
        if (state.MoveCounts == null)
        {
            return true;
        }

        foreach (var member in match.Members)
        {
            if (member == null || member.State == OnlineMatchMemberState.Left || member.State == OnlineMatchMemberState.Kicked)
            {
                continue;
            }

            int memberMoveCount;
            if (!state.MoveCounts.TryGetValue(member.PlayerId, out memberMoveCount) || memberMoveCount == 0)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 构造对局桥视图（保密投影）：四子棋无保密字段——回合方 / 胜者 / 每玩家落子数直接投影；
    /// 棋盘明细经落子事件流（<see cref="MovePlayedPayload.Board"/>）供客户端增量重放。
    /// </summary>
    /// <param name="roomId">房间标识。</param>
    /// <param name="snapshot">对局快照。</param>
    /// <param name="events">增量事件流。</param>
    /// <returns>桥接视图。</returns>
    public OnlineMatchBridgeGameView BuildBridgeView(long roomId, OnlineMatchSnapshot snapshot, IReadOnlyList<OnlineMatchServerEvent> events)
    {
        var state = Deserialize(snapshot == null ? null : snapshot.GameState);
        var view = new OnlineMatchBridgeGameView
        {
            RoomId = roomId,
            MatchId = snapshot == null ? string.Empty : snapshot.MatchId,
            Mode = snapshot == null ? MatchMode : snapshot.Mode,
            State = ProjectState(snapshot == null ? OnlineMatchState.Created : snapshot.State),
            Round = 1,
            WinnerPlayerId = 0,
            Players = new List<OnlineMatchBridgePlayerView>(),
        };

        if (snapshot == null)
        {
            return view;
        }

        foreach (var member in snapshot.Members)
        {
            if (member == null || member.State == OnlineMatchMemberState.Left || member.State == OnlineMatchMemberState.Kicked)
            {
                continue;
            }

            view.Players.Add(new OnlineMatchBridgePlayerView
            {
                PlayerId = member.PlayerId,
                HasGesture = state != null && state.CurrentPlayerId == member.PlayerId,
                Wins = 0,
                Gesture = 0,
            });
        }

        if (state != null)
        {
            view.WinnerPlayerId = state.WinnerPlayerId;
        }

        return view;
    }

    /// <summary>
    /// 计算重力落子目标行（列满返回 -1）。
    /// </summary>
    private static int FindDropRow(int[] board, int column)
    {
        for (var row = Height - 1; row >= 0; row--)
        {
            if (board[row * Width + column] == 0)
            {
                return row;
            }
        }

        return -1;
    }

    /// <summary>
    /// 以落子点为中心做四方向四连判定。
    /// </summary>
    private static bool CheckWin(int[] board, int row, int column, int seat)
    {
        var directions = new[] { (0, 1), (1, 0), (1, 1), (1, -1) };
        foreach (var (rowDelta, columnDelta) in directions)
        {
            var count = 1;
            count += CountDirection(board, row, column, rowDelta, columnDelta, seat);
            count += CountDirection(board, row, column, -rowDelta, -columnDelta, seat);
            if (count >= 4)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 沿单一方向统计连续同色子数。
    /// </summary>
    private static int CountDirection(int[] board, int row, int column, int rowDelta, int columnDelta, int seat)
    {
        var count = 0;
        var currentRow = row + rowDelta;
        var currentColumn = column + columnDelta;
        while (currentRow >= 0 && currentRow < Height && currentColumn >= 0 && currentColumn < Width && board[currentRow * Width + currentColumn] == seat)
        {
            count++;
            currentRow += rowDelta;
            currentColumn += columnDelta;
        }

        return count;
    }

    /// <summary>
    /// 取成员座位号（成员表顺序，1 起）。
    /// </summary>
    private static int SeatOf(OnlineMatch match, long playerId)
    {
        var seat = 0;
        foreach (var member in match.Members)
        {
            if (member == null || member.State == OnlineMatchMemberState.Left || member.State == OnlineMatchMemberState.Kicked)
            {
                continue;
            }

            seat += 1;
            if (member.PlayerId == playerId)
            {
                return seat;
            }
        }

        return 0;
    }

    /// <summary>
    /// 取对手玩家标识。
    /// </summary>
    private static long FindOpponent(OnlineMatch match, long playerId)
    {
        foreach (var member in match.Members)
        {
            if (member == null || member.PlayerId == playerId || member.State == OnlineMatchMemberState.Left || member.State == OnlineMatchMemberState.Kicked)
            {
                continue;
            }

            return member.PlayerId;
        }

        return 0;
    }

    /// <summary>
    /// 平台对局状态 → 桥接生命周期状态投影。
    /// </summary>
    private static OnlineMatchBridgeGameState ProjectState(OnlineMatchState state)
    {
        switch (state)
        {
            case OnlineMatchState.Created:
            case OnlineMatchState.Waiting:
                return OnlineMatchBridgeGameState.Waiting;
            case OnlineMatchState.Ready:
                return OnlineMatchBridgeGameState.Ready;
            case OnlineMatchState.Running:
                return OnlineMatchBridgeGameState.Running;
            case OnlineMatchState.Settling:
                return OnlineMatchBridgeGameState.Settling;
            case OnlineMatchState.Completed:
                return OnlineMatchBridgeGameState.Completed;
            case OnlineMatchState.SettlementFailed:
                return OnlineMatchBridgeGameState.SettlementFailed;
            default:
                return OnlineMatchBridgeGameState.Terminated;
        }
    }

    /// <summary>
    /// 构造服务器事件（序号由 Actor 统一分配，此处不预填）。
    /// </summary>
    private static OnlineMatchServerEvent BuildEvent(string eventType, object payload)
    {
        return new OnlineMatchServerEvent
        {
            EventType = eventType,
            Payload = JsonSerializer.SerializeToUtf8Bytes(payload),
        };
    }

    /// <summary>
    /// 组装拒绝结果。
    /// </summary>
    private static OnlineMatchGameStepResult Reject(OnlineMatchInputRejection rejection, string message)
    {
        return new OnlineMatchGameStepResult
        {
            Accepted = false,
            Rejection = rejection,
            Message = message,
        };
    }

    /// <summary>
    /// 序列化玩法状态。
    /// </summary>
    private static byte[] Serialize(OnlineConnectFourState state)
    {
        return JsonSerializer.SerializeToUtf8Bytes(state);
    }

    /// <summary>
    /// 反序列化玩法状态（损坏载荷按缺失处理）。
    /// </summary>
    private static OnlineConnectFourState Deserialize(byte[] payload)
    {
        if (payload == null || payload.Length == 0)
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<OnlineConnectFourState>(payload);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    /// <summary>
    /// 四子棋玩法私有状态（只存于对局 Actor 的 GameState 载荷）。
    /// </summary>
    public sealed class OnlineConnectFourState
    {
        /// <summary>当前回合玩家标识（终局为 0）。</summary>
        public long CurrentPlayerId { get; set; }

        /// <summary>胜者玩家标识（平局为 0）。</summary>
        public long WinnerPlayerId { get; set; }

        /// <summary>是否平局。</summary>
        public bool IsDraw { get; set; }

        /// <summary>总落子数。</summary>
        public int MoveCount { get; set; }

        /// <summary>每玩家落子数（成员标识 → 落子计数；参与奖抑制按此判定，不按总落子数）。</summary>
        public Dictionary<long, int> MoveCounts { get; set; }

        /// <summary>棋盘（行主序，0 = 空，1/2 = 座位号）。</summary>
        public int[] Board { get; set; }
    }

    /// <summary>落子事件载荷。</summary>
    private sealed class MovePlayedPayload
    {
        /// <summary>落子方。</summary>
        public long PlayerId { get; set; }

        /// <summary>列号（0 起）。</summary>
        public int Column { get; set; }

        /// <summary>行号（0 起，自顶向下）。</summary>
        public int Row { get; set; }

        /// <summary>落子后的棋盘快照。</summary>
        public int[] Board { get; set; }
    }

    /// <summary>终局事件载荷。</summary>
    private sealed class GameOverPayload
    {
        /// <summary>胜者玩家标识（平局为 0）。</summary>
        public long WinnerPlayerId { get; set; }

        /// <summary>是否平局。</summary>
        public bool IsDraw { get; set; }
    }
}
