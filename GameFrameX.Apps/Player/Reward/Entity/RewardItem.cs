namespace GameFrameX.Apps.Player.Reward.Entity;

/// <summary>
/// 单个奖励项，统一奖励发放接口的输入单元。
/// </summary>
public sealed class RewardItem
{
    /// <summary>
    /// 奖励类型。决定发放路由：普通道具写背包，其余类型一期返回 <c>Unsupported</c>。
    /// </summary>
    public RewardType RewardType { get; set; }

    /// <summary>
    /// 物品 ID。普通道具对应 <c>TbItemConfig</c> 配置 ID。
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 数量。必须大于 0，否则该项被判为 <c>InvalidReward</c>。
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// 扩展数据（预留）。一期发放接口不解析，仅随账本原样持久化。
    /// </summary>
    public string ExtraData { get; set; }
}
