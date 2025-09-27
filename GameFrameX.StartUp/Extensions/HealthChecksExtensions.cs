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



using GameFrameX.Utility.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.StartUp.Extensions;

/// <summary>
/// 健康检查配置扩展方法
/// </summary>
public static class HealthChecksExtensions
{
    private const string JsonContentType = "application/json; charset=utf-8";

    /// <summary>
    /// 默认健康检查端点路径
    /// </summary>
    /// <value>返回默认的健康检查端点路径 "/health"</value>
    public const string DefaultHealthCheckPath = "/health";

    /// <summary>
    /// 简单健康检查端点路径（兼容性端点）
    /// </summary>
    public const string SimpleHealthCheckPath = $"{DefaultHealthCheckPath}/simple";

    /// <summary>
    /// OpenTelemetry 健康检查端点路径
    /// </summary>
    public const string OpenTelemetryHealthCheckPath = $"{DefaultHealthCheckPath}/opentelemetry";

    /// <summary>
    /// 添加 GameFrameX 健康检查服务（包含多种检查项目）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="setting">应用设置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddGameFrameXHealthChecks(this IServiceCollection services, AppSetting setting)
    {
        var healthChecksBuilder = services.AddHealthChecks();
        // 基础应用程序健康检查
        healthChecksBuilder.AddCheck(DefaultHealthCheckPath, () => HealthCheckResult.Healthy("应用程序运行正常"));
        healthChecksBuilder.AddCheck(SimpleHealthCheckPath, () => HealthCheckResult.Healthy("应用程序运行正常"));

        // OpenTelemetry相关检查
        if (setting.IsOpenTelemetry)
        {
            healthChecksBuilder.AddCheck(OpenTelemetryHealthCheckPath, () => HealthCheckResult.Healthy("OpenTelemetry 配置正常"));
        }

        return services;
    }

    /// <summary>
    /// 使用 GameFrameX 健康检查中间件
    /// </summary>
    /// <param name="app">应用程序构建器，用于配置HTTP请求管道</param>
    /// <param name="setting">应用设置对象，包含应用程序的配置信息</param>
    /// <param name="ipList"></param>
    /// <returns>返回配置了健康检查端点的应用程序构建器</returns>
    /// <remarks>
    /// 此方法会配置以下健康检查端点：
    /// <list type="bullet">
    /// <item><description><c>/health</c> - 详细的JSON格式健康检查报告</description></item>
    /// <item><description><c>/health/simple</c> - 简单的"OK"响应（兼容性端点）</description></item>
    /// </list>
    /// 健康检查响应包含状态、检查项详情、持续时间、服务器信息和时间戳。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 使用默认路径
    /// app.UseGameFrameXHealthChecks(appSetting);
    /// 
    /// // 使用自定义路径
    /// app.UseGameFrameXHealthChecks(appSetting, "/api/health");
    /// </code>
    /// </example>
    /// <seealso cref="AddGameFrameXHealthChecks"/>
    /// <seealso cref="AppSetting"/>
    public static IApplicationBuilder UseGameFrameXHealthChecks(this IApplicationBuilder app, AppSetting setting, List<string> ipList)
    {
        var defaultHealthCheckOptions = new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = JsonContentType;

                var response = new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(entry => new
                    {
                        name = entry.Key,
                        status = entry.Value.Status.ToString(),
                        description = entry.Value.Description,
                        duration = entry.Value.Duration.TotalMilliseconds
                    }),
                    totalDuration = report.TotalDuration.TotalMilliseconds,
                    serverName = setting.ServerName ?? "Unknown",
                    tagName = setting.TagName ?? "Unknown",
                    timestamp = DateTime.UtcNow
                };

                await context.Response.WriteAsync(JsonHelper.Serialize(response));
            }
        };

        // 配置详细的健康检查端点
        app.UseHealthChecks(DefaultHealthCheckPath, defaultHealthCheckOptions);

        var simpleHealthCheckOptions = new HealthCheckOptions
        {
            Predicate = _ => false, // 不执行任何检查，直接返回成功
            ResponseWriter = async (context, _) =>
            {
                context.Response.ContentType = JsonContentType;
                await context.Response.WriteAsync("OK");
            },
        };

        // 配置简单的健康检查端点（兼容性）
        app.UseHealthChecks(SimpleHealthCheckPath, simpleHealthCheckOptions);
        // OpenTelemetry相关检查
        if (setting.IsOpenTelemetry)
        {
            var openTelemetryHealthCheckOptions = new HealthCheckOptions
            {
                Predicate = _ => false, // 不执行任何检查，直接返回成功
                ResponseWriter = async (context, _) =>
                {
                    context.Response.ContentType = JsonContentType;
                    await context.Response.WriteAsync("OpenTelemetry 配置正常");
                },
            };
            app.UseHealthChecks(OpenTelemetryHealthCheckPath, openTelemetryHealthCheckOptions);
        }

        LogHelper.InfoConsole("健康检查端点已启用:");
        foreach (var ip in ipList)
        {
            LogHelper.InfoConsole($"- 详细健康检查: http://{ip}:{setting.HttpPort}{DefaultHealthCheckPath}");
            LogHelper.InfoConsole($"- 简单健康检查: http://{ip}:{setting.HttpPort}{SimpleHealthCheckPath}");
            if (setting.IsOpenTelemetry)
            {
                LogHelper.InfoConsole($"- OpenTelemetry 检查: http://{ip}:{setting.HttpPort}{OpenTelemetryHealthCheckPath}");
            }
        }

        return app;
    }
}