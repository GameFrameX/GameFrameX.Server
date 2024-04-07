using GameFrameX.Apps.Server.Heart.Entity;

namespace GameFrameX.Apps.Server.Heart.Component;

[ComponentType(ActorType.Server)]
public class HeartBeatComponent : StateComponent<HeartBeatState>
{
}