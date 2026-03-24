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
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Localization.Providers;
using GameFrameX.Foundation.Options;
using GameFrameX.Foundation.Options.Attributes;
using GameFrameX.Foundation.Utility;
using GameFrameX.Localization;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.StartUp.Options;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Mapster;

namespace GameFrameX.StartUp;

/// <summary>
/// 游戏应用程序入口类。
/// </summary>
/// <remarks>
/// Game application entry point class.
/// This class provides the startup entry point for game servers, responsible for initializing various server components,
/// including logging system, configuration management, startup type discovery, and server startup flow.
/// Supports startup and management of multiple server types.
/// </remarks>
/// <example>
/// <code>
/// // 启动游戏服务器
/// await GameApp.Entry(args, () => {
///     // 初始化协议注册等
/// }, logOptions => {
///     // 配置日志选项
///     logOptions.IsConsole = true;
/// });
/// </code>
/// </example>
public static class GameApp
{
    /// <summary>
    /// 当前服务器实例的启动任务。
    /// </summary>
    /// <remarks>
    /// The launch task for the current server instance.
    /// Used to track the execution status of the server startup task.
    /// </remarks>
    private static Task _launchTask;

    /// <summary>
    /// 解析启动器选项。
    /// </summary>
    /// <remarks>
    /// Parse launcher options.
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <returns>解析后的启动器选项 / Parsed launcher options</returns>
    private static StartupOptions ParseLauncherOptions(string[] args)
    {
        try
        {
            return OptionsBuilder.CreateWithDebug<StartupOptions>(args);
        }
        catch (Exception e)
        {
            LogHelper.Error(e.Message);
            return null;
        }
    }

    /// <summary>
    /// 配置 Grafana Loki 标签。
    /// </summary>
    /// <remarks>
    /// Configure Grafana Loki labels.
    /// </remarks>
    /// <param name="launcherOptions">启动器选项 / Launcher options</param>
    private static void ConfigureGrafanaLokiLabels(StartupOptions launcherOptions)
    {
        LogOptions.Default.GrafanaLokiLabels = new Dictionary<string, string>();

        if (launcherOptions == null)
        {
            return;
        }

        var properties = typeof(StartupOptions).GetProperties();
        foreach (var property in properties)
        {
            var grafanaLokiLabelTagAttribute = property.GetCustomAttribute<GrafanaLokiLabelTagAttribute>();
            if (grafanaLokiLabelTagAttribute == null)
            {
                continue;
            }

            var value = property.GetValue(launcherOptions)?.ToString();
            if (string.IsNullOrEmpty(value))
            {
                continue;
            }

            if (!LogOptions.Default.GrafanaLokiLabels.TryAdd(property.Name, value))
            {
                LogHelper.Warning(LocalizationService.GetString(Keys.StartUp.GrafanaLokiLabelExists, property.Name));
            }
        }
    }

    /// <summary>
    /// 配置日志选项。
    /// </summary>
    /// <remarks>
    /// Configure log options.
    /// </remarks>
    /// <param name="launcherOptions">启动器选项 / Launcher options</param>
    private static void ConfigureLogOptions(StartupOptions launcherOptions)
    {
        if (launcherOptions == null)
        {
            return;
        }

        // 只有显式启用时区设置时才调用 SetTimeZone
        if (launcherOptions.IsUseTimeZone)
        {
            TimerHelper.SetTimeZone(launcherOptions.TimeZone);
        }

        // 设置日志配置信息
        LogOptions.Default.IsConsole = launcherOptions.LogIsConsole;
        LogOptions.Default.IsWriteToFile = launcherOptions.LogIsWriteToFile;
        LogOptions.Default.IsGrafanaLoki = launcherOptions.LogIsGrafanaLoki;
        LogOptions.Default.GrafanaLokiUrl = launcherOptions.LogGrafanaLokiUrl;
        LogOptions.Default.GrafanaLokiUserName = launcherOptions.LogGrafanaLokiUserName;
        LogOptions.Default.GrafanaLokiPassword = launcherOptions.LogGrafanaLokiPassword;
        LogOptions.Default.RetainedFileCountLimit = launcherOptions.LogRetainedFileCountLimit;
        LogOptions.Default.IsFileSizeLimit = launcherOptions.LogIsFileSizeLimit;
        LogOptions.Default.FileSizeLimitBytes = launcherOptions.LogFileSizeLimitBytes;
        LogOptions.Default.LogEventLevel = launcherOptions.LogEventLevel;
        LogOptions.Default.RollingInterval = launcherOptions.LogRollingInterval;

        // 构建LogType，当值为空或默认值时不拼接
        var logTypeParts = new List<string>();

        if (launcherOptions.ServerId > 0)
        {
            logTypeParts.Add(launcherOptions.ServerId.ToString());
        }

        if (launcherOptions.ServerInstanceId > 0)
        {
            logTypeParts.Add(launcherOptions.ServerInstanceId.ToString());
        }

        // 设置 LogTagName（按优先级：TagName > Note > Label > Description）
        if (launcherOptions.TagName.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = launcherOptions.TagName;
        }
        else if (launcherOptions.Note.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = launcherOptions.Note;
        }
        else if (launcherOptions.Label.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = launcherOptions.Label;
        }
        else if (launcherOptions.Description.IsNotNullOrWhiteSpace())
        {
            LogOptions.Default.LogTagName = launcherOptions.Description;
        }

        if (LogOptions.Default.LogTagName.IsNullOrEmpty() && logTypeParts.Count > 0)
        {
            LogOptions.Default.LogTagName = string.Join("_", logTypeParts);
        }
    }

    /// <summary>
    /// 启动游戏应用程序的主入口点。
    /// </summary>
    /// <remarks>
    /// Main entry point for starting the game application.
    /// This method is the startup entry point for the entire game server, performing the following main steps:
    /// 1. Parse startup arguments
    /// 2. Configure logging system
    /// 3. Load global settings
    /// 4. Discover and register startup types
    /// 5. Start corresponding services based on server type
    /// 6. Wait for all startup tasks to complete
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <param name="initAction">在启动服务器之前执行的初始化操作，用于外部协议注册 / Initialization action executed before starting the server, used for external protocol registration</param>
    /// <param name="logConfiguration">日志系统初始化回调，可以重写参数 / Callback for log system initialization, allows overriding parameters</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="args"/> 为 null 时抛出 / Thrown when <paramref name="args"/> is null</exception>
    /// <exception cref="InvalidOperationException">当启动配置无效时抛出 / Thrown when startup configuration is invalid</exception>
    /// <example>
    /// <code>
    /// // 基本启动
    /// await GameApp.Entry(args, null);
    ///
    /// // 带初始化和日志配置的启动
    /// await GameApp.Entry(args,
    ///     () => ProtocolManager.RegisterAll(),
    ///     logOptions => {
    ///         logOptions.IsConsole = true;
    ///         logOptions.LogEventLevel = LogEventLevel.Debug;
    ///     });
    /// </code>
    /// </example>
    public static async Task Entry(string[] args, Action initAction, Action<LogOptions> logConfiguration = null)
    {
        LocalizationService.Instance.RegisterProvider(new AssemblyResourceProvider(typeof(Keys).Assembly));

        // 1. 解析启动参数
        var launcherOptions = ParseLauncherOptions(args);

        // 2. 输出服务器类型日志
        var serverType = launcherOptions?.ServerType;
        if (!serverType.IsNullOrEmpty())
        {
            // LogHelper.Info(LocalizationService.GetString(Keys.StartUp.LaunchServerType, serverType));
        }

        // 3. 配置日志
        ConfigureGrafanaLokiLabels(launcherOptions);
        ConfigureLogOptions(launcherOptions);
        logConfiguration?.Invoke(LogOptions.Default);

        // 4. 初始化
        GlobalSettings.Load("Configs/app_config.json");
        initAction?.Invoke();

        // 5. 发现并启动服务器
        StartUpTypeRegistry.Instance.DiscoverAndRegister();
        var sortedStartUpTypes = StartUpTypeRegistry.Instance.GetSortedByPriority();
        TryLaunchServer(args, serverType, sortedStartUpTypes, launcherOptions);

        LogHelper.Info(LocalizationService.GetString(Keys.StartUp.StartupOver));
        ConsoleHelper.ConsoleLogo();

        // 6. 等待启动任务完成
        if (_launchTask == null)
        {
            var message = LocalizationService.GetString(Keys.StartUp.NoStartupTaskFound);
            LogHelper.Warning(message);
            return;
        }

        await _launchTask;
    }

    /// <summary>
    /// 为指定的服务器类型启动一个启动任务。
    /// </summary>
    /// <remarks>
    /// Launches a startup task for the specified server type.
    /// This method creates and starts a new server instance task, adding the task to the AppStartUpTasks list
    /// for subsequent concurrent execution and waiting.
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <param name="keyValuePair">包含启动类型及其属性的键值对 / Key-value pair containing the startup type and its attribute</param>
    /// <param name="appSetting">服务器的应用程序设置 / Application settings for the server</param>
    private static void Launcher(string[] args, KeyValuePair<Type, StartUpTagAttribute> keyValuePair, AppSetting appSetting = null)
    {
        _launchTask = Start(args, keyValuePair.Key, keyValuePair.Value.ServerType, appSetting);
    }

    /// <summary>
    /// 启动特定的服务器实例。
    /// </summary>
    /// <remarks>
    /// Starts a specific server instance.
    /// This method performs the following steps:
    /// 1. Create startup class instance
    /// 2. Initialize startup class
    /// 3. Log configuration information
    /// 4. Call AppEnter.Entry to start the server
    /// If initialization fails, returns a completed task.
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <param name="appStartUpType">启动类的类型 / The type of the startup class</param>
    /// <param name="serverType">服务器类型标识符 / The server type identifier</param>
    /// <param name="setting">服务器的应用程序设置 / Application settings for the server</param>
    /// <returns>表示服务器启动操作的任务 / A task representing the server startup operation</returns>
    /// <exception cref="InvalidOperationException">当启动类无法实例化时抛出 / Thrown when startup class cannot be instantiated</exception>
    /// <exception cref="ArgumentNullException">当 <paramref name="appStartUpType"/> 为 null 时抛出 / Thrown when <paramref name="appStartUpType"/> is null</exception>
    /// <example>
    /// <code>
    /// var task = Start(args, typeof(GameServerStartUp), "GameServer", appSetting);
    /// await task;
    /// </code>
    /// </example>
    private static Task Start(string[] args, Type appStartUpType, string serverType, AppSetting setting)
    {
        var startUp = (IAppStartUp)Activator.CreateInstance(appStartUpType);
        if (startUp == null)
        {
            return Task.CompletedTask;
        }

        LogOptions.Default.LogType = serverType;
        LogHandler.Create(LogOptions.Default);
        var isSuccess = startUp.Init(serverType, setting, args);
        if (!isSuccess)
        {
            return Task.CompletedTask;
        }

        LogHelper.ShowOption(LocalizationService.GetString(Keys.StartUp.StartingServerWithConfiguration, serverType), startUp.Setting.ToFormatString());
        // LogHelper.Info(LocalizationService.GetString(Keys.StartUp.StartingServerWithConfiguration, serverType));
        // LogHelper.Info(startUp.Setting.ToFormatString());
        var task = AppEnter.Entry(startUp);
        return task;
    }

    /// <summary>
    /// 尝试根据指定的服务器类型启动服务器。
    /// </summary>
    /// <remarks>
    /// Attempts to launch a server based on the specified server type.
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <param name="serverType">要启动的服务器类型，或为 null 以启动第一个可用的服务器 / The server type to launch, or null to launch the first available</param>
    /// <param name="sortedStartUpTypes">按优先级排序的启动类型集合 / Collection of startup types sorted by priority</param>
    /// <param name="launcherOptions">包含默认配置的启动器选项 / Launcher options containing default configuration</param>
    private static void TryLaunchServer(string[] args, string serverType, IEnumerable<KeyValuePair<Type, StartUpTagAttribute>> sortedStartUpTypes, StartupOptions launcherOptions)
    {
        var appSettings = GlobalSettings.GetSettings();

        if (serverType.IsNullOrWhiteSpace())
        {
            LogHelper.Error("ServerType 参数不能为空，请通过 --ServerType=xxx 指定服务类型。");
            return;
        }

        TryLaunchByServerType(args, serverType, sortedStartUpTypes, appSettings, launcherOptions);
    }

    /// <summary>
    /// 尝试按指定的服务器类型启动服务器。
    /// </summary>
    /// <remarks>
    /// Attempts to launch a server by the specified server type.
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <param name="serverType">服务器类型标识符 / The server type identifier</param>
    /// <param name="sortedStartUpTypes">按优先级排序的启动类型集合 / Collection of startup types sorted by priority</param>
    /// <param name="appSettings">配置中的应用程序设置集合 / Collection of application settings from configuration</param>
    /// <param name="launcherOptions">用于默认配置的启动器选项 / Launcher options for default configuration</param>
    private static void TryLaunchByServerType(string[] args, string serverType, IEnumerable<KeyValuePair<Type, StartUpTagAttribute>> sortedStartUpTypes, IEnumerable<AppSetting> appSettings, StartupOptions launcherOptions)
    {
        var startKv = sortedStartUpTypes.FirstOrDefault(m => m.Value.ServerType == serverType);
        if (startKv.Value == null)
        {
            return;
        }

        var appSetting = appSettings.FirstOrDefault(m => m.ServerType == serverType);
        if (appSetting != null)
        {
            // LogHelper.Info(LocalizationService.GetString(Keys.StartUp.FindingConfigurationForServerType, startKv.Value.ServerType));
        }
        else
        {
            // LogHelper.Warning(LocalizationService.GetString(Keys.StartUp.NoConfigurationUseDefault, startKv.Value.ServerType));
            appSetting = launcherOptions.Adapt<AppSetting>();
        }

        Launcher(args, startKv, appSetting);
    }

    /// <summary>
    /// 尝试启动第一个可用的服务器。
    /// </summary>
    /// <remarks>
    /// Attempts to launch the first available server.
    /// </remarks>
    /// <param name="args">命令行参数 / Command line arguments</param>
    /// <param name="sortedStartUpTypes">按优先级排序的启动类型集合 / Collection of startup types sorted by priority</param>
    /// <param name="appSettings">配置中的应用程序设置集合 / Collection of application settings from configuration</param>
    private static void TryLaunchFirstAvailable(string[] args, IEnumerable<KeyValuePair<Type, StartUpTagAttribute>> sortedStartUpTypes, IEnumerable<AppSetting> appSettings)
    {
        foreach (var keyValuePair in sortedStartUpTypes)
        {
            var appSetting = appSettings.FirstOrDefault(setting => keyValuePair.Value.ServerType == setting.ServerType);
            if (appSetting != null)
            {
                Launcher(args, keyValuePair, appSetting);
                return;
            }

            LogHelper.Warning(LocalizationService.GetString(Keys.StartUp.NoConfigurationUseDefault, keyValuePair.Value.ServerType));
            Launcher(args, keyValuePair);
            return;
        }
    }
}
