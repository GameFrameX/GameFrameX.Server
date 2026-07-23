// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Apps.Common.Session;
using GameFrameX.Apps.Game.DiceBattle.Component;
using GameFrameX.Apps.Game.DiceBattle.Entity;
using GameFrameX.Hotfix.Logic.Game.Room;

namespace GameFrameX.Hotfix.Logic.Game.DiceBattle;

public class DiceBattleGameComponentAgent : StateComponentAgent<DiceBattleGameComponent, DiceBattleGameListState>
{
    /// <summary>
    /// 骰子点数上界（Next 上界 exclusive），点数范围为 1-6。
    /// </summary>
    private const int DiceUpperBound = 7;

    /// <summary>
    /// 骰子点数下界（Next 下界 inclusive）。
    /// </summary>
    private const int DiceLowerBound = 1;

    public async Task OnReqGameInfoAsync(ReqDiceBattleGameInfo request, RespDiceBattleGameInfo response)
    {
        var roomAgent = await ActorManager.GetComponentAgent<RoomComponentAgent>();
        var room = await roomAgent.GetRoom(request.RoomId);
        if (!CheckRoom(room, response))
        {
            return;
        }

        var game = GetOrCreateGame(room.RoomId, room.Round);
        response.GameInfo = ToMessage(game, room.PlayerIds, CanRevealDice(room.Status));
    }

    public async Task OnRollDiceAsync(long roleId, ReqRollDiceBattle request, RespRollDiceBattle response)
    {
        if (!CheckRoleId(roleId, response))
        {
            return;
        }

        var roomAgent = await ActorManager.GetComponentAgent<RoomComponentAgent>();
        var room = await roomAgent.GetRoom(request.RoomId);
        if (!CheckRoom(room, response))
        {
            return;
        }

        if (room.Status != RoomStatus.Playing || !room.PlayerIds.Contains(roleId))
        {
            response.ErrorCode = (int)OperationStatusCode.Forbidden;
            response.GameInfo = BuildGameInfo(room, false);
            return;
        }

        var game = GetOrCreateGame(room.RoomId, room.Round);
        if (game.DiceMap.ContainsKey(roleId))
        {
            response.ErrorCode = (int)OperationStatusCode.HasExist;
            response.GameInfo = ToMessage(game, room.PlayerIds, false);
            return;
        }

        // 服务器端生成骰子点数，玩家无法篡改。
        game.DiceMap[roleId] = RandomHelper.Next(DiceLowerBound, DiceUpperBound);
        var revealDice = false;
        if (game.DiceMap.Count == room.PlayerIds.Count)
        {
            await roomAgent.EnterSettling(room.RoomId);
            CalculateWinners(room.PlayerIds, game);
            await OwnerComponent.WriteStateAsync();
            await roomAgent.MarkSettled(room.RoomId);
            revealDice = true;
        }
        else
        {
            await OwnerComponent.WriteStateAsync();
        }

        response.GameInfo = ToMessage(game, room.PlayerIds, revealDice);
        await NotifyGameChangedAsync(room.PlayerIds, response.GameInfo);
    }

    public async Task OnRestartGameAsync(long roleId, ReqRestartDiceBattle request, RespRestartDiceBattle response)
    {
        if (!CheckRoleId(roleId, response))
        {
            return;
        }

        var roomAgent = await ActorManager.GetComponentAgent<RoomComponentAgent>();
        var room = await roomAgent.GetRoom(request.RoomId);
        if (!CheckRoom(room, response))
        {
            return;
        }

        if (room.OwnerRoleId != roleId || room.Status != RoomStatus.Settled)
        {
            response.ErrorCode = (int)OperationStatusCode.Forbidden;
            response.GameInfo = BuildGameInfo(room, CanRevealDice(room.Status));
            return;
        }

        var roomInfo = await roomAgent.RestartRoomGame(room.RoomId);
        if (roomInfo == null || roomInfo.Status != RoomStatus.Playing)
        {
            response.ErrorCode = (int)OperationStatusCode.Unprocessable;
            response.GameInfo = BuildGameInfo(room, CanRevealDice(room.Status));
            return;
        }

        var game = GetOrCreateGame(room.RoomId, room.Round);
        game.DiceMap.Clear();
        game.WinnerRoleIds.Clear();
        game.MaxDiceValue = 0;
        await OwnerComponent.WriteStateAsync();

        response.GameInfo = ToMessage(game, room.PlayerIds, false);
        await NotifyGameChangedAsync(room.PlayerIds, response.GameInfo);
    }

    private DiceBattleGameInfo BuildGameInfo(GameFrameX.Apps.Game.Room.Entity.RoomState room, bool revealDice)
    {
        var game = GetOrCreateGame(room.RoomId, room.Round);
        return ToMessage(game, room.PlayerIds, revealDice);
    }

    private DiceBattleGameState GetOrCreateGame(long roomId, int round)
    {
        if (!State.Games.TryGetValue(roomId, out var game))
        {
            game = new DiceBattleGameState
            {
                RoomId = roomId,
                Round = round,
            };
            State.Games[roomId] = game;
        }

        if (game.Round != round)
        {
            game.Round = round;
            game.DiceMap.Clear();
            game.WinnerRoleIds.Clear();
            game.MaxDiceValue = 0;
        }

        return game;
    }

    private static bool CheckRoleId(long roleId, IResponseMessage response)
    {
        if (roleId > 0)
        {
            return true;
        }

        SetResponseErrorCode(response, (int)OperationStatusCode.Forbidden);
        return false;
    }

    private static bool CheckRoom(GameFrameX.Apps.Game.Room.Entity.RoomState room, IResponseMessage response)
    {
        if (room == null)
        {
            SetResponseErrorCode(response, (int)OperationStatusCode.NotFound);
            return false;
        }

        if (room.GameType != GameType.DiceBattle)
        {
            SetResponseErrorCode(response, (int)OperationStatusCode.Forbidden);
            return false;
        }

        return true;
    }

    private static void CalculateWinners(List<long> playerIds, DiceBattleGameState game)
    {
        var max = 0;
        foreach (var roleId in playerIds)
        {
            if (game.DiceMap.TryGetValue(roleId, out var value) && value > max)
            {
                max = value;
            }
        }

        game.MaxDiceValue = max;
        game.WinnerRoleIds.Clear();
        foreach (var roleId in playerIds)
        {
            if (game.DiceMap.TryGetValue(roleId, out var value) && value == max)
            {
                game.WinnerRoleIds.Add(roleId);
            }
        }
    }

    private static bool CanRevealDice(RoomStatus roomStatus)
    {
        return roomStatus == RoomStatus.Settling || roomStatus == RoomStatus.Settled;
    }

    private static DiceBattleGameInfo ToMessage(DiceBattleGameState game, List<long> playerIds, bool revealDice)
    {
        return new DiceBattleGameInfo
        {
            RoomId = game.RoomId,
            Round = game.Round,
            WinnerRoleIds = revealDice ? new List<long>(game.WinnerRoleIds) : new List<long>(),
            MaxDiceValue = revealDice ? game.MaxDiceValue : 0,
            Players = playerIds.Select(roleId => new DiceBattlePlayerInfo
            {
                RoleId = roleId,
                HasRolled = game.DiceMap.ContainsKey(roleId),
                DiceValue = revealDice && game.DiceMap.TryGetValue(roleId, out var value) ? value : 0,
            }).ToList(),
        };
    }

    private static void SetResponseErrorCode(IResponseMessage response, int errorCode)
    {
        var propertyInfo = response.GetType().GetProperty(nameof(RespDiceBattleGameInfo.ErrorCode));
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(response, errorCode);
        }
    }

    private static async Task NotifyGameChangedAsync(List<long> playerIds, DiceBattleGameInfo gameInfo)
    {
        var notify = new NotifyDiceBattleGameChanged
        {
            GameInfo = gameInfo,
        };

        foreach (var roleId in playerIds)
        {
            var session = SessionManager.GetByRoleId(roleId);
            if (session != null)
            {
                await session.WriteAsync(notify);
            }
        }
    }
}
