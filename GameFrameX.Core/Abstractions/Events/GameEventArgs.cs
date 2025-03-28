using GameFrameX.Utility;

namespace GameFrameX.Core.Abstractions.Events;

/// <summary>
/// 事件参数
/// </summary>
public struct GameEventArgs
{
    /// <summary>
    /// 默认事件
    /// </summary>
    public static readonly GameEventArgs Default = new GameEventArgs();

    /// <summary>
    /// 事件id
    /// </summary>
    public int EventId;

    /// <summary>
    /// 事件参数
    /// </summary>
    public Param Data;
}