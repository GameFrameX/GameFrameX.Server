using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Core.Events;

/// <summary>
/// 事件监听器类
/// </summary>
/// <typeparam name="T">组件代理类型</typeparam>
public abstract class EventListener<T> : IEventListener where T : IComponentAgent
{
    /// <summary>
    /// 事件处理函数
    /// </summary>
    /// <param name="agent">组件代理</param>
    /// <param name="evt">事件</param>
    /// <returns>异步任务</returns>
    public Task HandleEvent(IComponentAgent agent, Event evt)
    {
        return HandleEvent((T)agent, evt);
    }

    /// <summary>
    /// 事件处理函数
    /// </summary>
    /// <param name="agent">组件代理</param>
    /// <param name="evt">事件</param>
    /// <returns>异步任务</returns>
    protected abstract Task HandleEvent(T agent, Event evt);

    /// <summary>
    /// 获取事件代理类型
    /// </summary>
    /// <returns>事件代理类型</returns>
    public Type AgentType { get; } = typeof(T);
}