using GameFrameX.Core.Abstractions.Events;

namespace GameFrameX.Apps.Common.Event;

/// <summary>
/// 表示事件的特性。
/// </summary>
public sealed class EventAttribute : EventInfoAttribute
{
    /// <summary>
    /// 使用指定的事件ID初始化 <see cref="EventAttribute" /> 类的新实例。
    /// </summary>
    /// <param name="eventId">事件ID。</param>
    public EventAttribute(EventId eventId) : base((int)eventId)
    {
    }
}