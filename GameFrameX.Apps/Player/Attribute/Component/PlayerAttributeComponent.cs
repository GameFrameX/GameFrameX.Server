using GameFrameX.Apps.Player.Attribute.Entity;

namespace GameFrameX.Apps.Player.Attribute.Component;

[ComponentType(GlobalConst.ActorTypePlayer)]
public sealed class PlayerAttributeComponent : StateComponent<PlayerAttributeState>
{
}
