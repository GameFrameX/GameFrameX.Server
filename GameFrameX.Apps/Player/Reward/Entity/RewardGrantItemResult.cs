namespace GameFrameX.Apps.Player.Reward.Entity;

/// <summary>
/// 统一奖励发放接口的单项发放结果。
/// </summary>
/// <remarks>
/// 该结构同时作为幂等账本 <see cref="RewardGrantRecord"/> 的逐项快照持久化，使重复提交可短路返回上次结果（B6）。
/// </remarks>
public sealed class RewardGrantItemResult
{
    /// <summary>
    /// 奖励类型。
    /// </summary>
    public RewardType RewardType { get; set; }

    /// <summary>
    /// 物品 ID。
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 数量。
    /// </summary>
    public long Count { get; set; }

    /// <summary>
    /// 是否发放成功。
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// 该项错误码（<c>OperationStatusCode</c> 整数值）：<c>Ok</c>/<c>Unsupported</c>/<c>InvalidReward</c> 等。
    /// </summary>
    public int ErrorCode { get; set; }
}
