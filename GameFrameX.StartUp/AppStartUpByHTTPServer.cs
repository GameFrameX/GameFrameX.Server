// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利以及其他相关权利
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
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.HTTP;
using GameFrameX.StartUp.Extensions;
using GameFrameX.Utility;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Utility;
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
/// Application startup base class - provides TCP and WebSocket server basic functionality implementation / 程序启动器基类 - 提供TCP和WebSocket服务器的基础功能实现
/// </summary>
/// <remarks>
/// 此部分类专门处理HTTP服务器的启动和配置功能
/// </remarks>
public abstract partial class AppStartUpBase
{
    /// <summary>
    /// Start HTTP server synchronously / 启动 HTTP 服务器的同步方法
    /// </summary>
    /// <param name="baseHandler">HTTP handler list for processing different HTTP requests / HTTP处理器列表,用于处理不同的HTTP请求</param>
    /// <param name="httpFactory">HTTP handler factory that creates corresponding handler instances based on command identifiers / HTTP处理器工厂,根据命令标识符创建对应的处理器实例</param>
    /// <param name="aopHandlerTypes">AOP handler list for executing additional logic before and after HTTP request processing / AOP处理器列表,用于在HTTP请求处理前后执行额外的逻辑</param>
    /// <param name="minimumLevelLogLevel">Minimum level for logging to control log output / 日志记录的最小级别,用于控制日志输出</param>
    /// <returns>A task representing the asynchronous operation / 表示异步操作的任务</returns>
    /// <exception cref="ArgumentException">Thrown when HTTP URL format is incorrect / 当HTTP URL格式不正确时抛出</exception>
    /// <exception cref="NotImplementedException">Thrown when HTTPS is enabled but not implemented / 当启用HTTPS但未实现时抛出</exception>
    /// <remarks>
    /// 此方法负责配置和启动HTTP服务器，包括Swagger文档、健康检查、异常处理等功能
    /// </remarks>
    private async Task StartHttpServer(List<BaseHttpHandler> baseHandler, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes = null, LogLevel minimumLevelLogLevel = LogLevel.Debug)
    {
        // 检查是否启用HTTP服务
        if (!Setting.IsEnableHttp)
        {
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.ServiceDisabled));
            return;
        }

        // 验证HTTP URL格式
        if (!Setting.HttpUrl.StartsWith('/'))
        {
            throw new ArgumentException(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpExceptions.AddressMustStartWithSlash), nameof(Setting.HttpUrl));
        }

        if (!Setting.HttpUrl.EndsWith('/'))
        {
            throw new ArgumentException(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpExceptions.AddressMustEndWithSlash), nameof(Setting.HttpUrl));
        }

        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.StartingServer));
        if (!Setting.HttpPort.IsRange(5000, ushort.MaxValue - 1))
        {
            LogHelper.Warning(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.PortOutOfRange, Setting.HttpPort, 5000, ushort.MaxValue - 1));
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
                    throw new NotImplementedException(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpExceptions.HttpsNotImplemented));
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
                    options.SwaggerEndpoint(string.Format(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.SwaggerEndpointFormat), openApiInfo.Version), openApiInfo.Title);
                    options.RoutePrefix = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.SwaggerRoutePrefix);
                });

                foreach (var ip in ipList)
                {
                    LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.SwaggerUiAccess, ip, Setting.HttpPort));
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
                    LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.PrometheusMetricsEndpointEnabledInline, ip, Setting.HttpPort));
                }
            }
            else if (Setting.IsOpenTelemetry && Setting.IsOpenTelemetryMetrics && Setting.MetricsPort > 0)
            {
                LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.PrometheusMetricsServiceOnStandalonePort, Setting.MetricsPort));
                foreach (var ip in ipList)
                {
                    LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.PrometheusMetricsEndpointEnabled, ip, Setting.MetricsPort));
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
                        operation.Summary = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.HandlePostRequest);
                        operation.Description = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.HandleGameClientPostRequest);
                        return operation;
                    });
                }
            }

            await app.StartAsync();
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.StartupComplete, Setting.HttpPort));
        }
        else
        {
            LogHelper.Error(LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.PortOccupied, Setting.HttpPort));
        }
    }

    /// <summary>
    /// Get or create Swagger information / 获取或创建 Swagger信息
    /// </summary>
    /// <returns>OpenAPI information object containing API documentation details / 包含API文档详细信息的OpenAPI信息对象</returns>
    /// <remarks>
    /// 用于配置Swagger文档的基本信息，包括标题、版本、联系方式等
    /// </remarks>
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
            Title = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.ApiTitle),
            Version = $"v{version.Major}.{version.Minor}",
            TermsOfService = new Uri("https://gameframex.doc.alianblank.com"),
            Contact = new OpenApiContact() { Url = new Uri("https://gameframex.doc.alianblank.com"), Name = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.ApiContactName), Email = "wangfj11@foxmail.com", },
            License = new OpenApiLicense() { Name = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.ApiLicenseName), Url = new Uri("https://github.com/GameFrameX/GameFrameX"), },
            Description = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.HttpServer.ApiDescription),
        };
        return openApiInfo;
    }

    /// <summary>
    /// Exception handler for processing unhandled exceptions in HTTP requests / 异常处理器，用于处理HTTP请求中的未处理异常
    /// </summary>
    /// <param name="errorContext">Application builder for configuring error handling pipeline / 用于配置错误处理管道的应用程序构建器</param>
    /// <remarks>
    /// 提供全局异常处理机制，确保所有未处理的异常都能被适当处理并返回给客户端
    /// </remarks>
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