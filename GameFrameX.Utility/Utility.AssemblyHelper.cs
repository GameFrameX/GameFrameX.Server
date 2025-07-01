using System.Collections.Concurrent;
using System.Reflection;
using GameFrameX.Utility.Extensions;

namespace GameFrameX.Utility;

/// <summary>
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
/// </summary>
public static class AssemblyHelper
{
    /// <summary>
    /// 类型缓存字典，使用类型全名作为键，线程安全
    /// 使用 StringComparer.Ordinal 提供最佳性能的字符串比较
    /// </summary>
    private static readonly ConcurrentDictionary<string, Type> CachedTypes = new(StringComparer.Ordinal);
    
    /// <summary>
    /// 延迟加载的类型数组，确保类型只被加载一次
    /// 使用 Lazy&lt;T&gt; 保证线程安全的单次初始化
    /// </summary>
    private static readonly Lazy<Type[]> LazyTypes = new(() => LoadAllTypes());
    
    /// <summary>
    /// 程序集访问锁，用于同步程序集获取操作
    /// </summary>
    private static readonly object AssemblyLock = new();

    /// <summary>
    /// 加载所有类型，包含异常处理
    /// 遍历所有已加载的程序集，提取其中的类型信息
    /// </summary>
    /// <returns>所有已加载的类型数组</returns>
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
    /// 获取当前已加载的程序集
    /// 使用锁确保在多线程环境下安全访问程序集列表
    /// </summary>
    /// <returns>当前已加载的程序集数组</returns>
    private static Assembly[] GetCurrentAssemblies()
    {
        // 使用锁保护程序集访问，防止在程序集动态加载时出现竞态条件
        lock (AssemblyLock)
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
    }

    /// <summary>
    /// 获取已加载的程序集
    /// 返回当前应用程序域中所有已加载的程序集，包括动态加载的程序集
    /// </summary>
    /// <returns>已加载的程序集数组，包含系统程序集和用户程序集</returns>
    public static Assembly[] GetAssemblies()
    {
        return GetCurrentAssemblies();
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型
    /// 使用延迟加载和缓存机制，确保类型信息只被加载一次，提高性能
    /// </summary>
    /// <returns>已加载的程序集中的所有类型数组，包括类、接口、枚举、委托等</returns>
    public static Type[] GetTypes()
    {
        return LazyTypes.Value;
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型，并将结果添加到指定的列表中
    /// 此方法会清空目标列表，然后添加所有类型信息
    /// </summary>
    /// <param name="results">用于存储结果的列表，不能为 null</param>
    /// <exception cref="ArgumentNullException">当 results 参数为 null 时抛出</exception>
    public static void GetTypes(List<Type> results)
    {
        ArgumentNullException.ThrowIfNull(results, nameof(results));

        results.Clear();
        results.AddRange(LazyTypes.Value);
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型
    /// 支持完全限定名和简单类型名，使用缓存机制提高查找性能
    /// </summary>
    /// <param name="typeName">要获取的类型名，支持完全限定名（如 "System.String"）</param>
    /// <returns>已加载的程序集中的指定类型，如果未找到则返回 null</returns>
    /// <exception cref="ArgumentException">当 typeName 参数为 null 或空字符串时抛出</exception>
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
    /// 获取已加载的程序集中的指定类型的子类实例化列表
    /// 自动创建所有实现类型的实例，只实例化具有无参构造函数的类型
    /// </summary>
    /// <typeparam name="T">指定的基类型或接口类型</typeparam>
    /// <returns>指定类型的子类实例化列表，已排除无法实例化的类型</returns>
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
    /// 获取已加载的程序集中的指定类型的子类列表
    /// 泛型版本，提供类型安全的调用方式
    /// </summary>
    /// <typeparam name="T">指定的基类型或接口类型</typeparam>
    /// <returns>指定类型的子类列表，包括所有实现类和派生类</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T>()
    {
        return GetRuntimeImplementTypeNames(typeof(T));
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表，并过滤出具有指定特性的类型
    /// 结合类型继承和特性标记进行双重过滤，常用于插件系统和组件发现
    /// </summary>
    /// <typeparam name="T">指定的基类型或接口类型</typeparam>
    /// <typeparam name="TAttribute">指定的自定义特性标记类型</typeparam>
    /// <returns>指定类型的子类列表，且这些类型具有指定的特性标记</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T, TAttribute>() where TAttribute : Attribute
    {
        var types = GetRuntimeImplementTypeNames(typeof(T));

        // 过滤出具有指定特性标记的类型
        return types.Where(t => t.GetCustomAttribute<TAttribute>() != null).ToList();
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表，并返回它们的全名
    /// 返回类型的完全限定名称，便于序列化、配置和日志记录
    /// </summary>
    /// <param name="type">指定的基类型或接口类型</param>
    /// <returns>指定类型的子类列表的全名字符串集合</returns>
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
    /// 获取已加载的程序集中的指定类型的子类列表
    /// 核心实现方法，支持接口实现、类继承和类型分配的多种匹配模式
    /// </summary>
    /// <param name="type">指定的基类型或接口类型，不能为 null</param>
    /// <returns>指定类型的子类列表，包括实现类、派生类和可分配类型</returns>
    /// <exception cref="ArgumentNullException">当 type 参数为 null 时抛出</exception>
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