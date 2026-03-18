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
using GameFrameX.AppHost.ServiceDefaults;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.NetWork.HTTP;
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
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.ServiceDisabled));
            return;
        }

        // 验证HTTP URL格式
        if (!Setting.HttpUrl.StartsWith('/'))
        {
            throw new ArgumentException(LocalizationService.GetString(Localization.Keys.StartUp.HttpExceptions.AddressMustStartWithSlash), nameof(Setting.HttpUrl));
        }

        if (!Setting.HttpUrl.EndsWith('/'))
        {
            throw new ArgumentException(LocalizationService.GetString(Localization.Keys.StartUp.HttpExceptions.AddressMustEndWithSlash), nameof(Setting.HttpUrl));
        }

        LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.StartingServer));
        if (!Setting.HttpPort.IsRange(5000, ushort.MaxValue - 1))
        {
            LogHelper.Warning(LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.PortOutOfRange, Setting.HttpPort, 5000, ushort.MaxValue - 1));
            return;
        }

        // 检查端口是否可用
        if (NetHelper.PortIsAvailable(Setting.HttpPort))
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
                    throw new NotImplementedException(LocalizationService.GetString(Localization.Keys.StartUp.HttpExceptions.HttpsNotImplemented));
                }
            });

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Log.Logger);
                logging.SetMinimumLevel(minimumLevelLogLevel);
            });
            builder.AddServiceDefaults();
            var app = builder.Build();
            var ipList = NetHelper.GetLocalIpList();
            // 开发环境下的Swagger UI配置
            if (development)
            {
                // 添加 Swagger 中间件
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    var swaggerEndpoint = LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.SwaggerEndpointFormat, openApiInfo.Version);
                    options.SwaggerEndpoint(swaggerEndpoint, openApiInfo.Title);
                    options.RoutePrefix = LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.SwaggerRoutePrefix);
                });

                foreach (var ip in ipList)
                {
                    LogHelper.Debug(LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.SwaggerUiAccess, ip, Setting.HttpPort));
                }
            }

            // 配置全局异常处理
            app.UseExceptionHandler(ExceptionHandler);

            // 配置OpenTelemetry Prometheus端点（仅在未配置独立指标端口时）
            if (Setting.IsOpenTelemetry && Setting.IsOpenTelemetryMetrics && Setting.MetricsPort == 0)
            {
                app.MapPrometheusScrapingEndpoint();
                foreach (var ip in ipList)
                {
                    LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.PrometheusMetricsEndpointEnabledInline, ip, Setting.HttpPort));
                }
            }
            else if (Setting.IsOpenTelemetry && Setting.IsOpenTelemetryMetrics && Setting.MetricsPort > 0)
            {
                LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.PrometheusMetricsServiceOnStandalonePort, Setting.MetricsPort));
                foreach (var ip in ipList)
                {
                    LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.PrometheusMetricsEndpointEnabled, ip, Setting.MetricsPort));
                }
            }

            // 注册HTTP处理器路由（同时支持驼峰和下划线格式）
            foreach (var handler in baseHandler)
            {
                var handlerType = handler.GetType();
                var mappingAttribute = handlerType.GetCustomAttribute<HttpMessageMappingAttribute>();
                if (mappingAttribute == null)
                {
                    continue;
                }

                // 注册驼峰格式路由
                RegisterHandlerRoute(app, mappingAttribute, mappingAttribute.OriginalCmd, httpFactory, aopHandlerTypes, development);

                // 注册下划线格式路由（如果与驼峰格式不同）
                if (mappingAttribute.OriginalCmd != mappingAttribute.StandardCmd)
                {
                    RegisterHandlerRoute(app, mappingAttribute, mappingAttribute.StandardCmd, httpFactory, aopHandlerTypes, development);
                }
            }

            await app.StartAsync();
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.StartupComplete, Setting.HttpPort));
        }
        else
        {
            LogHelper.Error(LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.PortOccupied, Setting.HttpPort));
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
            Title = LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.ApiTitle),
            Version = $"v{version.Major}.{version.Minor}",
            TermsOfService = new Uri("https://gameframex.doc.alianblank.com"),
            Contact = new OpenApiContact { Url = new Uri("https://gameframex.doc.alianblank.com"), Name = LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.ApiContactName), Email = "wangfj11@foxmail.com", },
            License = new OpenApiLicense { Name = LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.ApiLicenseName), Url = new Uri("https://github.com/GameFrameX/GameFrameX"), },
            Description = LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.ApiDescription),
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

            // 记录详细错误日志
            LogHelper.Error($"HTTP request error: {exceptionHandlerPathFeature?.Error}");

            // 返回通用错误消息，避免泄露敏感信息
            await context.Response.WriteAsync("An error occurred while processing your request.");
        });
    }

    /// <summary>
    /// 获取 HTTP 方法的摘要描述
    /// </summary>
    /// <param name="httpMethod">HTTP 方法类型</param>
    /// <returns>摘要描述</returns>
    private static string GetHttpMethodSummary(HttpMethodType httpMethod)
    {
        switch (httpMethod)
        {
            case HttpMethodType.GET:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandleGetRequest);
            case HttpMethodType.PUT:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandlePutRequest);
            case HttpMethodType.DELETE:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandleDeleteRequest);
            default:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandlePostRequest);
        }
    }

    /// <summary>
    /// 获取 HTTP 方法的详细描述
    /// </summary>
    /// <param name="httpMethod">HTTP 方法类型</param>
    /// <returns>详细描述</returns>
    private static string GetHttpMethodDescription(HttpMethodType httpMethod)
    {
        switch (httpMethod)
        {
            case HttpMethodType.GET:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandleGameClientGetRequest);
            case HttpMethodType.PUT:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandleGameClientPutRequest);
            case HttpMethodType.DELETE:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandleGameClientDeleteRequest);
            default:
                return LocalizationService.GetString(Localization.Keys.StartUp.HttpServer.HandleGameClientPostRequest);
        }
    }

    /// <summary>
    /// 注册单个 HTTP 处理器路由
    /// </summary>
    /// <param name="app">Web 应用程序实例</param>
    /// <param name="mappingAttribute">HTTP 消息映射特性</param>
    /// <param name="cmd">命令标识符（用于构建 API 路径）</param>
    /// <param name="httpFactory">HTTP 处理器工厂</param>
    /// <param name="aopHandlerTypes">AOP 处理器列表</param>
    /// <param name="development">是否为开发环境</param>
    private static void RegisterHandlerRoute(WebApplication app, HttpMessageMappingAttribute mappingAttribute, string cmd, Func<string, BaseHttpHandler> httpFactory, List<IHttpAopHandler> aopHandlerTypes, bool development)
    {
        // 注册路由 - 根据 HttpMethod 选择不同的 Map 方法
        var apiPath = $"{GlobalSettings.CurrentSetting.HttpUrl}{cmd}";
        var httpMethod = mappingAttribute.HttpMethod;

        IEndpointConventionBuilder route;
        switch (httpMethod)
        {
            case HttpMethodType.GET:
                route = app.MapGet(apiPath, async (HttpContext context) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
                break;
            case HttpMethodType.PUT:
                route = app.MapPut(apiPath, async (HttpContext context, string _) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
                break;
            case HttpMethodType.DELETE:
                route = app.MapDelete(apiPath, async (HttpContext context, string _) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
                break;
            default:
                route = app.MapPost(apiPath, async (HttpContext context, string _) => { await HttpHandler.HandleRequest(context, httpFactory, aopHandlerTypes); });
                break;
        }

        // 开发环境下配置API文档
        if (development)
        {
            route.WithOpenApi(operation =>
            {
                operation.Summary = GetHttpMethodSummary(httpMethod);
                operation.Description = GetHttpMethodDescription(httpMethod);
                return operation;
            });
        }
    }
}