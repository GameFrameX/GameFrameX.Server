using GameFrameX.Extension;

namespace GameFrameX.Utility;

/// <summary>
/// 程序集辅助器
/// </summary>
public static class AssemblyHelper
{
    private static readonly System.Reflection.Assembly[] Assemblies = null;
    private static readonly Dictionary<string, Type> CachedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);

    static AssemblyHelper()
    {
        Assemblies = AppDomain.CurrentDomain.GetAssemblies();
    }

    /// <summary>
    /// 获取已加载的程序集。
    /// </summary>
    /// <returns>已加载的程序集。</returns>
    public static System.Reflection.Assembly[] GetAssemblies()
    {
        return Assemblies;
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型。
    /// </summary>
    /// <returns>已加载的程序集中的所有类型。</returns>
    public static Type[] GetTypes()
    {
        List<Type> results = new List<Type>();
        foreach (System.Reflection.Assembly assembly in Assemblies)
        {
            results.AddRange(assembly.GetTypes());
        }

        return results.ToArray();
    }

    /// <summary>
    /// 获取已加载的程序集中的所有类型。
    /// </summary>
    /// <param name="results">已加载的程序集中的所有类型。</param>
    public static void GetTypes(List<Type> results)
    {
        if (results == null)
        {
            throw new Exception("Results is invalid.");
        }

        results.Clear();
        foreach (System.Reflection.Assembly assembly in Assemblies)
        {
            results.AddRange(assembly.GetTypes());
        }
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型。
    /// </summary>
    /// <param name="typeName">要获取的类型名。</param>
    /// <returns>已加载的程序集中的指定类型。</returns>
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

        foreach (System.Reflection.Assembly assembly in Assemblies)
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
    /// 获取已加载的程序集中的指定类型的子类列表。
    /// </summary>
    /// <typeparam name="T">指定类型</typeparam>
    /// <returns>指定类型的子类列表。</returns>
    public static List<Type> GetRuntimeImplementTypeNames<T>()
    {
        return GetRuntimeImplementTypeNames(typeof(T));
    }

    /// <summary>
    /// 获取已加载的程序集中的指定类型的子类列表。
    /// </summary>
    /// <param name="type">指定类型</param>
    /// <returns>指定类型的子类列表。</returns>
    public static List<string> GetRuntimeTypeNames(Type type)
    {
        List<string> results = new List<string>();
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
    /// <param name="type">指定类型</param>
    /// <returns>指定类型的子类列表。</returns>
    public static List<Type> GetRuntimeImplementTypeNames(Type type)
    {
        var types = GetTypes();
        List<Type> results = new List<Type>();
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