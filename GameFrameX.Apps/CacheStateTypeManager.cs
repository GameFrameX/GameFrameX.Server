using System.Collections.Concurrent;
using GameFrameX.DBServer.State;
using GameFrameX.Extension;
using GameFrameX.Utility;

namespace GameFrameX.Apps;

public static class CacheStateTypeManager
{
    static readonly ConcurrentDictionary<long, Type> hashMap = new ConcurrentDictionary<long, Type>();

    /// <summary>
    /// 初始化对象实体集的扫描
    /// </summary>
    public static void Init()
    {
        var assembly = typeof(AppsHandler).Assembly;
        BsonClassMapHelper.SetConvention();

        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            var isImplWithInterface = type.IsImplWithInterface(typeof(ICacheState));
            if (isImplWithInterface)
            {
                hashMap.TryAdd(Hash.XXHash.Hash32(type.ToString()), type);
                BsonClassMapHelper.RegisterClass(type);
            }
        }
    }

    /// <summary>
    /// 根据类型ID获取类型
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public static Type? GetType(long id)
    {
        hashMap.TryGetValue(id, out var value);
        return value;
    }
}