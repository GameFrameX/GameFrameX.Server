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

using GameFrameX.Foundation.Logger;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.IO;
using System.Net.Http;
using System.Threading;
using GameFrameX.Foundation.Utility;

namespace GameFrameX.StartUp.Extensions;

/// <summary>
/// OpenTelemetry configuration extension methods / OpenTelemetry 配置扩展方法
/// </summary>
/// <remarks>
/// 提供用于配置和使用OpenTelemetry可观测性功能的扩展方法，支持指标收集、链路追踪和日志记录。
/// 包含Grafana集成、Prometheus指标导出和独立指标服务器创建等功能。
/// </remarks>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// Add GameFrameX OpenTelemetry configuration / 添加 GameFrameX OpenTelemetry 配置
    /// </summary>
    /// <param name="services">Service collection / 服务集合</param>
    /// <param name="setting">Application settings / 应用设置</param>
    /// <param name="servicePrefix">Service name prefix / 服务名前缀</param>
    /// <param name="tracingSourcePrefix">Tracing source prefix / 追踪源前缀</param>
    /// <returns>Service collection / 服务集合</returns>
    /// <remarks>
    /// 根据配置启用OpenTelemetry的指标收集和链路追踪功能。
    /// 自动配置ASP.NET Core、HTTP客户端和系统运行时的指标收集。
    /// 支持开发环境下的控制台输出和Prometheus指标导出。
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when services or setting is null / 当services或setting为null时抛出</exception>
    /// <example>
    /// <code>
    /// // 基本配置
    /// services.AddGameFrameXOpenTelemetry(appSetting);
    /// 
    /// // 带前缀配置
    /// services.AddGameFrameXOpenTelemetry(appSetting, "MyService", "MyApp");
    /// </code>
    /// </example>
    public static IServiceCollection AddGameFrameXOpenTelemetry(this IServiceCollection services, AppSetting setting, string servicePrefix = "", string tracingSourcePrefix = "")
    {
        if (!setting.IsOpenTelemetry)
        {
            return services;
        }

        var serviceName = string.IsNullOrEmpty(servicePrefix)
                              ? $"{setting.ServerName}-{setting.TagName}"
                              : $"{servicePrefix}:{setting.ServerName}-{setting.TagName}";

        var tracingSource = string.IsNullOrEmpty(tracingSourcePrefix)
                                ? $"GameFrameX.{setting.ServerName}.{setting.TagName}"
                                : $"{tracingSourcePrefix}:GameFrameX.{setting.ServerName}.{setting.TagName}";

        var builder = services.AddOpenTelemetry()
                              .ConfigureResource(configure =>
                              {
                                  configure.AddService(serviceName, string.IsNullOrEmpty(servicePrefix) ? "GameFrameX" : "GameFrameX.HTTP")
                                           .AddTelemetrySdk();
                              });

        if (setting.IsOpenTelemetryMetrics)
        {
            builder.WithMetrics(configure =>
            {
                configure.AddAspNetCoreInstrumentation();
                if (EnvironmentHelper.IsDevelopment() && LogOptions.Default.IsConsole)
                {
                    configure.AddConsoleExporter();
                }

                // Metrics provides by ASP.NET Core in .NET 8
                configure.AddMeter("Microsoft.AspNetCore.Hosting");
                configure.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

                // Metrics provided by System.Net libraries
                configure.AddMeter("System.Net.Http");
                configure.AddMeter("System.Net.NameResolution");

                // Metrics provided by .NET Runtime (available in .NET 9+)
                configure.AddMeter("System.Runtime");

                // Additional ASP.NET Core metrics for Blazor components (if applicable)
                // configure.AddMeter("Microsoft.AspNetCore.Components");
                // configure.AddMeter("Microsoft.AspNetCore.Components.Lifecycle");
                // configure.AddMeter("Microsoft.AspNetCore.Components.Server.Circuits");
                configure.AddPrometheusExporter();
            });
        }

        if (setting.IsOpenTelemetryTracing)
        {
            builder.WithTracing(configure =>
            {
                configure.AddAspNetCoreInstrumentation();
                configure.AddHttpClientInstrumentation();
                configure.AddSource(tracingSource);
                if (EnvironmentHelper.IsDevelopment() && LogOptions.Default.IsConsole)
                {
                    configure.AddConsoleExporter();
                }
            });
        }

        builder.UseGrafana();
        
        // 添加健康检查服务
        services.AddHealthChecks();
        
        return services;
    }

    /// <summary>
    /// Add GameFrameX OpenTelemetry logging configuration / 添加 GameFrameX OpenTelemetry 日志配置
    /// </summary>
    /// <param name="logging">Logging builder / 日志构建器</param>
    /// <param name="setting">Application settings / 应用设置</param>
    /// <returns>Logging builder / 日志构建器</returns>
    /// <remarks>
    /// 为日志系统集成OpenTelemetry功能，支持结构化日志和分布式追踪关联。
    /// 当启用OpenTelemetry时，自动配置Grafana集成，便于日志的可视化和分析。
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when logging or setting is null / 当logging或setting为null时抛出</exception>
    /// <example>
    /// <code>
    /// // 在Program.cs中配置
    /// builder.Logging.AddGameFrameXOpenTelemetryLogging(appSetting);
    /// </code>
    /// </example>
    public static ILoggingBuilder AddGameFrameXOpenTelemetryLogging(this ILoggingBuilder logging, AppSetting setting)
    {
        if (setting.IsOpenTelemetry)
        {
            logging.AddOpenTelemetry(configure => { configure.UseGrafana(); });
        }

        return logging;
    }

    /// <summary>
    /// Create standalone metrics server / 创建独立的指标服务器
    /// </summary>
    /// <param name="setting">Application settings / 应用设置</param>
    /// <param name="servicePrefix">Service name prefix / 服务名前缀</param>
    /// <returns>Metrics server task / 指标服务器任务</returns>
    /// <remarks>
    /// 创建一个独立的Web应用程序专门用于暴露Prometheus指标端点。
    /// 这样可以将指标收集与主应用程序分离，提高系统的可观测性和稳定性。
    /// 服务器会自动检查端口可用性，并在启动后输出访问地址。
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when setting is null / 当setting为null时抛出</exception>
    /// <exception cref="InvalidOperationException">Thrown when metrics port is not available / 当指标端口不可用时抛出</exception>
    /// <example>
    /// <code>
    /// // 创建独立指标服务器
    /// var metricsServer = await OpenTelemetryExtensions.CreateMetricsServerAsync(appSetting);
    /// 
    /// // 带服务前缀
    /// var metricsServer = await OpenTelemetryExtensions.CreateMetricsServerAsync(appSetting, "GameServer");
    /// </code>
    /// </example>
    /// <seealso cref="AddGameFrameXOpenTelemetry"/>
    public static async Task<WebApplication> CreateMetricsServerAsync(AppSetting setting, string servicePrefix = "")
    {
        if (!setting.IsOpenTelemetry || !setting.IsOpenTelemetryMetrics || setting.MetricsPort == 0)
        {
            return null;
        }

        // 检查指标端口是否可用
        if (!NetHelper.PortIsAvailable(setting.MetricsPort))
        {
            LogHelper.Error($"指标端口 [{setting.MetricsPort}] 被占用，无法启动独立指标服务器");
            return null;
        }

        var builder = WebApplication.CreateBuilder();

        var serviceName = string.IsNullOrEmpty(servicePrefix)
                              ? $"{setting.ServerName}-{setting.TagName}-Metrics"
                              : $"{servicePrefix}:{setting.ServerName}-{setting.TagName}-Metrics";

        // 配置Web主机
        builder.WebHost.UseKestrel(options => { options.ListenAnyIP(setting.MetricsPort); });

        // 配置OpenTelemetry指标
        builder.Services.AddOpenTelemetry()
               .ConfigureResource(configure =>
               {
                   configure.AddService(serviceName, "GameFrameX.Metrics")
                            .AddTelemetrySdk();
               })
               .WithMetrics(configure =>
               {
                   configure.AddAspNetCoreInstrumentation();
                   if (EnvironmentHelper.IsDevelopment() && LogOptions.Default.IsConsole)
                   {
                       configure.AddConsoleExporter();
                   }

                   // Metrics provides by ASP.NET Core in .NET 8
                   configure.AddMeter("Microsoft.AspNetCore.Hosting");
                   configure.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

                   // Metrics provided by System.Net libraries
                   configure.AddMeter("System.Net.Http");
                   configure.AddMeter("System.Net.NameResolution");

                   // Metrics provided by .NET Runtime (available in .NET 9+)
                   configure.AddMeter("System.Runtime");

                   // Additional ASP.NET Core metrics for Blazor components (if applicable)
                   // configure.AddMeter("Microsoft.AspNetCore.Components");
                   // configure.AddMeter("Microsoft.AspNetCore.Components.Lifecycle");
                   // configure.AddMeter("Microsoft.AspNetCore.Components.Server.Circuits");
                   configure.AddPrometheusExporter();
               })
               .UseGrafana();

        var app = builder.Build();

        // 配置Prometheus端点
        app.MapPrometheusScrapingEndpoint();

        // 添加健康检查端点
        app.MapGet("/health", () => "OK");

        await app.StartAsync();

        var ipList = NetHelper.GetLocalIpList();
        foreach (var ip in ipList)
        {
            LogHelper.InfoConsole($"独立 Prometheus metrics 端点已启用: http://{ip}:{setting.MetricsPort}/metrics");
            LogHelper.InfoConsole($"独立 Metrics 健康检查端点: http://{ip}:{setting.MetricsPort}/health");
        }

        return app;
    }
}