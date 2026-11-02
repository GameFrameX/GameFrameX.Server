// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证和 Apache 许可证（版本 2.0）双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source tree for the full license text.
// ==========================================================================================

using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Match;
using Xunit;

namespace GameFrameX.Hotfix.Tests.ConnectFour;

/// <summary>
/// 四子棋玩法插件防挂机参与奖抑制判定测试：抑制条件按「每玩家落子数」判定——
/// 可结算局总落子数恒 ≥ 4，以总量判定会使抑制永不生效（负方全程零落子的挂机局仍发参与奖）。
/// </summary>
public sealed class ConnectFourAntiIdleRewardTests
{
    private const long WinnerPlayerId = 8001;

    private const long LoserPlayerId = 8002;

    /// <summary>
    /// 负方全程零落子（胜方同列四连决胜）：参与奖抑制必须生效，负方零奖励；胜方奖励不受影响。
    /// </summary>
    [Fact]
    public void BuildResult_LoserWithoutMove_SuppressesParticipationRewards()
    {
        var match = BuildSettledMatch(winnerMoves: 4, loserMoves: 0);

        var result = new OnlineConnectFourGame().BuildResult(match);

        Assert.NotNull(result);
        Assert.True(result.ParticipationRewardsSuppressed);
        var loser = FindEntry(result, LoserPlayerId);
        Assert.NotNull(loser);
        Assert.Empty(loser.Rewards);
        var winner = FindEntry(result, WinnerPlayerId);
        Assert.NotNull(winner);
        Assert.Single(winner.Rewards);
        Assert.Equal(OnlineConnectFourGame.WinnerRewardAmount, winner.Rewards[0].Amount);
    }

    /// <summary>
    /// 双方均有落子的正常对局：参与奖正常发放（抑制不生效）。
    /// </summary>
    [Fact]
    public void BuildResult_BothPlayersMoved_DoesNotSuppress()
    {
        var match = BuildSettledMatch(winnerMoves: 4, loserMoves: 3);

        var result = new OnlineConnectFourGame().BuildResult(match);

        Assert.NotNull(result);
        Assert.False(result.ParticipationRewardsSuppressed);
        var loser = FindEntry(result, LoserPlayerId);
        Assert.NotNull(loser);
        Assert.Single(loser.Rewards);
        Assert.Equal(OnlineConnectFourGame.ParticipantRewardAmount, loser.Rewards[0].Amount);
    }

    /// <summary>
    /// 玩法状态中的每玩家落子计数必须随裁决落子累加（判定数据源正确性）。
    /// </summary>
    [Fact]
    public void ApplyInput_IncrementsPerPlayerMoveCount()
    {
        var game = new OnlineConnectFourGame();
        var match = new OnlineMatch
        {
            Members = new List<OnlineMatchMember>
            {
                new OnlineMatchMember { PlayerId = WinnerPlayerId, State = OnlineMatchMemberState.Ready },
                new OnlineMatchMember { PlayerId = LoserPlayerId, State = OnlineMatchMemberState.Ready },
            },
        };
        match.GameState = game.CreateInitialState(match);

        var accepted = game.ApplyInput(match, FindMember(match, WinnerPlayerId), new OnlineMatchInput { PlayerId = WinnerPlayerId, ActionId = 0 });

        Assert.True(accepted.Accepted, accepted.Message);
        match.GameState = accepted.GameState;
        var state = JsonSerializer.Deserialize<OnlineConnectFourGame.OnlineConnectFourState>(match.GameState);
        Assert.NotNull(state);
        Assert.Equal(1, state.MoveCounts[WinnerPlayerId]);
        Assert.Equal(0, state.MoveCounts[LoserPlayerId]);
    }

    private static OnlineMatch BuildSettledMatch(int winnerMoves, int loserMoves)
    {
        var state = new OnlineConnectFourGame.OnlineConnectFourState
        {
            CurrentPlayerId = 0,
            WinnerPlayerId = WinnerPlayerId,
            IsDraw = false,
            MoveCount = winnerMoves + loserMoves,
            MoveCounts = new Dictionary<long, int>
            {
                [WinnerPlayerId] = winnerMoves,
                [LoserPlayerId] = loserMoves,
            },
            Board = new int[OnlineConnectFourGame.Width * OnlineConnectFourGame.Height],
        };

        return new OnlineMatch
        {
            Mode = OnlineConnectFourGame.MatchMode,
            Members = new List<OnlineMatchMember>
            {
                new OnlineMatchMember { PlayerId = WinnerPlayerId, State = OnlineMatchMemberState.Ready },
                new OnlineMatchMember { PlayerId = LoserPlayerId, State = OnlineMatchMemberState.Ready },
            },
            GameState = JsonSerializer.SerializeToUtf8Bytes(state),
        };
    }

    private static OnlineMatchMember FindMember(OnlineMatch match, long playerId)
    {
        return match.Members.First(member => member.PlayerId == playerId);
    }

    private static OnlineMatchResultEntry FindEntry(OnlineMatchResult result, long playerId)
    {
        return result.Entries.FirstOrDefault(entry => entry.PlayerId == playerId);
    }
}
