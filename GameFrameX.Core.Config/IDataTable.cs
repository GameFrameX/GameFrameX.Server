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
public interface IDataTable<T> : IDataTable
{
    /// <summary>
    /// 根据指定的整数 ID 获取数据表中的对象。
    /// </summary>
    /// <param name="id">要获取的对象的唯一整数标识符。</param>
    /// <returns>与指定 ID 关联的数据对象；如果未找到匹配项，则可能返回 null 或抛出异常（取决于实现）。</returns>
    T Get(int id);

    /// <summary>
    /// 根据指定的长整数 ID 获取数据表中的对象。
    /// </summary>
    /// <param name="id">要获取的对象的唯一长整数标识符。</param>
    /// <returns>与指定 ID 关联的数据对象；如果未找到匹配项，则可能返回 null 或抛出异常（取决于实现）。</returns>
    T Get(long id);

    /// <summary>
    /// 根据指定的字符串 ID 获取数据表中的对象。
    /// </summary>
    /// <param name="id">要获取的对象的唯一字符串标识符。</param>
    /// <returns>与指定 ID 关联的数据对象；如果未找到匹配项，则可能返回 null 或抛出异常（取决于实现）。</returns>
    T Get(string id);

    /// <summary>
    /// 尝试根据整数ID获取对象
    /// </summary>
    /// <param name="id">要获取的对象的整数ID</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值</param>
    /// <returns>如果找到对应ID的对象则返回true，否则返回false</returns>
    bool TryGet(int id, out T value);

    /// <summary>
    /// 尝试根据长整数ID获取对象
    /// </summary>
    /// <param name="id">要获取的对象的长整数ID</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值</param>
    /// <returns>如果找到对应ID的对象则返回true，否则返回false</returns>
    bool TryGet(long id, out T value);

    /// <summary>
    /// 尝试根据字符串ID获取对象
    /// </summary>
    /// <param name="id">要获取的对象的字符串ID</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值</param>
    /// <returns>如果找到对应ID的对象则返回true，否则返回false</returns>
    bool TryGet(string id, out T value);

    /// <summary>
    /// 根据整数索引获取数据表中的对象
    /// </summary>
    /// <param name="index">要获取的对象在数据表中的从零开始的整数索引</param>
    /// <returns>位于指定索引位置的数据对象</returns>
    /// <exception cref="ArgumentOutOfRangeException">当索引超出数据表范围时抛出</exception>
    T this[int index] { get; }

    /// <summary>
    /// 根据长整数索引获取数据表中的对象
    /// </summary>
    /// <param name="id">要获取的对象在数据表中的长整数ID</param>
    /// <returns>与指定ID关联的数据对象</returns>
    /// <exception cref="ArgumentOutOfRangeException">当ID超出数据表范围时抛出</exception>
    T this[long id] { get; }

    /// <summary>
    /// 根据字符串键获取数据表中的对象
    /// </summary>
    /// <param name="id">要获取的对象在数据表中的字符串键</param>
    /// <returns>与指定键关联的数据对象</returns>
    /// <exception cref="KeyNotFoundException">当不存在具有指定键的对象时抛出</exception>
    T this[string id] { get; }

    /// <summary>
    /// 获取数据表中的第一个对象，如果数据表为空则返回默认值。
    /// </summary>
    /// <returns>数据表中的第一个对象，如果数据表为空则返回 null。</returns>
    T FirstOrDefault { get; }

    /// <summary>
    /// 获取数据表中的最后一个对象，如果数据表为空则返回默认值。
    /// </summary>
    /// <returns>数据表中的最后一个对象，如果数据表为空则返回 null。</returns>
    T LastOrDefault { get; }

    /// <summary>
    /// 获取数据表中所有对象的数组副本。
    /// </summary>
    /// <returns>包含数据表中所有对象的数组。</returns>
    T[] All { get; }

    /// <summary>
    /// 获取数据表中所有对象的数组副本。
    /// </summary>
    /// <returns>包含数据表中所有对象的数组。</returns>
    T[] ToArray();

    /// <summary>
    /// 获取数据表中所有对象的列表副本。
    /// </summary>
    /// <returns>包含数据表中所有对象的列表。</returns>
    List<T> ToList();

    /// <summary>
    /// 根据指定条件查找第一个匹配的对象。
    /// </summary>
    /// <param name="func">用于测试每个对象是否满足条件的函数。</param>
    /// <returns>第一个满足条件的对象，如果没有找到则返回 null。</returns>
    T Find(System.Func<T, bool> func);

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回数组。
    /// </summary>
    /// <param name="func">用于测试每个对象是否满足条件的函数。</param>
    /// <returns>包含所有满足条件的对象的数组。</returns>
    T[] FindListArray(System.Func<T, bool> func);

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回列表。
    /// </summary>
    /// <param name="func">用于测试每个对象是否满足条件的函数。</param>
    /// <returns>包含所有满足条件的对象的列表。</returns>
    List<T> FindList(Func<T, bool> func);

    /// <summary>
    /// 对数据表中的每个对象执行指定操作。
    /// </summary>
    /// <param name="func">要对每个对象执行的操作。</param>
    void ForEach(Action<T> func);

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最大值。
    /// </summary>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口。</typeparam>
    /// <param name="func">应用于每个对象的投影函数。</param>
    /// <returns>所有对象投影值中的最大值。</returns>
    Tk Max<Tk>(Func<T, Tk> func);

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最小值。
    /// </summary>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口。</typeparam>
    /// <param name="func">应用于每个对象的投影函数。</param>
    /// <returns>所有对象投影值中的最小值。</returns>
    Tk Min<Tk>(Func<T, Tk> func);

    /// <summary>
    /// 根据指定函数计算所有对象的整数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的整数投影函数。</param>
    /// <returns>所有对象整数投影值的总和。</returns>
    int Sum(Func<T, int> func);

    /// <summary>
    /// 根据指定函数计算所有对象的长整数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的长整数投影函数。</param>
    /// <returns>所有对象长整数投影值的总和。</returns>
    long Sum(Func<T, long> func);

    /// <summary>
    /// 根据指定函数计算所有对象的浮点数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的浮点数投影函数。</param>
    /// <returns>所有对象浮点数投影值的总和。</returns>
    float Sum(Func<T, float> func);

    /// <summary>
    /// 根据指定函数计算所有对象的双精度浮点数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的双精度浮点数投影函数。</param>
    /// <returns>所有对象双精度浮点数投影值的总和。</returns>
    double Sum(Func<T, double> func);

    /// <summary>
    /// 根据指定函数计算所有对象的十进制数值投影，并返回总和。
    /// </summary>
    /// <param name="func">应用于每个对象的十进制数投影函数。</param>
    /// <returns>所有对象十进制数投影值的总和。</returns>
    decimal Sum(Func<T, decimal> func);
}