using GameFrameX.Core.Session;
using GameFrameX.Apps.Game.Room.Component;
using GameFrameX.Apps.Game.Room.Entity;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Foundation.Utility;
using GameFrameX.Hotfix.Logic.Game.ConnectFour;

namespace GameFrameX.Hotfix.Logic.Game.Room;

public class RoomComponentAgent : StateComponentAgent<RoomComponent, RoomListState>
{
    private const long DisconnectedPlayerAutoLeaveSeconds = 120;
    private const long ClosedRoomRetentionSeconds = 10 * 60;
    private const string RoomMaintenanceCronExpression = "0/10 * * * * ? *";

    private static readonly Dictionary<GameType, RoomRule> RoomRuleMap = new Dictionary<GameType, RoomRule>
    {
        [GameType.RockPaperScissors] = new RoomRule
        {
            GameType = GameType.RockPaperScissors,
            MinPlayerCount = 2,
            MaxPlayerCount = 2,
            DefaultNamePrefix = "石头剪刀布房间",
        },
    };

    public override async Task<bool> ActiveAsync()
    {
        var isContinue = await base.ActiveAsync();
        if (isContinue)
        {
            await WithCronExpressionAsync<RoomMaintenanceTimer>(RoomMaintenanceCronExpression);
        }

        return isContinue;
    }

    public async Task OnReqRoomListAsync(ReqRoomList request, RespRoomList response)
    {
        var rooms = State.Rooms.Values
            .Where(room => request.GameType == GameType.None || room.GameType == request.GameType)
            .Where(room => request.IncludeClosed || room.Status != RoomStatus.Closed && room.Status != RoomStatus.Disbanded)
            .OrderBy(room => room.RoomId)
            .ToList();

        response.Rooms = new List<RoomInfo>();
        foreach (var room in rooms)
        {
            response.Rooms.Add(await ToMessageAsync(room));
        }
    }

    public async Task OnCreateRoomAsync(long playerId, ReqCreateRoom request, RespCreateRoom response)
    {
        if (!CheckPlayerId(playerId, response))
        {
            return;
        }

        if (!TryGetRule(request.GameType, response, out var rule))
        {
            return;
        }

        if (!TryResolvePlayerCount(request.MinPlayerCount, request.MaxPlayerCount, rule, response, out var minPlayerCount, out var maxPlayerCount))
        {
            return;
        }

        if (TryGetActiveRoom(playerId, out var activeRoom))
        {
            if (CanAutoCloseBeforeEnterNewRoom(activeRoom))
            {
                await CloseRoomAsync(activeRoom, RoomChangeType.Closed);
            }
            else
            {
                response.ErrorCode = (int)OperationStatusCode.HasExist;
                response.Room = await ToMessageAsync(activeRoom);
                return;
            }
        }

        var now = TimerHelper.UnixTimeSeconds();
        var roomId = State.NextRoomId++;
        var roomName = string.IsNullOrWhiteSpace(request.Name) ? $"{rule.DefaultNamePrefix}{roomId}" : request.Name.Trim();
        var room = new RoomState
        {
            RoomId = roomId,
            Name = roomName,
            GameType = request.GameType,
            OwnerRoleId = playerId,
            Status = RoomStatus.Waiting,
            MinPlayerCount = minPlayerCount,
            MaxPlayerCount = maxPlayerCount,
            Round = 1,
            CreatedTime = now,
            UpdatedTime = now,
        };
        room.PlayerIds.Add(playerId);
        State.Rooms[roomId] = room;
        State.PlayerRoomMap[playerId] = roomId;

        response.Room = await ToMessageAsync(room);
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Created);
    }

    public async Task OnJoinRoomAsync(long playerId, ReqJoinRoom request, RespJoinRoom response)
    {
        if (!CheckPlayerId(playerId, response))
        {
            return;
        }

        if (!TryGetRoom(request.RoomId, response, out var room))
        {
            return;
        }

        if (TryGetActiveRoom(playerId, out var activeRoom))
        {
            if (activeRoom.RoomId == request.RoomId)
            {
                response.Room = await ToMessageAsync(activeRoom);
                return;
            }

            if (CanAutoCloseBeforeEnterNewRoom(activeRoom))
            {
                await CloseRoomAsync(activeRoom, RoomChangeType.Closed);
            }
            else
            {
                response.ErrorCode = (int)OperationStatusCode.HasExist;
                response.Room = await ToMessageAsync(activeRoom);
                return;
            }
        }

        if (room.Status != RoomStatus.Waiting && room.Status != RoomStatus.Ready)
        {
            response.ErrorCode = (int)OperationStatusCode.Forbidden;
            response.Room = await ToMessageAsync(room);
            return;
        }

        if (room.PlayerIds.Count >= room.MaxPlayerCount)
        {
            response.ErrorCode = (int)OperationStatusCode.ServerFullyLoaded;
            response.Room = await ToMessageAsync(room);
            return;
        }

        room.PlayerIds.Add(playerId);
        room.Status = room.PlayerIds.Count >= room.MinPlayerCount ? RoomStatus.Ready : RoomStatus.Waiting;
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();
        State.PlayerRoomMap[playerId] = room.RoomId;

        response.Room = await ToMessageAsync(room);
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Joined);
    }

    public async Task OnLeaveRoomAsync(long playerId, ReqLeaveRoom request, RespLeaveRoom response)
    {
        if (!CheckPlayerId(playerId, response))
        {
            return;
        }

        if (!TryGetRoom(request.RoomId, response, out var room))
        {
            return;
        }

        if (!room.PlayerIds.Contains(playerId))
        {
            response.ErrorCode = (int)OperationStatusCode.NotFound;
            response.Room = await ToMessageAsync(room);
            return;
        }

        LeaveRoom(playerId, room);
        response.Room = await ToMessageAsync(room);
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Left);
    }

    [Service]
    public virtual Task MarkPlayerDisconnectedAsync(long playerId)
    {
        if (playerId <= 0 || !TryGetActiveRoom(playerId, out _))
        {
            return Task.CompletedTask;
        }

        State.DisconnectedPlayerTimeMap[playerId] = TimerHelper.UnixTimeSeconds();
        return OwnerComponent.WriteStateAsync();
    }

    [Service]
    public virtual Task MarkPlayerReconnectedAsync(long playerId)
    {
        if (playerId <= 0 || !State.DisconnectedPlayerTimeMap.Remove(playerId))
        {
            return Task.CompletedTask;
        }

        return OwnerComponent.WriteStateAsync();
    }

    public async Task OnStartRoomGameAsync(long playerId, ReqStartRoomGame request, RespStartRoomGame response)
    {
        if (!CheckPlayerId(playerId, response))
        {
            return;
        }

        if (await TryStartRoomGameViaBridgeAsync(playerId, request, response))
        {
            return;
        }

        if (!TryGetRoom(request.RoomId, response, out var room))
        {
            return;
        }

        if (!ValidateRoomForStart(playerId, room, response))
        {
            response.Room = await ToMessageAsync(room);
            return;
        }

        room.Status = RoomStatus.Playing;
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();

        response.Room = await ToMessageAsync(room);
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Started);
    }

    // 对局桥路径（开关开启 + 四子棋房间时独占先行；其余房间走旧链）：
    // 轻量前置校验（只读 GetRoom，与旧链同判语义）→ 桥先行创建平台对局（幂等）→
    // 失败显式负值错误码返回（不推进房间）；成功回落旧链完成 Room → Playing 推进。
    // 返回 true 表示已写响应（或桥已完整处理），调用方应立即返回；false 表示走旧链。
    private async Task<bool> TryStartRoomGameViaBridgeAsync(long playerId, ReqStartRoomGame request, RespStartRoomGame response)
    {
        if (!ConnectFourMatchBridge.IsBridgeEnabled)
        {
            return false;
        }

        var bridgeRoom = await GetRoomAsync(request.RoomId);
        if (bridgeRoom == null || bridgeRoom.GameType != GameType.ConnectFour)
        {
            return false;
        }

        if (!ValidateRoomForStart(playerId, bridgeRoom, response))
        {
            response.Room = await ToMessageAsync(bridgeRoom);
            return true;
        }

        var bridgeHandled = await ConnectFourMatchBridge.TryStartRoomGameAsync(playerId, bridgeRoom.RoomId, bridgeRoom.Round, bridgeRoom.PlayerIds, response);
        return !bridgeHandled || response.ErrorCode != 0;
    }

    // 开局前的属主/状态与人数校验（桥路径与旧链同判语义）：
    // 校验失败时已写入 ErrorCode（Forbidden/Unprocessable），调用方需补写 response.Room 后返回。
    private bool ValidateRoomForStart(long playerId, RoomState room, RespStartRoomGame response)
    {
        if (room.OwnerRoleId != playerId || room.Status != RoomStatus.Ready)
        {
            response.ErrorCode = (int)OperationStatusCode.Forbidden;
            return false;
        }

        if (room.PlayerIds.Count < room.MinPlayerCount || room.PlayerIds.Count > room.MaxPlayerCount)
        {
            response.ErrorCode = (int)OperationStatusCode.Unprocessable;
            return false;
        }

        return true;
    }

    private void LeaveRoom(long playerId, RoomState room)
    {
        var previousStatus = room.Status;
        room.PlayerIds.Remove(playerId);
        State.PlayerRoomMap.Remove(playerId);
        State.DisconnectedPlayerTimeMap.Remove(playerId);
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();
        if (room.PlayerIds.Count == 0)
        {
            room.Status = RoomStatus.Closed;
            room.OwnerRoleId = 0;
        }
        else
        {
            if (room.OwnerRoleId == playerId)
            {
                room.OwnerRoleId = room.PlayerIds[0];
            }

            if (previousStatus != RoomStatus.Waiting && previousStatus != RoomStatus.Ready)
            {
                room.Round++;
            }

            room.Status = room.PlayerIds.Count >= room.MinPlayerCount ? RoomStatus.Ready : RoomStatus.Waiting;
        }
    }

    private async Task CloseRoomAsync(RoomState room, RoomChangeType changeType)
    {
        foreach (var playerId in room.PlayerIds)
        {
            State.PlayerRoomMap.Remove(playerId);
            State.DisconnectedPlayerTimeMap.Remove(playerId);
        }

        room.PlayerIds.Clear();
        room.OwnerRoleId = 0;
        room.Status = RoomStatus.Closed;
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, changeType);
    }

    [Service]
    public virtual Task<RoomState> GetRoomAsync(long roomId)
    {
        State.Rooms.TryGetValue(roomId, out var room);
        return Task.FromResult(room);
    }

    [Service]
    public virtual async Task<RoomInfo> GetRoomInfoAsync(long roomId)
    {
        State.Rooms.TryGetValue(roomId, out var room);
        return room == null ? null : await ToMessageAsync(room);
    }

    [Service]
    public virtual async Task<RoomInfo> EnterSettlingAsync(long roomId)
    {
        if (!State.Rooms.TryGetValue(roomId, out var room))
        {
            return null;
        }

        room.Status = RoomStatus.Settling;
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Settling);
        return await ToMessageAsync(room);
    }

    [Service]
    public virtual async Task<RoomInfo> MarkSettledAsync(long roomId)
    {
        if (!State.Rooms.TryGetValue(roomId, out var room))
        {
            return null;
        }

        room.Status = RoomStatus.Settled;
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Settled);
        return await ToMessageAsync(room);
    }

    [Service]
    public virtual async Task<RoomInfo> RestartRoomGameAsync(long roomId)
    {
        if (!State.Rooms.TryGetValue(roomId, out var room))
        {
            return null;
        }

        if (room.Status != RoomStatus.Settled || room.PlayerIds.Count < room.MinPlayerCount)
        {
            return await ToMessageAsync(room);
        }

        room.Round++;
        room.Status = RoomStatus.Playing;
        room.UpdatedTime = TimerHelper.UnixTimeSeconds();
        await OwnerComponent.WriteStateAsync();
        await NotifyRoomChangedAsync(room, RoomChangeType.Reset);
        return await ToMessageAsync(room);
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

    private static bool TryGetRule(GameType gameType, IResponseMessage response, out RoomRule rule)
    {
        if (RoomRuleMap.TryGetValue(gameType, out rule))
        {
            return true;
        }

        SetResponseErrorCode(response, (int)OperationStatusCode.ParamErr);
        return false;
    }

    private static bool TryResolvePlayerCount(int requestMinPlayerCount, int requestMaxPlayerCount, RoomRule rule, IResponseMessage response, out int minPlayerCount, out int maxPlayerCount)
    {
        minPlayerCount = requestMinPlayerCount > 0 ? requestMinPlayerCount : rule.MinPlayerCount;
        maxPlayerCount = requestMaxPlayerCount > 0 ? requestMaxPlayerCount : rule.MaxPlayerCount;
        if (minPlayerCount <= 0 || maxPlayerCount <= 0 || minPlayerCount > maxPlayerCount)
        {
            SetResponseErrorCode(response, (int)OperationStatusCode.ParamErr);
            return false;
        }

        return true;
    }

    private bool TryGetRoom<TResponse>(long roomId, TResponse response, out RoomState room) where TResponse : IResponseMessage
    {
        if (roomId <= 0 || !State.Rooms.TryGetValue(roomId, out room))
        {
            room = null;
            SetResponseErrorCode(response, (int)OperationStatusCode.NotFound);
            return false;
        }

        return true;
    }

    private bool TryGetActiveRoom(long playerId, out RoomState room)
    {
        room = null;
        if (!State.PlayerRoomMap.TryGetValue(playerId, out var roomId))
        {
            return false;
        }

        if (!State.Rooms.TryGetValue(roomId, out room) || room.Status == RoomStatus.Closed || room.Status == RoomStatus.Disbanded)
        {
            State.PlayerRoomMap.Remove(playerId);
            room = null;
            return false;
        }

        return true;
    }

    private async Task<RoomInfo> ToMessageAsync(RoomState room)
    {
        var message = new RoomInfo
        {
            RoomId = room.RoomId,
            Name = room.Name,
            GameType = room.GameType,
            Status = room.Status,
            PlayerCount = room.PlayerIds.Count,
            MinPlayerCount = room.MinPlayerCount,
            MaxPlayerCount = room.MaxPlayerCount,
            OwnerPlayerId = room.OwnerRoleId,
            Players = new List<RoomPlayerInfo>(),
            Round = room.Round,
            CreatedTime = room.CreatedTime,
            UpdatedTime = room.UpdatedTime,
        };

        for (var index = 0; index < room.PlayerIds.Count; index++)
        {
            var playerId = room.PlayerIds[index];
            message.Players.Add(await ToPlayerMessageAsync(room, playerId, index));
        }

        return message;
    }

    private async Task<RoomPlayerInfo> ToPlayerMessageAsync(RoomState room, long playerId, int seatIndex)
    {
        var playerState = await GameDb.FindAsync<PlayerState>(playerId);
        return new RoomPlayerInfo
        {
            PlayerId = playerId,
            SeatIndex = seatIndex,
            IsOwner = playerId == room.OwnerRoleId,
            Name = string.IsNullOrWhiteSpace(playerState?.Name) ? $"玩家{playerId}" : playerState.Name,
            Avatar = playerState?.Avatar ?? 0,
            OnlineStatus = GetOnlineStatus(playerId),
            PlayerStatus = GetPlayerStatus(room.Status),
        };
    }

    private RoomPlayerOnlineStatus GetOnlineStatus(long playerId)
    {
        if (State.DisconnectedPlayerTimeMap.ContainsKey(playerId))
        {
            return RoomPlayerOnlineStatus.Reconnecting;
        }

        return PlayerSessionManager.Instance.GetByPlayerId(playerId) == null ? RoomPlayerOnlineStatus.Offline : RoomPlayerOnlineStatus.Online;
    }

    private static RoomPlayerStatus GetPlayerStatus(RoomStatus roomStatus)
    {
        switch (roomStatus)
        {
            case RoomStatus.Waiting:
                return RoomPlayerStatus.Idle;
            case RoomStatus.Ready:
                return RoomPlayerStatus.ReadyInRoom;
            case RoomStatus.Playing:
                return RoomPlayerStatus.InGame;
            case RoomStatus.Settling:
                return RoomPlayerStatus.SettlingInRoom;
            case RoomStatus.Settled:
                return RoomPlayerStatus.SettledInRoom;
            default:
                return RoomPlayerStatus.PlayerStatusNone;
        }
    }

    private static void SetResponseErrorCode(IResponseMessage response, int errorCode)
    {
        var propertyInfo = response.GetType().GetProperty(nameof(RespRoomList.ErrorCode));
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(response, errorCode);
        }
    }

    private async Task NotifyRoomChangedAsync(RoomState room, RoomChangeType changeType)
    {
        var notify = new NotifyRoomChanged
        {
            ChangeType = changeType,
            Room = await ToMessageAsync(room),
        };

        var sessions = PlayerSessionManager.Instance.GetList(session => session.PlayerId > 0);
        foreach (var session in sessions)
        {
            await session.WriteAsync(notify);
        }
    }

    private async Task CleanupExpiredDisconnectedPlayersAsync()
    {
        var now = TimerHelper.UnixTimeSeconds();
        var playerIds = State.DisconnectedPlayerTimeMap
            .Where(pair => now - pair.Value >= DisconnectedPlayerAutoLeaveSeconds)
            .Select(pair => pair.Key)
            .ToList();
        if (playerIds.Count == 0)
        {
            return;
        }

        var changed = false;
        foreach (var playerId in playerIds)
        {
            if (PlayerSessionManager.Instance.GetByPlayerId(playerId) != null)
            {
                State.DisconnectedPlayerTimeMap.Remove(playerId);
                changed = true;
                continue;
            }

            if (!TryGetActiveRoom(playerId, out var room))
            {
                State.DisconnectedPlayerTimeMap.Remove(playerId);
                changed = true;
                continue;
            }

            LeaveRoom(playerId, room);
            changed = true;
            await NotifyRoomChangedAsync(room, RoomChangeType.Left);
        }

        if (changed)
        {
            await OwnerComponent.WriteStateAsync();
        }
    }

    private async Task CleanupExpiredClosedRoomsAsync()
    {
        var now = TimerHelper.UnixTimeSeconds();
        var expiredRoomIds = State.Rooms.Values
            .Where(room => IsClosedRoom(room) && now - room.UpdatedTime >= ClosedRoomRetentionSeconds)
            .Select(room => room.RoomId)
            .ToList();
        if (expiredRoomIds.Count == 0)
        {
            return;
        }

        foreach (var roomId in expiredRoomIds)
        {
            if (!State.Rooms.TryGetValue(roomId, out var room))
            {
                continue;
            }

            foreach (var playerId in room.PlayerIds)
            {
                State.PlayerRoomMap.Remove(playerId);
                State.DisconnectedPlayerTimeMap.Remove(playerId);
            }

            State.Rooms.Remove(roomId);
        }

        await OwnerComponent.WriteStateAsync();
    }

    private static bool IsClosedRoom(RoomState room)
    {
        return room.Status == RoomStatus.Closed || room.Status == RoomStatus.Disbanded;
    }

    private static bool CanAutoCloseBeforeEnterNewRoom(RoomState room)
    {
        return room.Status == RoomStatus.Settled;
    }

    private async Task RunRoomMaintenanceAsync()
    {
        await CleanupExpiredDisconnectedPlayersAsync();
        await CleanupExpiredClosedRoomsAsync();
    }

    private sealed class RoomMaintenanceTimer : TimerHandler<RoomComponentAgent>
    {
        protected override Task HandleTimerAsync(RoomComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.RunRoomMaintenanceAsync();
        }
    }
}
