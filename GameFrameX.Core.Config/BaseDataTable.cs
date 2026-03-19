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

namespace GameFrameX.Core.Config;

/// <summary>
/// 基础数据表，提供数据表的通用操作实现。
/// </summary>
/// <remarks>
/// Base data table, providing common operation implementations for data tables.
/// </remarks>
/// <typeparam name="T">数据表中的数据类型 / The data type in the data table</typeparam>
public abstract class BaseDataTable<T> : IDataTable<T> where T : class
{
    /// <summary>
    /// 长整型键的数据映射字典。
    /// </summary>
    /// <remarks>
    /// Data map dictionary with long integer keys.
    /// </remarks>
    protected readonly SortedDictionary<long, T> LongDataMaps = new SortedDictionary<long, T>();

    /// <summary>
    /// 字符串键的数据映射字典。
    /// </summary>
    /// <remarks>
    /// Data map dictionary with string keys.
    /// </remarks>
    protected readonly SortedDictionary<string, T> StringDataMaps = new SortedDictionary<string, T>();

    /// <summary>
    /// 数据列表。
    /// </summary>
    /// <remarks>
    /// The data list.
    /// </remarks>
    protected readonly List<T> DataList = new List<T>();
    private bool _cacheInitialized;
    private T _firstOrDefaultCache;
    private T _lastOrDefaultCache;
    private bool _countCacheInitialized;
    private int _countCache;

    /// <summary>
    /// 异步加载器。
    /// </summary>
    /// <remarks>
    /// The asynchronous loader.
    /// </remarks>
    protected readonly Func<Task<JsonElement>> _loadFunc;

    /// <summary>
    /// 初始化基础数据表。
    /// </summary>
    /// <remarks>
    /// Initializes the base data table.
    /// </remarks>
    /// <param name="loadFunc">异步加载数据的委托 / The delegate for asynchronous data loading</param>
    public BaseDataTable(Func<Task<JsonElement>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    /// <summary>
    /// 异步加载数据表。
    /// </summary>
    /// <remarks>
    /// Asynchronously loads the data table.
    /// </remarks>
    /// <returns>一个任务表示异步操作 / A task representing the asynchronous operation</returns>
    public abstract Task LoadAsync();

    /// <summary>
    /// 尝试根据整数ID获取对象。
    /// </summary>
    /// <remarks>
    /// Attempts to get an object by its integer ID.
    /// </remarks>
    /// <param name="id">要获取的对象的整数ID / The integer ID of the object to get</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值 / The object if found; otherwise the default value</param>
    /// <returns>如果找到对应ID的对象则返回 <c>true</c>，否则返回 <c>false</c> / <c>true</c> if the object is found; otherwise <c>false</c></returns>
    public bool TryGet(int id, out T value)
    {
        return LongDataMaps.TryGetValue(id, out value);
    }

    /// <summary>
    /// 尝试根据长整数ID获取对象。
    /// </summary>
    /// <remarks>
    /// Attempts to get an object by its long integer ID.
    /// </remarks>
    /// <param name="id">要获取的对象的长整数ID / The long integer ID of the object to get</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值 / The object if found; otherwise the default value</param>
    /// <returns>如果找到对应ID的对象则返回 <c>true</c>，否则返回 <c>false</c> / <c>true</c> if the object is found; otherwise <c>false</c></returns>
    public bool TryGet(long id, out T value)
    {
        return LongDataMaps.TryGetValue(id, out value);
    }

    /// <summary>
    /// 尝试根据字符串ID获取对象。
    /// </summary>
    /// <remarks>
    /// Attempts to get an object by its string ID.
    /// </remarks>
    /// <param name="id">要获取的对象的字符串ID / The string ID of the object to get</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值 / The object if found; otherwise the default value</param>
    /// <returns>如果找到对应ID的对象则返回 <c>true</c>，否则返回 <c>false</c> / <c>true</c> if the object is found; otherwise <c>false</c></returns>
    public bool TryGet(string id, out T value)
    {
        return StringDataMaps.TryGetValue(id, out value);
    }

    /// <summary>
    /// 根据整数主键获取数据表中的对象。
    /// </summary>
    /// <remarks>
    /// Gets an object from the data table by its integer primary key.
    /// </remarks>
    /// <param name="id">要获取的对象的整数主键 / The integer primary key of the object to get</param>
    /// <value>与指定主键关联的数据对象；如果找不到则返回 <c>null</c> / The data object associated with the specified key; <c>null</c> if not found</value>
    public T this[int id]
    {
        get { return TryGet(id, out var value) ? value : null; }
    }

    /// <summary>
    /// 根据长整数主键获取数据表中的对象。
    /// </summary>
    /// <remarks>
    /// Gets an object from the data table by its long integer primary key.
    /// </remarks>
    /// <param name="id">要获取的对象的长整数主键 / The long integer primary key of the object to get</param>
    /// <value>与指定主键关联的数据对象；如果找不到则返回 <c>null</c> / The data object associated with the specified key; <c>null</c> if not found</value>
    public T this[long id]
    {
        get { return TryGet(id, out var value) ? value : null; }
    }

    /// <summary>
    /// 根据字符串键获取数据表中的对象。
    /// </summary>
    /// <remarks>
    /// Gets an object from the data table by its string key.
    /// </remarks>
    /// <param name="id">要获取的对象在数据表中的字符串键 / The string key of the object in the data table</param>
    /// <value>与指定键关联的数据对象；如果找不到则返回 <c>null</c> / The data object associated with the specified key; <c>null</c> if not found</value>
    public T this[string id]
    {
        get { return TryGet(id, out var value) ? value : null; }
    }

    /// <summary>
    /// 获取数据表中对象的数量。
    /// </summary>
    /// <remarks>
    /// Gets the count of objects in the data table.
    /// </remarks>
    /// <value>数据表中对象的数量 / The count of objects in the data table</value>
    public int Count
    {
        get
        {
            EnsureCountCache();
            return _countCache;
        }
    }

    /// <summary>
    /// 获取数据表中的第一个对象，如果数据表为空则返回默认值。
    /// </summary>
    /// <remarks>
    /// Gets the first object in the data table, or the default value if the table is empty.
    /// </remarks>
    /// <value>数据表中的第一个对象，如果数据表为空则返回 <c>null</c> / The first object in the data table; <c>null</c> if the table is empty</value>
    public T FirstOrDefault
    {
        get
        {
            EnsureFirstLastCache();
            return _firstOrDefaultCache;
        }
    }

    /// <summary>
    /// 获取数据表中的最后一个对象，如果数据表为空则返回默认值。
    /// </summary>
    /// <remarks>
    /// Gets the last object in the data table, or the default value if the table is empty.
    /// </remarks>
    /// <value>数据表中的最后一个对象，如果数据表为空则返回 <c>null</c> / The last object in the data table; <c>null</c> if the table is empty</value>
    public T LastOrDefault
    {
        get
        {
            EnsureFirstLastCache();
            return _lastOrDefaultCache;
        }
    }

    /// <summary>
    /// 获取数据表中所有对象的数组副本。
    /// </summary>
    /// <remarks>
    /// Gets an array copy of all objects in the data table.
    /// </remarks>
    /// <value>包含数据表中所有对象的数组 / An array containing all objects in the data table</value>
    public T[] All
    {
        get { return DataList.ToArray(); }
    }

    /// <summary>
    /// 获取数据表中所有对象的数组副本。
    /// </summary>
    /// <remarks>
    /// Gets an array copy of all objects in the data table.
    /// </remarks>
    /// <returns>包含数据表中所有对象的数组 / An array containing all objects in the data table</returns>
    public T[] ToArray()
    {
        return DataList.ToArray();
    }

    /// <summary>
    /// 获取数据表中所有对象的列表副本。
    /// </summary>
    /// <remarks>
    /// Gets a list copy of all objects in the data table.
    /// </remarks>
    /// <returns>包含数据表中所有对象的列表 / A list containing all objects in the data table</returns>
    public List<T> ToList()
    {
        return DataList.ToList();
    }

    /// <summary>
    /// 根据指定条件查找第一个匹配的对象。
    /// </summary>
    /// <remarks>
    /// Finds the first object that matches the specified condition.
    /// </remarks>
    /// <param name="func">用于测试每个对象是否满足条件的函数 / The function to test each object for a condition</param>
    /// <returns>第一个满足条件的对象，如果没有找到则返回 <c>null</c> / The first object that matches the condition; <c>null</c> if not found</returns>
    public T Find(Func<T, bool> func)
    {
        return DataList.FirstOrDefault(func);
    }

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回数组。
    /// </summary>
    /// <remarks>
    /// Finds all objects that match the specified condition and returns them as an array.
    /// </remarks>
    /// <param name="func">用于测试每个对象是否满足条件的函数 / The function to test each object for a condition</param>
    /// <returns>包含所有满足条件的对象的数组 / An array containing all objects that match the condition</returns>
    public T[] FindListArray(Func<T, bool> func)
    {
        return DataList.Where(func).ToArray();
    }

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回列表。
    /// </summary>
    /// <remarks>
    /// Finds all objects that match the specified condition and returns them as a list.
    /// </remarks>
    /// <param name="func">用于测试每个对象是否满足条件的函数 / The function to test each object for a condition</param>
    /// <returns>包含所有满足条件的对象的列表 / A list containing all objects that match the condition</returns>
    public List<T> FindList(Func<T, bool> func)
    {
        return DataList.Where(func).ToList();
    }

    /// <summary>
    /// 对数据表中的每个对象执行指定操作。
    /// </summary>
    /// <remarks>
    /// Performs the specified action on each object in the data table.
    /// </remarks>
    /// <param name="func">要对每个对象执行的操作 / The action to perform on each object</param>
    public void ForEach(Action<T> func)
    {
        DataList.ForEach(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最大值。
    /// </summary>
    /// <remarks>
    /// Computes the projection value for all objects using the specified function and returns the maximum value.
    /// </remarks>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口 / The type of the projection value, must implement IComparable</typeparam>
    /// <param name="func">应用于每个对象的投影函数 / The projection function to apply to each object</param>
    /// <returns>所有对象投影值中的最大值 / The maximum value among all projected values</returns>
    public Tk Max<Tk>(Func<T, Tk> func)
    {
        return DataList.Max(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最小值。
    /// </summary>
    /// <remarks>
    /// Computes the projection value for all objects using the specified function and returns the minimum value.
    /// </remarks>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口 / The type of the projection value, must implement IComparable</typeparam>
    /// <param name="func">应用于每个对象的投影函数 / The projection function to apply to each object</param>
    /// <returns>所有对象投影值中的最小值 / The minimum value among all projected values</returns>
    public Tk Min<Tk>(Func<T, Tk> func)
    {
        return DataList.Min(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的整数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the integer projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的整数投影函数 / The integer projection function to apply to each object</param>
    /// <returns>所有对象整数投影值的总和 / The sum of all integer projected values</returns>
    public int Sum(Func<T, int> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的长整数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the long integer projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的长整数投影函数 / The long integer projection function to apply to each object</param>
    /// <returns>所有对象长整数投影值的总和 / The sum of all long integer projected values</returns>
    public long Sum(Func<T, long> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的浮点数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the float projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的浮点数投影函数 / The float projection function to apply to each object</param>
    /// <returns>所有对象浮点数投影值的总和 / The sum of all float projected values</returns>
    public float Sum(Func<T, float> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的双精度浮点数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the double projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的双精度浮点数投影函数 / The double projection function to apply to each object</param>
    /// <returns>所有对象双精度浮点数投影值的总和 / The sum of all double projected values</returns>
    public double Sum(Func<T, double> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的十进制数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the decimal projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的十进制数投影函数 / The decimal projection function to apply to each object</param>
    /// <returns>所有对象十进制数投影值的总和 / The sum of all decimal projected values</returns>
    public decimal Sum(Func<T, decimal> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 确保首尾非空元素缓存已初始化。
    /// </summary>
    /// <remarks>
    /// Ensures the first and last non-null element cache is initialized.
    /// </remarks>
    private void EnsureFirstLastCache()
    {
        if (_cacheInitialized)
        {
            return;
        }

        _firstOrDefaultCache = null;
        _lastOrDefaultCache = null;

        if (DataList.Count == 0)
        {
            return;
        }

        _cacheInitialized = true;

        for (var i = 0; i < DataList.Count; i++)
        {
            var value = DataList[i];
            if (value != null)
            {
                _firstOrDefaultCache = value;
                break;
            }
        }

        for (var i = DataList.Count - 1; i >= 0; i--)
        {
            var value = DataList[i];
            if (value != null)
            {
                _lastOrDefaultCache = value;
                break;
            }
        }
    }

    /// <summary>
    /// 确保数量缓存已初始化。
    /// </summary>
    /// <remarks>
    /// Ensures the count cache is initialized.
    /// </remarks>
    private void EnsureCountCache()
    {
        if (_countCacheInitialized)
        {
            return;
        }

        var count = Math.Max(LongDataMaps.Count, StringDataMaps.Count);
        if (count == 0)
        {
            _countCache = 0;
            return;
        }

        _countCache = count;
        _countCacheInitialized = true;
    }
}