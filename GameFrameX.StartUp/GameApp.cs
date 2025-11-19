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
/// Game application entry point class / 游戏应用程序入口类
/// </summary>
/// <remarks>
/// 此类提供了游戏服务器的启动入口点，负责初始化各种服务器组件，
/// 包括日志系统、配置管理、启动类型发现和服务器启动流程。
/// 支持多种服务器类型的启动和管理。
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
    /// Dictionary containing startup types and their associated attributes / 包含启动类型及其关联属性的字典
    /// </summary>
    /// <remarks>
    /// 此字典存储了所有实现IAppStartUp接口并标记了StartUpTagAttribute的类型，
    /// 键为类型，值为对应的启动标签属性。
    /// </remarks>
    private static readonly Dictionary<Type, StartUpTagAttribute> StartUpTypes = new();

    private static Task _launchTask;

    /// <summary>
    /// Main entry point for starting the game application / 启动游戏应用程序的主入口点
    /// </summary>
    /// <param name="args">Command line arguments / 命令行参数</param>
    /// <param name="initAction">Initialization action executed before starting the server, used for external protocol registration / 在启动服务器之前执行的初始化操作，用于外部协议注册</param>
    /// <param name="logConfiguration">Callback for log system initialization, allows overriding parameters / 日志系统初始化回调，可以重写参数</param>
    /// <returns>A task representing the asynchronous operation / 表示异步操作的任务</returns>
    /// <exception cref="ArgumentNullException">Thrown when args is null / 当args为null时抛出</exception>
    /// <exception cref="InvalidOperationException">Thrown when startup configuration is invalid / 当启动配置无效时抛出</exception>
    /// <remarks>
    /// 此方法是整个游戏服务器的启动入口点，执行以下主要步骤：
    /// 1. 解析启动参数
    /// 2. 配置日志系统
    /// 3. 加载全局设置
    /// 4. 发现并注册启动类型
    /// 5. 根据服务器类型启动相应的服务
    /// 6. 等待所有启动任务完成
    /// </remarks>
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
        LauncherOptions launcherOptions = null;
        try
        {
            launcherOptions = OptionsBuilder.CreateWithDebug<LauncherOptions>(args);
        }
        catch (Exception e)
        {
            LogHelper.Error(e.Message);
        }

        var serverType = launcherOptions?.ServerType;
        if (!serverType.IsNullOrEmpty())
        {
            LogHelper.Info($"the type of server that is launched : {serverType}");
        }

        LogOptions.Default.GrafanaLokiLabels = new Dictionary<string, string>();

        if (launcherOptions != null)
        {
            // 将LauncherOptions的所有属性添加到标签中
            var properties = typeof(LauncherOptions).GetProperties();
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
                    LogHelper.Warning($"Grafana Loki label {property.Name} already exists, will be ignored");
                }
            }

            // 设置日志配置信息
            LogOptions.Default.IsConsole = launcherOptions.LogIsConsole;
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

            LogOptions.Default.LogTagName = logTypeParts.Count > 0 ? string.Join("_", logTypeParts) : null;
        }

        logConfiguration?.Invoke(LogOptions.Default);

        GlobalSettings.Load("Configs/app_config.json");
        initAction?.Invoke();

        var types = AssemblyHelper.GetTypes();
        if (types != null)
        {
            foreach (var type in types)
            {
                if (type.IsClass && type.IsImplWithInterface(typeof(IAppStartUp)) && type.GetCustomAttribute<StartUpTagAttribute>() != null)
                {
                    var startUpTag = type.GetCustomAttribute<StartUpTagAttribute>();
                    StartUpTypes.Add(type, startUpTag);
                }
            }
        }

        var sortedStartUpTypes = StartUpTypes.OrderBy(m => m.Value.Priority);


        var appSettings = GlobalSettings.GetSettings();
        if (serverType.IsNotNullOrWhiteSpace())
        {
            var startKv = sortedStartUpTypes.FirstOrDefault(m => m.Value.ServerType == serverType);
            if (startKv.Value != null)
            {
                var appSetting = appSettings.FirstOrDefault(m => m.ServerType == serverType);
                if (appSetting != null)
                {
                    LogHelper.Info($"Finding the boot configuration for the corresponding server type in the configuration file will be configured to boot=>{startKv.Value.ServerType}");
                }
                else
                {
                    LogHelper.Warning($"If no startup configuration is found for the server type, it will start with the default configuration=>{startKv.Value.ServerType}");
                    appSetting = launcherOptions.Adapt<AppSetting>();
                }

                Launcher(args, startKv, appSetting);
            }
        }
        else
        {
            foreach (var keyValuePair in sortedStartUpTypes)
            {
                var appSetting = appSettings.FirstOrDefault(appSetting => keyValuePair.Value.ServerType == appSetting.ServerType);
                if (appSetting != null)
                {
                    Launcher(args, keyValuePair, appSetting);
                    break;
                }

                LogHelper.Warning($"If no startup configuration is found for the server type, it will start with the default configuration=>{keyValuePair.Value.ServerType}");
                Launcher(args, keyValuePair);
                break;
            }
        }

        LogHelper.Info($"----------------------------The Startup Server Is Over------------------------------");

        ConsoleHelper.ConsoleLogo();

        if (_launchTask == default)
        {
            var message = LocalizationService.GetString(GameFrameX.Localization.Keys.StartUp.NoStartupTaskFound);
            Console.WriteLine($"[Warning] {message}");
            LogHelper.Warning(message);
            return;
        }

        await _launchTask;
    }

    /// <summary>
    /// Launches a startup task for the specified server type / 为指定的服务器类型启动一个启动任务
    /// </summary>
    /// <param name="args">Command line arguments / 命令行参数</param>
    /// <param name="keyValuePair">Key-value pair containing the startup type and its attribute / 包含启动类型及其属性的键值对</param>
    /// <param name="appSetting">Application settings for the server / 服务器的应用程序设置</param>
    /// <remarks>
    /// 此方法创建并启动一个新的服务器实例任务，将任务添加到AppStartUpTasks列表中
    /// 以便后续并发执行和等待。
    /// </remarks>
    private static void Launcher(string[] args, KeyValuePair<Type, StartUpTagAttribute> keyValuePair, AppSetting appSetting = null)
    {
        _launchTask = Start(args, keyValuePair.Key, keyValuePair.Value.ServerType, appSetting);
    }

    /// <summary>
    /// Starts a specific server instance / 启动特定的服务器实例
    /// </summary>
    /// <param name="args">Command line arguments / 命令行参数</param>
    /// <param name="appStartUpType">The type of the startup class / 启动类的类型</param>
    /// <param name="serverType">The server type identifier / 服务器类型标识符</param>
    /// <param name="setting">Application settings for the server / 服务器的应用程序设置</param>
    /// <returns>A task representing the server startup operation / 表示服务器启动操作的任务</returns>
    /// <exception cref="InvalidOperationException">Thrown when startup class cannot be instantiated / 当启动类无法实例化时抛出</exception>
    /// <exception cref="ArgumentNullException">Thrown when appStartUpType is null / 当appStartUpType为null时抛出</exception>
    /// <remarks>
    /// 此方法执行以下步骤：
    /// 1. 创建启动类实例
    /// 2. 初始化启动类
    /// 3. 记录配置信息
    /// 4. 调用AppEnter.Entry启动服务器
    /// 如果初始化失败，返回已完成的任务。
    /// </remarks>
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

        LogHelper.ShowOption($"Start Starting [{serverType}] Server- Configuration Information", startUp.Setting.ToFormatString());
        LogHelper.Info($"Start Starting [{serverType}] Server- Configuration Information");
        LogHelper.Info(startUp.Setting.ToFormatString());
        var task = AppEnter.Entry(startUp);
        return task;
    }
}