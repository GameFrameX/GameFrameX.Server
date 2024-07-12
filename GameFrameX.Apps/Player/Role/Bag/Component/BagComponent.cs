using GameFrameX.Apps.Player.Role.Bag.Entity;
using GameFrameX.Core.Abstractions;

namespace GameFrameX.Apps.Player.Role.Bag.Component
{
    [ComponentType(ActorType.Player)]
    public class BagComponent : StateComponent<BagState>
    {
    }
}