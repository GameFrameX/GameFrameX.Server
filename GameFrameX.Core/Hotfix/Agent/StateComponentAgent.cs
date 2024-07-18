using GameFrameX.Core.Comps;
using GameFrameX.DBServer.Mongo;
using GameFrameX.DBServer.State;
using GameFrameX.DBServer.Storage;

namespace GameFrameX.Core.Hotfix.Agent
{
    /// <summary>
    /// 状态组件代理
    /// </summary>
    /// <typeparam name="TComp">组件对象</typeparam>
    /// <typeparam name="TState">数据对象</typeparam>
    public abstract class StateComponentAgent<TComp, TState> : BaseComponentAgent<TComp> where TComp : StateComponent<TState> where TState : CacheState, new()
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        public TState State
        {
            get { return Comp.State; }
        }
    }
}