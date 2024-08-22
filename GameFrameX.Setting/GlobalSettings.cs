using Newtonsoft.Json;

namespace GameFrameX.Setting;

/// <summary>
/// 全局设置
/// </summary>
public static class GlobalSettings
{
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
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="Exception"></exception>
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
    /// 
    /// </summary>
    /// <returns></returns>
    public static List<AppSetting> GetSettings()
    {
        return Settings.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverType"></param>
    /// <returns></returns>
    public static List<AppSetting> GetSettings(ServerType serverType)
    {
        var result = new List<AppSetting>();
        foreach (var setting in Settings)
        {
            if ((setting.ServerType &= serverType) != 0)
            {
                result.Add(setting);
            }
        }

        return result;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serverType"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static AppSetting GetSetting<T>(ServerType serverType)
    {
        foreach (var setting in Settings)
        {
            if ((setting.ServerType &= serverType) != 0)
            {
                return setting;
            }
        }

        return null;
    }
}