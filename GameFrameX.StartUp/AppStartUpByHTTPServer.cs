using System.Net;
using System.Reflection;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.StartUp.Options;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using CloseReason = GameFrameX.SuperSocket.WebSocket.CloseReason;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// 启动 HTTP 服务器
    /// </summary>
    /// <param name="hostBuilder"></param>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    private async Task StartHttpServerAsync(MultipleServerHostBuilder hostBuilder, List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        var apiRootPath = Setting.HttpUrl;
        // 根路径必须以/开头和以/结尾
        if (!Setting.HttpUrl.StartsWith('/'))
        {
            apiRootPath = "/" + Setting.HttpUrl;
        }

        if (!Setting.HttpUrl.EndsWith('/'))
        {
            apiRootPath += "/";
        }

        GlobalSettings.ApiRootPath = apiRootPath;

        LogHelper.InfoConsole("启动 [HTTP] 服务器...");
        if (Setting.HttpPort is > 0 and < ushort.MaxValue && NetHelper.PortIsAvailable(Setting.HttpPort))
        {
            var builder = WebApplication.CreateBuilder();

            var development = Setting.HttpIsDevelopment || builder.Environment.IsDevelopment();

            var openApiInfo = GetOpenApiInfo();

            // hostBuilder.AddServer(hostBuilder =>
            // {
            //     hostBuilder.ConfigureServices((context, serviceCollection) =>
            //     {
            //         serviceCollection.Configure<ServerOptions>(options =>
            //         {
            //             var listenOptions = new ListenOptions()
            //             {
            //                 Ip = "Any",
            //                 Port = Setting.WsPort,
            //             };
            //             options.AddListener(listenOptions);
            //         });
            //     });
            // });

            if (development)
            {
                // 添加 Swagger 服务
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(openApiInfo.Version, openApiInfo);

                    // 使用自定义的 SchemaFilter 来保持属性名称大小写
                    options.SchemaFilter<PreservePropertyCasingSchemaFilter>();

                    // 添加自定义操作过滤器来处理动态路由
                    options.OperationFilter<SwaggerOperationFilter>(baseHandler);
                    // 使用完整的类型名称
                    options.CustomSchemaIds(type => type.Name);
                });
            }


            builder.WebHost.UseKestrel(options =>
            {
                options.ListenAnyIP(Setting.HttpPort);

                // HTTPS
                if (Setting.HttpsPort > 0 && NetHelper.PortIsAvailable(Setting.HttpsPort))
                {
                    throw new NotImplementedException("HTTPS 未实现,请取消HTTPS端口配置");

                    // options.ListenAnyIP(Setting.HttpsPort, listenOptions => { listenOptions.UseHttps(); });
                }
            }).ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger);
                logging.SetMinimumLevel(minimumLevelLogLevel);
            });

            var app = builder.Build();
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

            app.UseExceptionHandler(ExceptionHandler);

            // 每个http处理器，注册到路由中
            foreach (var handler in baseHandler)
            {
                var handlerType = handler.GetType();
                var mappingAttribute = handlerType.GetCustomAttribute<HttpMessageMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                // 只支持POST请求
                var route = app.MapPost($"{apiRootPath}{mappingAttribute.StandardCmd}", async (HttpContext context, string text) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
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
    /// 启动 HTTP 服务器
    /// </summary>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    private async Task StartHttpServer(List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        var apiRootPath = Setting.HttpUrl;
        // 根路径必须以/开头和以/结尾
        if (!Setting.HttpUrl.StartsWith('/'))
        {
            apiRootPath = "/" + Setting.HttpUrl;
        }

        if (!Setting.HttpUrl.EndsWith('/'))
        {
            apiRootPath += "/";
        }

        GlobalSettings.ApiRootPath = apiRootPath;

        LogHelper.InfoConsole("启动 [HTTP] 服务器...");
        if (Setting.HttpPort is > 0 and < ushort.MaxValue && NetHelper.PortIsAvailable(Setting.HttpPort))
        {
            var builder = WebApplication.CreateBuilder();

            var development = Setting.HttpIsDevelopment || EnvironmentHelper.IsDevelopment();

            var openApiInfo = GetOpenApiInfo();
            if (development)
            {
                // 添加 Swagger 服务
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(openApiInfo.Version, openApiInfo);

                    // 使用自定义的 SchemaFilter 来保持属性名称大小写
                    options.SchemaFilter<PreservePropertyCasingSchemaFilter>();

                    // 添加自定义操作过滤器来处理动态路由
                    options.OperationFilter<SwaggerOperationFilter>(baseHandler);
                    // 使用完整的类型名称
                    options.CustomSchemaIds(type => type.Name);
                });
            }


            builder.WebHost.UseKestrel(options =>
                   {
                       options.ListenAnyIP(Setting.HttpPort);

                       // HTTPS
                       if (Setting.HttpsPort > 0 && NetHelper.PortIsAvailable(Setting.HttpsPort))
                       {
                           throw new NotImplementedException("HTTPS 未实现,请取消HTTPS端口配置");

                           // options.ListenAnyIP(Setting.HttpsPort, listenOptions => { listenOptions.UseHttps(); });
                       }
                   }).ConfigureLogging(logging =>
                   {
                       logging.ClearProviders();
                       logging.AddSerilog(Log.Logger);
                       logging.SetMinimumLevel(minimumLevelLogLevel);
                   })
                   .ConfigureServices(services =>
                   {
                       services.AddOpenTelemetry()
                               .ConfigureResource(configure => { configure.AddService("HTTP:" + Setting.ServerName + "-" + Setting.TagName, "GameFrameX.HTTP").AddTelemetrySdk(); })
                               .WithTracing(configure =>
                               {
                                   configure.AddAspNetCoreInstrumentation();
                                   configure.AddConsoleExporter();
                               });
                   });
            var app = builder.Build();
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

            app.UseExceptionHandler(ExceptionHandler);

            // 每个http处理器，注册到路由中
            foreach (var handler in baseHandler)
            {
                var handlerType = handler.GetType();
                var mappingAttribute = handlerType.GetCustomAttribute<HttpMessageMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                // 只支持POST请求
                var route = app.MapPost($"{apiRootPath}{mappingAttribute.StandardCmd}", async (HttpContext context, string text) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
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