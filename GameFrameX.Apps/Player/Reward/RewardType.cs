namespace GameFrameX.Apps.Player.Reward;

/// <summary>
/// 统一奖励发放的奖励类型路由枚举。
/// </summary>
/// <remarks>
/// 编号一旦发布不可变：后续新增类型只追加，禁止复用既有编号。
/// 一期仅 <see cref="NormalItem"/> 可发放，其余为预留路由位，由后续隐藏资产 / 订阅 / 权益模块承接，发放接口一期返回 <c>Unsupported</c>。
/// </remarks>
public enum RewardType
{
    /// <summary>
    /// 普通道具：写入玩家背包 <c>BagState</c>，在线推送背包变更通知。
    /// </summary>
    NormalItem = 0,

    /// <summary>
    /// 隐藏道具：预留路由位，由后续隐藏资产模块承接，不进入普通背包展示。
    /// </summary>
    HiddenItem = 1,

    /// <summary>
    /// 月卡：预留路由位，由后续订阅 / 权益模块承接。
    /// </summary>
    MonthlyCard = 2,

    /// <summary>
    /// 终生卡：预留路由位，由后续订阅 / 权益模块承接。
    /// </summary>
    LifetimeCard = 3,

    /// <summary>
    /// VIP 点：预留路由位，由后续 VIP / 权益模块承接。
    /// </summary>
    VipPoint = 4,

    /// <summary>
    /// 权益类资产：预留路由位，由后续权益模块承接。
    /// </summary>
    Equity = 5,
}
