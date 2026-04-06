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
    /// 更新指定的文档。
    /// </summary>
    /// <remarks>
    /// Update the specified document.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要更新的文档实例 / Document instance to update</param>
    /// <returns>更新后的文档 / The updated document</returns>
    public static Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(state);
    }

    /// <summary>
    /// 更新指定的文档。
    /// </summary>
    /// <remarks>
    /// Update the specified document.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要更新的文档实例 / Document instance to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>更新后的文档 / The updated document</returns>
    public static Task<TState> UpdateAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(state, cancellationToken);
    }

    /// <summary>
    /// 批量更新多个文档。
    /// </summary>
    /// <remarks>
    /// Batch update multiple documents.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="stateList">要更新的文档集合 / Collection of documents to update</param>
    /// <returns>成功更新的文档数量 / Number of successfully updated documents</returns>
    public static Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(stateList);
    }

    /// <summary>
    /// 批量更新多个文档。
    /// </summary>
    /// <remarks>
    /// Batch update multiple documents.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="stateList">要更新的文档集合 / Collection of documents to update</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>成功更新的文档数量 / Number of successfully updated documents</returns>
    public static Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(stateList, cancellationToken);
    }

    /// <summary>
    /// 根据ID部分更新文档字段。
    /// </summary>
    /// <remarks>
    /// Partially update document fields by ID.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">文档ID / Document ID</param>
    /// <param name="updateFields">更新字段集合 / Update field dictionary</param>
    /// <returns>成功更新的文档数量 / Number of successfully updated documents</returns>
    public static Task<long> UpdatePartialAsync<TState>(long id, IReadOnlyDictionary<string, object> updateFields) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdatePartialAsync<TState>(id, updateFields);
    }

    /// <summary>
    /// 根据ID部分更新文档字段。
    /// </summary>
    /// <remarks>
    /// Partially update document fields by ID.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">文档ID / Document ID</param>
    /// <param name="updateFields">更新字段集合 / Update field dictionary</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>成功更新的文档数量 / Number of successfully updated documents</returns>
    public static Task<long> UpdatePartialAsync<TState>(long id, IReadOnlyDictionary<string, object> updateFields, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdatePartialAsync<TState>(id, updateFields, cancellationToken);
    }

    /// <summary>
    /// 在事务中执行操作。
    /// </summary>
    /// <remarks>
    /// Execute operation in a transaction.
    /// </remarks>
    /// <param name="action">事务内执行逻辑 / Action to execute within transaction</param>
    /// <returns>表示异步操作的任务 / Task representing asynchronous operation</returns>
    public static Task ExecuteInTransactionAsync(Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.ExecuteInTransactionAsync(action);
    }

    /// <summary>
    /// 在事务中执行操作。
    /// </summary>
    /// <remarks>
    /// Execute operation in a transaction.
    /// </remarks>
    /// <param name="action">事务内执行逻辑 / Action to execute within transaction</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>表示异步操作的任务 / Task representing asynchronous operation</returns>
    public static Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.ExecuteInTransactionAsync(action, cancellationToken);
    }
}
