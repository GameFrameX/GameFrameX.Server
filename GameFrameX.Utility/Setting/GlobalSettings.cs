using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 全局设置
/// </summary>
public static class GlobalSettings
{
    /// <summary>
    /// 存储应用设置的列表
    /// </summary>
    private static readonly List<AppSetting> Settings = new(16);

    /// <summary>
    /// 获取当前应用程序设置
    /// </summary>
    /// <remarks>
    /// 只能通过SetCurrentSetting方法设置，确保配置的安全性。
    /// 存储当前正在使用的应用程序配置信息。
    /// </remarks>
    public static AppSetting CurrentSetting { get; private set; }

    /// <summary>
    /// 是否运行中
    /// </summary>
    public static bool IsAppRunning { get; set; }

    /// <summary>
    /// 启动时间
    /// </summary>
    public static DateTime LaunchTime { get; set; }

    /// <summary>
    /// 加载启动配置
    /// </summary>
    /// <param name="path">配置文件路径</param>
    /// <exception cref="InvalidOperationException">当配置文件解析失败时抛出</exception>
    /// <exception cref="Exception">当服务器ID不在合法范围内时抛出</exception>
    public static void Load(string path)
    {
        Settings.Clear();

        if (!File.Exists(path))
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.Write("配置文件不存在。可能会导致启动失败==>>>" + path);
            Console.ResetColor();
            Console.WriteLine();
            return;
        }

        var configJson = File.ReadAllText(path);
        var settings = JsonHelper.Deserialize<List<AppSetting>>(configJson) ?? throw new InvalidOperationException();

        foreach (var setting in settings)
        {
            if (setting.ServerId < GlobalConst.MinServerId || setting.ServerId > GlobalConst.MaxServerId)
            {
                throw new Exception($"ServerId不合法{setting.ServerId},需要在[{GlobalConst.MinServerId},{GlobalConst.MaxServerId}]范围之内");
            }

            Settings.Add(setting);
        }
    }

    /// <summary>
    /// 设置当前应用程序设置
    /// </summary>
    /// <param name="setting">要设置的应用程序配置对象</param>
    /// <remarks>
    /// 此方法用于更新全局的当前设置。
    /// 通常在应用程序启动时或需要切换配置时调用。
    /// 该方法具有以下特点:
    /// 1. 只能设置一次，重复设置会抛出异常
    /// 2. 不允许传入null值
    /// 3. 会自动校正SaveDataInterval的值，如果小于5000毫秒则使用默认值
    /// </remarks>
    /// <exception cref="ArgumentNullException">当传入的setting参数为null时抛出此异常</exception>
    public static void SetCurrentSetting(AppSetting setting)
    {
        if (CurrentSetting.IsNotNull())
        {
            LogHelper.WarnConsole("当前设置已经存在，不能再次设置");
            return;
        }

        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        if (setting.SaveDataInterval < 5000)
        {
            setting.SaveDataInterval = GlobalConst.SaveIntervalInMilliSeconds;
        }

        if (setting.HttpUrl.IsNullOrEmptyOrWhiteSpace())
        {
            LogHelper.WarnConsole("HttpUrl为空，使用默认值为: /game/api/");
            setting.HttpUrl = "/game/api/";
        }

        CurrentSetting = setting;
    }

    /// <summary>
    /// 获取所有设置
    /// </summary>
    /// <returns>返回所有设置的列表</returns>
    public static List<AppSetting> GetSettings()
    {
        return Settings.ToList();
    }

    /// <summary>
    /// 根据服务器类型获取设置
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <returns>返回匹配的设置列表</returns>
    public static List<AppSetting> GetSettings(ServerType serverType)
    {
        var result = new List<AppSetting>();
        foreach (var setting in Settings)
        {
            if ((setting.ServerType & serverType) != 0)
            {
                result.Add(setting);
            }
        }

        return result;
    }

    /// <summary>
    /// 根据服务器名称获取特定类型的设置
    /// </summary>
    /// <param name="serverName">服务器名称，用于匹配AppSetting中的ServerName属性</param>
    /// <typeparam name="T">设置类型，用于类型安全检查，确保返回正确的设置类型</typeparam>
    /// <returns>返回匹配的设置，如果没有找到则返回null。返回的设置可以被转换为类型T</returns>
    /// <exception cref="ArgumentNullException">当serverName为null时抛出此异常</exception>
    public static AppSetting GetSettingByServerName<T>(string serverName)
    {
        ArgumentNullException.ThrowIfNull(serverName, nameof(serverName));
        foreach (var setting in Settings)
        {
            if (setting.ServerName == serverName)
            {
                return setting;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据服务器Id获取特定类型的设置
    /// </summary>
    /// <param name="tagName">服务器Id，用于匹配AppSetting中的ServerId属性</param>
    /// <typeparam name="T">设置类型，用于类型安全检查，确保返回正确的设置类型</typeparam>
    /// <returns>返回匹配的设置，如果没有找到则返回null。返回的设置可以被转换为类型T</returns>
    /// <remarks>此方法不会对传入的serverId进行有效性验证，请确保传入的值在有效范围内</remarks>
    public static AppSetting GetSettingByServerId<T>(int tagName)
    {
        foreach (var setting in Settings)
        {
            if (setting.ServerId == tagName)
            {
                return setting;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据服务器标签名称获取特定类型的设置
    /// </summary>
    /// <param name="tagName">服务器标签名称，用于匹配AppSetting中的TagName属性</param>
    /// <typeparam name="T">设置类型，用于类型安全检查，确保返回正确的设置类型</typeparam>
    /// <returns>返回匹配的设置，如果没有找到则返回null。返回的设置可以被转换为类型T</returns>
    /// <exception cref="ArgumentNullException">当tagName为null时抛出此异常</exception>
    public static AppSetting GetSettingByTagName<T>(string tagName)
    {
        ArgumentNullException.ThrowIfNull(tagName, nameof(tagName));
        foreach (var setting in Settings)
        {
            if (setting.TagName == tagName)
            {
                return setting;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据服务器类型获取特定类型的设置
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <typeparam name="T">设置类型</typeparam>
    /// <returns>返回匹配的设置，如果没有找到则返回null</returns>
    public static AppSetting GetSetting<T>(ServerType serverType)
    {
        foreach (var setting in Settings)
        {
            if ((setting.ServerType & serverType) != 0)
            {
                return setting;
            }
        }

        return null;
    }
}