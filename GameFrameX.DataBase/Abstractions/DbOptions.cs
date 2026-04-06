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


namespace GameFrameX.DataBase.Abstractions;

/// <summary>
/// 数据库选项。
/// </summary>
/// <remarks>
/// Database configuration options.
/// </remarks>
public sealed record DbOptions
{
    private static readonly int[] DefaultReadRetryDelaysMilliseconds = { 120, 300, 700 };
    private static readonly int[] DefaultIdempotentWriteRetryDelaysMilliseconds = { 150, 400, 900 };
    private static readonly int[] DefaultTransactionRetryDelaysMilliseconds = { 200, 500, 1000 };

    /// <summary>
    /// 数据库类型。
    /// </summary>
    /// <remarks>
    /// Database type.
    /// </remarks>
    /// <value>数据库类型 / Database type</value>
    public string Type { get; init; }

    /// <summary>
    /// 连接字符串。
    /// </summary>
    /// <remarks>
    /// Connection string.
    /// </remarks>
    /// <value>连接字符串 / Connection string</value>
    public string ConnectionString { get; init; }

    /// <summary>
    /// 数据库名称。
    /// </summary>
    /// <remarks>
    /// Database name.
    /// </remarks>
    /// <value>数据库名称 / Database name</value>
    public string Name { get; init; }

    /// <summary>
    /// 是否使用时区时间记录。
    /// </summary>
    /// <remarks>
    /// Whether to use time zone for time recording.
    /// </remarks>
    /// <value>是否使用时区时间记录 / Whether to use time zone for time recording</value>
    public bool IsUseTimeZone { get; init; } = false;

    /// <summary>
    /// MongoDB ServerSelection 超时（毫秒）。
    /// </summary>
    /// <value>默认 5000 毫秒。</value>
    public int ServerSelectionTimeoutMilliseconds { get; init; } = 5000;

    /// <summary>
    /// MongoDB Connect 超时（毫秒）。
    /// </summary>
    /// <value>默认 5000 毫秒。</value>
    public int ConnectTimeoutMilliseconds { get; init; } = 5000;

    /// <summary>
    /// MongoDB Socket 超时（毫秒）。
    /// </summary>
    /// <value>默认 10000 毫秒。</value>
    public int SocketTimeoutMilliseconds { get; init; } = 10000;

    /// <summary>
    /// 读操作重试延迟（毫秒）。
    /// </summary>
    /// <value>默认 [120, 300, 700]。</value>
    public int[] ReadRetryDelaysMilliseconds { get; init; } = (int[])DefaultReadRetryDelaysMilliseconds.Clone();

    /// <summary>
    /// 幂等写重试延迟（毫秒）。
    /// </summary>
    /// <value>默认 [150, 400, 900]。</value>
    public int[] IdempotentWriteRetryDelaysMilliseconds { get; init; } = (int[])DefaultIdempotentWriteRetryDelaysMilliseconds.Clone();

    /// <summary>
    /// 事务重试延迟（毫秒）。
    /// </summary>
    /// <value>默认 [200, 500, 1000]。</value>
    public int[] TransactionRetryDelaysMilliseconds { get; init; } = (int[])DefaultTransactionRetryDelaysMilliseconds.Clone();

    /// <summary>
    /// 后台恢复探活基础延迟（毫秒）。
    /// </summary>
    /// <value>默认 3000 毫秒。</value>
    public int RecoveryProbeBaseDelayMilliseconds { get; init; } = 3000;

    /// <summary>
    /// 后台恢复探活随机抖动上限（毫秒）。
    /// </summary>
    /// <value>默认 2000 毫秒。</value>
    public int RecoveryProbeJitterDelayMilliseconds { get; init; } = 2000;

    /// <summary>
    /// Recovering 半开探测窗口（毫秒）。
    /// </summary>
    /// <value>默认 1000 毫秒。</value>
    public int RecoveringProbeWindowMilliseconds { get; init; } = 1000;

    /// <summary>
    /// Healthy 进入 Degraded 的连续失败阈值。
    /// </summary>
    /// <value>默认 3。</value>
    public int HealthyToDegradedFailureThreshold { get; init; } = 3;

    /// <summary>
    /// Degraded 进入 Unhealthy 的连续失败阈值。
    /// </summary>
    /// <value>默认 5。</value>
    public int DegradedToUnhealthyFailureThreshold { get; init; } = 5;

    /// <summary>
    /// Recovering 恢复到 Healthy 的连续成功阈值。
    /// </summary>
    /// <value>默认 3。</value>
    public int RecoveringToHealthySuccessThreshold { get; init; } = 3;

    /// <summary>
    /// Degraded 恢复到 Healthy 的连续成功阈值。
    /// </summary>
    /// <value>默认 3。</value>
    public int DegradedToHealthySuccessThreshold { get; init; } = 3;

    /// <summary>
    /// Recovering 状态下每秒允许的最大探测数。
    /// </summary>
    /// <value>默认 5。</value>
    public int RecoveringMaxProbePerSecond { get; init; } = 5;
}
