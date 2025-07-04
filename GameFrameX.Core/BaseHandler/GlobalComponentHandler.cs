using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Components;
using GameFrameX.Core.Utility;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 全局组件处理器
/// </summary>
public abstract class GlobalComponentHandler : BaseComponentHandler
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    protected override Task<bool> InitActor()
    {
        if (ActorId <= 0)
        {
            var compType = ComponentAgentType.BaseType.GetGenericArguments()[0];
            var actorType = ComponentRegister.GetActorType(compType);
            ActorId = ActorIdGenerator.GetActorId(actorType);
        }

        return Task.FromResult(true);
    }
}

/// <summary>
/// 全局组件处理器
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class GlobalComponentHandler<T> : GlobalComponentHandler where T : IComponentAgent
{
    /// <summary>
    /// 组件代理类型
    /// </summary>
    protected override Type ComponentAgentType
    {
        get { return typeof(T); }
    }

    /// <summary>
    /// 缓存组件代理对象
    /// </summary>
    protected T ComponentAgent
    {
        get { return (T)CacheComponent; }
    }
}