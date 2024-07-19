using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Core.Events
{
    /// <summary>
    /// 事件监听器
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EventListener<T> : IEventListener where T : IComponentAgent
    {
        /// <summary>
        /// 事件处理函数
        /// </summary>
        /// <param name="agent"> 组件代理 </param>
        /// <param name="evt"> 事件 </param>
        /// <returns></returns>
        public Task HandleEvent(IComponentAgent agent, Event evt)
        {
            return HandleEvent((T)agent, evt);
        }

        /// <summary>
        /// 事件处理函数
        /// </summary>
        /// <param name="agent"></param>
        /// <param name="evt"></param>
        /// <returns></returns>
        protected abstract Task HandleEvent(T agent, Event evt);

        /// <summary>
        /// 事件代理类型
        /// </summary>
        /// <returns></returns>
        public Type AgentType { get; } = typeof(T);
    }
}