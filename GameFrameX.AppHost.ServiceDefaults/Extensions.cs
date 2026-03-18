using GameFrameX.Foundation.Utility;
using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace GameFrameX.AppHost.ServiceDefaults;

/// <summary>
/// 新增了常见的Aspire服务：服务发现、弹性、健康检查和OpenTelemetry。
/// 该项目应被解决方案中的每个服务项目引用。
/// 想了解更多关于使用该项目的信息，请参见 https://aka.ms/dotnet/aspire/service-defaults
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 健康检查端点路径
    /// </summary>
    private const string HealthEndpointPath = "/health";

    /// <summary>
    /// 存活性检查端点路径
    /// </summary>
    private const string AlivenessEndpointPath = "/alive";

    /// <summary>
    /// 为应用程序构建器添加默认服务，包括服务发现、弹性、健康检查和OpenTelemetry
    /// </summary>
    /// <typeparam name="TBuilder">主机应用程序构建器类型</typeparam>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static TBuilder AddServiceDefaults<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // 配置OpenTelemetry以收集遥测数据
        builder.ConfigureOpenTelemetry();

        // 添加默认健康检查
        builder.AddDefaultHealthChecks();

        // 添加服务发现功能
        builder.Services.AddServiceDiscovery();

        // 配置HTTP客户端默认值
        builder.Services.ConfigureHttpClientDefaults(http =>
        {
            // 默认启用弹性处理
            http.AddStandardResilienceHandler();

            // 默认启用服务发现
            http.AddServiceDiscovery();
        });

        // 配置Prometheus端点
        // builder.Services.MapPrometheusScrapingEndpoint();
        // 取消以下注释以限制服务发现允许使用的协议方案
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }


    /// <summary>
    /// 为应用程序构建器添加默认服务，包括服务发现、弹性、健康检查和OpenTelemetry
    /// </summary>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static IServiceCollection AddServiceDefaults(this IServiceCollection builder)
    {
        // 配置OpenTelemetry以收集遥测数据
        builder.ConfigureOpenTelemetry();

        // 添加默认健康检查
        builder.AddDefaultHealthChecks();

        // 添加服务发现功能
        builder.AddServiceDiscovery();

        // 配置HTTP客户端默认值
        builder.ConfigureHttpClientDefaults(http =>
        {
            // 默认启用弹性处理
            http.AddStandardResilienceHandler();

            // 默认启用服务发现
            http.AddServiceDiscovery();
        });

        // 配置Prometheus端点
        // builder.Services.MapPrometheusScrapingEndpoint();
        // 取消以下注释以限制服务发现允许使用的协议方案
        // builder.Services.Configure<ServiceDiscoveryOptions>(options =>
        // {
        //     options.AllowedSchemes = ["https"];
        // });

        return builder;
    }

    /// <summary>
    /// 配置OpenTelemetry服务以收集指标和追踪信息
    /// </summary>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static ILoggingBuilder ConfigureOpenTelemetryLogger(this ILoggingBuilder builder)
    {
        // 为日志添加OpenTelemetry支持
        builder.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.UseGrafana();
        });

        return builder;
    }

    /// <summary>
    /// 配置OpenTelemetry服务以收集指标和追踪信息
    /// </summary>
    /// <typeparam name="TBuilder">主机应用程序构建器类型</typeparam>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static TBuilder ConfigureOpenTelemetry<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // 为日志添加OpenTelemetry支持
        builder.Logging.ConfigureOpenTelemetryLogger();

        // 添加并配置OpenTelemetry服务
        builder.Services.ConfigureOpenTelemetry();

        return builder;
    }

    /// <summary>
    /// 配置OpenTelemetry服务以收集指标和追踪信息
    /// </summary>
    /// <param name="serviceCollection">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static IServiceCollection ConfigureOpenTelemetry(this IServiceCollection serviceCollection)
    {
        // 为日志添加OpenTelemetry支持
        var serviceName = Environment.GetEnvironmentVariable("ServerType") ?? AppDomain.CurrentDomain.FriendlyName;
        // 添加并配置OpenTelemetry服务
        var openTelemetryBuilder = serviceCollection.AddOpenTelemetry()
                                                    // 配置资源
                                                    .ConfigureResource(configure =>
                                                    {
                                                        if (!string.IsNullOrWhiteSpace(serviceName))
                                                        {
                                                            configure.AddService(serviceName).AddTelemetrySdk();
                                                        }

                                                        // 添加操作系统检测器以收集操作系统相关遥测数据
                                                        configure.AddOperatingSystemDetector();
                                                        // 添加进程运行时检测器以收集进程运行时信息
                                                        configure.AddProcessRuntimeDetector();
                                                        // 添加主机检测器以收集主机环境信息
                                                        configure.AddHostDetector();
                                                        // 添加进程检测器以收集当前进程信息
                                                        configure.AddProcessDetector();
                                                    })
                                                    // 配置指标
                                                    .WithMetrics(metrics =>
                                                    {
                                                        // 添加ASP.NET Core、HTTP客户端和运行时指标检测
                                                        metrics.AddAspNetCoreInstrumentation()
                                                               .AddHttpClientInstrumentation()
                                                               .AddRuntimeInstrumentation();
                                                        // Metrics provides by ASP.NET Core in .NET 8
                                                        metrics.AddMeter("Microsoft.AspNetCore.Hosting");
                                                        metrics.AddMeter("Microsoft.AspNetCore.Server.Kestrel");

                                                        // Metrics provided by System.Net libraries
                                                        metrics.AddMeter("System.Net.Http");
                                                        metrics.AddMeter("System.Net.NameResolution");
                                                    })
                                                    // 配置追踪
                                                    .WithTracing(tracing =>
                                                    {
                                                        // 添加追踪源并配置ASP.NET Core追踪
                                                        tracing.AddSource(serviceName)
                                                               .AddAspNetCoreInstrumentation(aspNetCoreTraceInstrumentationOptions =>
                                                                                                 // 排除健康检查请求的追踪
                                                                                             {
                                                                                                 aspNetCoreTraceInstrumentationOptions.Filter = context => { return !context.Request.Path.StartsWithSegments(HealthEndpointPath) && !context.Request.Path.StartsWithSegments(AlivenessEndpointPath); };
                                                                                             })
                                                               // 取消以下注释以启用gRPC检测（需要OpenTelemetry.Instrumentation.GrpcNetClient包）
                                                               //.AddGrpcClientInstrumentation()
                                                               .AddHttpClientInstrumentation();
                                                        // 开发环境中增加控制台输出
                                                        if (EnvironmentHelper.IsDevelopment())
                                                        {
                                                            tracing.AddConsoleExporter();
                                                        }
                                                    }).UseGrafana();

        // 添加OpenTelemetry导出器
        // serviceCollection.AddOpenTelemetryExporters(openTelemetryBuilder);

        return serviceCollection;
    }

    /// <summary>
    /// 根据配置添加OpenTelemetry导出器
    /// </summary>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <param name="openTelemetryBuilder"></param>
    /// <returns>更新后的构建器实例</returns>
    private static IServiceCollection AddOpenTelemetryExporters(this IServiceCollection builder, IOpenTelemetryBuilder openTelemetryBuilder)
    {
        if (openTelemetryBuilder != null)
        {
            var otelExporterOtlpEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");
            var otelExporterOtlpProtocol = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL");
            // 检查是否配置了OTLP导出端点
            var useOtlpExporter = !string.IsNullOrWhiteSpace(otelExporterOtlpEndpoint);
            var otlpProtocol = Convert.ToInt32(otelExporterOtlpProtocol);

            if (useOtlpExporter)
            {
                // 使用OTLP导出器
                openTelemetryBuilder.UseOtlpExporter(otlpProtocol > 0 ? OtlpExportProtocol.HttpProtobuf : OtlpExportProtocol.Grpc, new Uri(otelExporterOtlpEndpoint));
            }
        }


        // builder.AddPrometheusExporter();
        // 取消以下注释以启用Azure Monitor导出器（需要Azure.Monitor.OpenTelemetry.AspNetCore包）
        //if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
        //{
        //    builder.Services.AddOpenTelemetry()
        //       .UseAzureMonitor();
        //}

        return builder;
    }

    /// <summary>
    /// 为应用程序构建器添加默认健康检查
    /// </summary>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static IServiceCollection AddDefaultHealthChecks(this IServiceCollection builder)
    {
        // 添加健康检查服务并配置默认存活性检查
        builder.AddHealthChecks()
               // 添加一个默认的存活性检查以确保应用程序响应正常
               .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live", });

        return builder;
    }

    /// <summary>
    /// 为应用程序构建器添加默认健康检查
    /// </summary>
    /// <typeparam name="TBuilder">主机应用程序构建器类型</typeparam>
    /// <param name="builder">主机应用程序构建器实例</param>
    /// <returns>更新后的构建器实例</returns>
    public static TBuilder AddDefaultHealthChecks<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // 添加健康检查服务并配置默认存活性检查
        builder.Services.AddHealthChecks()
               // 添加一个默认的存活性检查以确保应用程序响应正常
               .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "live", });

        return builder;
    }

    /// <summary>
    /// 为Web应用程序映射默认端点（健康检查等）
    /// </summary>
    /// <param name="app">Web应用程序实例</param>
    /// <returns>更新后的Web应用程序实例</returns>
    public static WebApplication MapDefaultEndpoints(this WebApplication app)
    {
        // 在非开发环境中向应用程序添加健康检查端点存在安全影响。
        // 在非开发环境中启用这些端点之前，请参阅 https://aka.ms/dotnet/aspire/healthchecks 了解详情。
        if (app.Environment.IsDevelopment())
        {
            // 应用程序启动后，所有健康检查必须通过才能认为准备好接受流量
            app.MapHealthChecks(HealthEndpointPath);

            // 只有标记为"live"的健康检查必须通过才能认为应用程序处于活动状态
            app.MapHealthChecks(AlivenessEndpointPath, new HealthCheckOptions
            {
                // 仅对带有"live"标签的健康检查执行检查
                Predicate = r => r.Tags.Contains("live"),
            });
        }

        return app;
    }
}