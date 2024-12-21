namespace GameFrameX.Core.Config;

/// <summary>
/// 数据表基础接口
/// </summary>
public interface IDataTable
{
    /// <summary>
    /// 获取数据表中对象的数量。
    /// </summary>
    /// <returns>数据表中对象的数量。</returns>
    int Count { get; }

    /// <summary>
    /// 异步加载数据表。
    /// </summary>
    /// <returns>一个任务表示异步操作。</returns>
    Task LoadAsync();
}

/// <summary>
/// 泛型数据表基础接口。
/// </summary>
/// <typeparam name="T">数据表中对象的类型。</typeparam>
public interface IDataTable<out T> : IDataTable
{
    /// <summary>
    /// 根据ID获取对象。
    /// </summary>
    /// <param name="id">表唯一主键ID。</param>
    /// <returns>指定ID的对象。</returns>
    T this[int id] { get; }

    /// <summary>
    /// 根据ID获取对象。
    /// </summary>
    /// <param name="id">表唯一主键ID。</param>
    /// <returns>指定ID的对象。</returns>
    T this[string id] { get; }

    /// <summary>
    /// 获取数据表中第一个对象。
    /// </summary>
    /// <returns>数据表中的第一个对象。</returns>
    T FirstOrDefault { get; }

    /// <summary>
    /// 获取数据表中最后一个对象。
    /// </summary>
    /// <returns>数据表中的最后一个对象。</returns>
    T LastOrDefault { get; }

    /// <summary>
    /// 获取数据表中所有对象。
    /// </summary>
    /// <returns>数据表中的所有对象数组。</returns>
    T[] All { get; }

    /// <summary>
    /// 根据ID获取对象。
    /// </summary>
    /// <param name="id">表唯一主键ID。</param>
    /// <returns>指定ID的对象。</returns>
    T Get(int id);

    /// <summary>
    /// 根据ID获取对象。
    /// </summary>
    /// <param name="id">表唯一主键ID。</param>
    /// <returns>指定ID的对象。</returns>
    T Get(string id);

    /// <summary>
    /// 获取数据表中所有对象。
    /// </summary>
    /// <returns>数据表中的所有对象数组。</returns>
    T[] ToArray();

    /// <summary>
    /// 根据条件查找，返回第一个满足条件的对象。
    /// </summary>
    /// <param name="func">查询条件表达式。</param>
    /// <returns>第一个满足条件的对象。</returns>
    T Find(Func<T, bool> func);

    /// <summary>
    /// 根据条件查找，返回所有满足条件的对象。
    /// </summary>
    /// <param name="func">查询条件表达式。</param>
    /// <returns>所有满足条件的对象数组。</returns>
    T[] FindList(Func<T, bool> func);
}