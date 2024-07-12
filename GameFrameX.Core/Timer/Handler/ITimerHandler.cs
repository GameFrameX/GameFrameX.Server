using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.Utility;

namespace GameFrameX.Core.Timer.Handler
{
    /// <summary>
    /// 计时器处理器接口
    /// </summary>
    public interface ITimerHandler
    {
        /// <summary>
        /// 内部计时器处理器调用函数
        /// </summary>
        /// <param name="agent">组件代理对象</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Task InnerHandleTimer(IComponentAgent agent, Param param);
    }
}