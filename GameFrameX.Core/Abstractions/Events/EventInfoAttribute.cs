namespace GameFrameX.Core.Abstractions.Events;

/// <summary>
/// 事件监听器
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EventInfoAttribute : System.Attribute
{
    /// <summary>
    /// 事件监听器
    /// </summary>
    /// <param name="eventId">事件id</param>
    public EventInfoAttribute(int eventId)
    {
        EventId = eventId;
    }

    /// <summary>
    /// 事件id
    /// </summary>
    public int EventId { get; }
}