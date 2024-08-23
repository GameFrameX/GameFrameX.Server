using GameFrameX.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog;
using GameFrameX.Log;

namespace GameFrameX.NetWork.HTTP
{
    /// <summary>
    /// HTTP服务器
    /// </summary>
    public static class HttpServer
    {
        private static WebApplication App { get; set; }

        /// <summary>
        /// 游戏API根地址
        /// </summary>
        private const string GameApiPath = "/game/api/";

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
        /// <param name="apiRootPath">接口根路径,必须以 / 开头和以 / 结尾,默认为 [/game/api]</param>
        /// <param name="minimumLevelLogLevel">日志记录最小级别</param>
        public static Task Start(int httpPort, int httpsPort, Func<string, BaseHttpHandler> baseHandler, string apiRootPath = GameApiPath, LogLevel minimumLevelLogLevel = LogLevel.Debug)
        {
            LogHelper.Info("开始启动 HTTP 服务器...");
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
                       if (httpPort > 0)
                       {
                           options.ListenAnyIP(httpPort);
                       }

                       // HTTPS
                       if (httpsPort > 0)
                       {
                           options.ListenAnyIP(httpsPort, listenOptions => { listenOptions.UseHttps(); });
                       }
                   })
                   .ConfigureLogging(logging => { logging.SetMinimumLevel(minimumLevelLogLevel); })
                   .UseSerilog();

            App = builder.Build();
            App.UseExceptionHandler((errorContext) =>
            {
                errorContext.Run(async (context) =>
                {
                    // 获取异常信息
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    // 自定义返回Json信息；
                    await context.Response.WriteAsync(exceptionHandlerPathFeature.Error.Message);
                });
            });
            string routePath = $"{ApiRootPath}{{text}}";
            App.MapGet(routePath, context => HttpHandler.HandleRequest(context, baseHandler));
            App.MapPost(routePath, context => HttpHandler.HandleRequest(context, baseHandler));
            var task = App.StartAsync();
            LogHelper.Info("启动 HTTP 服务器完成...");
            return task;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public static Task Stop()
        {
            if (App != null)
            {
                LogHelper.Info("停止http服务...");
                var task = App.StopAsync();
                App = null;
                return task;
            }

            return Task.CompletedTask;
        }
    }
}