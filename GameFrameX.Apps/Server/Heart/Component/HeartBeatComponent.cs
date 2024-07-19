using GameFrameX.Apps.Server.Heart.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Components;

namespace GameFrameX.Apps.Server.Heart.Component;

[ComponentType(ActorType.Server)]
public class HeartBeatComponent : StateComponent<HeartBeatState>
{
}