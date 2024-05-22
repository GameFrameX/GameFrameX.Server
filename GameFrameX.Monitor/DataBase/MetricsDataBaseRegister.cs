using Prometheus;

namespace GameFrameX.Monitor.DataBase;

public static class MetricsDataBaseRegister
{
    static Counter _findCounterOptions;

    /// <summary>
    /// 数据库查询次数
    /// </summary>
    public static Counter FindCounterOptions
    {
        get { return _findCounterOptions ??= Prometheus.Metrics.CreateCounter("database_find_count", "数据库查询次数"); }
    }
}