namespace GameFrameX.DataBase.Abstractions;

/// <summary>
/// 数据库可用性状态。
/// </summary>
public enum DatabaseAvailabilityState
{
    /// <summary>
    /// 健康状态，可全量提供服务。
    /// </summary>
    Healthy = 0,

    /// <summary>
    /// 降级状态，出现轻度失败抖动。
    /// </summary>
    Degraded = 1,

    /// <summary>
    /// 不健康状态，触发后台恢复。
    /// </summary>
    Unhealthy = 2,

    /// <summary>
    /// 恢复中状态，半开限量探测。
    /// </summary>
    Recovering = 3,
}
