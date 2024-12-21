using System.Linq.Expressions;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.Utility;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameFrameX.DataBase.Mongo;

/// <summary>
/// MongoDB服务连接类，实现了
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// 接口。
/// </summary>
public sealed partial class MongoDbService : IDatabaseService
{
    /// <summary>
    /// 替换选项，用于替换文档。设置
    /// <see>
    ///     <cref>IsUpsert</cref>
    /// </see>
    /// 属性为 true 可以在找不到匹配的文档时插入新文档。
    /// </summary>
    public static readonly ReplaceOptions ReplaceOptions = new() { IsUpsert = true, };

    /// <summary>
    /// 更新选项，用于更新文档。设置
    /// <see>
    ///     <cref>IsUpsert</cref>
    /// </see>
    /// 属性为 true 可以在找不到匹配的文档时插入新文档。
    /// </summary>
    public static readonly UpdateOptions UpdateOptions = new() { IsUpsert = true, };

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public async Task<TState> UpdateAsync<TState>(TState state) where TState : class, ICacheState, new()
    {
        var isChanged = state.IsModify();
        if (isChanged)
        {
            state.UpdateTime = TimeHelper.UnixTimeMilliseconds();
            state.UpdateCount++;

            var filter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, state.Id);
            var collection = GetCollection<TState>();
            var result = await collection.ReplaceOneAsync(filter, state, ReplaceOptions);
            if (result.IsAcknowledged)
            {
                state.SaveToDbPostHandler();
            }
        }

        return state;
    }

    /// <summary>
    /// 保存多条数据
    /// </summary>
    /// <param name="stateList">数据列表对象</param>
    /// <returns>返回更新成功的数量</returns>
    public async Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : class, ICacheState, new()
    {
        long resultCount = 0;
        foreach (var state in stateList)
        {
            var isChanged = state.IsModify();
            if (isChanged)
            {
                state.UpdateTime = TimeHelper.UnixTimeMilliseconds();
                state.UpdateCount++;
                var filter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, state.Id);
                var collection = GetCollection<TState>();
                var result = await collection.ReplaceOneAsync(filter, state, ReplaceOptions);
                if (result.IsAcknowledged)
                {
                    resultCount++;
                    state.SaveToDbPostHandler();
                }
            }
        }

        return resultCount;
    }

    /// <summary>
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public async Task<long> UpdateCountAsync<TState>(TState state) where TState : class, ICacheState, new()
    {
        var isChanged = state.IsModify();
        if (isChanged)
        {
            state.UpdateTime = TimeHelper.UnixTimeMilliseconds();
            state.UpdateCount++;

            var filter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, state.Id);
            var collection = GetCollection<TState>();
            var result = await collection.ReplaceOneAsync(filter, state, ReplaceOptions);
            if (result.IsAcknowledged)
            {
                state.SaveToDbPostHandler();
                return result.ModifiedCount;
            }
        }

        return 0;
    }

    #region 更新

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public UpdateResult UpdateOne<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert = true) where TState : class, ICacheState, new()
    {
        return GetCollection<TState>().UpdateOne(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public UpdateResult UpdateOne(string collectionName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert)
    {
        return GetCollection(collectionName).UpdateOne(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public async Task<UpdateResult> UpdateOneAsync<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert) where TState : class, ICacheState, new()
    {
        return await GetCollection<TState>().UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public async Task<UpdateResult> UpdateOneAsync(string collectionName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert)
    {
        return await GetCollection(collectionName).UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public long UpdateMany<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert = false) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().UpdateMany(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public long UpdateMany(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert = false)
    {
        var result = GetCollection(collName).UpdateMany(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改多个文档
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public async Task<long> UpdateManyAsync<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert = false) where TState : class, ICacheState, new()
    {
        var result = await GetCollection<TState>().UpdateManyAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改多个文档
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public async Task<long> UpdateManyAsync(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert = false)
    {
        var result = await GetCollection(collName).UpdateManyAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的数据</param>
    /// <returns></returns>
    public TState UpdateOne<TState>(string collName, Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().FindOneAndUpdate(filter, update);
        return result;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的Bson数据</param>
    /// <returns></returns>
    public BsonDocument UpdateOne(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update)
    {
        var result = GetCollection(collName).FindOneAndUpdate(filter, update);
        return result;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的数据</param>
    /// <returns></returns>
    public async Task<TState> UpdateOneAsync<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update) where TState : class, ICacheState, new()
    {
        var result = await GetCollection<TState>().FindOneAndUpdateAsync(filter, update);
        return result;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的Bson数据</param>
    /// <returns></returns>
    public async Task<BsonDocument> UpdateOneAsync(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update)
    {
        var result = await GetCollection(collName).FindOneAndUpdateAsync(filter, update);
        return result;
    }

    #endregion 更新
}