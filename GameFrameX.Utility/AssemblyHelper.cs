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

using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Foundation.Extensions;

namespace GameFrameX.Utility;

/// <summary>
/// Assembly helper utility class
/// Provides assembly and type loading, caching, and querying functionality with thread-safe operations
///  Main Features:
/// 1. Assembly Management: Get all loaded assemblies in the current application domain
/// 2. Type Discovery: Find and cache type information from assemblies
/// 3. Inheritance Analysis: Find implementations, derived classes, and subclasses of specified types
/// 4. Instance Creation: Automatically instantiate types that meet specified conditions
/// 5. Attribute Filtering: Filter types based on custom attribute markers
/// 
/// Performance Features:
/// - Uses ConcurrentDictionary for thread-safe type caching
/// - Implements lazy loading with Lazy{T} to avoid repeated type scanning
/// - Built-in exception handling ensures partial assembly loading failures don't affect overall functionality
/// 
/// Use Cases:
/// - Type discovery and loading for plugin systems
/// - Type registration and resolution for IoC containers
/// - Module scanning for component-based architectures
/// - Performance optimization for reflection operations
/// 
/// </summary>
/// <remarks>
/// 程序集辅助工具类
/// 提供程序集和类型的加载、缓存和查询功能，支持线程安全操作
/// 
/// 主要功能：
/// 1. 程序集管理：获取当前应用程序域中的所有已加载程序集
/// 2. 类型发现：查找和缓存程序集中的类型信息
/// 3. 继承关系分析：查找指定类型的实现类、派生类和子类
/// 4. 实例创建：自动实例化符合条件的类型
/// 5. 特性过滤：基于自定义特性标记进行类型筛选
/// 
/// 性能特性：
/// - 使用 ConcurrentDictionary 提供线程安全的类型缓存
/// - 采用 Lazy&lt;T&gt; 实现延迟加载，避免重复的类型扫描
/// - 内置异常处理，确保部分程序集加载失败不影响整体功能
/// 
/// 适用场景：
/// - 插件系统的类型发现和加载
/// - IoC 容器的类型注册和解析
/// - 组件化架构的模块扫描
/// - 反射操作的性能优化
/// </remarks>
public static class AssemblyHelper
{
    /// <summary>
    /// Type cache dictionary using type full name as key, thread-safe
    /// Uses StringComparer.Ordinal for optimal string comparison performance
    /// </summary>
    /// <remarks>
    /// 类型缓存字典，使用类型全名作为键，线程安全
    /// 使用 StringComparer.Ordinal 提供最佳性能的字符串比较
    /// </remarks>
    private static readonly ConcurrentDictionary<string, Type> CachedTypes = new(StringComparer.Ordinal);

    /// <summary>
    /// Lazy-loaded type array ensuring types are loaded only once
    /// Uses Lazy{T} to guarantee thread-safe single initialization
    /// </summary>
    /// <remarks>
    /// 延迟加载的类型数组，确保类型只被加载一次
    /// 使用 Lazy&lt;T&gt; 保证线程安全的单次初始化
    /// </remarks>
    private static readonly Lazy<Type[]> LazyTypes = new(() => LoadAllTypes());

    /// <summary>
    /// Assembly access lock for synchronizing assembly retrieval operations
    /// </summary>
    /// <remarks>
    /// 程序集访问锁，用于同步程序集获取操作
    /// </remarks>
    private static readonly object AssemblyLock = new();

    /// <summary>
    /// Load all types with exception handling
    /// Iterates through all loaded assemblies and extracts type information
    /// </summary>
    /// <remarks>
    /// 加载所有类型，包含异常处理
    /// 遍历所有已加载的程序集，提取其中的类型信息
    /// </remarks>
    /// <returns>Array of all loaded types / 所有已加载的类型数组</returns>
    private static Type[] LoadAllTypes()
    {
        var results = new List<Type>();
        var assemblies = GetCurrentAssemblies();

        // 遍历每个程序集，提取类型信息
        foreach (var assembly in assemblies)
        {
            try
            {
                // 获取程序集中的所有类型
                results.AddRange(assembly.GetTypes());
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 处理部分类型加载失败的情况
                // 只添加成功加载的类型，忽略加载失败的类型
                // 这种情况通常发生在程序集依赖缺失时
                results.AddRange(ex.Types.Where(t => t != null));
            }
            catch (Exception)
            {
                // 忽略其他异常（如安全异常、文件访问异常等）
                // 继续处理下一个程序集，确保整个加载过程不会中断
                continue;
            }
        }

        return results.ToArray();
    }

    /// <summary>
    /// Get currently loaded assemblies
    /// Uses lock to ensure safe access to assembly list in multi-threaded environment
    /// </summary>
    /// <remarks>
    /// 获取当前已加载的程序集
    /// 使用锁确保在多线程环境下安全访问程序集列表
    /// </remarks>
    /// <returns>Array of currently loaded assemblies / 当前已加载的程序集数组</returns>
    private static Assembly[] GetCurrentAssemblies()
    {
        // 使用锁保护程序集访问，防止在程序集动态加载时出现竞态条件
        lock (AssemblyLock)
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    /// <summary>
    /// Get loaded assemblies
    /// Returns all loaded assemblies in the current application domain, including dynamically loaded assemblies
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集
    /// 返回当前应用程序域中所有已加载的程序集，包括动态加载的程序集
    /// </remarks>
    /// <returns>Array of loaded assemblies, including system and user assemblies / 已加载的程序集数组，包含系统程序集和用户程序集</returns>
    public static Assembly[] GetAssemblies()
    {
        return GetCurrentAssemblies();
    }

    /// <summary>
    /// Get all types from loaded assemblies
    /// Uses lazy loading and caching mechanism to ensure type information is loaded only once, improving performance
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的所有类型
    /// 使用延迟加载和缓存机制，确保类型信息只被加载一次，提高性能
    /// </remarks>
    /// <returns>Array of all types from loaded assemblies, including classes, interfaces, enums, delegates, etc. / 已加载的程序集中的所有类型数组，包括类、接口、枚举、委托等</returns>
    public static Type[] GetTypes()
    {
        return LazyTypes.Value;
    }

    /// <summary>
    /// Get all types from loaded assemblies and add results to the specified list
    /// This method clears the target list and then adds all type information
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的所有类型，并将结果添加到指定的列表中
    /// 此方法会清空目标列表，然后添加所有类型信息
    /// </remarks>
    /// <param name="results">List for storing results, cannot be null / 用于存储结果的列表，不能为 null</param>
    /// <exception cref="ArgumentNullException">Thrown when results parameter is null / 当 results 参数为 null 时抛出</exception>
    public static void GetTypes(List<Type> results)
    {
        ArgumentNullException.ThrowIfNull(results, nameof(results));

        results.Clear();
        results.AddRange(LazyTypes.Value);
    }

    /// <summary>
    /// Get specified type from loaded assemblies
    /// Supports fully qualified names and simple type names, uses caching mechanism to improve lookup performance
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的指定类型
    /// 支持完全限定名和简单类型名，使用缓存机制提高查找性能
    /// </remarks>
    /// <param name="typeName">Type name to get, supports fully qualified names (e.g., "System.String") / 要获取的类型名，支持完全限定名（如 "System.String"）</param>
    /// <returns>Specified type from loaded assemblies, returns null if not found / 已加载的程序集中的指定类型，如果未找到则返回 null</returns>
    /// <exception cref="ArgumentException">Thrown when typeName parameter is null or empty string / 当 typeName 参数为 null 或空字符串时抛出</exception>
    public static Type GetType(string typeName)
    {
        ArgumentException.ThrowIfNullOrEmpty(typeName, nameof(typeName));

        return CachedTypes.GetOrAdd(typeName, name =>
        {
            // 首先尝试直接获取类型（适用于系统类型和当前程序集中的类型）
            var type = Type.GetType(name);
            if (type != null)
            {
                return type;
            }

            // 如果直接获取失败，遍历所有程序集查找类型
            // 这种方式适用于跨程序集的类型查找
            var assemblies = GetCurrentAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    // 使用程序集限定名称查找类型
                    type = Type.GetType($"{name}, {assembly.FullName}");
                    if (type != null) return type;
                }
                catch (Exception)
                {
                    // 忽略异常（如类型不存在、安全异常等）
                    // 继续查找下一个程序集
                    continue;
                }
            }

            // 如果在所有程序集中都找不到该类型，返回 null
            return null;
        });
    }

    /// <summary>
    /// Get instantiated list of subclasses of specified type from loaded assemblies
    /// Automatically creates instances of all implementing types, only instantiates types with parameterless constructors
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的指定类型的子类实例化列表
    /// 自动创建所有实现类型的实例，只实例化具有无参构造函数的类型
    /// </remarks>
    /// <typeparam name="T">Specified base type or interface type / 指定的基类型或接口类型</typeparam>
    /// <returns>Instantiated list of subclasses of specified type, excluding types that cannot be instantiated / 指定类型的子类实例化列表，已排除无法实例化的类型</returns>
    public static List<T> GetRuntimeImplementTypeNamesInstance<T>()
    {
        var types = GetRuntimeImplementTypeNames(typeof(T));

        var results = new List<T>(types.Count);
        foreach (var type in types)
        {
            try
            {
                // 检查类型是否有无参构造函数
                // 只有具有无参构造函数的类型才能被实例化
                if (type.GetConstructor(Type.EmptyTypes) != null)
                {
                    // 创建类型实例并转换为目标类型
                    var instance = (T)Activator.CreateInstance(type);
                    results.Add(instance);
                }
            }
            catch (Exception)
            {
                // 忽略无法实例化的类型（如抽象类、接口、泛型定义等）
                // 继续处理下一个类型，确保整个过程不会中断
                continue;
            }
        }

        return results;
    }

    /// <summary>
    /// Get subclass list of specified type from loaded assemblies
    /// Generic version providing type-safe calling method
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的指定类型的子类列表
    /// 泛型版本，提供类型安全的调用方式
    /// </remarks>
    /// <typeparam name="T">Specified base type or interface type / 指定的基类型或接口类型</typeparam>
    /// <returns>Subclass list of specified type, including all implementing classes and derived classes / 指定类型的子类列表，包括所有实现类和派生类</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T>()
    {
        return GetRuntimeImplementTypeNames(typeof(T));
    }

    /// <summary>
    /// Get subclass list of specified type from loaded assemblies and filter types with specified attributes
    /// Combines type inheritance and attribute marking for dual filtering, commonly used in plugin systems and component discovery
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的指定类型的子类列表，并过滤出具有指定特性的类型
    /// 结合类型继承和特性标记进行双重过滤，常用于插件系统和组件发现
    /// </remarks>
    /// <typeparam name="T">Specified base type or interface type / 指定的基类型或接口类型</typeparam>
    /// <typeparam name="TAttribute">Specified custom attribute marker type / 指定的自定义特性标记类型</typeparam>
    /// <returns>Subclass list of specified type with specified attribute markers / 指定类型的子类列表，且这些类型具有指定的特性标记</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T, TAttribute>() where TAttribute : Attribute
    {
        var types = GetRuntimeImplementTypeNames(typeof(T));

        // 过滤出具有指定特性标记的类型
        return types.Where(t => t.GetCustomAttribute<TAttribute>() != null).ToList();
    }

    /// <summary>
    /// Get subclass list of specified type from loaded assemblies and return their full names
    /// Returns fully qualified names of types, convenient for serialization, configuration, and logging
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的指定类型的子类列表，并返回它们的全名
    /// 返回类型的完全限定名称，便于序列化、配置和日志记录
    /// </remarks>
    /// <param name="type">Specified base type or interface type / 指定的基类型或接口类型</param>
    /// <returns>Collection of full name strings of subclass list of specified type / 指定类型的子类列表的全名字符串集合</returns>
    public static List<string> GetRuntimeTypeNames(Type type)
    {
        var results = new List<string>();
        // 获取所有实现指定类型的子类
        var types = GetRuntimeImplementTypeNames(type);

        // 将类型转换为完全限定名称字符串
        foreach (var t in types)
        {
            results.Add(t.FullName);
        }

        return results;
    }

    /// <summary>
    /// Get subclass list of specified type from loaded assemblies
    /// Core implementation method supporting multiple matching modes including interface implementation, class inheritance, and type assignment
    /// </summary>
    /// <remarks>
    /// 获取已加载的程序集中的指定类型的子类列表
    /// 核心实现方法，支持接口实现、类继承和类型分配的多种匹配模式
    /// </remarks>
    /// <param name="type">Specified base type or interface type, cannot be null / 指定的基类型或接口类型，不能为 null</param>
    /// <returns>Subclass list of specified type, including implementing classes, derived classes, and assignable types / 指定类型的子类列表，包括实现类、派生类和可分配类型</returns>
    /// <exception cref="ArgumentNullException">Thrown when type parameter is null / 当 type 参数为 null 时抛出</exception>
    public static List<Type> GetRuntimeImplementTypeNames(Type type)
    {
        ArgumentNullException.ThrowIfNull(type, nameof(type));

        var types = GetTypes();
        var results = new List<Type>();

        // 遍历所有类型，查找符合条件的实现类型
        foreach (var t in types)
        {
            // 跳过抽象类型和非类/值类型（如接口、指针类型等）
            // 这些类型无法被实例化或不适合作为实现类型
            if (t.IsAbstract || (!t.IsClass && !t.IsValueType))
            {
                continue;
            }

            // 根据目标类型的不同特性，使用不同的匹配策略
            bool isMatch;
            if (type.IsInterface)
            {
                // 对于接口类型，检查是否实现了该接口
                isMatch = t.IsImplWithInterface(type);
            }
            else if (type.IsClass)
            {
                // 对于类类型，检查是否为其子类
                isMatch = t.IsSubclassOf(type);
            }
            else
            {
                // 对于其他类型（如值类型、委托等），检查是否可分配
                // 排除类型本身，只查找派生类型
                isMatch = type.IsAssignableFrom(t) && t != type;
            }

            // 如果类型匹配，添加到结果列表
            if (isMatch)
            {
                results.Add(t);
            }
        }

        return results;
    }
}