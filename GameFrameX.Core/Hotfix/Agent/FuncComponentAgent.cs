using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Comps;

namespace GameFrameX.Core.Hotfix.Agent
{
    /// <summary>
    /// 无返回值组件代理
    /// </summary>
    /// <typeparam name="TComp"></typeparam>
    public abstract class FuncComponentAgent<TComp> : BaseComponentAgent<TComp> where TComp : BaseComponent
    {
    }
}