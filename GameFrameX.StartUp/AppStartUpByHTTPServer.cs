using System.Net;
using System.Reflection;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.HTTP;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Grafana.OpenTelemetry;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// 启动 HTTP 服务器的同步方法
    /// </summary>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    /// <exception cref="ArgumentException">当HTTP URL格式不正确时抛出</exception>
    /// <exception cref="NotImplementedException">当启用HTTPS但未实现时抛出</exception>
    private async Task StartHttpServer(List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        // 验证HTTP URL格式
        if (!Setting.HttpUrl.StartsWith('/'))
        {
            throw new ArgumentException("Http 地址必须以/开头", nameof(Setting.HttpUrl));
        }

        if (!Setting.HttpUrl.EndsWith('/'))
        {
            throw new ArgumentException("Http 地址必须以/结尾", nameof(Setting.HttpUrl));
        }

        LogHelper.InfoConsole("启动 [HTTP] 服务器...");
        if (!Setting.HttpPort.IsRange(1, ushort.MaxValue - 1))
        {
            LogHelper.WarnConsole($"启动 [HTTP] 服务器 端口 [{Setting.HttpPort}] 超出范围 [1-{ushort.MaxValue - 1}]，无法启动HTTP服务,启动被忽略");
            return;
        }

        // 检查端口是否可用
        if (Setting.HttpPort is > 0 and < ushort.MaxValue && NetHelper.PortIsAvailable(Setting.HttpPort))
        {
            var builder = WebApplication.CreateBuilder();

            // 确定是否为开发环境
            var development = Setting.HttpIsDevelopment || EnvironmentHelper.IsDevelopment();

            var openApiInfo = GetOpenApiInfo();

            // 在开发环境下配置Swagger
            if (development)
            {
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(openApiInfo.Version, openApiInfo);
                    options.SchemaFilter<PreservePropertyCasingSchemaFilter>();
                    options.OperationFilter<SwaggerOperationFilter>(baseHandler);
                    options.CustomSchemaIds(type => type.Name);
                });
            }

            // 配置Web主机
            var hostBuilder = builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(Setting.HttpPort);

                if (Setting.HttpsPort > 0 && NetHelper.PortIsAvailable(Setting.HttpsPort))
                {
                    throw new NotImplementedException("HTTPS 未实现,请取消HTTPS端口配置");
                }
            });

            if (Setting.IsOpenTelemetry)
            {
                // 配置OpenTelemetry服务
                hostBuilder.ConfigureServices(services =>
                {
                    var openTelemetryBuilder = services.AddOpenTelemetry()
                                                       .ConfigureResource(configure =>
                                                       {
                                                           configure.AddService("HTTP:" + Setting.ServerName + "-" + Setting.TagName, "GameFrameX.HTTP")
                                                                    .AddTelemetrySdk();
                                                       });
                    if (Setting.IsOpenTelemetryMetrics)
                    {
                        openTelemetryBuilder.WithMetrics(configure =>
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

                    if (Setting.IsOpenTelemetryTracing)
                    {
                        openTelemetryBuilder.WithTracing(configure =>
                        {
                            configure.AddAspNetCoreInstrumentation();
                            configure.AddHttpClientInstrumentation();
                            configure.AddSource("HTTP:GameFrameX." + Setting.ServerName + "." + Setting.TagName);
                            if (EnvironmentHelper.IsDevelopment())
                            {
                                configure.AddConsoleExporter();
                            }
                        });
                    }
                });
            }

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger);
                logging.SetMinimumLevel(minimumLevelLogLevel);
                if (Setting.IsOpenTelemetry)
                {
                    logging.AddOpenTelemetry(configure => { configure.UseGrafana(); });
                }
            });
            var app = builder.Build();

            // 开发环境下的Swagger UI配置
            if (development)
            {
                // 添加 Swagger 中间件
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint($"/swagger/{openApiInfo.Version}/swagger.json", openApiInfo.Title);
                    options.RoutePrefix = "swagger";
                });
                var ipList = NetHelper.GetLocalIpList();
                foreach (var ip in ipList)
                {
                    LogHelper.DebugConsole($"Swagger UI 可通过 http://{ip}:{Setting.HttpPort}/swagger 访问");
                }
            }

            // 配置全局异常处理
            app.UseExceptionHandler(ExceptionHandler);

            // 注册HTTP处理器路由
            foreach (var handler in baseHandler)
            {
                var handlerType = handler.GetType();
                var mappingAttribute = handlerType.GetCustomAttribute<HttpMessageMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                // 注册POST路由
                var apiPath = $"{GlobalSettings.CurrentSetting.HttpUrl}{mappingAttribute.StandardCmd}";
                var route = app.MapPost(apiPath, async (HttpContext context, string text) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });

                // 开发环境下配置API文档
                if (development)
                {
                    // 开发模式，启用 Swagger
                    route.WithOpenApi(operation =>
                    {
                        operation.Summary = "处理 POST 请求";
                        operation.Description = "处理来自游戏客户端的 POST 请求";
                        return operation;
                    });
                }
            }

            await app.StartAsync();
            LogHelper.InfoConsole($"启动 [HTTP] 服务器启动完成 - 端口: {Setting.HttpPort}");
        }
        else
        {
            LogHelper.Error($"启动 [HTTP] 服务器 端口 [{Setting.HttpPort}] 被占用，无法启动HTTP服务");
        }
    }

    /// <summary>
    /// 配置启动,当InnerIP为空时.将使用Any
    /// </summary>
    /// <param name="options"></param>
    protected virtual void ConfigureHttp(ServerOptions options)
    {
        // configApp.AddInMemoryCollection(new Dictionary<string, string>
        // {
        //     { "serverOptions:name", "TestServer" },
        //     { "serverOptions:listeners:0:ip", "Any" },
        //     { "serverOptions:listeners:0:port", "4040" }
        // });

        var listenOptions = new ListenOptions
        {
            Ip = IPAddress.Any.ToString(),
            Port = Setting.HttpPort,
        };
        options.AddListener(listenOptions);
    }

    /// <summary>
    /// 获取或创建 Swagger信息
    /// </summary>
    /// <returns></returns>
    private OpenApiInfo GetOpenApiInfo()
    {
        // 添加 Swagger 服务
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        if (version == null)
        {
            version = new Version(1, 0, 0);
        }

        var openApiInfo = new OpenApiInfo
        {
            Title = "GameFrameX API",
            Version = $"v{version.Major}.{version.Minor}",
            TermsOfService = new Uri("https://gameframex.doc.alianblank.com"),
            Contact = new OpenApiContact() { Url = new Uri("https://gameframex.doc.alianblank.com"), Name = "Blank", Email = "wangfj11@foxmail.com", },
            License = new OpenApiLicense() { Name = "GameFrameX", Url = new Uri("https://github.com/GameFrameX/GameFrameX"), },
            Description = "GameFrameX HTTP API documentation",
        };
        return openApiInfo;
    }

    /// <summary>
    /// 异常处理
    /// </summary>
    /// <param name="errorContext"></param>
    private static void ExceptionHandler(IApplicationBuilder errorContext)
    {
        errorContext.Run(async context =>
        {
            // 获取异常信息
            var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

            // 自定义返回Json信息；
            await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
        });
    }
}