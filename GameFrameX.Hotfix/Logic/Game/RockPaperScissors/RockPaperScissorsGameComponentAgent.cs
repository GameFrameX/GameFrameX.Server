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
