using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Core.Utility;

/// <summary>
/// 代理调用回调
/// </summary>
public interface IAgentCallback
{
    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="agent">组件代理</param>
    /// <param name="gameEventArgs">参数</param>
    /// <returns></returns>
    Task<bool> Invoke(IComponentAgent agent, GameEventArgs gameEventArgs = null);

    /// <summary>
    /// 组件代理类型
    /// </summary>
    /// <returns></returns>
    Type CompAgentType();
}

/// <summary>
/// 代理调用回调
/// </summary>
/// <typeparam name="TAgent"></typeparam>
public abstract class AgentCallback<TAgent> : IAgentCallback where TAgent : IComponentAgent
{
    /// <summary>
    /// </summary>
    /// <returns></returns>
    public Type CompAgentType()
    {
        return typeof(TAgent);
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="agent"></param>
    /// <param name="gameEventArgs"></param>
    /// <returns></returns>
    public Task<bool> Invoke(IComponentAgent agent, GameEventArgs gameEventArgs = null)
    {
        return OnCall((TAgent)agent, gameEventArgs);
    }

    /// <summary>
    /// 回调
    /// </summary>
    /// <param name="comp"></param>
    /// <param name="gameEventArgs"></param>
    /// <returns></returns>
    protected abstract Task<bool> OnCall(TAgent comp, GameEventArgs gameEventArgs);
}