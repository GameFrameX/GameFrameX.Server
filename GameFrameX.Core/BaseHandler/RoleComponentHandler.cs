using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Setting;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 角色组件处理器
/// </summary>
public abstract class RoleComponentHandler : BaseComponentHandler
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns></returns>
    protected override Task InitActor()
    {
        if (ActorId <= 0)
        {
            ActorId = NetWorkChannel.GetData<long>(GlobalConst.SessionIdKey);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// 角色组件处理器
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class RoleComponentHandler<T> : RoleComponentHandler where T : IComponentAgent
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