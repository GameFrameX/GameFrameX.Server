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

using GameFrameX.DataBase.Abstractions;
using MongoDB.Driver;
using System.Diagnostics.Metrics;

namespace GameFrameX.DataBase.Mongo;

/// <summary>
/// MongoDB服务连接类，实现了
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// 接口。
/// </summary>
/// <remarks>
/// MongoDB service connection class that implements the
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// interface.
/// </remarks>
public sealed partial class MongoDbService : IDatabaseService
{
    private static int _healthStatusValue = (int)DatabaseAvailabilityState.Healthy;
    private static readonly Meter DbMeter = new("GameFrameX.DataBase.Mongo", "1.0.0");
    private static readonly Counter<long> DbOpenRetryTotal = DbMeter.CreateCounter<long>("db_open_retry_total");
    private static readonly Counter<long> DbOperationRetryTotal = DbMeter.CreateCounter<long>("db_operation_retry_total");
    private static readonly Counter<long> DbOperationFailTotal = DbMeter.CreateCounter<long>("db_operation_fail_total");
    private static readonly Counter<long> DbHealthStateTransitionTotal = DbMeter.CreateCounter<long>("db_health_state_transition_total");
    private static readonly Counter<long> DbDegradeActionTotal = DbMeter.CreateCounter<long>("db_degrade_action_total");
    private static readonly ObservableGauge<int> DbHealthStatus = DbMeter.CreateObservableGauge("db_health_status", () => Volatile.Read(ref _healthStatusValue));
    private static readonly Histogram<double> DbOperationLatencyMilliseconds = DbMeter.CreateHistogram<double>("db_operation_latency_ms", unit: "ms");
    private static readonly Histogram<double> DbRecoveryDurationMilliseconds = DbMeter.CreateHistogram<double>("db_recovery_duration_ms", unit: "ms");
    private static readonly int[] DefaultReadRetryDelaysMilliseconds = { 120, 300, 700 };
    private static readonly int[] DefaultIdempotentWriteRetryDelaysMilliseconds = { 150, 400, 900 };
    private static readonly int[] DefaultTransactionRetryDelaysMilliseconds = { 200, 500, 1000 };
    private static readonly HashSet<string> NonCriticalReadOperationWhiteList = new(StringComparer.Ordinal)
    {
        nameof(FindListAsync),
        nameof(FindByIdsAsync),
        nameof(FindPageAsync),
        nameof(FindSortDescendingAsync),
        nameof(FindSortAscendingAsync),
        nameof(FindProjectedAsync),
        nameof(CountAsync),
        nameof(AnyAsync),
        nameof(ExistsByIdAsync),
    };
    private static readonly HashSet<string> CoreWriteOperationWhiteList = new(StringComparer.Ordinal)
    {
        nameof(AddAsync),
        nameof(AddListAsync),
        nameof(UpdateAsync),
        nameof(UpdatePartialAsync),
        nameof(DeleteAsync),
        nameof(DeleteListAsync),
        nameof(DeleteListIdAsync),
        nameof(HardDeleteAsync),
        nameof(RestoreAsync),
        nameof(AddOrUpdateAsync),
        nameof(AddOrUpdateListAsync),
        nameof(ExecuteInTransactionAsync),
    };

    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库。
    /// </summary>
    /// <remarks>
    /// Gets or sets the currently used MongoDB database.
    /// </remarks>
    public IMongoDatabase CurrentDatabase { get; private set; }

    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库配置选项。
    /// </summary>
    /// <remarks>
    /// Gets or sets the currently used MongoDB database configuration options.
    /// </remarks>
    public DbOptions Options{ get; private set; }

    /// <summary>
    /// 获取当前数据库可用性状态。
    /// </summary>
    public DatabaseAvailabilityState AvailabilityState => (DatabaseAvailabilityState)Volatile.Read(ref _availabilityState);

    private MongoDbContext _mongoDbContext;
    private MongoClient _mongoClient;
    private readonly object _availabilityLock = new();
    private int _availabilityState = (int)DatabaseAvailabilityState.Healthy;
    private int _consecutiveFailures;
    private int _consecutiveSuccesses;
    private int _recoveringProbeSuccessCount;
    private int _recoveringProbeWindowCount;
    private long _recoveringProbeWindowStartTicks;
    private long _unhealthySinceTicks;
    private CancellationTokenSource _recoveryTaskCancellationTokenSource;
    private Task _recoveryTask;
    private int _recoveryTaskStartedFlag;

    private TimeSpan _serverSelectionTimeout = TimeSpan.FromSeconds(5);
    private TimeSpan _connectTimeout = TimeSpan.FromSeconds(5);
    private TimeSpan _socketTimeout = TimeSpan.FromSeconds(10);
    private TimeSpan _recoveryProbeBaseDelay = TimeSpan.FromSeconds(3);
    private TimeSpan _recoveryProbeJitterDelay = TimeSpan.FromSeconds(2);
    private TimeSpan _recoveringProbeWindow = TimeSpan.FromSeconds(1);
    private int[] _readRetryDelaysMilliseconds = (int[])DefaultReadRetryDelaysMilliseconds.Clone();
    private int[] _idempotentWriteRetryDelaysMilliseconds = (int[])DefaultIdempotentWriteRetryDelaysMilliseconds.Clone();
    private int[] _transactionRetryDelaysMilliseconds = (int[])DefaultTransactionRetryDelaysMilliseconds.Clone();
    private int _healthyToDegradedFailureThreshold = 3;
    private int _degradedToUnhealthyFailureThreshold = 5;
    private int _recoveringToHealthySuccessThreshold = 3;
    private int _degradedToHealthySuccessThreshold = 3;
    private int _recoveringMaxProbePerSecond = 5;

    /// <summary>
    /// 应用数据库运行时配置并执行默认值/边界归一化。
    /// </summary>
    /// <param name="options">数据库选项。</param>
    private void ApplyRuntimeOptions(DbOptions options)
    {
        _serverSelectionTimeout = TimeSpan.FromMilliseconds(NormalizeMilliseconds(options.ServerSelectionTimeoutMilliseconds, 5000, 100));
        _connectTimeout = TimeSpan.FromMilliseconds(NormalizeMilliseconds(options.ConnectTimeoutMilliseconds, 5000, 100));
        _socketTimeout = TimeSpan.FromMilliseconds(NormalizeMilliseconds(options.SocketTimeoutMilliseconds, 10000, 100));
        _recoveryProbeBaseDelay = TimeSpan.FromMilliseconds(NormalizeMilliseconds(options.RecoveryProbeBaseDelayMilliseconds, 3000, 100));
        _recoveryProbeJitterDelay = TimeSpan.FromMilliseconds(NormalizeMilliseconds(options.RecoveryProbeJitterDelayMilliseconds, 2000, 0));
        _recoveringProbeWindow = TimeSpan.FromMilliseconds(NormalizeMilliseconds(options.RecoveringProbeWindowMilliseconds, 1000, 100));
        _readRetryDelaysMilliseconds = NormalizeRetryDelays(options.ReadRetryDelaysMilliseconds, DefaultReadRetryDelaysMilliseconds);
        _idempotentWriteRetryDelaysMilliseconds = NormalizeRetryDelays(options.IdempotentWriteRetryDelaysMilliseconds, DefaultIdempotentWriteRetryDelaysMilliseconds);
        _transactionRetryDelaysMilliseconds = NormalizeRetryDelays(options.TransactionRetryDelaysMilliseconds, DefaultTransactionRetryDelaysMilliseconds);
        _healthyToDegradedFailureThreshold = NormalizeThreshold(options.HealthyToDegradedFailureThreshold, 3);
        _degradedToUnhealthyFailureThreshold = NormalizeThreshold(options.DegradedToUnhealthyFailureThreshold, 5);
        _recoveringToHealthySuccessThreshold = NormalizeThreshold(options.RecoveringToHealthySuccessThreshold, 3);
        _degradedToHealthySuccessThreshold = NormalizeThreshold(options.DegradedToHealthySuccessThreshold, 3);
        _recoveringMaxProbePerSecond = NormalizeThreshold(options.RecoveringMaxProbePerSecond, 5);
    }

    /// <summary>
    /// 归一化毫秒配置。
    /// </summary>
    /// <param name="configuredValue">配置值。</param>
    /// <param name="defaultValue">默认值。</param>
    /// <param name="minValue">最小值。</param>
    /// <returns>归一化后的值。</returns>
    private static int NormalizeMilliseconds(int configuredValue, int defaultValue, int minValue)
    {
        var value = configuredValue > 0 ? configuredValue : defaultValue;
        return Math.Max(value, minValue);
    }

    /// <summary>
    /// 归一化阈值配置。
    /// </summary>
    /// <param name="configuredValue">配置值。</param>
    /// <param name="defaultValue">默认值。</param>
    /// <returns>归一化后的阈值。</returns>
    private static int NormalizeThreshold(int configuredValue, int defaultValue)
    {
        var value = configuredValue > 0 ? configuredValue : defaultValue;
        return Math.Max(value, 1);
    }

    /// <summary>
    /// 归一化重试延迟配置。
    /// </summary>
    /// <param name="configuredValue">配置值。</param>
    /// <param name="defaultValue">默认值。</param>
    /// <returns>归一化后的重试延迟数组。</returns>
    private static int[] NormalizeRetryDelays(int[] configuredValue, int[] defaultValue)
    {
        if (configuredValue == null || configuredValue.Length == 0)
        {
            return (int[])defaultValue.Clone();
        }

        var normalized = configuredValue.Where(static delay => delay >= 0).Distinct().OrderBy(static delay => delay).ToArray();
        if (normalized.Length == 0)
        {
            return (int[])defaultValue.Clone();
        }

        return normalized;
    }
}
