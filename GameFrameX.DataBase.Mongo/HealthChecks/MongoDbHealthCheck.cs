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

using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Driver;

namespace GameFrameX.DataBase.Mongo.HealthChecks;

/// <summary>
/// MongoDB 健康检查类。
/// </summary>
/// <remarks>
/// MongoDB health check class.
/// </remarks>
public sealed class MongoDbHealthCheck : IHealthCheck
{
    private const int ConsecutiveFailureThreshold = 5;
    private readonly IMongoClient _mongoClient;
    private int _consecutiveFailures;

    /// <summary>
    /// 初始化 MongoDbHealthCheck 的新实例。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of MongoDbHealthCheck.
    /// </remarks>
    /// <param name="mongoClient">MongoDB 客户端 / MongoDB client</param>
    public MongoDbHealthCheck(IMongoClient mongoClient)
    {
        _mongoClient = mongoClient;
    }

    /// <summary>
    /// 执行健康检查。
    /// </summary>
    /// <remarks>
    /// Performs the health check.
    /// </remarks>
    /// <param name="context">健康检查上下文 / Health check context</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>健康检查结果 / Health check result</returns>
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var databases = await _mongoClient.ListDatabaseNamesAsync(cancellationToken);

            if (databases != null && await databases.AnyAsync(cancellationToken))
            {
                Interlocked.Exchange(ref _consecutiveFailures, 0);
                return HealthCheckResult.Healthy("MongoDB connection is healthy");
            }

            var currentFailures = Interlocked.Increment(ref _consecutiveFailures);
            if (currentFailures >= ConsecutiveFailureThreshold)
            {
                return HealthCheckResult.Unhealthy($"MongoDB connection is unhealthy after {currentFailures} consecutive failures");
            }

            return HealthCheckResult.Degraded($"MongoDB connection is degraded: no databases found. consecutiveFailures={currentFailures}");
        }
        catch (Exception ex)
        {
            var currentFailures = Interlocked.Increment(ref _consecutiveFailures);
            if (currentFailures >= ConsecutiveFailureThreshold)
            {
                return HealthCheckResult.Unhealthy($"MongoDB connection is unhealthy after {currentFailures} consecutive failures", ex);
            }

            return HealthCheckResult.Degraded($"MongoDB connection is temporarily unavailable. consecutiveFailures={currentFailures}", ex);
        }
    }
}
