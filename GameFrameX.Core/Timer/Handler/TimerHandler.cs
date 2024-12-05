using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Utility;

namespace GameFrameX.Core.Timer.Handler;

/// <summary>
/// 热更新程序集的计时器处理器
/// </summary>
/// <typeparam name="TAgent">组件类型，必须实现 <see cref="IComponentAgent"/> 接口</typeparam>
public abstract class TimerHandler<TAgent> : ITimerHandler where TAgent : IComponentAgent
{
    /// <summary>
    /// 内部计时器处理器调用。由 Quartz 调用
    /// </summary>
    /// <param name="agent">组件代理对象，用于与系统其他部分交互</param>
    /// <param name="param">传递给处理器的参数</param>
    /// <returns>一个任务，表示异步操作的结果</returns>
    public Task InnerHandleTimer(IComponentAgent agent, Param param)
    {
        return HandleTimer((TAgent)agent, param);
    }

    /// <summary>
    /// 计时器调用
    /// </summary>
    /// <param name="agent">调用对象，具体类型的组件代理对象</param>
    /// <param name="param">参数对象，传递给处理器的参数</param>
    /// <returns>一个任务，表示异步操作的结果</returns>
    protected abstract Task HandleTimer(TAgent agent, Param param);
}