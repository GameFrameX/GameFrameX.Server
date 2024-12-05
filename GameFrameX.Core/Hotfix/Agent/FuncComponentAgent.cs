using GameFrameX.Core.Components;

namespace GameFrameX.Core.Hotfix.Agent;

/// <summary>
/// 无返回值组件代理
/// </summary>
/// <typeparam name="TComponent">具体的组件类型</typeparam>
public abstract class FuncComponentAgent<TComponent> : BaseComponentAgent<TComponent> where TComponent : BaseComponent
{
}