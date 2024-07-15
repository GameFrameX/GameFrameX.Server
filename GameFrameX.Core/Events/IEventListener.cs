using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.Utility;

namespace GameFrameX.Core.Events
{
    /// <summary>
    /// 每个实例其实都是单例的
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// 事件处理函数
        /// </summary>
        /// <param name="agent"> 组件代理 </param>
        /// <param name="evt"> 事件 </param>
        /// <returns></returns>
        Task HandleEvent(IComponentAgent agent, Event evt);

        /// <summary>
        /// 事件代理类型
        /// </summary>
        /// <returns></returns>
        Type AgentType { get; }
    }

    /// <summary>
    /// 事件监听器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventInfoAttribute : Attribute
    {
        /// <summary>
        /// 事件id
        /// </summary>
        public int EventId { get; }

        /// <summary>
        /// 事件监听器
        /// </summary>
        /// <param name="eventId">事件id</param>
        public EventInfoAttribute(int eventId)
        {
            EventId = eventId;
        }
    }

    /// <summary>
    /// 事件参数
    /// </summary>
    public struct Event
    {
        /// <summary>
        /// 空
        /// </summary>
        public static Event Null = new Event();

        /// <summary>
        /// 事件id
        /// </summary>
        public int EventId;

        /// <summary>
        /// 事件参数
        /// </summary>
        public Param Data;
    }
}