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
public sealed partial class MongoDbService
{
    #region 删除

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long Delete<TState>(string collectionName, BsonDocument document)
    {
        DeleteResult result = CurrentDatabase.GetCollection<TState>(collectionName).DeleteOne(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long DeleteMany<TState>(BsonDocument document) where TState : class, ICacheState, new()
    {
        DeleteResult result = GetCollection<TState>().DeleteMany(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long Delete(string collectionName, BsonDocument document)
    {
        DeleteResult result = GetCollection(collectionName).DeleteOne(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long DeleteMany(string collectionName, BsonDocument document)
    {
        DeleteResult result = GetCollection(collectionName).DeleteMany(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync<TState>(BsonDocument document) where TState : class, ICacheState, new()
    {
        DeleteResult result = await GetCollection<TState>().DeleteOneAsync(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync<TState>(BsonDocument document) where TState : class, ICacheState, new()
    {
        DeleteResult result = await GetCollection<TState>().DeleteManyAsync(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync(string collectionName, BsonDocument document)
    {
        DeleteResult result = await GetCollection(collectionName).DeleteOneAsync(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync(string collectionName, BsonDocument document)
    {
        DeleteResult result = await GetCollection(collectionName).DeleteManyAsync(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long Delete<TState>(string json) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().DeleteOne(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long DeleteMany<TState>(string json) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().DeleteMany(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long Delete(string collName, string json)
    {
        var result = GetCollection(collName).DeleteOne(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long DeleteMany(string collName, string json)
    {
        var result = GetCollection(collName).DeleteMany(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync<TState>(string collName, string json)
    {
        var result = await CurrentDatabase.GetCollection<TState>(collName).DeleteOneAsync(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync<TState>(string json) where TState : class, ICacheState, new()
    {
        var result = await GetCollection<TState>().DeleteManyAsync(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync(string collName, string json)
    {
        var result = await GetCollection(collName).DeleteOneAsync(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync(string collName, string json)
    {
        var result = await GetCollection(collName).DeleteManyAsync(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public long Delete<TState>(Expression<Func<TState, bool>> predicate) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().DeleteOne(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public long DeleteMany<TState>(Expression<Func<TState, bool>> predicate) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().DeleteMany(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public long Delete(string collName, Expression<Func<BsonDocument, bool>> predicate)
    {
        var result = GetCollection(collName).DeleteOne(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public long DeleteMany(string collName, Expression<Func<BsonDocument, bool>> predicate)
    {
        var result = GetCollection(collName).DeleteMany(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync<TState>(Expression<Func<TState, bool>> predicate) where TState : class, ICacheState, new()
    {
        var result = await GetCollection<TState>().DeleteManyAsync(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync(string collName, Expression<Func<BsonDocument, bool>> predicate)
    {
        var result = await GetCollection(collName).DeleteOneAsync(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync(string collName, Expression<Func<BsonDocument, bool>> predicate)
    {
        var result = await GetCollection(collName).DeleteManyAsync(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public long Delete<TState>(string collName, FilterDefinition<TState> filter) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().DeleteOne(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public long DeleteMany<TState>(FilterDefinition<TState> filter) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().DeleteMany(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public long Delete(string collName, FilterDefinition<BsonDocument> filter)
    {
        var result = GetCollection(collName).DeleteOne(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public long DeleteMany(string collName, FilterDefinition<BsonDocument> filter)
    {
        var result = GetCollection(collName).DeleteMany(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync<TState>(FilterDefinition<TState> filter) where TState : class, ICacheState, new()
    {
        var result = await GetCollection<TState>().DeleteOneAsync(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync<TState>(FilterDefinition<TState> filter) where TState : class, ICacheState, new()
    {
        var result = await GetCollection<TState>().DeleteManyAsync(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync(string collName, FilterDefinition<BsonDocument> filter)
    {
        var result = await GetCollection(collName).DeleteOneAsync(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按检索条件删除
    /// 建议用Builders&lt;T&gt;构建复杂的查询条件
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync(string collName, FilterDefinition<BsonDocument> filter)
    {
        var result = await GetCollection(collName).DeleteManyAsync(filter);
        return result.DeletedCount;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public TState DeleteOne<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        var result = GetCollection<TState>().FindOneAndDelete(filter);
        return result;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public BsonDocument DeleteOne(string collName, Expression<Func<BsonDocument, bool>> filter)
    {
        var result = GetCollection(collName).FindOneAndDelete(filter);
        return result;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<TState> DeleteOneAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        TState result = await GetCollection<TState>().FindOneAndDeleteAsync(filter);
        return result;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<BsonDocument> DeleteOneAsync(string collName, Expression<Func<BsonDocument, bool>> filter)
    {
        var result = await GetCollection(collName).FindOneAndDeleteAsync(filter);
        return result;
    }

    #endregion 删除

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public async Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        var collection = GetCollection<TState>();
        var state = await FindAsync(filter);
        var newFilter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, state.Id);
        state.DeleteTime = TimeHelper.UnixTimeMilliseconds();
        state.IsDeleted = true;
        var result = await collection.ReplaceOneAsync(newFilter, state, ReplaceOptions);
        return result.ModifiedCount;
    }

    /// <summary>
    /// 删除一条数据
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    public async Task<long> DeleteAsync<TState>(TState state) where TState : class, ICacheState, new()
    {
        var filter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, state.Id);
        var collection = GetCollection<TState>();
        state.DeleteTime = TimeHelper.UnixTimeMilliseconds();
        state.IsDeleted = true;
        var result = await collection.ReplaceOneAsync(filter, state, ReplaceOptions);
        return result.ModifiedCount;
    }
}