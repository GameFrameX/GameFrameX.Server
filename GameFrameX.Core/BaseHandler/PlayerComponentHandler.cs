using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Utility;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 玩家组件处理器
/// </summary>
public abstract class PlayerComponentHandler : BaseComponentHandler
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    protected override Task<bool> InitActor()
    {
        if (ActorId <= 0)
        {
            ActorId = NetWorkChannel.GetData<long>(GlobalConst.ActorIdKey);
        }

        if (ActorId <= 0)
        {
            ActorId = ActorIdGenerator.GetActorId(GlobalConst.ActorTypePlayer);
        }

        return Task.FromResult(true);
    }
}

/// <summary>
/// 玩家组件处理器
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class PlayerComponentHandler<T> : PlayerComponentHandler where T : IComponentAgent
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