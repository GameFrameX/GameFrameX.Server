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
using System.Threading.Tasks;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Capability.Online.Assets;
using GameFrameX.Capability.Online.Competition.Leaderboard;
using GameFrameX.Capability.Online.Match;
using GameFrameX.Hotfix.Tests.Online;
using Xunit;

namespace GameFrameX.Hotfix.Tests.ConnectFour;

/// <summary>
/// 四子棋接 Online 桥黄金路径端到端验收：零 Online JSON 的真实族组合——
/// 四子棋经对局桥开局 / 落子（平台权威裁决，先手四连决胜）→ 结算管线（结算事实先落定）→
/// 结算 outbox 经统一资产入口（OnlineGrantService）发奖 → 对局结算投影写入默认榜，
/// 断言钱包余额与默认榜条目同时出现（结算 → 发奖 → 上榜全链闭环）。
/// </summary>
[Collection(CapabilityRuntimeCollection.Name)]
public class ConnectFourOnlineGoldenPathTests
{
    /// <summary>玩家一（先手，纵向四连决胜）。</summary>
    private const long PlayerOne = 2001;

    /// <summary>玩家二（负方）。</summary>
    private const long PlayerTwo = 2002;

    /// <summary>房间标识（确定性对局标识派生入参）。</summary>
    private const long RoomId = 6001;

    /// <summary>局号（同房同局幂等）。</summary>
    private const int Round = 1;

    /// <summary>驱动的最大 tick 轮数（每轮覆盖 match-tick / match-settlement-tick / 投影重投 job）。</summary>
    private const int MaxTickRounds = 10;

    /// <summary>
    /// 验证四子棋最短闭环：平台裁决四连 → 结算 → 发奖 → 默认榜条目出现（钱包与榜单双断言）。
    /// </summary>
    [Fact]
    public async Task ZeroSettings_ConnectFourMatch_Settles_Grants_And_ProjectsOntoDefaultBoard()
    {
        await OnlineCapabilityTestHarness.WithRuntimeAsync(async runtime =>
        {
            await runtime.Host.StartAsync();
            var services = runtime.Host.Services;
            var scope = new OnlineScope(OnlineCapabilityTestHarness.ServerId);

            // 默认榜已随装配预建（ActivateAsync 查询-创建）：Top 探针即成功。
            var probe = await services.GetService<OnlineLeaderboardService>().GetTopAsync(scope, new OnlineLeaderboardTopQuery { LeaderboardId = OnlineLeaderboardProjectionOptions.DefaultMatchSettledBoardId, Count = 1, });
            Assert.True(probe.IsSuccess, probe.Message);

            // 开局（桥服务 create-or-reuse，幂等）。
            var bridge = services.GetService<IOnlineMatchGameBridge>();
            var started = await bridge.StartRoomGameAsync(new OnlineScope(scope.ServerId, PlayerOne), RoomId, Round, new List<long> { PlayerOne, PlayerTwo });
            Assert.True(started.IsSuccess, started.Message);

            // 落子至先手纵向四连：双方轮流同列落子，先手第 4 手在底行起第四连。
            var moveSequence = new[]
            {
                (PlayerOne, 0), (PlayerTwo, 1),
                (PlayerOne, 0), (PlayerTwo, 1),
                (PlayerOne, 0), (PlayerTwo, 1),
                (PlayerOne, 0),
            };

            OnlineMatchBridgeGameView finalView = null;
            foreach (var (playerId, column) in moveSequence)
            {
                var submitted = await bridge.SubmitGestureAsync(new OnlineScope(scope.ServerId, playerId), RoomId, Round, column);
                Assert.True(submitted.IsSuccess, submitted.Message);
                finalView = submitted.Data;
            }

            // 平台权威裁决：先手四连胜出。
            Assert.NotNull(finalView);
            Assert.Equal(PlayerOne, finalView.WinnerPlayerId);

            // 结算 → 发奖 → 投影：显式驱动调度轮直至默认榜出现条目（事件订阅链同步投影，重投 job 兜底）。
            OnlineLeaderboardEntry winnerEntry = null;
            for (var tickRound = 0; tickRound < MaxTickRounds; tickRound++)
            {
                await runtime.Host.Scheduler.TickAsync();
                winnerEntry = await services.GetService<IOnlineLeaderboardStore>().FindEntryAsync(OnlineLeaderboardProjectionOptions.DefaultMatchSettledBoardId, PlayerOne);
                if (winnerEntry != null)
                {
                    break;
                }
            }

            // 发奖断言：胜方 +WinnerRewardAmount、负方 +ParticipantRewardAmount（样例币资产，经统一资产入口落账）。
            var assetStore = services.GetService<IOnlineAssetStore>();
            var winnerWallet = await assetStore.FindWalletAsync(PlayerOne, OnlineConnectFourGame.RewardAssetId);
            var loserWallet = await assetStore.FindWalletAsync(PlayerTwo, OnlineConnectFourGame.RewardAssetId);
            Assert.NotNull(winnerWallet);
            Assert.Equal(OnlineConnectFourGame.WinnerRewardAmount, winnerWallet.Balance);
            Assert.NotNull(loserWallet);
            Assert.Equal(OnlineConnectFourGame.ParticipantRewardAmount, loserWallet.Balance);

            // 上榜断言：默认榜出现双方条目（胜方分高在前）。
            Assert.NotNull(winnerEntry);
            var loserEntry = await services.GetService<IOnlineLeaderboardStore>().FindEntryAsync(OnlineLeaderboardProjectionOptions.DefaultMatchSettledBoardId, PlayerTwo);
            Assert.NotNull(loserEntry);
            Assert.True(winnerEntry.Score >= loserEntry.Score, "winner score (" + winnerEntry.Score + ") should rank above loser score (" + loserEntry.Score + ")");
        }, null, options =>
        {
            options.Filter = new CapabilityAssemblyFilter(new[] { "online-foundation", "online-social", "online-matchmaking", "online-match", "online-assets", "online-leaderboard", "online-competition" });
            options.HostOptions.SchedulerIntervalMilliseconds = 3600000;
        });
    }
}
