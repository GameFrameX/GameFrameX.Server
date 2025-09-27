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

namespace GameFrameX.StartUp.Extensions;

/// <summary>
/// OpenTelemetry 配置扩展方法
/// </summary>
public static class OpenTelemetryExtensions
{
    /// <summary>
    /// 添加 GameFrameX OpenTelemetry 配置
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="setting">应用设置</param>
    /// <param name="servicePrefix">服务名前缀</param>
    /// <param name="tracingSourcePrefix">追踪源前缀</param>
    /// <returns>服务集合</returns>
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
    /// 添加 GameFrameX OpenTelemetry 日志配置
    /// </summary>
    /// <param name="logging">日志构建器</param>
    /// <param name="setting">应用设置</param>
    /// <returns>日志构建器</returns>
    public static ILoggingBuilder AddGameFrameXOpenTelemetryLogging(this ILoggingBuilder logging, AppSetting setting)
    {
        if (setting.IsOpenTelemetry)
        {
            logging.AddOpenTelemetry(configure => { configure.UseGrafana(); });
        }

        return logging;
    }

    /// <summary>
    /// 创建独立的指标服务器
    /// </summary>
    /// <param name="setting">应用设置</param>
    /// <param name="servicePrefix">服务名前缀</param>
    /// <returns>指标服务器任务</returns>
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