// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Utility.Setting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace GameFrameX.StartUp.Extensions;

/// <summary>
/// 健康检查配置扩展方法
/// </summary>
public static class HealthChecksExtensions
{
    /// <summary>
    /// 添加 GameFrameX 健康检查服务（包含多种检查项目）
    /// </summary>
    /// <param name="services">服务集合</param>
    /// <param name="setting">应用设置</param>
    /// <returns>服务集合</returns>
    public static IServiceCollection AddGameFrameXHealthChecks(this IServiceCollection services, AppSetting setting)
    {
        var healthChecksBuilder = services.AddHealthChecks()
                                          // 基础应用程序健康检查
                                          .AddCheck("self", () => HealthCheckResult.Healthy("应用程序运行正常"));

        // OpenTelemetry相关检查
        if (setting.IsOpenTelemetry)
        {
            healthChecksBuilder.AddCheck("opentelemetry", () => HealthCheckResult.Healthy("OpenTelemetry 配置正常"));
        }

        // 配置检查
        healthChecksBuilder.AddCheck("configuration", () =>
        {
            var issues = new List<string>();

            if (string.IsNullOrEmpty(setting.ServerName))
            {
                issues.Add("ServerName未配置");
            }

            if (string.IsNullOrEmpty(setting.TagName))
            {
                issues.Add("TagName未配置");
            }

            if (setting.SaveDataInterval <= 0)
            {
                issues.Add("SaveDataInterval配置无效");
            }

            if (issues.Any())
            {
                return HealthCheckResult.Degraded($"配置问题: {string.Join(", ", issues)}");
            }

            return HealthCheckResult.Healthy("配置检查通过");
        });

        return services;
    }
}