using GameFrameX.Apps.Player.Role.Bag.Entity;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Components;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Apps.Player.Role.Bag.Component;

[ComponentType(GlobalConst.ActorTypePlayer)]
public class BagComponent : StateComponent<BagState>
{
}