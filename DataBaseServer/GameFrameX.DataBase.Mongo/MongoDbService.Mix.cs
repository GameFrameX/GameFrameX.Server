using GameFrameX.DataBase.Abstractions;
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
public sealed partial class MongoDbService
{
    /// <summary>
    /// 增加或更新数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">数据类型</typeparam>
    /// <returns>返回增加或更新的条数</returns>
    public async Task<long> AddOrUpdateAsync<TState>(TState state) where TState : class, ICacheState, new()
    {
        var newFilter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, state.Id);
        var findExpression = GetDefaultFindExpression<TState>(null);
        var filterDefinition = Builders<TState>.Filter.And(newFilter, findExpression);
        var col = GetCollection<TState>();
        using var cursor = await col.FindAsync(filterDefinition);
        var resultState = await cursor.FirstOrDefaultAsync();

        if (resultState == null)
        {
            // Add
            return await AddAsync(state);
        }

        // Update
        return await UpdateCountAsync(state);
    }
}