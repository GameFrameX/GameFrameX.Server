// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using Prometheus;

namespace GameFrameX.Monitor.DataBase;

/// <summary>
/// 数据库监控帮助类
/// </summary>
public static class MetricsDataBaseHelper
{
    private static Counter _findCounterOptions;

    /// <summary>
    /// 数据库查询次数
    /// </summary>
    public static Counter FindCounterOptions
    {
        get { return _findCounterOptions ??= Metrics.CreateCounter("database_find_count", "数据库查询次数"); }
    }

    private static Counter _deleteCounterOptions;
    private static Counter _createCounterOptions;
    private static Counter _updateCounterOptions;

    /// <summary>
    /// 数据库删除次数
    /// </summary>
    public static Counter DeleteCounterOptions
    {
        get { return _deleteCounterOptions ??= Metrics.CreateCounter("database_delete_count", "数据库删除次数"); }
    }

    /// <summary>
    /// 数据库创建次数
    /// </summary>
    public static Counter CreateCounterOptions
    {
        get { return _createCounterOptions ??= Metrics.CreateCounter("database_create_count", "数据库创建次数"); }
    }

    /// <summary>
    /// 数据库修改次数
    /// </summary>
    public static Counter UpdateCounterOptions
    {
        get { return _updateCounterOptions ??= Metrics.CreateCounter("database_update_count", "数据库修改次数"); }
    }

    private static Gauge _connectionPoolGauge;
    private static Gauge _activeConnectionsGauge;
    private static Histogram _queryDurationHistogram;
    private static Counter _queryErrorCounter;
    private static Counter _deadlockCounter;
    private static Gauge _transactionActiveGauge;

    /// <summary>
    /// 数据库连接池大小
    /// </summary>
    public static Gauge ConnectionPoolGauge
    {
        get { return _connectionPoolGauge ??= Metrics.CreateGauge("database_connection_pool_size", "数据库连接池大小"); }
    }

    /// <summary>
    /// 活跃连接数
    /// </summary>
    public static Gauge ActiveConnectionsGauge
    {
        get { return _activeConnectionsGauge ??= Metrics.CreateGauge("database_active_connections", "当前活跃数据库连接数"); }
    }

    /// <summary>
    /// 查询执行时间分布
    /// </summary>
    public static Histogram QueryDurationHistogram
    {
        get
        {
            return _queryDurationHistogram ??= Metrics.CreateHistogram(
                       "database_query_duration_seconds",
                       "数据库查询执行时间",
                       new HistogramConfiguration
                       {
                           Buckets = new[] { .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10 }
                       });
        }
    }

    /// <summary>
    /// 查询错误计数
    /// </summary>
    public static Counter QueryErrorCounter
    {
        get { return _queryErrorCounter ??= Metrics.CreateCounter("database_query_errors_total", "数据库查询错误总数"); }
    }

    /// <summary>
    /// 死锁次数
    /// </summary>
    public static Counter DeadlockCounter
    {
        get { return _deadlockCounter ??= Metrics.CreateCounter("database_deadlocks_total", "数据库死锁发生次数"); }
    }

    /// <summary>
    /// 活跃事务数
    /// </summary>
    public static Gauge TransactionActiveGauge
    {
        get { return _transactionActiveGauge ??= Metrics.CreateGauge("database_active_transactions", "当前活跃事务数"); }
    }
}