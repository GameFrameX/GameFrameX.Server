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
/// 数据表基础接口，定义数据表的基本操作。
/// </summary>
/// <remarks>
/// Data table base interface, defining basic operations for data tables.
/// </remarks>
public interface IDataTable
{
    /// <summary>
    /// 获取数据表中对象的数量。
    /// </summary>
    /// <remarks>
    /// Gets the count of objects in the data table.
    /// </remarks>
    /// <value>数据表中对象的数量 / The count of objects in the data table</value>
    int Count { get; }

    /// <summary>
    /// 异步加载数据表。
    /// </summary>
    /// <remarks>
    /// Asynchronously loads the data table.
    /// </remarks>
    /// <returns>一个任务表示异步操作 / A task representing the asynchronous operation</returns>
    Task LoadAsync();
}

/// <summary>
/// 泛型数据表基础接口，提供类型安全的数据访问操作。
/// </summary>
/// <remarks>
/// Generic data table base interface, providing type-safe data access operations.
/// </remarks>
/// <typeparam name="T">数据表中对象的类型 / The type of objects in the data table</typeparam>
public interface IDataTable<T> : IDataTable
{
    /// <summary>
    /// 尝试根据整数ID获取对象。
    /// </summary>
    /// <remarks>
    /// Attempts to get an object by its integer ID.
    /// </remarks>
    /// <param name="id">要获取的对象的整数ID / The integer ID of the object to get</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值 / The object if found; otherwise the default value</param>
    /// <returns>如果找到对应ID的对象则返回 <c>true</c>，否则返回 <c>false</c> / <c>true</c> if the object is found; otherwise <c>false</c></returns>
    bool TryGet(int id, out T value);

    /// <summary>
    /// 尝试根据长整数ID获取对象。
    /// </summary>
    /// <remarks>
    /// Attempts to get an object by its long integer ID.
    /// </remarks>
    /// <param name="id">要获取的对象的长整数ID / The long integer ID of the object to get</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值 / The object if found; otherwise the default value</param>
    /// <returns>如果找到对应ID的对象则返回 <c>true</c>，否则返回 <c>false</c> / <c>true</c> if the object is found; otherwise <c>false</c></returns>
    bool TryGet(long id, out T value);

    /// <summary>
    /// 尝试根据字符串ID获取对象。
    /// </summary>
    /// <remarks>
    /// Attempts to get an object by its string ID.
    /// </remarks>
    /// <param name="id">要获取的对象的字符串ID / The string ID of the object to get</param>
    /// <param name="value">当找到对应ID的对象时，返回该对象；否则返回默认值 / The object if found; otherwise the default value</param>
    /// <returns>如果找到对应ID的对象则返回 <c>true</c>，否则返回 <c>false</c> / <c>true</c> if the object is found; otherwise <c>false</c></returns>
    bool TryGet(string id, out T value);

    /// <summary>
    /// 根据整数主键获取数据表中的对象。
    /// </summary>
    /// <remarks>
    /// Gets an object from the data table by its integer primary key.
    /// </remarks>
    /// <param name="id">要获取的对象的整数主键 / The integer primary key of the object to get</param>
    /// <value>与指定主键关联的数据对象；如果找不到则返回 <c>null</c> / The data object associated with the specified key; <c>null</c> if not found</value>
    T this[int id] { get; }

    /// <summary>
    /// 根据长整数主键获取数据表中的对象。
    /// </summary>
    /// <remarks>
    /// Gets an object from the data table by its long integer primary key.
    /// </remarks>
    /// <param name="id">要获取的对象的长整数主键 / The long integer primary key of the object to get</param>
    /// <value>与指定主键关联的数据对象；如果找不到则返回 <c>null</c> / The data object associated with the specified key; <c>null</c> if not found</value>
    T this[long id] { get; }

    /// <summary>
    /// 根据字符串键获取数据表中的对象。
    /// </summary>
    /// <remarks>
    /// Gets an object from the data table by its string key.
    /// </remarks>
    /// <param name="id">要获取的对象在数据表中的字符串键 / The string key of the object in the data table</param>
    /// <value>与指定键关联的数据对象；如果找不到则返回 <c>null</c> / The data object associated with the specified key; <c>null</c> if not found</value>
    T this[string id] { get; }

    /// <summary>
    /// 获取数据表中的第一个对象，如果数据表为空则返回默认值。
    /// </summary>
    /// <remarks>
    /// Gets the first object in the data table, or the default value if the table is empty.
    /// </remarks>
    /// <value>数据表中的第一个对象，如果数据表为空则返回 <c>null</c> / The first object in the data table; <c>null</c> if the table is empty</value>
    T FirstOrDefault { get; }

    /// <summary>
    /// 获取数据表中的最后一个对象，如果数据表为空则返回默认值。
    /// </summary>
    /// <remarks>
    /// Gets the last object in the data table, or the default value if the table is empty.
    /// </remarks>
    /// <value>数据表中的最后一个对象，如果数据表为空则返回 <c>null</c> / The last object in the data table; <c>null</c> if the table is empty</value>
    T LastOrDefault { get; }

    /// <summary>
    /// 获取数据表中所有对象的数组副本。
    /// </summary>
    /// <remarks>
    /// Gets an array copy of all objects in the data table.
    /// </remarks>
    /// <value>包含数据表中所有对象的数组 / An array containing all objects in the data table</value>
    T[] All { get; }

    /// <summary>
    /// 获取数据表中所有对象的数组副本。
    /// </summary>
    /// <remarks>
    /// Gets an array copy of all objects in the data table.
    /// </remarks>
    /// <returns>包含数据表中所有对象的数组 / An array containing all objects in the data table</returns>
    T[] ToArray();

    /// <summary>
    /// 获取数据表中所有对象的列表副本。
    /// </summary>
    /// <remarks>
    /// Gets a list copy of all objects in the data table.
    /// </remarks>
    /// <returns>包含数据表中所有对象的列表 / A list containing all objects in the data table</returns>
    List<T> ToList();

    /// <summary>
    /// 根据指定条件查找第一个匹配的对象。
    /// </summary>
    /// <remarks>
    /// Finds the first object that matches the specified condition.
    /// </remarks>
    /// <param name="func">用于测试每个对象是否满足条件的函数 / The function to test each object for a condition</param>
    /// <returns>第一个满足条件的对象，如果没有找到则返回 <c>null</c> / The first object that matches the condition; <c>null</c> if not found</returns>
    T Find(Func<T, bool> func);

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回数组。
    /// </summary>
    /// <remarks>
    /// Finds all objects that match the specified condition and returns them as an array.
    /// </remarks>
    /// <param name="func">用于测试每个对象是否满足条件的函数 / The function to test each object for a condition</param>
    /// <returns>包含所有满足条件的对象的数组 / An array containing all objects that match the condition</returns>
    T[] FindListArray(Func<T, bool> func);

    /// <summary>
    /// 根据指定条件查找所有匹配的对象并返回列表。
    /// </summary>
    /// <remarks>
    /// Finds all objects that match the specified condition and returns them as a list.
    /// </remarks>
    /// <param name="func">用于测试每个对象是否满足条件的函数 / The function to test each object for a condition</param>
    /// <returns>包含所有满足条件的对象的列表 / A list containing all objects that match the condition</returns>
    List<T> FindList(Func<T, bool> func);

    /// <summary>
    /// 对数据表中的每个对象执行指定操作。
    /// </summary>
    /// <remarks>
    /// Performs the specified action on each object in the data table.
    /// </remarks>
    /// <param name="func">要对每个对象执行的操作 / The action to perform on each object</param>
    void ForEach(Action<T> func);

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最大值。
    /// </summary>
    /// <remarks>
    /// Computes the projection value for all objects using the specified function and returns the maximum value.
    /// </remarks>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口 / The type of the projection value, must implement IComparable</typeparam>
    /// <param name="func">应用于每个对象的投影函数 / The projection function to apply to each object</param>
    /// <returns>所有对象投影值中的最大值 / The maximum value among all projected values</returns>
    Tk Max<Tk>(Func<T, Tk> func);

    /// <summary>
    /// 根据指定函数计算所有对象的投影值，并返回最小值。
    /// </summary>
    /// <remarks>
    /// Computes the projection value for all objects using the specified function and returns the minimum value.
    /// </remarks>
    /// <typeparam name="Tk">投影值的类型，必须实现 IComparable 接口 / The type of the projection value, must implement IComparable</typeparam>
    /// <param name="func">应用于每个对象的投影函数 / The projection function to apply to each object</param>
    /// <returns>所有对象投影值中的最小值 / The minimum value among all projected values</returns>
    Tk Min<Tk>(Func<T, Tk> func);

    /// <summary>
    /// 根据指定函数计算所有对象的整数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the integer projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的整数投影函数 / The integer projection function to apply to each object</param>
    /// <returns>所有对象整数投影值的总和 / The sum of all integer projected values</returns>
    int Sum(Func<T, int> func);

    /// <summary>
    /// 根据指定函数计算所有对象的长整数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the long integer projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的长整数投影函数 / The long integer projection function to apply to each object</param>
    /// <returns>所有对象长整数投影值的总和 / The sum of all long integer projected values</returns>
    long Sum(Func<T, long> func);

    /// <summary>
    /// 根据指定函数计算所有对象的浮点数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the float projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的浮点数投影函数 / The float projection function to apply to each object</param>
    /// <returns>所有对象浮点数投影值的总和 / The sum of all float projected values</returns>
    float Sum(Func<T, float> func);

    /// <summary>
    /// 根据指定函数计算所有对象的双精度浮点数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the double projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的双精度浮点数投影函数 / The double projection function to apply to each object</param>
    /// <returns>所有对象双精度浮点数投影值的总和 / The sum of all double projected values</returns>
    double Sum(Func<T, double> func);

    /// <summary>
    /// 根据指定函数计算所有对象的十进制数值投影，并返回总和。
    /// </summary>
    /// <remarks>
    /// Computes the decimal projection for all objects using the specified function and returns the sum.
    /// </remarks>
    /// <param name="func">应用于每个对象的十进制数投影函数 / The decimal projection function to apply to each object</param>
    /// <returns>所有对象十进制数投影值的总和 / The sum of all decimal projected values</returns>
    decimal Sum(Func<T, decimal> func);
}