using GameFrameX.Core.Abstractions.Agent;

namespace GameFrameX.Core.Abstractions.Events;

/// <summary>
/// 事件监听器接口
/// 每个实例其实都是单例的，用于处理特定类型的事件
/// 实现此接口的类需要保证线程安全
/// </summary>
public interface IEventListener
{
    /// <summary>
    /// 获取事件代理类型
    /// 用于标识此监听器可以处理哪种类型的事件代理
    /// </summary>
    /// <returns>事件代理的类型</returns>
    Type AgentType { get; }

    /// <summary>
    /// 事件处理函数
    /// 处理带有组件代理的事件，可以访问和操作组件的状态
    /// </summary>
    /// <param name="agent">组件代理，提供对组件的访问能力</param>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>一个表示异步操作的任务，当事件处理完成时完成</returns>
    Task HandleEvent(IComponentAgent agent, GameEventArgs gameEventArgs);

    /// <summary>
    /// 事件处理函数
    /// 处理不需要组件代理的独立事件
    /// </summary>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>一个表示异步操作的任务，当事件处理完成时完成</returns>
    Task HandleEvent(GameEventArgs gameEventArgs);
}