using GameFrameX.Core.Components;
using GameFrameX.DataBase;
using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.Core.Hotfix.Agent;

/// <summary>
/// 状态组件代理类
/// </summary>
/// <typeparam name="TComponent">组件对象类型</typeparam>
/// <typeparam name="TState">数据对象类型</typeparam>
public abstract class StateComponentAgent<TComponent, TState> : BaseComponentAgent<TComponent> where TComponent : StateComponent<TState> where TState : BaseCacheState,new()
{
    /// <summary>
    /// 获取数据对象
    /// </summary>
    public TState State
    {
        get { return OwnerComponent.State; }
    }
}