using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Log;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;

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
    /// 启动
    /// </summary>
    /// <param name="httpPort">HTTP端口,如果没有指定端口，则默认为28080</param>
    /// <param name="httpsPort">HTTPS端口,如果没有指定端口，则不监听HTTPS</param>
    /// <param name="baseHandler">根据命令Id获得处理器</param>
    /// <param name="aopHandlerTypes">Aop处理器列表</param>
    /// <param name="apiRootPath">接口根路径,必须以 / 开头和以 / 结尾,默认为 [/game/api]</param>
    /// <param name="minimumLevelLogLevel">日志记录最小级别</param>
    public static Task Start(int httpPort, int httpsPort, Func<string, BaseHttpHandler> baseHandler, List<IHttpAopHandler> aopHandlerTypes = null, string apiRootPath = GameApiPath, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        LogHelper.InfoConsole("开始启动 HTTP 服务器...");
        baseHandler.CheckNotNull(nameof(baseHandler));

        // 如果没有指定端口，则默认为28080
        if (httpPort <= 0)
        {
            httpPort = 28080;
        }

        // 如果没有指定根路径，则默认为/game/api/
        if (apiRootPath.IsNullOrEmpty())
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
        App.UseExceptionHandler(ExceptionHandler);
        var routePath = $"{ApiRootPath}{{text}}";
        App.MapGet(routePath, context => HttpHandler.HandleRequest(context, baseHandler, aopHandlerTypes));
        App.MapPost(routePath, context => HttpHandler.HandleRequest(context, baseHandler, aopHandlerTypes));
        var task = App.StartAsync();
        LogHelper.InfoConsole($"启动 HTTP 服务器完成...端口号:{httpPort}");
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