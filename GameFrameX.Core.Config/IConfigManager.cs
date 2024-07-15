namespace GameFrameX.Core.Config;

/// <summary>
/// 全局配置管理器接口。
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// 获取全局配置项数量。
    /// </summary>
    int Count { get; }

    /// <summary>
    /// 检查是否存在指定全局配置项。
    /// </summary>
    /// <param name="configName">要检查全局配置项的名称。</param>
    /// <returns>指定的全局配置项是否存在。</returns>
    bool HasConfig(string configName);

    /// <summary>
    /// 增加指定全局配置项。
    /// </summary>
    /// <param name="configName">要增加全局配置项的名称。</param>
    /// <param name="configValue">全局配置项的值。</param>
    /// <returns>是否增加全局配置项成功。</returns>
    void AddConfig(string configName, IDataTable configValue);

    /// <summary>
    /// 移除指定全局配置项。
    /// </summary>
    /// <param name="configName">要移除全局配置项的名称。</param>
    /// <returns>是否移除全局配置项成功。</returns>
    bool RemoveConfig(string configName);

    /// <summary>
    /// 获取指定全局配置项。
    /// </summary>
    /// <param name="configName">要获取全局配置项的名称。</param>
    /// <returns>要获取全局配置项的全局配置项。</returns>
    IDataTable GetConfig(string configName);

    /// <summary>
    /// 清空所有全局配置项。
    /// </summary>
    void RemoveAllConfigs();
}