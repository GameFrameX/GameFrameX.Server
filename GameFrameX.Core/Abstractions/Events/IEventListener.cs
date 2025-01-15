using GameFrameX.Core.Abstractions.Agent;

namespace GameFrameX.Core.Abstractions.Events;

/// <summary>
/// 每个实例其实都是单例的
/// </summary>
public interface IEventListener
{
    /// <summary>
    /// 获取事件代理类型
    /// </summary>
    /// <returns>事件代理的类型</returns>
    Type AgentType { get; }

    /// <summary>
    /// 事件处理函数
    /// </summary>
    /// <param name="agent">组件代理</param>
    /// <param name="evt">事件</param>
    /// <returns>一个表示异步操作的任务</returns>
    Task HandleEvent(IComponentAgent agent, Event evt);
}