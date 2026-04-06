// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using System.Threading;
using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.DataBase;

/// <summary>
/// 游戏数据库静态工具类,提供对数据库的基本操作封装。
/// </summary>
/// <remarks>
/// Static utility class for game database operations, providing basic database operation encapsulation.
/// </remarks>
public static partial class GameDb
{
    /// <summary>
    /// 保存单个文档到数据库。
    /// </summary>
    /// <remarks>
    /// Save a single document to the database.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要保存的文档实例 / Document instance to save</param>
    /// <returns>保存操作的结果 / Result of the save operation</returns>
    public static Task SaveOneAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddAsync(state);
    }

    /// <summary>
    /// 保存单个文档到数据库。
    /// </summary>
    /// <remarks>
    /// Save a single document to the database.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要保存的文档实例 / Document instance to save</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>保存操作的结果 / Result of the save operation</returns>
    public static Task SaveOneAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddAsync(state, cancellationToken);
    }

    /// <summary>
    /// 添加或更新文档(如果存在则更新,不存在则添加)。
    /// </summary>
    /// <remarks>
    /// Add or update a document (update if exists, add if not).
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数 / Document type, must inherit from BaseCacheState and have a parameterless constructor</typeparam>
    /// <param name="state">要保存或更新的文档实例 / Document instance to save or update</param>
    /// <returns>保存或更新后的文档 / The saved or updated document</returns>
    public static Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddOrUpdateAsync(state);
    }

    /// <summary>
    /// 添加或更新文档(如果存在则更新,不存在则添加)。
    /// </summary>
    /// <remarks>
    /// Add or update a document (update if exists, add if not).
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数 / Document type, must inherit from BaseCacheState and have a parameterless constructor</typeparam>
    /// <param name="state">要保存或更新的文档实例 / Document instance to save or update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>保存或更新后的文档 / The saved or updated document</returns>
    public static Task<TState> AddOrUpdateAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddOrUpdateAsync(state, cancellationToken);
    }

    /// <summary>
    /// 批量添加或更新文档。
    /// </summary>
    /// <remarks>
    /// Batch add or update documents.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="states">要处理的文档集合 / Document collection to process</param>
    /// <returns>处理记录数 / Number of processed records</returns>
    public static Task<long> AddOrUpdateListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddOrUpdateListAsync(states);
    }

    /// <summary>
    /// 批量添加或更新文档。
    /// </summary>
    /// <remarks>
    /// Batch add or update documents.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="states">要处理的文档集合 / Document collection to process</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>处理记录数 / Number of processed records</returns>
    public static Task<long> AddOrUpdateListAsync<TState>(IEnumerable<TState> states, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddOrUpdateListAsync(states, cancellationToken);
    }

    /// <summary>
    /// 批量保存多个文档到数据库。
    /// </summary>
    /// <remarks>
    /// Batch save multiple documents to the database.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="states">要保存的文档集合 / Collection of documents to save</param>
    /// <returns>保存操作的任务 / Task representing the save operation</returns>
    public static Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddListAsync(states);
    }

    /// <summary>
    /// 批量保存多个文档到数据库。
    /// </summary>
    /// <remarks>
    /// Batch save multiple documents to the database.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="states">要保存的文档集合 / Collection of documents to save</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>保存操作的任务 / Task representing the save operation</returns>
    public static Task AddListAsync<TState>(IEnumerable<TState> states, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddListAsync(states, cancellationToken);
    }
}
