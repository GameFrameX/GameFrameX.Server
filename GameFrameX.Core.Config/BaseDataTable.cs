using System.Text.Json;

namespace GameFrameX.Core.Config;

/// <summary>
/// 基础数据表
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseDataTable<T> : IDataTable<T>
{
    /// <summary>
    /// 数据表
    /// </summary>
    protected readonly SortedDictionary<long, T> LongDataMaps = new SortedDictionary<long, T>();

    /// <summary>
    /// 数据表
    /// </summary>
    protected readonly SortedDictionary<string, T> StringDataMaps = new SortedDictionary<string, T>();

    /// <summary>
    /// 数据列表
    /// </summary>
    protected readonly List<T> DataList = new List<T>();

    /// <summary>
    /// 异步加载器
    /// </summary>
    protected readonly System.Func<System.Threading.Tasks.Task<JsonElement>> _loadFunc;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="loadFunc"></param>
    public BaseDataTable(Func<Task<JsonElement>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    /// <summary>
    /// 
    /// </summary>
    public BaseDataTable()
    {
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    /// <returns></returns>
    public abstract Task LoadAsync();

    /// <summary>
    /// 根据ID获取对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T Get(int id)
    {
        LongDataMaps.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 根据ID获取对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T Get(string id)
    {
        StringDataMaps.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 根据ID获取对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T this[int id]
    {
        get { return Get(id); }
    }

    /// <summary>
    /// 根据ID获取对象
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public T this[string id]
    {
        get { return Get(id); }
    }

    /// <summary>
    /// 获取数据表中对象的数量
    /// </summary>
    /// <returns></returns>
    public int Count
    {
        get { return Math.Max(LongDataMaps.Count, StringDataMaps.Count); }
    }

    /// <summary>
    /// 获取数据表中第一个对象
    /// </summary>
    /// <returns></returns>
    public T FirstOrDefault
    {
        get { return DataList.FirstOrDefault(m => m != null); }
    }

    /// <summary>
    /// 获取数据表中最后一个对象
    /// </summary>
    /// <returns></returns>
    public T LastOrDefault
    {
        get { return DataList.LastOrDefault(m => m != null); }
    }

    /// <summary>
    /// 获取数据表中所有对象
    /// </summary>
    /// <returns></returns>
    public T[] All
    {
        get { return DataList.ToArray(); }
    }

    /// <summary>
    /// 获取数据表中所有对象
    /// </summary>
    /// <returns></returns>
    public T[] ToArray()
    {
        return DataList.ToArray();
    }

    /// <summary>
    /// 根据条件查找
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public T Find(Func<T, bool> func)
    {
        return DataList.FirstOrDefault(func);
    }

    /// <summary>
    /// 根据条件查找
    /// </summary>
    /// <param name="func"></param>
    /// <returns></returns>
    public T[] FindList(Func<T, bool> func)
    {
        return DataList.Where(func).ToArray();
    }
}