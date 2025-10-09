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

using System.Reflection;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.HTTP;
using GameFrameX.StartUp.Extensions;
using GameFrameX.Utility;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Utility.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
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
            throw new ArgumentException("The HTTP address must start with /", nameof(Setting.HttpUrl));
        }

        if (!Setting.HttpUrl.EndsWith('/'))
        {
            throw new ArgumentException("The HTTP address must end in /", nameof(Setting.HttpUrl));
        }

        LogHelper.InfoConsole("start the [HTTP] server...");
        if (!Setting.HttpPort.IsRange(5000, ushort.MaxValue - 1))
        {
            LogHelper.WarningConsole($"start the [HTTP] server port [{Setting.HttpPort}] out of range [5000-{ushort.MaxValue - 1}],The HTTP service cannot be started, and the startup is ignored");
            return;
        }

        // 检查端口是否可用
        if (NetHelper.PortIsAvailable(Setting.HttpPort))
        {
            var builder = WebApplication.CreateBuilder();

            // 确定是否为开发环境
            var development = Setting.HttpIsDevelopment || EnvironmentHelper.IsDevelopment();

            var openApiInfo = GetOpenApiInfo();

            // 添加健康检查服务
            builder.Services.AddGameFrameXHealthChecks(Setting);

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
                    throw new NotImplementedException("If HTTPS is not implemented, cancel the HTTPS port configuration");
                }
            });

            if (Setting.IsOpenTelemetry)
            {
                // 配置OpenTelemetry服务
                hostBuilder.ConfigureServices(services => { services.AddGameFrameXOpenTelemetry(Setting, "HTTP", "HTTP"); });
            }

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger);
                logging.SetMinimumLevel(minimumLevelLogLevel);
                logging.AddGameFrameXOpenTelemetryLogging(Setting);
            });
            var app = builder.Build();
            var ipList = NetHelper.GetLocalIpList();
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

                foreach (var ip in ipList)
                {
                    LogHelper.DebugConsole($"Swagger UI can be passed http://{ip}:{Setting.HttpPort}/swagger access");
                }
            }

            // 配置健康检查端点
            app.UseGameFrameXHealthChecks(Setting, ipList);

            // 配置全局异常处理
            app.UseExceptionHandler(ExceptionHandler);

            // 配置OpenTelemetry Prometheus端点（仅在未配置独立指标端口时）
            if (Setting.IsOpenTelemetry && Setting.IsOpenTelemetryMetrics && Setting.MetricsPort == 0)
            {
                app.MapPrometheusScrapingEndpoint();
                foreach (var ip in ipList)
                {
                    LogHelper.InfoConsole($"Prometheus metrics the endpoint is enabled: http://{ip}:{Setting.HttpPort}/metrics");
                }
            }
            else if (Setting.IsOpenTelemetry && Setting.IsOpenTelemetryMetrics && Setting.MetricsPort > 0)
            {
                LogHelper.InfoConsole($"Prometheus metrics The service will be provided on the standalone port {Setting.MetricsPort}");
                foreach (var ip in ipList)
                {
                    LogHelper.InfoConsole($"Prometheus metrics the endpoint is enabled: http://{ip}:{Setting.MetricsPort}/metrics");
                }
            }

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
                var route = app.MapPost(apiPath, async (HttpContext context, string _) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });

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
            LogHelper.InfoConsole($"Start [HTTP] Server Startup Complete - Port: {Setting.HttpPort}");
        }
        else
        {
            LogHelper.Error($"Start [HTTP] server port [{Setting.HttpPort}] is occupied and the HTTP service cannot be started");
        }
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