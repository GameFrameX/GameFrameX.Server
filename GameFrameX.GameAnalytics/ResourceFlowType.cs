namespace GameFrameX.GameAnalytics;

/// <summary>
/// 资源流动类型
/// </summary>
public enum ResourceFlowType
{
    /// <summary>
    /// 未定义
    /// </summary>
    Undefined,

    /// <summary>
    /// 玩家获得或获得资源的时间
    /// </summary>
    Source,

    /// <summary>
    /// 玩家丢失或花费资源
    /// </summary>
    Sink,
}