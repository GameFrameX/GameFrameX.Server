using GameFrameX.Apps.Player.Role.Bag.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Components;
using GameFrameX.Setting;

namespace GameFrameX.Apps.Player.Role.Bag.Component;

[ComponentType(GlobalConst.ActorTypePlayer)]
public class BagComponent : StateComponent<BagState>
{
}