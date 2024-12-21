using System.Linq.Expressions;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.Extension;
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
    /// 加载指定ID的缓存状态。
    /// </summary>
    /// <typeparam name="TState">缓存状态的类型。</typeparam>
    /// <param name="id">要加载的缓存状态的ID。</param>
    /// <param name="filter">默认值获取器。</param>
    /// <returns>加载的缓存状态。</returns>
    public async Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null) where TState : class, ICacheState, new()
    {
        var newFilter = Builders<TState>.Filter.Eq(BaseCacheState.UniqueId, id);
        var findExpression = GetDefaultFindExpression(filter);
        var filterDefinition = Builders<TState>.Filter.And(newFilter, findExpression);
        var col = GetCollection<TState>();
        using var cursor = await col.FindAsync(filterDefinition);
        var state = await cursor.FirstOrDefaultAsync();
        var isNew = state == null;

        if (state == null)
        {
            state = new TState { Id = id, };
        }

        state.LoadFromDbPostHandler(isNew);
        return state;
    }

    /// <summary>
    /// 异步查找满足指定条件的缓存状态列表。
    /// </summary>
    /// <typeparam name="TState">缓存状态的类型。</typeparam>
    /// <param name="filter">查询条件。</param>
    /// <returns>满足条件的缓存状态列表。</returns>
    public async Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        var result = new List<TState>();
        var collection = GetCollection<TState>();
        using var cursor = await collection.FindAsync<TState>(GetDefaultFindExpression(filter));
        while (await cursor.MoveNextAsync())
        {
            result.AddRange(cursor.Current);
        }

        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler(false);
        }

        return result;
    }

    /// <summary>
    /// 异步查找满足指定条件的缓存状态。
    /// </summary>
    /// <typeparam name="TState">缓存状态的类型。</typeparam>
    /// <param name="filter">查询条件。</param>
    /// <returns>满足条件的缓存状态。</returns>
    public async Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        var collection = GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var filterDefinition = Builders<TState>.Filter.Where(findExpression);
        using var cursor = await collection.FindAsync<TState>(filterDefinition);
        var state = await cursor.FirstOrDefaultAsync();
        state?.LoadFromDbPostHandler(false);
        return state;
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <returns>符合条件的第一个元素。</returns>
    public async Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : class, ICacheState, new()
    {
        var collection = GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Ascending(sortExpression);
        var cursor = collection.Aggregate().Match(findExpression).Sort(sortDefinition).Limit(1);
        var state = await cursor.FirstOrDefaultAsync();
        state?.LoadFromDbPostHandler(false);
        return state;
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <returns>符合条件的第一个元素。</returns>
    public async Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : class, ICacheState, new()
    {
        var collection = GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Descending(sortExpression);
        var cursor = collection.Aggregate().Match(findExpression).Sort(sortDefinition).Limit(1);
        var state = await cursor.FirstOrDefaultAsync();
        state?.LoadFromDbPostHandler(false);
        return state;
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <param name="pageIndex">页码，从0开始。</param>
    /// <param name="pageSize">每页数量，默认为10。</param>
    /// <returns>符合条件的元素列表。</returns>
    public async Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : class, ICacheState, new()
    {
        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var result = new List<TState>();
        var collection = GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Descending(sortExpression);
        var cursor = await collection.Aggregate().Match(findExpression).Sort(sortDefinition).Skip(pageIndex * pageSize).Limit(pageSize).ToCursorAsync();
        while (await cursor.MoveNextAsync())
        {
            result.AddRange(cursor.Current);
        }

        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler(false);
        }

        return result;
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <param name="pageIndex">页码，从0开始。</param>
    /// <param name="pageSize">每页数量，默认为10。</param>
    /// <returns>符合条件的元素列表。</returns>
    public async Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : class, ICacheState, new()
    {
        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var result = new List<TState>();
        var collection = GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Ascending(sortExpression);
        var cursor = await collection.Aggregate().Match(findExpression).Sort(sortDefinition).Skip(pageIndex * pageSize).Limit(pageSize).ToCursorAsync();
        while (await cursor.MoveNextAsync())
        {
            result.AddRange(cursor.Current);
        }

        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler(false);
        }

        return result;
    }

    /// <summary>
    /// 查询数据长度
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public async Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        var collection = GetCollection<TState>();
        var newFilter = GetDefaultFindExpression(filter);
        var count = await collection.CountDocumentsAsync<TState>(newFilter);
        return count;
    }

    /// <summary>
    /// 获取默认的查询表达式。
    /// </summary>
    /// <typeparam name="TState">缓存状态的类型。</typeparam>
    /// <param name="filter">自定义查询表达式。</param>
    /// <returns>默认的查询表达式。</returns>
    private static Expression<Func<TState, bool>> GetDefaultFindExpression<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        Expression<Func<TState, bool>> expression = m => m.IsDeleted == false;
        if (filter != null)
        {
            expression = expression.And(filter);
        }

        return expression;
    }

    #region 查询

    /*/// <summary>
    /// 查询，复杂查询直接用Linq处理
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <returns>要查询的对象</returns>
    public IMongoQueryable<TState> GetQueryable<TState>(string collName)
    {
        return CurrentDatabase.GetCollection<TState>(collName).AsQueryable();
    }*/

    /*
    /// <summary>
    /// 查询，复杂查询直接用Linq处理
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <returns>要查询的对象</returns>
    public IMongoQueryable<BsonDocument> GetQueryable(string collName)
    {
        return GetCollection(collName).AsQueryable();
    }*/

    /*
    /// <summary>
    /// 获取一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public TState Get<TState>(FilterDefinition<TState> filter) where TState : ICacheState, new()
    {
        var find = GetCollection<TState>().Find(filter);
        return find.FirstOrDefault();
    }*/

    /*/// <summary>
    /// 获取一条数据
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public BsonDocument Get(string collName, FilterDefinition<BsonDocument> filter)
    {
        var find = GetCollection(collName).Find(filter);
        return find.FirstOrDefault();
    }*/

    /*/// <summary>
    /// 获取一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<TState> GetAsync<TState>(string collName, FilterDefinition<TState> filter) where TState : ICacheState, new()
    {
        var find = await GetCollection<TState>().FindAsync(filter);
        return await find.FirstOrDefaultAsync();
    }*/

    /*
    /// <summary>
    /// 获取一条数据
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<BsonDocument> GetAsync(string collName, FilterDefinition<BsonDocument> filter)
    {
        var find = await GetCollection(collName).FindAsync(filter);
        return await find.FirstOrDefaultAsync();
    }
    */

    /*
    /// <summary>
    /// 获取多条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public IEnumerable<TState> GetMany<TState>(string collName, FilterDefinition<TState> filter) where TState : ICacheState, new()
    {
        var find = GetCollection<TState>().Find(filter);
        return find.ToEnumerable();
    }*/

    /*
    /// <summary>
    /// 获取多条数据
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public IEnumerable<BsonDocument> GetMany(string collName, FilterDefinition<BsonDocument> filter)
    {
        var find = GetCollection(collName).Find(filter);
        return find.ToEnumerable();
    }
    */

    /*
    /// <summary>
    /// 获取多条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<IEnumerable<TState>> GetManyAsync<TState>(string collName, FilterDefinition<TState> filter) where TState : ICacheState, new()
    {
        var find = await GetCollection<TState>().FindAsync(filter);
        return find.ToEnumerable();
    }
    */

    /*
    /// <summary>
    /// 获取多条数据
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<IEnumerable<BsonDocument>> GetManyAsync(string collName, FilterDefinition<BsonDocument> filter)
    {
        var find = await GetCollection(collName).FindAsync(filter);
        return find.ToEnumerable();
    }
    */


    /// <summary>
    /// 判断是否存在符合条件的数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public bool Any<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        filter = GetDefaultFindExpression(filter);
        var find = GetCollection<TState>().Find(filter);
        return find.Any();
    }

    /*
    /// <summary>
    /// 判断是否存在符合条件的数据
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public bool Any(string collName, FilterDefinition<BsonDocument> filter)
    {
        var find = GetCollection(collName).Find(filter);
        return find.Any();
    }
    */

    /// <summary>
    /// 判断是否存在符合条件的数据
    /// </summary>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
    {
        filter = GetDefaultFindExpression(filter);
        var find = await GetCollection<TState>().FindAsync(filter);
        return await find.AnyAsync();
    }

    /*
    /// <summary>
    /// 判断是否存在符合条件的数据
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <returns></returns>
    public async Task<bool> AnyAsync(string collName, FilterDefinition<BsonDocument> filter)
    {
        var find = await GetCollection(collName).FindAsync(filter);
        return await find.AnyAsync();
    }
    */

    #endregion 查询
}