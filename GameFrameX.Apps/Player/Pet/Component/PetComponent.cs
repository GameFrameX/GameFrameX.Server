using GameFrameX.Apps.Player.Pet.Entity;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Components;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Apps.Player.Pet.Component;

[ComponentType(GlobalConst.ActorTypePlayer)]
public class PetComponent : StateComponent<PetState>
{
}
