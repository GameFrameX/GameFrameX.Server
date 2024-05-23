using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.Setting;

namespace GameFrameX.Core.Net.BaseHandler;

public abstract class RoleComponentHandler : BaseComponentHandler
{
    protected override Task InitActor()
    {
        if (ActorId <= 0)
        {
            ActorId = NetWorkChannel.GetData<long>(GlobalConst.SessionIdKey);
        }

        return Task.CompletedTask;
    }
}

public abstract class RoleComponentHandler<T> : RoleComponentHandler where T : IComponentAgent
{
    protected override Type ComponentAgentType => typeof(T);
    protected T Comp => (T)CacheComponent;
}