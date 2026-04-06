// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Localization.Providers;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Options.Attributes;
using GameFrameX.Localization;
using GameFrameX.StartUp.Options;

namespace GameFrameX.AppHost;

/// <summary>
/// 应用程序主机入口类，负责配置和启动分布式应用。
/// </summary>
/// <remarks>
/// Application host entry class, responsible for configuring and starting distributed applications.
/// </remarks>
internal static class Program
{
    /// <summary>
    /// 应用程序主入口点，初始化并运行分布式应用。
    /// </summary>
    /// <remarks>
    /// Main entry point of the application, initializes and runs the distributed application.
    /// </remarks>
    /// <param name="args">命令行参数数组 / Command line arguments array</param>
    /// <returns>表示异步操作的任务 / Task representing the asynchronous operation</returns>
    private static async Task Main(string[] args)
    {
        LocalizationService.Instance.RegisterProvider(new AssemblyResourceProvider(typeof(Keys).Assembly));
        var builder = DistributedApplication.CreateBuilder(args);
        LogOptions.Default.LogType = "AppHost";

        StartupOptions startupOptions;
        try
        {
            startupOptions = Foundation.Options.OptionsBuilder.CreateWithDebug<StartupOptions>(args);
        }
        catch (Exception e)
        {
            LogHelper.Error(e.Message);
            throw;
        }

        var serverType = startupOptions.ServerType;
        if (!serverType.IsNullOrEmpty())
        {
            LogHelper.Info(LocalizationService.GetString(Keys.StartUp.LaunchServerType, serverType));
        }

        ConfigureLogOptions(startupOptions);

        LogHandler.Create(LogOptions.Default);

        var mongoDb = builder.AddMongoDB("mongo").WithLifetime(ContainerLifetime.Persistent);
        mongoDb.AddDatabase("mongodb");

        if (startupOptions.IsSingleMode)
        {
            if (startupOptions.ServerType.IsNullOrWhiteSpace())
            {
                throw new InvalidOperationException("单进程模式必须通过 --ServerType 指定服务类型，例如 --ServerType=Game");
            }

            var singleModeServerType = startupOptions.ServerType.Trim();
            if (singleModeServerType.Contains(',', StringComparison.Ordinal))
            {
                throw new InvalidOperationException("单进程模式不支持多个服务类型，请仅传一个 --ServerType 值，例如 --ServerType=Game");
            }

            var launcherArgs = BuildLauncherArgs(args, singleModeServerType);
            builder
                .AddProject<Projects.GameFrameX_Launcher>("single-service")
                .WithArgs(launcherArgs.ToArray())
                .WithReference(mongoDb)
                .WaitFor(mongoDb);
        }
        else
        {
            var multiServerTypes = ResolveMultiServerTypes(startupOptions);
            var services = new List<IResourceBuilder<ProjectResource>>();
            foreach (var multiServerType in multiServerTypes)
            {
                var serviceName = $"{multiServerType.ToLowerInvariant()}-service";
                var service = builder
                              .AddProject<Projects.GameFrameX_Launcher>(serviceName)
                              .WithArgs(BuildLauncherArgs(args, multiServerType).ToArray())
                              .WithReference(mongoDb)
                              .WaitFor(mongoDb);
                services.Add(service);
            }

            for (var i = 1; i < services.Count; i++)
            {
                services[i].WaitFor(services[i - 1]);
            }
        }

        await builder.Build().RunAsync();
    }

    /// <summary>
    /// 根据启动选项配置日志选项。
    /// </summary>
    /// <remarks>
    /// Configures log options based on startup options.
    /// </remarks>
    /// <param name="startupOptions">启动选项实例 / Startup options instance</param>
    private static void ConfigureLogOptions(StartupOptions startupOptions)
    {
        var properties = typeof(StartupOptions).GetProperties();
        foreach (var property in properties)
        {
            var grafanaLokiLabelTagAttribute = property.GetCustomAttribute<GrafanaLokiLabelTagAttribute>();
            if (grafanaLokiLabelTagAttribute == null)
            {
                continue;
            }

            if (property.Name.Equals(nameof(StartupOptions.ServerType), StringComparison.Ordinal))
            {
                continue;
            }

            var value = property.GetValue(startupOptions)?.ToString();
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            if (!LogOptions.Default.GrafanaLokiLabels.TryAdd(property.Name, value))
            {
                LogHelper.Warning(LocalizationService.GetString(Keys.StartUp.GrafanaLokiLabelExists, property.Name));
            }
        }

        LogOptions.Default.IsConsole = startupOptions.LogIsConsole;
        LogOptions.Default.IsGrafanaLoki = startupOptions.LogIsGrafanaLoki;
        LogOptions.Default.GrafanaLokiUrl = startupOptions.LogGrafanaLokiUrl;
        LogOptions.Default.GrafanaLokiUserName = startupOptions.LogGrafanaLokiUserName;
        LogOptions.Default.GrafanaLokiPassword = startupOptions.LogGrafanaLokiPassword;
        LogOptions.Default.RetainedFileCountLimit = startupOptions.LogRetainedFileCountLimit;
        LogOptions.Default.IsFileSizeLimit = startupOptions.LogIsFileSizeLimit;
        LogOptions.Default.FileSizeLimitBytes = startupOptions.LogFileSizeLimitBytes;
        LogOptions.Default.LogEventLevel = startupOptions.LogEventLevel;
        LogOptions.Default.RollingInterval = startupOptions.LogRollingInterval;

        var logTypeParts = new List<string>();
        if (startupOptions.ServerId > 0)
        {
            logTypeParts.Add(startupOptions.ServerId.ToString());
        }

        if (startupOptions.ServerInstanceId > 0)
        {
            logTypeParts.Add(startupOptions.ServerInstanceId.ToString());
        }

        if (startupOptions.TagName.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = startupOptions.TagName;
        }
        else if (startupOptions.Note.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = startupOptions.Note;
        }
        else if (startupOptions.Label.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = startupOptions.Label;
        }
        else if (startupOptions.Description.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = startupOptions.Description;
        }

        LogOptions.Default.LogTagName = logTypeParts.Count > 0 ? string.Join("_", logTypeParts) : null;
    }

    /// <summary>
    /// 根据服务器类型参数解析多个服务器类型。
    /// </summary>
    /// <remarks>
    /// Resolves multiple server types based on server type argument.
    /// </remarks>
    /// <param name="startupOptions">启动选项实例 / Startup options instance</param>
    /// <returns>服务器类型列表 / List of server types</returns>
    private static List<string> ResolveMultiServerTypes(StartupOptions startupOptions)
    {
        if (startupOptions.ServerType.IsNullOrWhiteSpace())
        {
            throw new InvalidOperationException("多进程模式必须通过 --ServerType 指定服务列表，例如 --ServerType=Game,Social");
        }

        var serverTypeText = startupOptions.ServerType.Trim();
        var serverTypes = serverTypeText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                     .Distinct(StringComparer.OrdinalIgnoreCase)
                                     .ToList();
        if (serverTypes.Count == 0)
        {
            throw new InvalidOperationException("ServerType 未解析到有效服务类型，请使用逗号分隔列表，例如 --ServerType=Game,Social");
        }

        return serverTypes;
    }

    /// <summary>
    /// 构建启动器参数列表，过滤掉AppHost专属参数并添加服务器类型参数。
    /// </summary>
    /// <remarks>
    /// Builds launcher arguments list, filters out AppHost-specific parameters and adds server type argument.
    /// </remarks>
    /// <param name="args">原始命令行参数数组 / Original command line arguments array</param>
    /// <param name="serverType">服务器类型 / Server type</param>
    /// <returns>构建后的启动器参数列表 / Built launcher arguments list</returns>
    private static List<string> BuildLauncherArgs(string[] args, string serverType)
    {
        var launcherArgs = new List<string>();
        foreach (var arg in args)
        {
            if (arg.IsNullOrWhiteSpace())
            {
                continue;
            }

            var optionName = ParseOptionName(arg);
            if (optionName.IsNullOrWhiteSpace())
            {
                launcherArgs.Add(arg);
                continue;
            }

            if (optionName.Equals(nameof(StartupOptions.IsSingleMode), StringComparison.OrdinalIgnoreCase) ||
                optionName.Equals(nameof(StartupOptions.ServerType), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            launcherArgs.Add(arg);
        }

        launcherArgs.Add($"--{nameof(StartupOptions.ServerType)}={serverType}");
        return launcherArgs;
    }

    /// <summary>
    /// 从命令行参数中解析选项名称。
    /// </summary>
    /// <remarks>
    /// Parses option name from command line argument.
    /// </remarks>
    /// <param name="arg">命令行参数字符串 / Command line argument string</param>
    /// <returns>选项名称；如果格式不正确则返回空字符串 / Option name; empty string if format is invalid</returns>
    private static string ParseOptionName(string arg)
    {
        var optionText = arg.Trim();
        if (!optionText.StartsWith("--", StringComparison.Ordinal))
        {
            return string.Empty;
        }

        optionText = optionText[2..];
        var equalIndex = optionText.IndexOf('=');
        return equalIndex < 0 ? optionText : optionText[..equalIndex];
    }
}
