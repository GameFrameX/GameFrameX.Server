using GameFrameX.Apps.Player.Reward.Entity;

namespace GameFrameX.Apps.Player.Reward.Component;

/// <summary>
/// 统一奖励发放组件，挂载玩家粒度的发放幂等账本 <see cref="RewardGrantRecordState"/>。
/// </summary>
/// <remarks>
/// 通过 <c>[ComponentType(GlobalConst.ActorTypePlayer)]</c> 绑定到玩家 Actor，
/// 与背包、玩家等组件并列；发放代理 <c>RewardGrantComponentAgent</c> 提供跨组件可调用的 <c>GrantAsync</c>。
/// </remarks>
[ComponentType(GlobalConst.ActorTypePlayer)]
public sealed class RewardGrantComponent : StateComponent<RewardGrantRecordState>
{
}
