namespace GameFrameX.Apps.Game.RockPaperScissors.Entity;

public sealed class RockPaperScissorsGameListState : BaseCacheState
{
    /// <summary>
    /// Key: 房间ID，Value: 石头剪刀布玩法数据。
    /// </summary>
    public Dictionary<long, RockPaperScissorsGameState> Games { get; set; } = new Dictionary<long, RockPaperScissorsGameState>();
}
