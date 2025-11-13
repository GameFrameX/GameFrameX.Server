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

namespace GameFrameX.DiscoveryCenterManager.Strategy.Models;

/// <summary>
/// 实例指标
/// </summary>
public class InstanceMetrics
{
    /// <summary>
    /// 实例ID
    /// </summary>
    public long InstanceId { get; set; }

    /// <summary>
    /// 总请求数
    /// </summary>
    public long TotalRequests { get; set; }

    /// <summary>
    /// 成功请求数
    /// </summary>
    public long SuccessfulRequests { get; set; }

    /// <summary>
    /// 失败请求数
    /// </summary>
    public long FailedRequests { get; set; }

    /// <summary>
    /// 当前活跃连接数
    /// </summary>
    public int ActiveConnections { get; set; }

    /// <summary>
    /// 平均响应时间（毫秒）
    /// </summary>
    public double AverageResponseTimeMs { get; set; }

    /// <summary>
    /// CPU使用率（百分比）
    /// </summary>
    public double CpuUsage { get; set; }

    /// <summary>
    /// 内存使用率（百分比）
    /// </summary>
    public double MemoryUsage { get; set; }

    /// <summary>
    /// 网络吞吐量（KB/s）
    /// </summary>
    public double NetworkThroughputKbps { get; set; }

    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// 成功率
    /// </summary>
    public double SuccessRate
    {
        get { return TotalRequests > 0 ? (double)SuccessfulRequests / TotalRequests * 100 : 100; }
    }

    /// <summary>
    /// 错误率
    /// </summary>
    public double ErrorRate => 100 - SuccessRate;

    /// <summary>
    /// 每秒请求数
    /// </summary>
    public double RequestsPerSecond
    {
        get { return CalculateRps(); }
    }

    /// <summary>
    /// 负载分数（用于负载均衡决策）
    /// </summary>
    public double LoadScore
    {
        get { return CalculateLoadScore(); }
    }

    /// <summary>
    /// 健康分数（用于健康评估）
    /// </summary>
    public double HealthScore
    {
        get { return CalculateHealthScore(); }
    }

    private double CalculateRps()
    {
        var timeSinceLastUpdate = System.DateTime.UtcNow - LastUpdated;
        var seconds = Math.Max(1, timeSinceLastUpdate.TotalSeconds);
        return TotalRequests / seconds;
    }

    private double CalculateLoadScore()
    {
        // 权重配置
        const double responseTimeWeight = 0.4;
        const double connectionWeight = 0.2;
        const double errorRateWeight = 0.2;
        const double resourceWeight = 0.2;

        // 响应时间分数（0-100，越高越好）
        var responseTimeScore = Math.Max(0, 100 - AverageResponseTimeMs / 10);

        // 连接数分数（0-100，越高越好，连接数越少分数越高）
        var connectionScore = Math.Max(0, 100 - ActiveConnections);

        // 错误率分数（0-100，越高越好）
        var errorRateScore = Math.Max(0, 100 - ErrorRate);

        // 资源使用率分数（0-100，越高越好，使用率越低分数越高）
        var resourceScore = Math.Max(0, 200 - (CpuUsage + MemoryUsage)) / 2;

        // 计算综合负载分数（0-100，越高越好）
        return (responseTimeScore * responseTimeWeight) +
               (connectionScore * connectionWeight) +
               (errorRateScore * errorRateWeight) +
               (resourceScore * resourceWeight);
    }

    private double CalculateHealthScore()
    {
        if (TotalRequests == 0)
        {
            return 100.0;
        }

        // 基于错误率和响应时间的健康分数
        var errorHealth = Math.Max(0, 100 - ErrorRate * 2);
        var responseHealth = Math.Max(0, 100 - AverageResponseTimeMs / 20);
        var resourceHealth = Math.Max(0, 200 - (CpuUsage + MemoryUsage)) / 2;

        return (errorHealth + responseHealth + resourceHealth) / 4;
    }

    /// <summary>
    /// 克隆指标
    /// </summary>
    public InstanceMetrics Clone()
    {
        return new InstanceMetrics
        {
            InstanceId = InstanceId,
            TotalRequests = TotalRequests,
            SuccessfulRequests = SuccessfulRequests,
            FailedRequests = FailedRequests,
            ActiveConnections = ActiveConnections,
            AverageResponseTimeMs = AverageResponseTimeMs,
            CpuUsage = CpuUsage,
            MemoryUsage = MemoryUsage,
            NetworkThroughputKbps = NetworkThroughputKbps,
            LastUpdated = LastUpdated
        };
    }
}