using System.Diagnostics.Metrics;

namespace GameFrameX.Monitor.DataBase;

/// <summary>
/// 数据库监控帮助类
/// </summary>
public static class MetricsDataBaseHelper
{
    private static Counter<ulong> _findCounterOptions;

    private const string ModuleName = "database.";

    /// <summary>
    /// 数据库查询次数
    /// </summary>
    public static Counter<ulong> FindCounterOptions
    {
        get { return _findCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}find_count", "次", "数据库查询次数"); }
    }

    private static Counter<ulong> _deleteCounterOptions;
    private static Counter<ulong> _createCounterOptions;
    private static Counter<ulong> _updateCounterOptions;

    /// <summary>
    /// 数据库删除次数
    /// </summary>
    public static Counter<ulong> DeleteCounterOptions
    {
        get { return _deleteCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}delete_count", "次", "数据库删除次数"); }
    }

    /// <summary>
    /// 数据库创建次数
    /// </summary>
    public static Counter<ulong> CreateCounterOptions
    {
        get { return _createCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}create_count", "次", "数据库创建次数"); }
    }

    /// <summary>
    /// 数据库修改次数
    /// </summary>
    public static Counter<ulong> UpdateCounterOptions
    {
        get { return _updateCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}update_count", "次", "数据库修改次数"); }
    }

    private static Histogram<float> _queryDurationHistogram;
    private static Counter<ulong> _queryErrorCounter;

    /// <summary>
    /// 查询执行时间分布
    /// </summary>
    public static Histogram<float> QueryDurationHistogram
    {
        get { return _queryDurationHistogram ??= MetricsHelper.Meter.CreateHistogram<float>($"{ModuleName}query_duration_seconds", "ms", "数据库查询执行时间"); }
    }

    /// <summary>
    /// 查询错误计数
    /// </summary>
    public static Counter<ulong> QueryErrorCounter
    {
        get { return _queryErrorCounter ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}query_errors_total", "次", "数据库查询错误总数"); }
    }
}