using GameFrameX.Apps.Server.Heart.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Setting;

namespace GameFrameX.Apps.Server.Heart.Component;

[ComponentType(GlobalConst.ActorTypeServer)]
public class HeartBeatComponent : StateComponent<HeartBeatState>
{
}