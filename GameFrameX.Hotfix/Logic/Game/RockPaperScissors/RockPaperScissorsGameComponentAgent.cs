using GameFrameX.Core.Session;
using GameFrameX.Apps.Game.RockPaperScissors.Component;
using GameFrameX.Apps.Game.RockPaperScissors.Entity;
using GameFrameX.Hotfix.Logic.Game.Room;

namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;

public class RockPaperScissorsGameComponentAgent : StateComponentAgent<RockPaperScissorsGameComponent, RockPaperScissorsGameListState>
{
    public async Task OnReqGameInfoAsync(long playerId, ReqRockPaperScissorsGameInfo request, RespRockPaperScissorsGameInfo response)
    {
        var roomAgent = await ActorManager.GetComponentAgentAsync<RoomComponentAgent>();
        var room = await roomAgent.GetRoomAsync(request.RoomId);

        // 对局桥路径（开关开启 + RPS 房间时独占；不含 Playing 判定——Settling/Settled 也可查）：
        // 校验请求主体为房间成员 → 桥查询（保密投影）→ 视图映射回填 + Room 状态映射（无推送）。
        if (RockPaperScissorsMatchBridge.IsBridgeEnabled && room != null && room.GameType == GameType.RockPaperScissors)
        {
            if (!CheckPlayerId(playerId, response) || !room.PlayerIds.Contains(playerId))
            {
                if (response.ErrorCode == 0)
                {
                    response.ErrorCode = (int)OperationStatusCode.Forbidden;
                    response.GameInfo = null;
                }

                return;
            }

            var infoResult = await RockPaperScissorsMatchBridge.TryGetGameInfoAsync(playerId, room.RoomId, room.Round, response);
            if (infoResult.Handled)
            {
                if (infoResult.View == null)
                {
                    return;
                }

                await RockPaperScissorsMatchBridge.ApplyRoomStateAsync(roomAgent, room, infoResult.View);
                return;
            }
        }

        if (!CheckRoom(room, response))
        {
            return;
        }

        var game = GetOrCreateGame(room.RoomId, room.Round);
        response.GameInfo = ToMessage(game, room.PlayerIds, CanRevealGestures(room.Status));
    }

    public async Task OnSubmitGestureAsync(long playerId, ReqSubmitRockPaperScissorsGesture request, RespSubmitRockPaperScissorsGesture response)
    {
        if (!CheckPlayerId(playerId, response))
        {
            return;
        }

        if (!IsValidGesture(request.Gesture))
        {
            response.ErrorCode = (int)OperationStatusCode.ParamErr;
            return;
        }

        var roomAgent = await ActorManager.GetComponentAgentAsync<RoomComponentAgent>();
        var room = await roomAgent.GetRoomAsync(request.RoomId);

        if (await TrySubmitGestureViaBridgeAsync(playerId, request, room, roomAgent, response))
        {
            return;
        }

        if (!CheckRoom(room, response))
        {
            return;
        }

        if (room.Status != RoomStatus.Playing || !room.PlayerIds.Contains(playerId))
        {
            response.ErrorCode = (int)OperationStatusCode.Forbidden;
            response.GameInfo = BuildGameInfo(room, false);
            return;
        }

        var game = GetOrCreateGame(room.RoomId, room.Round);
        if (game.GestureMap.ContainsKey(playerId))
        {
            response.ErrorCode = (int)OperationStatusCode.HasExist;
            response.GameInfo = ToMessage(game, room.PlayerIds, false);
            return;
        }

        game.GestureMap[playerId] = request.Gesture;
        var revealGestures = false;
        if (game.GestureMap.Count == room.PlayerIds.Count)
        {
            await roomAgent.EnterSettlingAsync(room.RoomId);
            game.WinnerRoleId = CalculateWinner(room.PlayerIds, game);
            await OwnerComponent.WriteStateAsync();
            await roomAgent.MarkSettledAsync(room.RoomId);
            revealGestures = true;
        }
        else
        {
            await OwnerComponent.WriteStateAsync();
        }

        response.GameInfo = ToMessage(game, room.PlayerIds, revealGestures);
        await NotifyGameChangedAsync(room.PlayerIds, response.GameInfo);
    }

    public async Task OnRestartGameAsync(long playerId, ReqRestartRockPaperScissorsGame request, RespRestartRockPaperScissorsGame response)
    {
        if (!CheckPlayerId(playerId, response))
        {
            return;
        }

        var roomAgent = await ActorManager.GetComponentAgentAsync<RoomComponentAgent>();
        var room = await roomAgent.GetRoomAsync(request.RoomId);

        if (await TryRestartGameViaBridgeAsync(playerId, room, roomAgent, response))
        {
            return;
        }

        if (!CheckRoom(room, response))
        {
            return;
        }

        if (room.OwnerRoleId != playerId || room.Status != RoomStatus.Settled)
        {
            response.ErrorCode = (int)OperationStatusCode.Forbidden;
            response.GameInfo = BuildGameInfo(room, CanRevealGestures(room.Status));
            return;
        }

        var roomInfo = await roomAgent.RestartRoomGameAsync(room.RoomId);
        if (roomInfo == null || roomInfo.Status != RoomStatus.Playing)
        {
            response.ErrorCode = (int)OperationStatusCode.Unprocessable;
            response.GameInfo = BuildGameInfo(room, CanRevealGestures(room.Status));
            return;
        }

        var game = GetOrCreateGame(room.RoomId, room.Round);
        game.GestureMap.Clear();
        game.WinnerRoleId = 0;
        await OwnerComponent.WriteStateAsync();

        response.GameInfo = ToMessage(game, room.PlayerIds, false);
        await NotifyGameChangedAsync(room.PlayerIds, response.GameInfo);
    }

    private async Task<bool> TrySubmitGestureViaBridgeAsync(long playerId, ReqSubmitRockPaperScissorsGesture request, GameFrameX.Apps.Game.Room.Entity.RoomState room, RoomComponentAgent roomAgent, RespSubmitRockPaperScissorsGesture response)
    {
        // 对局桥路径（开关开启 + RPS 房间时独占；失败显式负值错误码返回）：
        // 校验房间与成员位（与旧链同判语义）→ 桥提交（服务端裁决）→ 视图映射回填 +
        // Room 状态映射 + 全员推送。桥未接管（开关竞态关闭）时落旧链。
        if (RockPaperScissorsMatchBridge.IsBridgeEnabled && room != null && room.GameType == GameType.RockPaperScissors)
        {
            if (room.Status != RoomStatus.Playing || !room.PlayerIds.Contains(playerId))
            {
                response.ErrorCode = (int)OperationStatusCode.Forbidden;
                response.GameInfo = null;
                return true;
            }

            var submitResult = await RockPaperScissorsMatchBridge.TrySubmitGestureAsync(playerId, room.RoomId, room.Round, (int)request.Gesture, response);
            if (submitResult.Handled)
            {
                if (submitResult.View == null)
                {
                    return true;
                }

                await RockPaperScissorsMatchBridge.ApplyRoomStateAsync(roomAgent, room, submitResult.View);
                await NotifyGameChangedAsync(room.PlayerIds, response.GameInfo);
                return true;
            }
        }

        return false;
    }

    private async Task<bool> TryRestartGameViaBridgeAsync(long playerId, GameFrameX.Apps.Game.Room.Entity.RoomState room, RoomComponentAgent roomAgent, RespRestartRockPaperScissorsGame response)
    {
        // 对局桥路径（开关开启 + RPS 房间时独占）：桥以新局号先行创建新对局（确定性局号幂等）→
        // 成功后旧链 RestartRoomGame（round++ → Playing）完成房间推进，视图映射回填 + 全员推送；
        // 失败显式负值错误码返回（不推进房间）。桥未接管（开关竞态关闭）时落旧链。
        if (RockPaperScissorsMatchBridge.IsBridgeEnabled && room != null && room.GameType == GameType.RockPaperScissors)
        {
            if (room.OwnerRoleId != playerId || room.Status != RoomStatus.Settled)
            {
                response.ErrorCode = (int)OperationStatusCode.Forbidden;
                response.GameInfo = null;
                return true;
            }

            var nextRoundResult = await RockPaperScissorsMatchBridge.TryStartNextRoundAsync(playerId, room.RoomId, room.Round + 1, room.PlayerIds, response);
            if (nextRoundResult.Handled)
            {
                if (nextRoundResult.View == null)
                {
                    return true;
                }

                var restarted = await roomAgent.RestartRoomGameAsync(room.RoomId);
                response.GameInfo = RockPaperScissorsMatchBridge.MapGameInfo(nextRoundResult.View);
                if (restarted == null || restarted.Status != RoomStatus.Playing)
                {
                    response.ErrorCode = (int)OperationStatusCode.Unprocessable;
                    return true;
                }

                await NotifyGameChangedAsync(room.PlayerIds, response.GameInfo);
                return true;
            }
        }

        return false;
    }

    private RockPaperScissorsGameInfo BuildGameInfo(GameFrameX.Apps.Game.Room.Entity.RoomState room, bool revealGestures)
    {
        var game = GetOrCreateGame(room.RoomId, room.Round);
        return ToMessage(game, room.PlayerIds, revealGestures);
    }

    private RockPaperScissorsGameState GetOrCreateGame(long roomId, int round)
    {
        if (!State.Games.TryGetValue(roomId, out var game))
        {
            game = new RockPaperScissorsGameState
            {
                RoomId = roomId,
                Round = round,
            };
            State.Games[roomId] = game;
        }

        if (game.Round != round)
        {
            game.Round = round;
            game.GestureMap.Clear();
            game.WinnerRoleId = 0;
        }

        return game;
    }

    private static bool CheckPlayerId(long playerId, IResponseMessage response)
    {
        if (playerId > 0)
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

        if (room.GameType != GameType.RockPaperScissors)
        {
            SetResponseErrorCode(response, (int)OperationStatusCode.Forbidden);
            return false;
        }

        return true;
    }

    private static bool IsValidGesture(RockPaperScissorsGesture gesture)
    {
        return gesture == RockPaperScissorsGesture.Rock ||
               gesture == RockPaperScissorsGesture.Scissors ||
               gesture == RockPaperScissorsGesture.Paper;
    }

    private static long CalculateWinner(List<long> playerIds, RockPaperScissorsGameState game)
    {
        if (playerIds.Count != 2)
        {
            return 0;
        }

        var firstPlayerId = playerIds[0];
        var secondPlayerId = playerIds[1];
        var firstGesture = game.GestureMap[firstPlayerId];
        var secondGesture = game.GestureMap[secondPlayerId];
        if (firstGesture == secondGesture)
        {
            return 0;
        }

        return IsFirstWin(firstGesture, secondGesture) ? firstPlayerId : secondPlayerId;
    }

    private static bool IsFirstWin(RockPaperScissorsGesture firstGesture, RockPaperScissorsGesture secondGesture)
    {
        return firstGesture == RockPaperScissorsGesture.Rock && secondGesture == RockPaperScissorsGesture.Scissors ||
               firstGesture == RockPaperScissorsGesture.Scissors && secondGesture == RockPaperScissorsGesture.Paper ||
               firstGesture == RockPaperScissorsGesture.Paper && secondGesture == RockPaperScissorsGesture.Rock;
    }

    private static bool CanRevealGestures(RoomStatus roomStatus)
    {
        return roomStatus == RoomStatus.Settling || roomStatus == RoomStatus.Settled;
    }

    private static RockPaperScissorsGameInfo ToMessage(RockPaperScissorsGameState game, List<long> playerIds, bool revealGestures)
    {
        return new RockPaperScissorsGameInfo
        {
            RoomId = game.RoomId,
            Round = game.Round,
            WinnerPlayerId = revealGestures ? game.WinnerRoleId : 0,
            Players = playerIds.Select(playerId => new RockPaperScissorsPlayerInfo
            {
                PlayerId = playerId,
                HasGesture = game.GestureMap.ContainsKey(playerId),
                Gesture = revealGestures && game.GestureMap.TryGetValue(playerId, out var gesture) ? gesture : RockPaperScissorsGesture.None,
            }).ToList(),
        };
    }

    private static void SetResponseErrorCode(IResponseMessage response, int errorCode)
    {
        var propertyInfo = response.GetType().GetProperty(nameof(RespRockPaperScissorsGameInfo.ErrorCode));
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(response, errorCode);
        }
    }

    private static async Task NotifyGameChangedAsync(List<long> playerIds, RockPaperScissorsGameInfo gameInfo)
    {
        var notify = new NotifyRockPaperScissorsGameChanged
        {
            GameInfo = gameInfo,
        };

        foreach (var playerId in playerIds)
        {
            var session = PlayerSessionManager.Instance.GetByPlayerId(playerId);
            if (session != null)
            {
                await session.WriteAsync(notify);
            }
        }
    }
}
