using GameFrameX.Apps.Player.Role.Pet.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Components;

namespace GameFrameX.Apps.Player.Role.Pet.Component
{
    [ComponentType(ActorType.Player)]
    public class PetComponent : StateComponent<PetState>
    {
    }
}