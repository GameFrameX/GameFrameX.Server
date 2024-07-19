using GameFrameX.Utility;

namespace GameFrameX.Core.Abstractions.Events;

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