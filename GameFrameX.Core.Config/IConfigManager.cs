namespace GameFrameX.Core.Config;

/// <summary>
/// 全局配置管理器接口。
/// </summary>
public interface IConfigManager
{
    /// <summary>
    /// 获取全局配置项数量。
    /// </summary>
    /// <returns>全局配置项的数量。</returns>
    int Count { get; }

    /// <summary>
    /// 检查是否存在指定全局配置项。
    /// </summary>
    /// <param name="configName">要检查的全局配置项的名称。</param>
    /// <returns>如果存在指定的全局配置项，则返回 true；否则返回 false。</returns>
    bool HasConfig(string configName);

    /// <summary>
    /// 增加指定全局配置项。
    /// </summary>
    /// <param name="configName">要增加的全局配置项的名称。</param>
    /// <param name="configValue">全局配置项的值。</param>
    void AddConfig(string configName, IDataTable configValue);

    /// <summary>
    /// 移除指定全局配置项。
    /// </summary>
    /// <param name="configName">要移除的全局配置项的名称。</param>
    /// <returns>如果成功移除指定的全局配置项，则返回 true；否则返回 false。</returns>
    bool RemoveConfig(string configName);

    /// <summary>
    /// 获取指定全局配置项。
    /// </summary>
    /// <param name="configName">要获取的全局配置项的名称。</param>
    /// <returns>指定名称的全局配置项。</returns>
    IDataTable GetConfig(string configName);

    /// <summary>
    /// 清空所有全局配置项。
    /// </summary>
    void RemoveAllConfigs();
}