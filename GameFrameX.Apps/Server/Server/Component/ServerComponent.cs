using GameFrameX.Apps.Server.Server.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Components;

namespace GameFrameX.Apps.Server.Server.Component
{
    [ComponentType(ActorType.Server)]
    public class ServerComponent : StateComponent<ServerState>
    {
        /// <summary>
        /// 存放在此处的数据不会回存到数据库
        /// </summary>
        public HashSet<long> OnlineSet = new();
    }
}