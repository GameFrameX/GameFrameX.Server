using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Core.Events;

/// <summary>
/// 事件监听器基类，用于处理特定组件代理类型的事件
/// </summary>
/// <typeparam name="T">组件代理类型，必须实现IComponentAgent接口</typeparam>
public abstract class EventListener<T> : IEventListener where T : IComponentAgent
{
    /// <summary>
    /// 实现IEventListener接口的事件处理函数
    /// </summary>
    /// <param name="agent">组件代理实例，类型为IComponentAgent</param>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>异步任务，表示事件处理的完成状态</returns>
    /// <remarks>
    /// 该方法会将传入的IComponentAgent类型转换为泛型类型T后调用保护方法HandleEvent
    /// </remarks>
    public Task HandleEvent(IComponentAgent agent, GameEventArgs gameEventArgs)
    {
        return HandleEvent((T)agent, gameEventArgs);
    }

    /// <summary>
    /// 无代理对象的事件处理函数重载
    /// </summary>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>异步任务，表示事件处理的完成状态</returns>
    /// <remarks>
    /// 该方法会以null作为agent参数调用另一个HandleEvent重载
    /// </remarks>
    public Task HandleEvent(GameEventArgs gameEventArgs)
    {
        return HandleEvent(null, gameEventArgs);
    }

    /// <summary>
    /// 获取事件代理类型属性
    /// </summary>
    /// <remarks>
    /// 返回泛型参数T的实际类型，用于运行时类型识别
    /// </remarks>
    public Type AgentType { get; } = typeof(T);

    /// <summary>
    /// 具体的事件处理实现方法
    /// </summary>
    /// <param name="agent">泛型类型的组件代理实例</param>
    /// <param name="gameEventArgs">需要处理的事件对象</param>
    /// <returns>异步任务，表示事件处理的完成状态</returns>
    /// <remarks>
    /// 派生类必须实现该抽象方法来提供具体的事件处理逻辑
    /// </remarks>
    protected abstract Task HandleEvent(T agent, GameEventArgs gameEventArgs);
}