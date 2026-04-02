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

using System.Linq.Expressions;
using System.Threading;
using GameFrameX.Foundation.Utility;
using GameFrameX.Utility;
using MongoDB.Driver;

namespace GameFrameX.DataBase.Mongo;

/// <summary>
/// MongoDB服务连接类，实现了
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// 接口。
/// </summary>
/// <remarks>
/// MongoDB service connection class that implements the
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// interface.
/// </remarks>
public sealed partial class MongoDbService
{
    /// <summary>
    /// 根据条件删除单条数据（软删除）。
    /// </summary>
    /// <remarks>
    /// Deletes a single data record by condition (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await DeleteAsync(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据条件删除单条数据（软删除）。
    /// </summary>
    /// <remarks>
    /// Deletes a single data record by condition (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var state = await FindAsync(filter, false).WaitAsync(cancellationToken).ConfigureAwait(false);
        if (state == null)
        {
            return 0;
        }

        state.DeleteTime = GetCurrentTimestamp();
        state.IsDeleted = true;

        var collection = _mongoDbContext.GetCollection<TState>();
        var mongoFilter = Builders<TState>.Filter.Eq(m => m.Id, state.Id);
        var update = Builders<TState>.Update
            .Set(x => x.IsDeleted, state.IsDeleted)
            .Set(x => x.DeleteTime, state.DeleteTime);

        var result = await ExecuteWriteWithRetryAsync(token => collection.UpdateOneAsync(mongoFilter, update, cancellationToken: token), cancellationToken, nameof(DeleteAsync), true).ConfigureAwait(false);
        return result.ModifiedCount;
    }

    /// <summary>
    /// 根据条件批量删除数据（软删除）。
    /// </summary>
    /// <remarks>
    /// Batch deletes data by condition (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await DeleteListAsync(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据条件批量删除数据（软删除）。
    /// </summary>
    /// <remarks>
    /// Batch deletes data by condition (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var list = await FindListAsync(filter, cancellationToken).ConfigureAwait(false);
        if (list == null || list.Count == 0)
        {
            return 0;
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        var deleteTime = GetCurrentTimestamp();
        var writeModels = new List<WriteModel<TState>>();

        foreach (var state in list)
        {
            state.DeleteTime = deleteTime;
            state.IsDeleted = true;

            var mongoFilter = Builders<TState>.Filter.Eq(m => m.Id, state.Id);
            var update = Builders<TState>.Update
                .Set(x => x.IsDeleted, state.IsDeleted)
                .Set(x => x.DeleteTime, state.DeleteTime);
            writeModels.Add(new UpdateOneModel<TState>(mongoFilter, update));
        }

        var result = await ExecuteWriteWithRetryAsync(token => collection.BulkWriteAsync(writeModels, BulkWriteOptions, token), cancellationToken, nameof(DeleteListAsync), true).ConfigureAwait(false);
        return result.ModifiedCount;
    }

    /// <summary>
    /// 根据ID列表批量删除数据（软删除）。
    /// </summary>
    /// <remarks>
    /// Batch deletes data by ID list (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        return await DeleteListIdAsync<TState>(ids, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据ID列表批量删除数据（软删除）。
    /// </summary>
    /// <remarks>
    /// Batch deletes data by ID list (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var idArray = ids as long[] ?? ids?.ToArray();
        if (idArray == null || idArray.Length == 0)
        {
            return 0;
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        var deleteTime = GetCurrentTimestamp();
        var writeModels = new List<WriteModel<TState>>();

        foreach (var id in idArray)
        {
            var mongoFilter = Builders<TState>.Filter.Eq(m => m.Id, id);
            var update = Builders<TState>.Update
                .Set(x => x.IsDeleted, true)
                .Set(x => x.DeleteTime, deleteTime);
            writeModels.Add(new UpdateOneModel<TState>(mongoFilter, update));
        }

        var result = await ExecuteWriteWithRetryAsync(token => collection.BulkWriteAsync(writeModels, BulkWriteOptions, token), cancellationToken, nameof(DeleteListIdAsync), true).ConfigureAwait(false);
        return result.ModifiedCount;
    }

    /// <summary>
    /// 删除指定对象（软删除）。
    /// </summary>
    /// <remarks>
    /// Deletes the specified object (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要删除的对象 / Object to delete</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        return await DeleteAsync(state, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 删除指定对象（软删除）。
    /// </summary>
    /// <remarks>
    /// Deletes the specified object (soft delete).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要删除的对象 / Object to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回修改的记录数 / The number of modified records</returns>
    public async Task<long> DeleteAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        state.DeleteTime = GetCurrentTimestamp();
        state.IsDeleted = true;

        var collection = _mongoDbContext.GetCollection<TState>();
        var filter = Builders<TState>.Filter.Eq(m => m.Id, state.Id);
        var update = Builders<TState>.Update
            .Set(x => x.IsDeleted, state.IsDeleted)
            .Set(x => x.DeleteTime, state.DeleteTime);

        var result = await ExecuteWriteWithRetryAsync(token => collection.UpdateOneAsync(filter, update, cancellationToken: token), cancellationToken, nameof(DeleteAsync), true).ConfigureAwait(false);
        return result.ModifiedCount;
    }

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <returns>返回删除的记录数 / The number of deleted records</returns>
    public async Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await HardDeleteAsync<TState>(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回删除的记录数 / The number of deleted records</returns>
    public async Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var deleteFilter = filter ?? (_ => true);
        var result = await ExecuteWriteWithRetryAsync(token => collection.DeleteManyAsync(deleteFilter, token), cancellationToken, nameof(HardDeleteAsync), true).ConfigureAwait(false);
        return result.DeletedCount;
    }

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <returns>返回恢复的记录数 / The number of restored records</returns>
    public async Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await RestoreAsync<TState>(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回恢复的记录数 / The number of restored records</returns>
    public async Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var expression = filter ?? (_ => true);
        var mongoFilter = Builders<TState>.Filter.Where(expression) & Builders<TState>.Filter.Eq(m => m.IsDeleted, true);
        var update = Builders<TState>.Update
            .Set(x => x.IsDeleted, false)
            .Unset(x => x.DeleteTime);
        var result = await ExecuteWriteWithRetryAsync(token => collection.UpdateManyAsync(mongoFilter, update, cancellationToken: token), cancellationToken, nameof(RestoreAsync), true).ConfigureAwait(false);
        return result.ModifiedCount;
    }
}
