// ========================================================
// 描述：Mapper.cs
// 作者：Bambomtan 
// 创建时间：2024-12-31 16:03:41 星期二 
// Email：837659628@qq.com
// 版 本：1.0
// ========================================================

namespace FastMapper;
public static partial class Mapper  // 注意这里改成 partial class
{
    // 存储编译时生成的所有映射方法的委托
    private static readonly Dictionary<(Type, Type), Delegate> _mappers = new();

    static Mapper()
    {
        // 在静态构造函数中注册所有生成的映射器
        RegisterGeneratedMappers();
    }

    // 声明为 partial 方法，让 Source Generator 来实现
    static partial void RegisterGeneratedMappers();

    public static TDestination Map<TSource, TDestination>(TSource source)
    {
        var key = (typeof(TSource), typeof(TDestination));
        if (!_mappers.TryGetValue(key, out var mapper))
        {
            throw new InvalidOperationException(
                $"No mapper registered for {typeof(TSource)} -> {typeof(TDestination)}");
        }

        return ((Func<TSource, TDestination>)mapper)(source);
    }
}