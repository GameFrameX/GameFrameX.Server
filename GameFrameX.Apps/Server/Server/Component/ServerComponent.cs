using GameFrameX.Apps.Server.Server.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Core.Components;
using GameFrameX.Setting;

namespace GameFrameX.Apps.Server.Server.Component;

[ComponentType(GlobalConst.ActorTypeServer)]
public class ServerComponent : StateComponent<ServerState>
{
    /// <summary>
    /// 存放在此处的数据不会回存到数据库
    /// </summary>
    public HashSet<long> OnlineSet = new();
}