using GameFrameX.Apps.Player.Role.Pet.Entity;
using GameFrameX.Core.Abstractions;

namespace GameFrameX.Apps.Player.Role.Pet.Component
{
    [ComponentType(ActorType.Player)]
    public class PetComponent : StateComponent<PetState>
    {
    }
}