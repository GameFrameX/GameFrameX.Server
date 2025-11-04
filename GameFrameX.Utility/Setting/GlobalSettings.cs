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

using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Utility.DistributedSystem.Snowflake;

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
        if (path.IsNullOrEmptyOrWhiteSpace())
        {
            return;
        }

        if (!File.Exists(path))
        {
            var fullPath = Path.GetFullPath(path);
            LogHelper.ShowOption("Load Global Settings", fullPath);
            return;
        }

        var configJson = File.ReadAllText(path);
        var settings = JsonHelper.Deserialize<List<AppSetting>>(configJson) ?? throw new InvalidOperationException();

        foreach (var setting in settings)
        {
            setting.ServerId.IsRange(GlobalConst.MinServerId, GlobalConst.MaxServerId);
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
            LogHelper.WarningConsole("The current setting already exists, cannot be set again, and this setting has been ignored.");
            return;
        }

        ArgumentNullException.ThrowIfNull(setting, nameof(setting));
        if (setting.SaveDataInterval < 5000)
        {
            LogHelper.WarningConsole($"SaveDataInterval小于5000毫秒，使用默认值为:{GlobalConst.SaveIntervalInMilliSeconds} 毫秒");
            setting.SaveDataInterval = GlobalConst.SaveIntervalInMilliSeconds;
        }

        if (setting.HttpUrl.IsNullOrEmptyOrWhiteSpace())
        {
            LogHelper.WarningConsole("HttpUrl is empty and uses the default value of : /game/api/");
            setting.HttpUrl = "/game/api/";
        }

        if (setting.NetWorkSendTimeOutSeconds < 1)
        {
            LogHelper.WarningConsole("NetWorkSendTimeOutSeconds is less than 1 second, and the default value is 5 seconds");
            setting.NetWorkSendTimeOutSeconds = 5;
        }

        if (setting.ActorRecycleTime < 1)
        {
            LogHelper.WarningConsole("ActorRecycleTime is less than 1 minute, and the default value is 5 minutes");
            setting.ActorRecycleTime = 5;
        }

        // 创建ID生成器配置，WorkerId设为0
        if (setting.WorkerId > 0)
        {
            SnowFlakeIdHelper.WorkId = setting.WorkerId;
        }

        // 设置数据中心ID
        if (setting.DataCenterId > 0)
        {
            SnowFlakeIdHelper.DataCenterId = setting.DataCenterId;
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
    public static List<AppSetting> GetSettings(string serverType)
    {
        var result = new List<AppSetting>();
        foreach (var setting in Settings)
        {
            if (setting.ServerType == serverType)
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
    public static AppSetting GetSetting<T>(string serverType)
    {
        foreach (var setting in Settings)
        {
            if (setting.ServerType == serverType)
            {
                return setting;
            }
        }

        return null;
    }
}