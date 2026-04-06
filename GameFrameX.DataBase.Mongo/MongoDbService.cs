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
    private static readonly TimeSpan ServerSelectionTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan ConnectTimeout = TimeSpan.FromSeconds(5);
    private static readonly TimeSpan SocketTimeout = TimeSpan.FromSeconds(10);
    private static readonly int[] ReadRetryDelaysMilliseconds = { 120, 300, 700 };
    private static readonly int[] IdempotentWriteRetryDelaysMilliseconds = { 150, 400, 900 };
    private static readonly int[] TransactionRetryDelaysMilliseconds = { 200, 500, 1000 };
    private static readonly TimeSpan RecoveryProbeBaseDelay = TimeSpan.FromSeconds(3);
    private static readonly TimeSpan RecoveryProbeJitterDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan RecoveringProbeWindow = TimeSpan.FromSeconds(1);
    private static readonly int HealthyToDegradedFailureThreshold = 3;
    private static readonly int DegradedToUnhealthyFailureThreshold = 5;
    private static readonly int RecoveringToHealthySuccessThreshold = 3;
    private static readonly int DegradedToHealthySuccessThreshold = 3;
    private static readonly int RecoveringMaxProbePerSecond = 5;
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
}
