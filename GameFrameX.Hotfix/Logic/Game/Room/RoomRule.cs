namespace GameFrameX.Hotfix.Logic.Game.Room;

internal sealed class RoomRule
{
    public GameType GameType { get; set; }

    public int MinPlayerCount { get; set; }

    public int MaxPlayerCount { get; set; }

    public string DefaultNamePrefix { get; set; }
}
