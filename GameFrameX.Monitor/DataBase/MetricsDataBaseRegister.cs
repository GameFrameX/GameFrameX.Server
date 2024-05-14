using Prometheus;

namespace GameFrameX.Monitor.DataBase;

public static class MetricsDataBaseRegister
{
    static Counter _findCounterOptions;

    public static Counter FindCounterOptions
    {
        get { return _findCounterOptions ??= Prometheus.Metrics.CreateCounter("database_find", "DataBase"); }
    }
}