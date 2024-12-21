using System.Reflection;
using GameFrameX.Extension;

namespace GameFrameX.Utility;

/// <summary>
/// 程序集辅助器
/// </summary>
public static class AssemblyHelper
{
    private static readonly Assembly[] Assemblies;
    private static readonly Dictionary<string, Type> CachedTypes = new(StringComparer.Ordinal);

    static AssemblyHelper()
    {
        Assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    /// <summary>
    /// 获取已加载的程序集。
    /// </summary>
    /// <returns>已加载的程序集数组。</returns>
    public static Assembly[] GetAssemblies()
    {
        return Assemblies;
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型。
    /// </summary>
    /// <returns>已加载的程序集中的所有类型数组。</returns>
    public static Type[] GetTypes()
    {
        var results = new List<Type>();
        foreach (var assembly in Assemblies)
        {
            results.AddRange(assembly.GetTypes());
        }

        return results.ToArray();
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型，并将结果添加到指定的列表中。
    /// </summary>
    /// <param name="results">用于存储结果的列表。</param>
    public static void GetTypes(List<Type> results)
    {
        if (results == null)
        {
            throw new Exception("Results is invalid.");
        }

        results.Clear();
        foreach (var assembly in Assemblies)
        {
            results.AddRange(assembly.GetTypes());
        }
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型。
    /// </summary>
    /// <param name="typeName">要获取的类型名。</param>
    /// <returns>已加载的程序集中的指定类型，如果未找到则返回 null。</returns>
    public static Type GetType(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            throw new Exception("Type name is invalid.");
        }

        if (CachedTypes.TryGetValue(typeName, out var type))
        {
            return type;
        }

        type = Type.GetType(typeName);
        if (type != null)
        {
            CachedTypes.Add(typeName, type);
            return type;
        }

        foreach (var assembly in Assemblies)
        {
            type = Type.GetType($"{typeName}, {assembly.FullName}");
            if (type != null)
            {
                CachedTypes.Add(typeName, type);
                return type;
            }
        }

        return null;
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类实例化列表。
    /// </summary>
    /// <typeparam name="T">指定类型。</typeparam>
    /// <returns>指定类型的子类实例化列表。</returns>
    public static List<T> GetRuntimeImplementTypeNamesInstance<T>()
    {
        var types = GetRuntimeImplementTypeNames(typeof(T));

        var results = new List<T>(types.Count);
        foreach (var type in types)
        {
            results.Add((T)Activator.CreateInstance(type));
        }

        return results;
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表。
    /// </summary>
    /// <typeparam name="T">指定类型。</typeparam>
    /// <returns>指定类型的子类列表。</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T>()
    {
        return GetRuntimeImplementTypeNames(typeof(T));
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表，并过滤出具有指定特性的类型。
    /// </summary>
    /// <typeparam name="T">指定类型。</typeparam>
    /// <typeparam name="TAttribute">指定自定义的特性标记。</typeparam>
    /// <returns>指定类型的子类列表，且这些类型具有指定的特性。</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T, TAttribute>() where TAttribute : Attribute
    {
        var types = GetRuntimeImplementTypeNames(typeof(T));

        return types.Where(t => t.GetCustomAttribute<TAttribute>() != null).ToList();
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表，并返回它们的全名。
    /// </summary>
    /// <param name="type">指定类型。</param>
    /// <returns>指定类型的子类列表的全名。</returns>
    public static List<string> GetRuntimeTypeNames(Type type)
    {
        var results = new List<string>();
        var types = GetRuntimeImplementTypeNames(type);
        foreach (var t in types)
        {
            results.Add(t.FullName);
        }

        return results;
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表。
    /// </summary>
    /// <param name="type">指定类型。</param>
    /// <returns>指定类型的子类列表。</returns>
    public static List<Type> GetRuntimeImplementTypeNames(Type type)
    {
        var types = GetTypes();
        var results = new List<Type>();
        foreach (var t in types)
        {
            if (t.IsAbstract || !t.IsClass)
            {
                continue;
            }

            if (t.IsSubclassOf(type) || t.IsImplWithInterface(type))
            {
                results.Add(t);
            }
        }

        return results;
    }
}