using GameFrameX.Apps.Player.Mail.Entity;

namespace GameFrameX.Apps.Player.Mail.Component;

/// <summary>
/// 玩家邮件组件，挂载玩家粒度的邮件箱 <see cref="MailBoxState"/>。
/// </summary>
/// <remarks>
/// 通过 <c>[ComponentType(GlobalConst.ActorTypePlayer)]</c> 绑定到玩家 Actor，与背包、奖励发放等组件并列；
/// 邮件代理 <c>MailComponentAgent</c> 提供懒创建、列表、读信、删除、过期清理、撤回作废等能力。
/// </remarks>
[ComponentType(GlobalConst.ActorTypePlayer)]
public sealed class MailComponent : StateComponent<MailBoxState>
{
}
