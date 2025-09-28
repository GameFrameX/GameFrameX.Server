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
/// 健康检查配置扩展方法
/// </summary>
public static class HealthChecksExtensions
{
    private const string JsonContentType = "application/json; charset=utf-8";

    /// <summary>
    /// 默认健康检查端点路径
    /// </summary>
    /// <value>返回默认的健康检查端点路径 "/health"</value>
    public const string DefaultHealthCheckPath = "/health";

    /// <summary>
    /// 简单健康检查端点路径（兼容性端点）
    /// </summary>
    public const string SimpleHealthCheckPath = $"{DefaultHealthCheckPath}/simple";

    /// <summary>
    /// OpenTelemetry 健康检查端点路径
    /// </summary>
    public const string OpenTelemetryHealthCheckPath = $"{DefaultHealthCheckPath}/opentelemetry";

    /// <summary>
    /// 添加 GameFrameX 健康检查服务（包含多种检查项目）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="setting">应用设置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddGameFrameXHealthChecks(this IServiceCollection services, AppSetting setting)
    {
        var healthChecksBuilder = services.AddHealthChecks();
        // 基础应用程序健康检查
        healthChecksBuilder.AddCheck(DefaultHealthCheckPath, () => HealthCheckResult.Healthy("应用程序运行正常"));
        healthChecksBuilder.AddCheck(SimpleHealthCheckPath, () => HealthCheckResult.Healthy("应用程序运行正常"));

        // OpenTelemetry相关检查
        if (setting.IsOpenTelemetry)
        {
            healthChecksBuilder.AddCheck(OpenTelemetryHealthCheckPath, () => HealthCheckResult.Healthy("OpenTelemetry 配置正常"));
        }

        return services;
    }

    /// <summary>
    /// 使用 GameFrameX 健康检查中间件
    /// </summary>
    /// <param name="app">应用程序构建器，用于配置HTTP请求管道</param>
    /// <param name="setting">应用设置对象，包含应用程序的配置信息</param>
    /// <param name="ipList"></param>
    /// <returns>返回配置了健康检查端点的应用程序构建器</returns>
    /// <remarks>
    /// 此方法会配置以下健康检查端点：
    /// <list type="bullet">
    /// <item><description><c>/health</c> - 详细的JSON格式健康检查报告</description></item>
    /// <item><description><c>/health/simple</c> - 简单的"OK"响应（兼容性端点）</description></item>
    /// </list>
    /// 健康检查响应包含状态、检查项详情、持续时间、服务器信息和时间戳。
    /// </remarks>
    /// <example>
    /// <code>
    /// // 使用默认路径
    /// app.UseGameFrameXHealthChecks(appSetting);
    /// 
    /// // 使用自定义路径
    /// app.UseGameFrameXHealthChecks(appSetting, "/api/health");
    /// </code>
    /// </example>
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
                    await context.Response.WriteAsync("OpenTelemetry 配置正常");
                },
            };
            app.UseHealthChecks(OpenTelemetryHealthCheckPath, openTelemetryHealthCheckOptions);
        }

        LogHelper.InfoConsole("健康检查端点已启用:");
        foreach (var ip in ipList)
        {
            LogHelper.InfoConsole($"- 详细健康检查: http://{ip}:{setting.HttpPort}{DefaultHealthCheckPath}");
            LogHelper.InfoConsole($"- 简单健康检查: http://{ip}:{setting.HttpPort}{SimpleHealthCheckPath}");
            if (setting.IsOpenTelemetry)
            {
                LogHelper.InfoConsole($"- OpenTelemetry 检查: http://{ip}:{setting.HttpPort}{OpenTelemetryHealthCheckPath}");
            }
        }

        return app;
    }
}

/// <summary>
/// 健康检查响应信息
/// </summary>
public sealed class HealthCheckResponse
{
    /// <summary>
    /// 健康检查项目信息
    /// </summary>
    public sealed class HealthCheckItem
    {
        /// <summary>
        /// 获取或设置健康检查项目的名称
        /// </summary>
        /// <value>健康检查项目的名称</value>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置健康检查项目的状态
        /// </summary>
        /// <value>健康检查项目的状态，如"Healthy"、"Unhealthy"等</value>
        public string Status { get; set; }

        /// <summary>
        /// 获取或设置健康检查项目的描述信息
        /// </summary>
        /// <value>健康检查项目的详细描述信息</value>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置健康检查项目的执行持续时间
        /// </summary>
        /// <value>健康检查项目的执行持续时间，单位为毫秒</value>
        public double Duration { get; set; }
    }

    /// <summary>
    /// 健康检查设置信息类
    /// </summary>
    /// <remarks>
    /// 该类用于在健康检查响应中携带服务器的基本配置信息，
    /// 便于运维人员通过健康检查接口快速了解服务器的运行状态和配置参数。
    /// 包含服务器标识、类型、时间等关键信息。
    /// </remarks>
    public sealed class HealthCheckSetting
    {
        /// <summary>
        /// 获取或设置应用程序启动时间
        /// </summary>
        /// <value>应用程序的启动时间戳</value>
        /// <remarks>
        /// 用于计算服务器运行时长，便于监控服务器的稳定性和运行状态。
        /// 该时间通常在应用程序初始化时设置，不会在运行期间改变。
        /// </remarks>
        public DateTime LaunchTime { get; set; }

        /// <summary>
        /// 获取或设置服务器类型
        /// </summary>
        /// <value>当前服务器的类型，如游戏服、网关服、账号服等</value>
        /// <remarks>
        /// 服务器类型决定了服务器的功能和职责范围。
        /// 支持的类型包括：Game（游戏服）、Gateway（网关服）、Account（账号服）、
        /// DiscoveryCenter（服务发现中心）、Login（登录服）等。
        /// </remarks>
        /// <seealso cref="ServerType"/>
        public ServerType ServerType { get; set; }

        /// <summary>
        /// 获取或设置服务器ID
        /// </summary>
        /// <value>服务器的唯一标识符</value>
        /// <remarks>
        /// 服务器ID用于在集群环境中唯一标识一个服务器实例。
        /// 通常在配置文件中指定，用于服务发现、负载均衡和消息路由等场景。
        /// </remarks>
        public int ServerId { get; set; }

        /// <summary>
        /// 获取或设置服务器实例ID
        /// </summary>
        /// <value>服务器实例的唯一标识符</value>
        /// <remarks>
        /// 服务器实例ID是一个长整型的唯一标识符，通常在服务器启动时生成。
        /// 用于区分同一服务器类型的不同实例，在分布式环境中确保实例的唯一性。
        /// 与ServerId不同，实例ID通常是动态生成的。
        /// </remarks>
        public long ServerInstanceId { get; set; }

        /// <summary>
        /// 获取或设置服务器名称
        /// </summary>
        /// <value>服务器的显示名称</value>
        /// <remarks>
        /// 服务器名称通常基于服务器类型自动生成，也可以手动指定。
        /// 用于日志记录、监控界面显示和调试信息输出等场景。
        /// 例如："Game"、"Gateway"、"Account"等。
        /// </remarks>
        public string ServerName { get; set; }

        /// <summary>
        /// 获取或设置标记名称
        /// </summary>
        /// <value>服务器的标记名称</value>
        /// <remarks>
        /// 标记名称用于对服务器进行分类或标记，便于管理和识别。
        /// 可以用于环境区分（如dev、test、prod）或功能标记等。
        /// </remarks>
        public string TagName { get; set; }

        /// <summary>
        /// 获取或设置语言设置
        /// </summary>
        /// <value>服务器使用的语言代码</value>
        /// <remarks>
        /// 语言设置用于国际化和本地化支持，影响服务器返回的消息和日志的语言。
        /// 通常使用标准的语言代码，如"zh-CN"、"en-US"等。
        /// </remarks>
        public string Language { get; set; }

        /// <summary>
        /// 获取或设置描述信息
        /// </summary>
        /// <value>服务器的详细描述信息</value>
        /// <remarks>
        /// 描述信息提供服务器的详细说明，包括功能介绍、版本信息等。
        /// 主要用于文档记录和运维人员了解服务器的具体用途。
        /// </remarks>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置备注信息
        /// </summary>
        /// <value>服务器的备注信息</value>
        /// <remarks>
        /// 备注信息用于记录服务器的额外说明、注意事项或临时信息。
        /// 可以包含运维相关的提醒、配置说明或其他重要信息。
        /// </remarks>
        public string Note { get; set; }

        /// <summary>
        /// 获取或设置标签信息
        /// </summary>
        /// <value>服务器的标签信息</value>
        /// <remarks>
        /// 标签信息用于对服务器进行分类和过滤，支持多标签管理。
        /// 可以用于服务发现、监控分组、资源管理等场景。
        /// 标签通常以逗号分隔的字符串形式存储。
        /// </remarks>
        public string Label { get; set; }
    }

    /// <summary>
    /// 获取或设置整体健康检查状态
    /// </summary>
    /// <value>整体健康检查状态，如"Healthy"、"Unhealthy"等</value>
    public string Status { get; set; }

    /// <summary>
    /// 获取或设置所有健康检查项目的总执行时间
    /// </summary>
    /// <value>所有健康检查项目的总执行时间，单位为毫秒</value>
    public double TotalDuration { get; set; }

    /// <summary>
    /// 获取或设置健康检查执行的时间戳
    /// </summary>
    /// <value>健康检查执行的时间戳</value>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// 获取或设置应用设置信息
    /// </summary>
    /// <remarks>
    /// 该属性用于在健康检查响应中携带当前应用程序的配置信息，便于调试与监控。
    /// 包含端口、功能开关等关键配置项，方便在健康检查接口中直接查看运行参数。
    /// </remarks>
    /// <value>当前应用程序的 <see cref="AppSetting"/> 实例，可能为 null。</value>
    public HealthCheckSetting Setting { get; set; }

    /// <summary>
    /// 获取或设置健康检查项目列表
    /// </summary>
    /// <value>包含所有健康检查项目的列表</value>
    /// <seealso cref="HealthCheckItem"/>
    public List<HealthCheckItem> Checks { get; set; }
}