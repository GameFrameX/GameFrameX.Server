using System.Collections;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommandLine;
using GameFrameX.Monitor;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.StartUp.Options;
using GameFrameX.Utility;
using GameFrameX.Utility.Extensions;
using GameFrameX.Utility.Log;
using GameFrameX.Utility.Setting;

namespace GameFrameX.StartUp;

/// <summary>
/// 程序入口类
/// </summary>
public static class GameApp
{
    private static readonly Dictionary<Type, StartUpTagAttribute> StartUpTypes = new();
    private static readonly List<Task> AppStartUpTasks = new();
    private static readonly List<IAppStartUp> AppStartUps = new();

    /// <summary>
    /// 启动入口函数
    /// </summary>
    /// <param name="args">启动参数</param>
    /// <param name="initAction">在启动服务器之前执行,需要外部初始化协议注册</param>
    /// <param name="logConfiguration">初始化日志系统之前回调,可以重写参数</param>
    public static async Task Entry(string[] args, Action initAction, Action<LogOptions> logConfiguration = null)
    {
        var environmentVariablesList = new List<string>(args);
        LogHelper.Console("启动参数：" + string.Join(" ", args));
        LogHelper.Console("当前环境变量START---------------------");
        var environmentVariables = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry environmentVariable in environmentVariables)
        {
            if (environmentVariable.Value == null || environmentVariable.Key.ToString().IsNullOrWhiteSpace())
            {
                continue;
            }

            var key = environmentVariable.Key.ToString().StartsWith("--") ? environmentVariable.Key.ToString() : "--" + environmentVariable.Key;
            if (environmentVariablesList.Contains(key))
            {
                continue;
            }

            environmentVariablesList.Add(key);
            environmentVariablesList.Add(environmentVariable.Value.ToString());
        }

        LogHelper.Console("当前环境变量END---------------------");
        LogHelper.Console(string.Empty);
        LogHelper.Console(string.Empty);
        var commandLineParser = new Parser(configuration => { configuration.IgnoreUnknownArguments = true; });

        var launcherOptions = commandLineParser.ParseArguments<LauncherOptions>(environmentVariablesList).WithParsed(LauncherOptionsValidate)?.Value;
        var serverType = launcherOptions?.ServerType;
        if (!serverType.IsNullOrEmpty())
        {
            LogHelper.Console("启动的服务器类型 ServerType: " + serverType);
        }

        LogOptions.Default.ServerType = serverType;
        logConfiguration?.Invoke(LogOptions.Default);
        LoggerHandler.Start(LogOptions.Default);

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

        LogHelper.InfoConsole("----------------------------开始启动服务器啦------------------------------");
        var appSettings = GlobalSettings.GetSettings();
        if (serverType != null && Enum.TryParse(serverType, out ServerType serverTypeValue))
        {
            var startKv = sortedStartUpTypes.FirstOrDefault(m => m.Value.ServerType == serverTypeValue);
            if (startKv.Value != null)
            {
                var appSetting = appSettings.FirstOrDefault(m => m.ServerType == serverTypeValue);
                if (appSetting != null)
                {
                    LogHelper.InfoConsole($"从配置文件中找到对应的服务器类型的启动配置,将以配置启动=>{startKv.Value.ServerType}");
                }
                else
                {
                    LogHelper.WarnConsole($"没有找到对应的服务器类型的启动配置,将以默认配置启动=>{startKv.Value.ServerType}");
                    appSetting = new AppSetting
                    {
                        ServerId = launcherOptions.ServerId,
                        ServerType = serverTypeValue,
                        APMPort = launcherOptions.APMPort,
                        IsDebug = launcherOptions.IsDebug,
                        IsDebugSend = launcherOptions.IsDebugSend,
                        IsDebugReceive = launcherOptions.IsDebugReceive,
                        Language = launcherOptions.Language,
                        DataCenter = launcherOptions.DataCenter,
                        DiscoveryCenterIp = launcherOptions.DiscoveryCenterIp,
                        DiscoveryCenterPort = launcherOptions.DiscoveryCenterPort,
                        DBIp = launcherOptions.DBIp,
                        DBPort = launcherOptions.DBPort,
                        SaveDataInterval = launcherOptions.SaveDataInterval,
                        HttpCode = launcherOptions.HttpCode,
                        HttpPort = launcherOptions.HttpPort,
                        HttpsPort = launcherOptions.HttpsPort,
                        HttpUrl = launcherOptions.HttpUrl,
                        InnerIp = launcherOptions.InnerIp,
                        InnerPort = launcherOptions.InnerPort,
                        OuterIp = launcherOptions.OuterIp,
                        OuterPort = launcherOptions.OuterPort,
                        WsPort = launcherOptions.WsPort,
                        WssPort = launcherOptions.WssPort,
                        WssCertFilePath = launcherOptions.WssCertFilePath,
                        DataBaseUrl = launcherOptions.DataBaseUrl,
                        DataBaseName = launcherOptions.DataBaseName,
                        MinModuleId = launcherOptions.MinModuleId,
                        MaxModuleId = launcherOptions.MaxModuleId,
                        TagName = launcherOptions.TagName,
                    };
                }

                Launcher(args, startKv, appSetting);
            }
        }
        else
        {
            foreach (var keyValuePair in sortedStartUpTypes)
            {
                var isFind = false;

                foreach (var appSetting in appSettings)
                {
                    if (keyValuePair.Value.ServerType == appSetting.ServerType)
                    {
                        Launcher(args, keyValuePair, appSetting);
                        isFind = true;
                        break;
                    }
                }

                if (isFind == false)
                {
                    LogHelper.WarnConsole($"没有找到对应的服务器类型的启动配置,将以默认配置启动=>{keyValuePair.Value.ServerType}");
                    Launcher(args, keyValuePair);
                }
            }
        }

        LogHelper.InfoConsole("----------------------------启动服务器结束啦------------------------------");
        ApplicationPerformanceMonitorStart(serverType);
        ConsoleHelper.ConsoleLogo();

        await Task.WhenAll(AppStartUpTasks);
    }

    private static void LauncherOptionsValidate(LauncherOptions options)
    {
        if (!options.ServerType.IsNullOrEmpty() && Enum.TryParse(options.ServerType, out ServerType serverTypeValue))
        {
            // options.CheckAPMPort();

            options.CheckServerId();

            switch (serverTypeValue)
            {
                case ServerType.Log:
                    break;
                case ServerType.DataBase:
                {
                    options.CheckDataBaseUrl();

                    options.CheckDataBaseName();

                    options.CheckOuterIp();

                    options.CheckOuterPort();
                }
                    break;
                case ServerType.Cache:
                    break;
                case ServerType.Gateway:
                {
                    options.CheckOuterIp();
                    options.CheckOuterPort();
                }
                    break;
                case ServerType.Account:
                    break;
                case ServerType.Router:
                {
                    options.CheckOuterIp();
                    options.CheckOuterPort();
                    options.CheckWsPort();
                    options.CheckDiscoveryCenterIp();
                    options.CheckDiscoveryCenterPort();
                }
                    break;
                case ServerType.DiscoveryCenter:
                {
                    options.CheckOuterIp();
                    options.CheckOuterPort();
                }
                    break;
                case ServerType.Backup:
                    break;
                case ServerType.Login:
                    break;
                case ServerType.Game:
                {
                    // options.CheckMinModuleId();
                    // options.CheckMaxModuleId();
                    // options.CheckOuterIp();
                    // options.CheckOuterPort();
                    // options.CheckDiscoveryCenterIp();
                    // options.CheckDiscoveryCenterPort();
                    options.CheckDataBaseUrl();
                    options.CheckDataBaseName();
                }
                    break;
                case ServerType.Recharge:
                    break;
                case ServerType.Logic:
                    break;
                case ServerType.Chat:
                    break;
                case ServerType.Mail:
                    break;
                case ServerType.Guild:
                    break;
                case ServerType.Room:
                    break;
                case ServerType.All:
                    break;
            }
        }
    }

    private static void Launcher(string[] args, KeyValuePair<Type, StartUpTagAttribute> keyValuePair, AppSetting appSetting = null)
    {
        var task = Start(args, keyValuePair.Key, keyValuePair.Value.ServerType, appSetting, out var startUp);
        AppStartUps.Add(startUp);
        AppStartUpTasks.Add(task);
    }

    private static void ApplicationPerformanceMonitorStart(string serverType)
    {
        if (serverType != null && Enum.TryParse(serverType, out ServerType serverTypeValue))
        {
            foreach (var appStartUp in AppStartUps)
            {
                if (appStartUp.ServerType == serverTypeValue && appStartUp.Setting.APMPort is > 0 and < ushort.MaxValue)
                {
                    if (!Net.PortIsAvailable(appStartUp.Setting.APMPort))
                    {
                        LogHelper.ErrorConsole("APM端口已被占用!=>" + appStartUp.Setting.APMPort);
                        return;
                    }

                    MetricsHelper.Start(appStartUp.Setting.APMPort);
                    return;
                }
            }
        }

        MetricsHelper.Start();
    }


    private static Task Start(string[] args, Type appStartUpType, ServerType serverType, AppSetting setting, out IAppStartUp startUp)
    {
        startUp = (IAppStartUp)Activator.CreateInstance(appStartUpType);
        if (startUp != null)
        {
            var isSuccess = startUp.Init(serverType, setting, args);
            if (isSuccess)
            {
                LogHelper.InfoConsole($"----------------------------START-----{serverType}------------------------------");
                LogHelper.InfoConsole("----------------------------配置信息----------------------------------------------");
                LogHelper.InfoConsole($"{startUp.Setting.ToFormatString()}");
                LogHelper.InfoConsole("--------------------------------------------------------------------------------");
                var task = AppEnter.Entry(startUp);
                LogHelper.InfoConsole($"-----------------------------END------{serverType}------------------------------");
                return task;
            }
        }

        return Task.CompletedTask;
    }
}