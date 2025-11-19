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
/// 基础数据表
/// </summary>
/// <typeparam name="T">数据表中的数据类型</typeparam>
public abstract class BaseDataTable<T> : IDataTable<T> where T : class
{
    /// <summary>
    /// 异步加载器
    /// </summary>
    protected readonly Func<Task<JsonElement>> _loadFunc;

    /// <summary>
    /// 数据列表
    /// </summary>
    protected readonly List<T> DataList = new();

    /// <summary>
    /// 长整型键的数据表
    /// </summary>
    protected readonly SortedDictionary<long, T> LongDataMaps = new();

    /// <summary>
    /// 字符串键的数据表
    /// </summary>
    protected readonly SortedDictionary<string, T> StringDataMaps = new();

    /// <summary>
    /// 初始化基础数据表
    /// </summary>
    /// <param name="loadFunc">异步加载数据的委托</param>
    public BaseDataTable(Func<Task<JsonElement>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    /// <summary>
    /// 默认构造函数
    /// </summary>
    public BaseDataTable()
    {
    }

    /// <summary>
    /// 异步加载数据
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    public abstract Task LoadAsync();

    /// <summary>
    /// 根据整型ID获取对象
    /// </summary>
    /// <param name="id">对象的ID</param>
    /// <returns>找到的对象，如果未找到则返回默认值</returns>
    public T Get(int id)
    {
        LongDataMaps.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 根据长整型ID获取对象
    /// </summary>
    /// <param name="id">对象的ID</param>
    /// <returns>找到的对象，如果未找到则返回默认值</returns>
    public T Get(long id)
    {
        LongDataMaps.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 根据字符串ID获取对象
    /// </summary>
    /// <param name="id">对象的ID</param>
    /// <returns>找到的对象，如果未找到则返回默认值</returns>
    public T Get(string id)
    {
        StringDataMaps.TryGetValue(id, out var value);
        return value;
    }

    /// <summary>
    /// 尝试根据整数ID获取对象
    /// </summary>
    /// <param name="id">要获取的对象的整数ID</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值</param>
    /// <returns>如果找到对应ID的对象则返回true，否则返回false</returns>
    public bool TryGet(int id, out T value)
    {
        return LongDataMaps.TryGetValue(id, out value);
    }

    /// <summary>
    /// 尝试根据长整数ID获取对象
    /// </summary>
    /// <param name="id">要获取的对象的长整数ID</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值</param>
    /// <returns>如果找到对应ID的对象则返回true，否则返回false</returns>
    public bool TryGet(long id, out T value)
    {
        return LongDataMaps.TryGetValue(id, out value);
    }

    /// <summary>
    /// 尝试根据字符串ID获取对象
    /// </summary>
    /// <param name="id">要获取的对象的字符串ID</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值</param>
    /// <returns>如果找到对应ID的对象则返回true，否则返回false</returns>
    public bool TryGet(string id, out T value)
    {
        return StringDataMaps.TryGetValue(id, out value);
    }


    /// <summary>
    /// 根据整数索引获取数据表中的对象
    /// </summary>
    /// <param name="index">要获取的对象在数据表中的从零开始的整数索引</param>
    /// <returns>位于指定索引位置的数据对象</returns>
    /// <exception cref="ArgumentOutOfRangeException">当索引超出数据表范围时抛出</exception>
    public T this[int index]
    {
        get
        {
            if (index >= Count || index < 0)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            return DataList[index];
        }
    }

    /// <summary>
    /// 根据长整型ID获取对象
    /// </summary>
    /// <param name="id">对象的ID</param>
    /// <returns>找到的对象，如果未找到则返回默认值</returns>
    public T this[long id]
    {
        get { return Get(id); }
    }

    /// <summary>
    /// 根据字符串ID获取对象
    /// </summary>
    /// <param name="id">对象的ID</param>
    /// <returns>找到的对象，如果未找到则返回默认值</returns>
    public T this[string id]
    {
        get { return Get(id); }
    }

    /// <summary>
    /// 获取数据表中对象的数量
    /// </summary>
    /// <returns>数据表中对象的数量</returns>
    public int Count
    {
        get { return Math.Max(LongDataMaps.Count, StringDataMaps.Count); }
    }

    /// <summary>
    /// 获取数据表中第一个对象
    /// </summary>
    /// <returns>数据表中的第一个对象，如果数据表为空则返回默认值</returns>
    public T FirstOrDefault
    {
        get { return DataList.FirstOrDefault(m => m != null); }
    }

    /// <summary>
    /// 获取数据表中最后一个对象
    /// </summary>
    /// <returns>数据表中的最后一个对象，如果数据表为空则返回默认值</returns>
    public T LastOrDefault
    {
        get { return DataList.LastOrDefault(m => m != null); }
    }

    /// <summary>
    /// 获取数据表中所有对象
    /// </summary>
    /// <returns>数据表中的所有对象数组</returns>
    public T[] All
    {
        get { return DataList.ToArray(); }
    }

    /// <summary>
    /// 获取数据表中所有对象
    /// </summary>
    /// <returns>数据表中的所有对象数组</returns>
    public T[] ToArray()
    {
        return DataList.ToArray();
    }

    /// <summary>
    /// 获取数据表中所有对象的列表副本。
    /// </summary>
    /// <returns>包含数据表中所有对象的列表。</returns>
    public List<T> ToList()
    {
        return DataList.ToList();
    }

    /// <summary>
    /// 根据条件查找对象
    /// </summary>
    /// <param name="func">查找条件</param>
    /// <returns>满足条件的第一个对象，如果未找到则返回默认值</returns>
    public T Find(Func<T, bool> func)
    {
        return DataList.FirstOrDefault(func);
    }

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回数组。
    /// </summary>
    /// <param name="func">用于测试每个对象是否满足条件的函数。</param>
    /// <returns>包含所有满足条件的对象的数组。</returns>
    public T[] FindListArray(Func<T, bool> func)
    {
        return DataList.Where(func).ToArray();
    }

    /// <summary>
    /// 根据条件查找多个对象
    /// </summary>
    /// <param name="func">查找条件</param>
    /// <returns>满足条件的所有对象数组</returns>
    public List<T> FindList(Func<T, bool> func)
    {
        return DataList.Where(func).ToList();
    }

    /// <summary>
    /// 对数据表中的每个对象执行指定操作。
    /// </summary>
    /// <param name="func">要对每个对象执行的操作。</param>
    public void ForEach(Action<T> func)
    {
        DataList.ForEach(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最大值。
    /// </summary>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口。</typeparam>
    /// <param name="func">应用于每个对象的投影函数。</param>
    /// <returns>所有对象投影值中的最大值。</returns>
    public Tk Max<Tk>(Func<T, Tk> func)
    {
        return DataList.Max(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最小值。
    /// </summary>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口。</typeparam>
    /// <param name="func">应用于每个对象的投影函数。</param>
    /// <returns>所有对象投影值中的最小值。</returns>
    public Tk Min<Tk>(Func<T, Tk> func)
    {
        return DataList.Min(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的整数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的整数投影函数。</param>
    /// <returns>所有对象整数投影值的总和。</returns>
    public int Sum(Func<T, int> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的长整数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的长整数投影函数。</param>
    /// <returns>所有对象长整数投影值的总和。</returns>
    public long Sum(Func<T, long> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的浮点数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的浮点数投影函数。</param>
    /// <returns>所有对象浮点数投影值的总和。</returns>
    public float Sum(Func<T, float> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的双精度浮点数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的双精度浮点数投影函数。</param>
    /// <returns>所有对象双精度浮点数投影值的总和。</returns>
    public double Sum(Func<T, double> func)
    {
        return DataList.Sum(func);
    }

    /// <summary>
    /// 根据指定函数计算所有对象的十进制数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的十进制数投影函数。</param>
    /// <returns>所有对象十进制数投影值的总和。</returns>
    public decimal Sum(Func<T, decimal> func)
    {
        return DataList.Sum(func);
    }
}