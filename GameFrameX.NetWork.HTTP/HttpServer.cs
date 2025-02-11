using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using GameFrameX.Foundation.Http.Normalization;
using Microsoft.Extensions.Hosting;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP服务器
/// </summary>
public static class HttpServer
{
    /// <summary>
    /// 游戏API根地址
    /// </summary>
    private const string GameApiPath = "/game/api/";

    private static WebApplication App { get; set; }

    /// <summary>
    /// API根地址
    /// </summary>
    public static string ApiRootPath { get; private set; }

    /// <summary>
    /// 启动HTTP服务器
    /// </summary>
    /// <param name="httpPort">HTTP端口,如果没有指定端口，则默认为28080</param>
    /// <param name="httpsPort">HTTPS端口,如果没有指定端口，则不监听HTTPS</param>
    /// <param name="baseHandler">HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="apiRootPath">API接口根路径,必须以/开头和以/结尾,默认为[/game/api/]</param>
    /// <param name="minimumLevelLogLevel">日志记录的最小级别,用于控制日志输出</param>
    /// <param name="openApiInfo">Swagger API文档信息配置,包含API标题、版本、描述等</param>
    /// <returns>表示异步操作的Task</returns>
    public static Task Start(int httpPort, int httpsPort, List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, string apiRootPath = GameApiPath, LogLevel minimumLevelLogLevel = LogLevel.Debug, OpenApiInfo openApiInfo = null)
    {
        LogHelper.InfoConsole("开始启动 HTTP 服务器...");
        baseHandler.CheckNotNull(nameof(baseHandler));

        // 如果没有指定端口，则默认为28080
        if (httpPort <= 0)
        {
            httpPort = 28080;
        }

        // 如果没有指定根路径，则默认为/game/api/
        if (apiRootPath.IsNullOrEmptyOrWhiteSpace())
        {
            apiRootPath = GameApiPath;
        }

        // 根路径必须以/开头和以/结尾
        if (!apiRootPath.StartsWith("/"))
        {
            apiRootPath = "/" + apiRootPath;
        }

        if (!apiRootPath.EndsWith("/"))
        {
            apiRootPath += "/";
        }

        ApiRootPath = apiRootPath;
        var builder = WebApplication.CreateBuilder();

        bool isDevelopment = builder.Environment.IsDevelopment();
        if (isDevelopment)
        {
            // 添加 Swagger 服务
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                openApiInfo ??= new OpenApiInfo
                {
                    Title = "GameFrameX API",
                    Version = "v1",
                    Description = "GameFrameX HTTP API documentation",
                };

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
            // HTTP
            if (httpPort > 0 && Net.PortIsAvailable(httpPort))
            {
                options.ListenAnyIP(httpPort);
            }

            // HTTPS
            if (httpsPort > 0 && Net.PortIsAvailable(httpPort))
            {
                options.ListenAnyIP(httpsPort, listenOptions => { listenOptions.UseHttps(); });
            }
        }).ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Serilog.Log.Logger);
            logging.SetMinimumLevel(minimumLevelLogLevel);
        });
        App = builder.Build();
        if (isDevelopment)
        {
            // 添加 Swagger 中间件
            App.UseSwagger();
            App.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "GameFrameX API V1");
                options.RoutePrefix = "swagger";
            });
        }

        App.UseExceptionHandler(ExceptionHandler);

        foreach (var handler in baseHandler)
        {
            var handlerType = handler.GetType();
            var mappingAttribute = handlerType.GetCustomAttribute<HttpMessageMappingAttribute>();
            if (mappingAttribute == null)
            {
                continue;
            }

            var route = App.MapPost($"{ApiRootPath}{mappingAttribute.StandardCmd}", async (HttpContext context, string text) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
            if (isDevelopment)
            {
                route.WithOpenApi(operation =>
                {
                    operation.Summary = "处理 POST 请求";
                    operation.Description = "处理来自游戏客户端的 POST 请求";
                    return operation;
                });
            }
        }


        var task = App.StartAsync();
        LogHelper.InfoConsole($"启动 HTTP 服务器完成...端口号:{httpPort}");
        if (isDevelopment)
        {
            LogHelper.InfoConsole($"Swagger UI 可通过 http://localhost:{httpPort}/swagger 访问");
        }

        return task;
    }

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

    /// <summary>
    /// 停止
    /// </summary>
    public static Task Stop()
    {
        LogHelper.InfoConsole("停止http服务...");
        if (App == null)
        {
            return Task.CompletedTask;
        }

        var task = App.StopAsync();
        App = null;
        return task;
    }
}