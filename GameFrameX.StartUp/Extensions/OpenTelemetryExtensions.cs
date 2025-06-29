using GameFrameX.Foundation.Logger;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

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
                if (EnvironmentHelper.IsDevelopment())
                {
                    configure.AddConsoleExporter();
                }

                // Metrics provides by ASP.NET Core in .NET 8
                configure.AddMeter("Microsoft.AspNetCore.Hosting");
                configure.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                // Metrics provided by System.Net libraries
                configure.AddMeter("System.Net.Http");
                configure.AddMeter("System.Net.NameResolution");
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
                if (EnvironmentHelper.IsDevelopment())
                {
                    configure.AddConsoleExporter();
                }
            });
        }

        builder.UseGrafana();
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
        builder.WebHost.UseKestrel(options =>
        {
            options.ListenAnyIP(setting.MetricsPort);
        });

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
                if (EnvironmentHelper.IsDevelopment())
                {
                    configure.AddConsoleExporter();
                }

                // Metrics provides by ASP.NET Core in .NET 8
                configure.AddMeter("Microsoft.AspNetCore.Hosting");
                configure.AddMeter("Microsoft.AspNetCore.Server.Kestrel");
                // Metrics provided by System.Net libraries
                configure.AddMeter("System.Net.Http");
                configure.AddMeter("System.Net.NameResolution");
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