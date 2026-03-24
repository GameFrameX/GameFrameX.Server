using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Localization.Providers;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Options.Attributes;
using GameFrameX.Localization;
using GameFrameX.StartUp.Options;

namespace GameFrameX.AppHost;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        LocalizationService.Instance.RegisterProvider(new AssemblyResourceProvider(typeof(Keys).Assembly));
        var builder = DistributedApplication.CreateBuilder(args);
        LogOptions.Default.LogType = "AppHost";

        LauncherOptions launcherOptions;
        try
        {
            launcherOptions = Foundation.Options.OptionsBuilder.CreateWithDebug<LauncherOptions>(args);
        }
        catch (Exception e)
        {
            LogHelper.Error(e.Message);
            throw;
        }

        var serverType = launcherOptions.ServerType;
        if (!serverType.IsNullOrEmpty())
        {
            LogHelper.Info(LocalizationService.GetString(Keys.StartUp.LaunchServerType, serverType));
        }

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
                    LogHelper.Warning(LocalizationService.GetString(Keys.StartUp.GrafanaLokiLabelExists, property.Name));
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

        LogHandler.Create(LogOptions.Default);

        var mongoDb = builder.AddMongoDB("mongo").WithLifetime(ContainerLifetime.Persistent);
        mongoDb.AddDatabase("mongodb");
        var gameService = builder
                          .AddProject<Projects.GameFrameX_Launcher>("game-service")
                          .WithArgs($"--{nameof(LauncherOptions.ServerType)}=Game")
                          .WithReference(mongoDb)
                          .WaitFor(mongoDb);

        var socialService = builder
                            .AddProject<Projects.GameFrameX_Launcher>("social-service")
                            .WithArgs($"--{nameof(LauncherOptions.ServerType)}=Social")
                            .WithReference(mongoDb)
                            .WaitFor(mongoDb);

        gameService.WaitFor(socialService);
        await builder.Build().RunAsync();
    }
}
