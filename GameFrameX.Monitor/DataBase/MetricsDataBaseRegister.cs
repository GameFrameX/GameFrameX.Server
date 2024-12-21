using Prometheus;

namespace GameFrameX.Monitor.DataBase;

/// <summary>
/// 数据库监控帮助类
/// </summary>
public static class MetricsDataBaseRegister
{
    private static Counter _findCounterOptions;

    /// <summary>
    /// 数据库查询次数
    /// </summary>
    public static Counter FindCounterOptions
    {
        get { return _findCounterOptions ??= Metrics.CreateCounter("database_find_count", "数据库查询次数"); }
    }
}