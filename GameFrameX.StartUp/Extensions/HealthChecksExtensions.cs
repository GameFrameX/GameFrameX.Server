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


using GameFrameX.Utility.Setting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using Mapster;
using System.Text.Json;

namespace GameFrameX.StartUp.Extensions;

/// <summary>
/// Health check configuration extension methods / 健康检查配置扩展方法
/// </summary>
/// <remarks>
/// 提供用于配置和使用健康检查功能的扩展方法，支持多种健康检查端点和自定义响应格式。
/// 包含详细的JSON格式健康检查报告和简单的兼容性端点，支持OpenTelemetry集成。
/// </remarks>
public static class HealthChecksExtensions
{
    /// <summary>
    /// JSON content type constant / JSON内容类型常量
    /// </summary>
    /// <value>Returns the JSON content type with UTF-8 encoding / 返回带UTF-8编码的JSON内容类型</value>
    private const string JsonContentType = "application/json; charset=utf-8";

    /// <summary>
    /// Default health check endpoint path / 默认健康检查端点路径
    /// </summary>
    /// <value>Returns the default health check endpoint path "/health" / 返回默认的健康检查端点路径 "/health"</value>
    public const string DefaultHealthCheckPath = "/health";

    /// <summary>
    /// Simple health check endpoint path (compatibility endpoint) / 简单健康检查端点路径（兼容性端点）
    /// </summary>
    /// <value>Returns the simple health check endpoint path / 返回简单健康检查端点路径</value>
    public const string SimpleHealthCheckPath = $"{DefaultHealthCheckPath}/simple";

    /// <summary>
    /// OpenTelemetry health check endpoint path / OpenTelemetry 健康检查端点路径
    /// </summary>
    /// <value>Returns the OpenTelemetry health check endpoint path / 返回OpenTelemetry健康检查端点路径</value>
    public const string OpenTelemetryHealthCheckPath = $"{DefaultHealthCheckPath}/opentelemetry";

    /// <summary>
    /// Add GameFrameX health check services (including multiple check items) / 添加 GameFrameX 健康检查服务（包含多种检查项目）
    /// </summary>
    /// <param name="services">Service collection / 服务集合</param>
    /// <param name="setting">Application settings / 应用设置</param>
    /// <returns>Service collection / 服务集合</returns>
    /// <remarks>
    /// 注册多个健康检查项目，包括基础应用程序检查、简单兼容性检查和OpenTelemetry检查。
    /// 根据配置自动启用或禁用相应的检查项目，确保系统健康状态的全面监控。
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when services or setting is null / 当services或setting为null时抛出</exception>
    public static IServiceCollection AddGameFrameXHealthChecks(this IServiceCollection services, AppSetting setting)
    {
        var healthChecksBuilder = services.AddHealthChecks();
        // 基础应用程序健康检查
        healthChecksBuilder.AddCheck(DefaultHealthCheckPath, () => HealthCheckResult.Healthy("the application is working fine"));
        // 简单健康检查（兼容性端点）
        healthChecksBuilder.AddCheck(SimpleHealthCheckPath, () => HealthCheckResult.Healthy("the application is working fine"));

        // OpenTelemetry相关检查
        if (setting.IsOpenTelemetry)
        {
            healthChecksBuilder.AddCheck(OpenTelemetryHealthCheckPath, () => HealthCheckResult.Healthy("OpenTelemetry the configuration is normal"));
        }

        return services;
    }

    /// <summary>
    /// Use GameFrameX health check middleware / 使用 GameFrameX 健康检查中间件
    /// </summary>
    /// <param name="app">Application builder for configuring HTTP request pipeline / 应用程序构建器，用于配置HTTP请求管道</param>
    /// <param name="setting">Application settings object containing application configuration / 应用设置对象，包含应用程序的配置信息</param>
    /// <param name="ipList">List of IP addresses for health check endpoints / 健康检查端点的IP地址列表</param>
    /// <returns>Returns the application builder configured with health check endpoints / 返回配置了健康检查端点的应用程序构建器</returns>
    /// <remarks>
    /// 此方法会配置以下健康检查端点：
    /// <list type="bullet">
    /// <item><description><c>/health</c> - 详细的JSON格式健康检查报告</description></item>
    /// <item><description><c>/health/simple</c> - 简单的"OK"响应（兼容性端点）</description></item>
    /// <item><description><c>/health/opentelemetry</c> - OpenTelemetry相关检查（如果启用）</description></item>
    /// </list>
    /// 健康检查响应包含状态、检查项详情、持续时间、服务器信息和时间戳。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 使用默认路径
    /// app.UseGameFrameXHealthChecks(appSetting, ipList);
    /// 
    /// // 配置健康检查端点
    /// var ipList = new List&lt;string&gt; { "127.0.0.1", "192.168.1.100" };
    /// app.UseGameFrameXHealthChecks(appSetting, ipList);
    /// </code>
    /// </example>
    /// <exception cref="ArgumentNullException">Thrown when app, setting, or ipList is null / 当app、setting或ipList为null时抛出</exception>
    /// <seealso cref="AddGameFrameXHealthChecks"/>
    /// <seealso cref="AppSetting"/>
    public static IApplicationBuilder UseGameFrameXHealthChecks(this IApplicationBuilder app, AppSetting setting, List<string> ipList)
    {
        var defaultHealthCheckOptions = new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = JsonContentType;

                var response = new HealthCheckResponse
                {
                    Timestamp = DateTime.UtcNow,
                    Status = report.Status.ToString(),
                    Checks = report.Entries.Select(entry => new HealthCheckResponse.HealthCheckItem
                    {
                        Name = entry.Key,
                        Status = entry.Value.Status.ToString(),
                        Description = entry.Value.Description,
                        Duration = entry.Value.Duration.TotalMilliseconds,
                    }).ToList(),
                    TotalDuration = report.TotalDuration.TotalMilliseconds,
                    Setting = setting.Adapt<HealthCheckResponse.HealthCheckSetting>(),
                };

                await context.Response.WriteAsync(JsonHelper.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                }));
            }
        };

        // 配置详细的健康检查端点
        app.UseHealthChecks(DefaultHealthCheckPath, defaultHealthCheckOptions);

        var simpleHealthCheckOptions = new HealthCheckOptions
        {
            Predicate = _ => false, // 不执行任何检查，直接返回成功
            ResponseWriter = async (context, _) =>
            {
                context.Response.ContentType = JsonContentType;
                await context.Response.WriteAsync("OK");
            },
        };

        // 配置简单的健康检查端点（兼容性）
        app.UseHealthChecks(SimpleHealthCheckPath, simpleHealthCheckOptions);
        // OpenTelemetry相关检查
        if (setting.IsOpenTelemetry)
        {
            var openTelemetryHealthCheckOptions = new HealthCheckOptions
            {
                Predicate = _ => false, // 不执行任何检查，直接返回成功
                ResponseWriter = async (context, _) =>
                {
                    context.Response.ContentType = JsonContentType;
                    await context.Response.WriteAsync("OpenTelemetry is configured normally");
                },
            };
            app.UseHealthChecks(OpenTelemetryHealthCheckPath, openTelemetryHealthCheckOptions);
        }

        LogHelper.Info("the health check endpoint is enabled:");
        foreach (var ip in ipList)
        {
            LogHelper.Info($"- detailed health checks: http://{ip}:{setting.HttpPort}{DefaultHealthCheckPath}");
            LogHelper.Info($"- simple health check: http://{ip}:{setting.HttpPort}{SimpleHealthCheckPath}");
            if (setting.IsOpenTelemetry)
            {
                LogHelper.Info($"- OpenTelemetry check: http://{ip}:{setting.HttpPort}{OpenTelemetryHealthCheckPath}");
            }
        }

        return app;
    }
}

/// <summary>
/// Health check response information / 健康检查响应信息
/// </summary>
/// <remarks>
/// 包含健康检查的完整响应数据，包括整体状态、各项检查详情、执行时间和服务器配置信息。
/// 用于向客户端提供结构化的健康检查结果，便于监控和故障排查。
/// </remarks>
public sealed class HealthCheckResponse
{
    /// <summary>
    /// Health check item information / 健康检查项目信息
    /// </summary>
    /// <remarks>
    /// 表示单个健康检查项目的详细信息，包括名称、状态、描述和执行时间。
    /// 每个检查项目都会生成一个对应的实例，用于构建完整的健康检查报告。
    /// </remarks>
    public sealed class HealthCheckItem
    {
        /// <summary>
        /// Gets or sets the name of the health check item / 获取或设置健康检查项目的名称
        /// </summary>
        /// <value>The name of the health check item / 健康检查项目的名称</value>
        /// <remarks>
        /// 健康检查项目的唯一标识名称，用于区分不同的检查项目。
        /// 通常与注册时使用的名称保持一致，便于日志记录和问题定位。
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the status of the health check item / 获取或设置健康检查项目的状态
        /// </summary>
        /// <value>The status of the health check item, such as "Healthy", "Unhealthy", etc. / 健康检查项目的状态，如"Healthy"、"Unhealthy"等</value>
        /// <remarks>
        /// 表示当前检查项目的健康状态，可能的值包括：
        /// Healthy（健康）、Degraded（降级）、Unhealthy（不健康）。
        /// </remarks>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the description of the health check item / 获取或设置健康检查项目的描述信息
        /// </summary>
        /// <value>Detailed description of the health check item / 健康检查项目的详细描述信息</value>
        /// <remarks>
        /// 提供检查项目的详细说明，包括检查内容、检查结果的具体信息。
        /// 有助于运维人员理解检查项目的具体含义和当前状态。
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the execution duration of the health check item / 获取或设置健康检查项目的执行持续时间
        /// </summary>
        /// <value>The execution duration of the health check item in milliseconds / 健康检查项目的执行持续时间，单位为毫秒</value>
        /// <remarks>
        /// 记录单个检查项目的执行时间，用于性能监控和优化。
        /// 过长的执行时间可能表明检查项目存在性能问题。
        /// </remarks>
        public double Duration { get; set; }
    }

    /// <summary>
    /// Health check settings information class / 健康检查设置信息类
    /// </summary>
    /// <remarks>
    /// 该类用于在健康检查响应中携带服务器的基本配置信息，
    /// 便于运维人员通过健康检查接口快速了解服务器的运行状态和配置参数。
    /// 包含服务器标识、类型、时间等关键信息。
    /// </remarks>
    public sealed class HealthCheckSetting
    {
        /// <summary>
        /// Gets or sets the application launch time / 获取或设置应用程序启动时间
        /// </summary>
        /// <value>The timestamp when the application was launched / 应用程序的启动时间戳</value>
        /// <remarks>
        /// 用于计算服务器运行时长，便于监控服务器的稳定性和运行状态。
        /// 该时间通常在应用程序初始化时设置，不会在运行期间改变。
        /// </remarks>
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// Gets or sets the server type / 获取或设置服务器类型
        /// </summary>
        /// <value>The type of the current server, such as game server, gateway server, account server, etc. / 当前服务器的类型，如游戏服、网关服、账号服等</value>
        /// <remarks>
        /// 服务器类型决定了服务器的功能和职责范围。
        /// 支持的类型包括：Game（游戏服）、Gateway（网关服）、Account（账号服）、
        /// DiscoveryCenter（服务发现中心）、Login（登录服）等。
        /// </remarks>
        /// <seealso cref="ServerType"/>
        public string ServerType { get; set; }

        /// <summary>
        /// Gets or sets the server ID / 获取或设置服务器ID
        /// </summary>
        /// <value>The unique identifier of the server / 服务器的唯一标识符</value>
        /// <remarks>
        /// 服务器ID用于在集群环境中唯一标识一个服务器实例。
        /// 通常在配置文件中指定，用于服务发现、负载均衡和消息路由等场景。
        /// </remarks>
        public int ServerId { get; set; }

        /// <summary>
        /// Gets or sets the server instance ID / 获取或设置服务器实例ID
        /// </summary>
        /// <value>The unique identifier of the server instance / 服务器实例的唯一标识符</value>
        /// <remarks>
        /// 服务器实例ID是一个长整型的唯一标识符，通常在服务器启动时生成。
        /// 用于区分同一服务器类型的不同实例，在分布式环境中确保实例的唯一性。
        /// 与ServerId不同，实例ID通常是动态生成的。
        /// </remarks>
        public long ServerInstanceId { get; set; }

        /// <summary>
        /// Gets or sets the server name / 获取或设置服务器名称
        /// </summary>
        /// <value>The display name of the server / 服务器的显示名称</value>
        /// <remarks>
        /// 服务器名称通常基于服务器类型自动生成，也可以手动指定。
        /// 用于日志记录、监控界面显示和调试信息输出等场景。
        /// 例如："Game"、"Gateway"、"Account"等。
        /// </remarks>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the tag name / 获取或设置标记名称
        /// </summary>
        /// <value>The tag name of the server / 服务器的标记名称</value>
        /// <remarks>
        /// 标记名称用于对服务器进行分类或标记，便于管理和识别。
        /// 可以用于环境区分（如dev、test、prod）或功能标记等。
        /// </remarks>
        public string TagName { get; set; }

        /// <summary>
        /// Gets or sets the language setting / 获取或设置语言设置
        /// </summary>
        /// <value>The language code used by the server / 服务器使用的语言代码</value>
        /// <remarks>
        /// 语言设置用于国际化和本地化支持，影响服务器返回的消息和日志的语言。
        /// 通常使用标准的语言代码，如"zh-CN"、"en-US"等。
        /// </remarks>
        public string Language { get; set; }

        /// <summary>
        /// Gets or sets the description information / 获取或设置描述信息
        /// </summary>
        /// <value>Detailed description information of the server / 服务器的详细描述信息</value>
        /// <remarks>
        /// 描述信息提供服务器的详细说明，包括功能介绍、版本信息等。
        /// 主要用于文档记录和运维人员了解服务器的具体用途。
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the note information / 获取或设置备注信息
        /// </summary>
        /// <value>Note information of the server / 服务器的备注信息</value>
        /// <remarks>
        /// 备注信息用于记录服务器的额外说明、注意事项或临时信息。
        /// 可以包含运维相关的提醒、配置说明或其他重要信息。
        /// </remarks>
        public string Note { get; set; }

        /// <summary>
        /// Gets or sets the label information / 获取或设置标签信息
        /// </summary>
        /// <value>Label information of the server / 服务器的标签信息</value>
        /// <remarks>
        /// 标签信息用于对服务器进行分类和过滤，支持多标签管理。
        /// 可以用于服务发现、监控分组、资源管理等场景。
        /// 标签通常以逗号分隔的字符串形式存储。
        /// </remarks>
        public string Label { get; set; }
    }

    /// <summary>
    /// Gets or sets the overall health check status / 获取或设置整体健康检查状态
    /// </summary>
    /// <value>Overall health check status, such as "Healthy", "Unhealthy", etc. / 整体健康检查状态，如"Healthy"、"Unhealthy"等</value>
    /// <remarks>
    /// 表示所有健康检查项目的综合状态，通常取最差的状态作为整体状态。
    /// 用于快速判断系统的整体健康状况。
    /// </remarks>
    public string Status { get; set; }

    /// <summary>
    /// Gets or sets the total execution time of all health check items / 获取或设置所有健康检查项目的总执行时间
    /// </summary>
    /// <value>Total execution time of all health check items in milliseconds / 所有健康检查项目的总执行时间，单位为毫秒</value>
    /// <remarks>
    /// 记录所有健康检查项目的总执行时间，用于性能监控和系统优化。
    /// 过长的总执行时间可能影响健康检查接口的响应性能。
    /// </remarks>
    public double TotalDuration { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the health check was executed / 获取或设置健康检查执行的时间戳
    /// </summary>
    /// <value>Timestamp when the health check was executed / 健康检查执行的时间戳</value>
    /// <remarks>
    /// 记录健康检查执行的具体时间，用于追踪检查历史和问题排查。
    /// 时间戳采用UTC格式，确保跨时区的一致性。
    /// </remarks>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the application settings information / 获取或设置应用设置信息
    /// </summary>
    /// <value>Current application's <see cref="AppSetting"/> instance, may be null / 当前应用程序的 <see cref="AppSetting"/> 实例，可能为 null</value>
    /// <remarks>
    /// 该属性用于在健康检查响应中携带当前应用程序的配置信息，便于调试与监控。
    /// 包含端口、功能开关等关键配置项，方便在健康检查接口中直接查看运行参数。
    /// </remarks>
    public HealthCheckSetting Setting { get; set; }

    /// <summary>
    /// Gets or sets the list of health check items / 获取或设置健康检查项目列表
    /// </summary>
    /// <value>List containing all health check items / 包含所有健康检查项目的列表</value>
    /// <remarks>
    /// 包含所有已注册的健康检查项目的详细信息，每个项目包含名称、状态、描述和执行时间。
    /// 用于提供完整的健康检查报告，便于详细分析系统各组件的健康状况。
    /// </remarks>
    /// <seealso cref="HealthCheckItem"/>
    public List<HealthCheckItem> Checks { get; set; }
}