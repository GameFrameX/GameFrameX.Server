using GameFrameX.DataBase.Abstractions;
using GameFrameX.Monitor.DataBase;
using GameFrameX.Utility;
using MongoDB.Driver;
using MongoDB.Entities;

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
    /// 批量写入选项，用于批量写入文档。设置
    /// <see>
    ///     <cref>IsOrdered</cref>
    /// </see>
    /// 属性为 false 可以并行执行写入操作。
    /// </summary>
    public static readonly BulkWriteOptions BulkWriteOptions = new() { IsOrdered = false, };

    #region 插入

    /// <summary>
    /// 增加一条数据
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns>返回修改的条数</returns>
    public async Task AddAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        MetricsDataBaseHelper.CreateCounterOptions.Add(1);
        state.CreateTime = TimeHelper.UnixTimeMilliseconds();
        state.UpdateTime = state.CreateTime;
        await _mongoDbContext.SaveAsync(state);
    }

    /// <summary>
    /// 增加一个列表数据
    /// </summary>
    /// <param name="states"></param>
    /// <typeparam name="TState"></typeparam>
    public async Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new()
    {MetricsDataBaseHelper.CreateCounterOptions.Add(1);
        var cacheStates = states.ToList();
        foreach (var cacheState in cacheStates)
        {
            cacheState.CreateTime = TimeHelper.UnixTimeMilliseconds();
            cacheState.UpdateTime = cacheState.CreateTime;
        }

        await _mongoDbContext.SaveAsync(cacheStates);
    }


    /*
    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state">数据</param>
    public void InsertOne<TState>(TState state) where TState : ICacheState, new()
    {
        GetCollection<TState>().InsertOne(state);
    }*/

    /*/// <summary>
    /// 插入一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="doc">文档</param>
    public void InsertOne(string collectionName, BsonDocument doc)
    {
        GetCollection(collectionName).InsertOne(doc);
    }*/

    /*
    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="t">数据</param>
    public Task InsertOneAsync<TState>(TState t) where TState : ICacheState, new()
    {
        return GetCollection<TState>().InsertOneAsync(t);
    }
    */

    /*
    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="doc">文档</param>
    public Task InsertOneAsync(string collectionName, BsonDocument doc)
    {
        return GetCollection(collectionName).InsertOneAsync(doc);
    }*/

    /*/// <summary>
    /// 插入多条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">集合</param>
    public void InsertMany<TState>(IEnumerable<TState> list) where TState : ICacheState, new()
    {
        GetCollection<TState>().InsertMany(list);
    }*/

    /*
    /// <summary>
    /// 插入多条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson集合</param>
    public void InsertMany(string collectionName, IEnumerable<BsonDocument> list)
    {
        GetCollection(collectionName).InsertMany(list);
    }*/

    /*/// <summary>
    /// 插入多条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">集合</param>
    public Task InsertManyAsync<TState>(IEnumerable<TState> list) where TState : ICacheState, new()
    {
        return GetCollection<TState>().InsertManyAsync(list);
    }*/

    /*
    /// <summary>
    /// 插入多条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson集合</param>
    public Task InsertManyAsync(string collectionName, IEnumerable<BsonDocument> list)
    {
        return GetCollection(collectionName).InsertManyAsync(list);
    }*/

    /*
    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">数据集合</param>
    /// <returns></returns>
    public List<WriteModel<TState>> BulkInsert<TState>(IEnumerable<WriteModel<TState>> list) where TState : ICacheState, new()
    {
        var result = GetCollection<TState>().BulkWrite(list);
        return result.ProcessedRequests.ToList();
    }

    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson数据集合</param>
    /// <returns></returns>
    public List<WriteModel<BsonDocument>> BulkInsert(string collectionName, IEnumerable<WriteModel<BsonDocument>> list)
    {
        var result = GetCollection(collectionName).BulkWrite(list);
        return result.ProcessedRequests.ToList();
    }

    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">数据集合</param>
    /// <returns></returns>
    public async Task<List<WriteModel<TState>>> BulkInsertAsync<TState>(IEnumerable<WriteModel<TState>> list) where TState : ICacheState, new()
    {
        var result = await GetCollection<TState>().BulkWriteAsync(list);
        return result.ProcessedRequests.ToList();
    }

    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson数据集合</param>
    /// <returns></returns>
    public async Task<List<WriteModel<BsonDocument>>> BulkInsertAsync(string collectionName, IEnumerable<WriteModel<BsonDocument>> list)
    {
        var result = await GetCollection(collectionName).BulkWriteAsync(list);
        return result.ProcessedRequests.ToList();
    }*/

    #endregion 插入
}