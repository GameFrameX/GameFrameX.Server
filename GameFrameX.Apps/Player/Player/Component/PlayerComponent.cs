using GameFrameX.Apps.Player.Player.Entity;

namespace GameFrameX.Apps.Player.Player.Component;

[ComponentType(GlobalConst.ActorTypePlayer)]
public sealed class PlayerComponent : StateComponent<PlayerState>
{
}
