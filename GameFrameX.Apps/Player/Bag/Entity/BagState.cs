namespace GameFrameX.Apps.Player.Bag.Entity;

public sealed class BagState : BaseCacheState
{
    /// <summary>
    /// 背包物品
    /// </summary>
    public Dictionary<int, BagItemState> List { get; set; } = new Dictionary<int, BagItemState>();
}
