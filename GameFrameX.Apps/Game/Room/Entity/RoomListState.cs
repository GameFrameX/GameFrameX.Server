namespace GameFrameX.Apps.Game.Room.Entity;

public sealed class RoomListState : BaseCacheState
{
    /// <summary>
    /// 下一个房间ID。
    /// </summary>
    public long NextRoomId { get; set; } = 1;

    /// <summary>
    /// 通用房间列表。
    /// </summary>
    public Dictionary<long, RoomState> Rooms { get; set; } = new Dictionary<long, RoomState>();

    /// <summary>
    /// 玩家当前所在房间。Key: 玩家ID, Value: 房间ID。
    /// </summary>
    public Dictionary<long, long> PlayerRoomMap { get; set; } = new Dictionary<long, long>();

    /// <summary>
    /// 房间内玩家断线时间。Key: 玩家ID, Value: 断线 Unix 秒。
    /// </summary>
    public Dictionary<long, long> DisconnectedPlayerTimeMap { get; set; } = new Dictionary<long, long>();
}
