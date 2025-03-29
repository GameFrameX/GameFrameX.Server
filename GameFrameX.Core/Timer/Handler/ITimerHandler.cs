using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Core.Timer.Handler;

/// <summary>
/// 计时器处理器接口
/// </summary>
public interface ITimerHandler
{
    /// <summary>
    /// 内部计时器处理器调用函数
    /// </summary>
    /// <param name="agent">组件代理对象，用于与系统其他部分交互</param>
    /// <param name="gameEventArgs">传递给处理器的参数</param>
    /// <returns>一个任务，表示异步操作的结果</returns>
    Task InnerHandleTimer(IComponentAgent agent, GameEventArgs gameEventArgs);
}