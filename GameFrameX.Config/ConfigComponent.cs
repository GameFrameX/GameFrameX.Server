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

using System.Text.Json;
using GameFrameX.Core.Config;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Config;

public class ConfigComponent
{
    private readonly ConfigManager _configManager;

    private ConfigComponent()
    {
        _configManager = new ConfigManager();
        Tables = new TablesComponent();
    }

    public static ConfigComponent Instance { get; } = new();

    private TablesComponent Tables { get; }

    public async Task LoadConfig()
    {
        Tables.Init(Instance);
        LogHelper.Debug("Load Config Start...");
        Instance.RemoveAllConfigs();
        await Tables.LoadAsync(Loader);
        LogHelper.Debug("Load Config End...");
    }

    private static async Task<ByteBuf> Loader(string file, bool tag)
    {
        var configBytes = await File.ReadAllBytesAsync($"json/{file}.bytes");
        return ByteBuf.Wrap(configBytes);
    }

    private static async Task<JsonElement> Loader(string file)
    {
        var configJson = await File.ReadAllTextAsync($"json/{file}.json");
        var jsonElement = JsonDocument.Parse(configJson).RootElement;
        return jsonElement;
    }

    /// <summary>
    /// 获取指定全局配置项。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetConfig<T>() where T : IDataTable
    {
        if (HasConfig<T>())
        {
            var configName = typeof(T).Name;
            var config = _configManager.GetConfig(configName);
            if (config != null)
            {
                return (T)config;
            }
        }

        return default;
    }

    /// <summary>
    /// 检查是否存在指定全局配置项。
    /// </summary>
    /// <returns>指定的全局配置项是否存在。</returns>
    public bool HasConfig<T>() where T : IDataTable
    {
        var configName = typeof(T).Name;
        return _configManager.HasConfig(configName);
    }

    /// <summary>
    /// 移除指定全局配置项。
    /// </summary>
    /// <returns>是否移除全局配置项成功。</returns>
    public bool RemoveConfig<T>() where T : IDataTable<T>
    {
        var configName = typeof(T).Name;
        return _configManager.RemoveConfig(configName);
    }

    /// <summary>
    /// 清空所有全局配置项。
    /// </summary>
    public void RemoveAllConfigs()
    {
        _configManager.RemoveAllConfigs();
    }

    /// <summary>
    /// 增加
    /// </summary>
    /// <param name="configName"></param>
    /// <param name="dataTable"></param>
    public void Add(string configName, IDataTable dataTable)
    {
        _configManager.AddConfig(configName, dataTable);
    }
}