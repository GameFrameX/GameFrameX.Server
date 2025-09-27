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
public abstract class BaseDataTable<T> : IDataTable<T>
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
    /// 根据整型ID获取对象
    /// </summary>
    /// <param name="id">对象的ID</param>
    /// <returns>找到的对象，如果未找到则返回默认值</returns>
    public T this[int id]
    {
        get { return Get(id); }
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
    /// 根据条件查找对象
    /// </summary>
    /// <param name="func">查找条件</param>
    /// <returns>满足条件的第一个对象，如果未找到则返回默认值</returns>
    public T Find(Func<T, bool> func)
    {
        return DataList.FirstOrDefault(func);
    }

    /// <summary>
    /// 根据条件查找多个对象
    /// </summary>
    /// <param name="func">查找条件</param>
    /// <returns>满足条件的所有对象数组</returns>
    public T[] FindList(Func<T, bool> func)
    {
        return DataList.Where(func).ToArray();
    }
}