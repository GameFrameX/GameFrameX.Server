using GameFrameX.Core.Components;
using GameFrameX.DataBase.State;

namespace GameFrameX.Core.Hotfix.Agent
{
    /// <summary>
    /// 状态组件代理
    /// </summary>
    /// <typeparam name="TComponent">组件对象</typeparam>
    /// <typeparam name="TState">数据对象</typeparam>
    public abstract class StateComponentAgent<TComponent, TState> : BaseComponentAgent<TComponent> where TComponent : StateComponent<TState> where TState : class, ICacheState, new()
    {
        /// <summary>
        /// 数据对象
        /// </summary>
        public TState State
        {
            get { return OwnerComponent.State; }
        }
    }
}