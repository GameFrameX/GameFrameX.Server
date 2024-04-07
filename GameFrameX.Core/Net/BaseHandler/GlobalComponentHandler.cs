using GameFrameX.Core.Actors;
using GameFrameX.Core.Comps;
using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.Core.Utility;

namespace GameFrameX.Core.Net.BaseHandler;

public abstract class GlobalComponentHandler : BaseComponentHandler
{
    protected override Task InitActor()
    {
        if (ActorId <= 0)
        {
            var compType = ComponentAgentType.BaseType.GetGenericArguments()[0];
            ActorType actorType = ComponentRegister.GetActorType(compType);
            ActorId = IdGenerator.GetActorID(actorType);
        }

        return Task.CompletedTask;
    }
}

public abstract class GlobalComponentHandler<T> : GlobalComponentHandler where T : IComponentAgent
{
    protected override Type ComponentAgentType => typeof(T);
    protected T Comp => (T)CacheComponent;
}