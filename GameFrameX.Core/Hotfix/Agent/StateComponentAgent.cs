using GameFrameX.Core.Comps;
using GameFrameX.DBServer.State;
using GameFrameX.DBServer.Storage;

namespace GameFrameX.Core.Hotfix.Agent
{
    public abstract class StateComponentAgent<TComp, TState> : BaseComponentAgent<TComp> where TComp : StateComponent<TState> where TState : CacheState, new()
    {
        public TState State => Comp.State;
    }
}