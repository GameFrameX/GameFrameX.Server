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
            var singleModeServerType = ResolveServerType(startupOptions);
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

    private static string ResolveServerType(StartupOptions startupOptions)
    {
        if (startupOptions.ServerType.IsNotNullOrWhiteSpace())
        {
            return startupOptions.ServerType;
        }

        return "Game";
    }

    private static List<string> ResolveMultiServerTypes(StartupOptions startupOptions)
    {
        if (startupOptions.TopologyProfile.IsNullOrWhiteSpace())
        {
            return new List<string> { "Game", "Social", };
        }

        var profileText = startupOptions.TopologyProfile.Trim();
        if (profileText.Contains(',', StringComparison.Ordinal))
        {
            var serverTypes = profileText.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                                         .Distinct(StringComparer.OrdinalIgnoreCase)
                                         .ToList();
            return serverTypes.Count > 0 ? serverTypes : new List<string> { "Game", "Social", };
        }

        return profileText.ToLowerInvariant() switch
        {
            "default" => new List<string> { "Game", "Social", },
            "game" => new List<string> { "Game", },
            "social" => new List<string> { "Social", },
            _ => new List<string> { "Game", "Social", },
        };
    }

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
                optionName.Equals(nameof(StartupOptions.TopologyProfile), StringComparison.OrdinalIgnoreCase) ||
                optionName.Equals(nameof(StartupOptions.ServerType), StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            launcherArgs.Add(arg);
        }

        launcherArgs.Add($"--{nameof(StartupOptions.ServerType)}={serverType}");
        return launcherArgs;
    }

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
