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