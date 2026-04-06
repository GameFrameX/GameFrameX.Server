// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.StartUp.Abstractions;
using GameFrameX.Utility;

namespace GameFrameX.StartUp;

/// <summary>
/// 启动类型注册表。
/// </summary>
/// <remarks>
/// Startup type registry.
/// This class is responsible for managing all startup types that implement the IAppStartUp interface and are marked with StartUpTagAttribute.
/// Provides type discovery, registration, sorting, and query functionality.
/// </remarks>
public sealed class StartUpTypeRegistry : Singleton<StartUpTypeRegistry>
{
    /// <summary>
    /// 启动类型字典。
    /// </summary>
    /// <remarks>
    /// Dictionary of startup types.
    /// </remarks>
    private readonly Dictionary<Type, StartUpTagAttribute> _startUpTypes = new();

    /// <summary>
    /// 发现并注册所有启动类型。
    /// </summary>
    /// <remarks>
    /// Discover and register all startup types.
    /// This method scans all loaded assemblies for classes that implement the IAppStartUp interface
    /// and are marked with StartUpTagAttribute, registering them to an internal dictionary for subsequent startup use.
    /// </remarks>
    public void DiscoverAndRegister()
    {
        var types = AssemblyHelper.GetTypes();
        if (types == null)
        {
            return;
        }

        foreach (var type in types)
        {
            if (!type.IsClass || !type.IsImplWithInterface(typeof(IAppStartUp)))
            {
                continue;
            }

            var startUpTag = type.GetCustomAttribute<StartUpTagAttribute>();
            if (startUpTag != null)
            {
                _startUpTypes.Add(type, startUpTag);
            }
        }
    }

    /// <summary>
    /// 按优先级排序获取所有启动类型。
    /// </summary>
    /// <remarks>
    /// Get all startup types sorted by priority.
    /// </remarks>
    /// <returns>按优先级排序的启动类型集合 / Collection of startup types sorted by priority</returns>
    public IEnumerable<KeyValuePair<Type, StartUpTagAttribute>> GetSortedByPriority()
    {
        return _startUpTypes.OrderBy(m => m.Value.Priority);
    }

    /// <summary>
    /// 按服务器类型查找启动类型。
    /// </summary>
    /// <remarks>
    /// Find startup type by server type.
    /// </remarks>
    /// <param name="serverType">服务器类型标识符 / Server type identifier</param>
    /// <returns>匹配的键值对；如果未找到则返回 null / Matching key-value pair, or null if not found</returns>
    public KeyValuePair<Type, StartUpTagAttribute>? FindByServerType(string serverType)
    {
        var result = _startUpTypes.FirstOrDefault(m => m.Value.ServerType == serverType);
        if (result.Value == null)
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// 获取所有启动类型。
    /// </summary>
    /// <remarks>
    /// Get all startup types.
    /// </remarks>
    /// <returns>启动类型的只读字典 / Read-only dictionary of startup types</returns>
    public IReadOnlyDictionary<Type, StartUpTagAttribute> GetAll()
    {
        return _startUpTypes;
    }

    /// <summary>
    /// 清空注册表（主要用于测试）。
    /// </summary>
    /// <remarks>
    /// Clear the registry (mainly for testing).
    /// </remarks>
    public void Clear()
    {
        _startUpTypes.Clear();
    }
}