using GameFrameX.Apps.Server.Heart.Entity;
using GameFrameX.Core.Abstractions;

namespace GameFrameX.Apps.Server.Heart.Component;

[ComponentType(ActorType.Server)]
public class HeartBeatComponent : StateComponent<HeartBeatState>
{
}