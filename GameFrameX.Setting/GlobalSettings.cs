using Newtonsoft.Json;

namespace GameFrameX.Setting;

/// <summary>
/// 全局设置
/// </summary>
public static class GlobalSettings
{
    /// <summary>
    /// 存储应用设置的列表
    /// </summary>
    private static readonly List<AppSetting> Settings = new List<AppSetting>(16);

    /// <summary>
    /// 是否运行中
    /// </summary>
    public static bool IsAppRunning { get; set; }

    /// <summary>
    /// 启动时间
    /// </summary>
    public static DateTime LaunchTime { get; set; }

    /// <summary>
    /// 是否是调试模式
    /// </summary>
    public static bool IsDebug { get; set; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public static int ServerId { get; set; }

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
        var settings = JsonConvert.DeserializeObject<List<AppSetting>>(configJson) ?? throw new InvalidOperationException();

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