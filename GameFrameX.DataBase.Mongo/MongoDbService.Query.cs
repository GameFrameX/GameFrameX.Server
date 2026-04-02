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
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Utility;
using GameFrameX.Utility;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

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
    /// 异步加载指定ID的缓存状态。
    /// 此方法尝试从MongoDB中查找与给定ID匹配的缓存状态。
    /// 如果找到状态，则返回该状态；如果未找到，则创建一个新的状态实例。
    /// </summary>
    /// <remarks>
    /// Asynchronously loads the cache state with the specified ID.
    /// This method attempts to find a cache state matching the given ID from MongoDB.
    /// If found, returns the state; if not found, creates a new state instance.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型，必须是BaseCacheState的子类，并具有无参数构造函数 / The type of cache state, must be a subclass of BaseCacheState with a parameterless constructor</typeparam>
    /// <param name="id">要加载的缓存状态的ID / The ID of the cache state to load</param>
    /// <param name="filter">可选的过滤器，用于进一步限制查询结果的条件 / Optional filter to further restrict query results</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create document if it doesn't exist</param>
    /// <returns>加载的缓存状态，如果未找到则返回新创建的状态 / The loaded cache state, or a new state if not found</returns>
    public async Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null, bool isCreateIfNotExists = true) where TState : BaseCacheState, new()
    {
        return await FindAsync(id, filter, isCreateIfNotExists, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步加载指定ID的缓存状态。
    /// </summary>
    /// <remarks>
    /// Asynchronously loads the cache state with the specified ID.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型，必须是BaseCacheState的子类，并具有无参数构造函数 / The type of cache state, must be a subclass of BaseCacheState with a parameterless constructor</typeparam>
    /// <param name="id">要加载的缓存状态的ID / The ID of the cache state to load</param>
    /// <param name="filter">可选的过滤器，用于进一步限制查询结果的条件 / Optional filter to further restrict query results</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create document if it doesn't exist</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>加载的缓存状态，如果未找到则返回新创建的状态 / The loaded cache state, or a new state if not found</returns>
    public async Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter, bool isCreateIfNotExists, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var mongoFilter = Builders<TState>.Filter.Eq(m => m.Id, id) & Builders<TState>.Filter.Where(findExpression);
        var state = await ExecuteReadWithRetryAsync(token => collection.Find(mongoFilter).FirstOrDefaultAsync(token), cancellationToken, nameof(FindAsync)).ConfigureAwait(false);

        if (!isCreateIfNotExists)
        {
            return state;
        }

        var isNew = state == null;

        if (state == null)
        {
            // 如果未找到状态，则创建一个新的状态实例，并设置其ID和创建时间
            state = new TState { Id = id, CreatedTime = GetCurrentTimestamp(), };
        }

        // 调用后处理方法以加载状态的其他数据
        state.LoadFromDbPostHandler(isNew);

        return state;
    }

    /// <summary>
    /// 异步查找满足指定条件的缓存状态。
    /// 如果没有找到满足条件的状态，则会创建一个新的状态实例。
    /// </summary>
    /// <remarks>
    /// Asynchronously finds the cache state that satisfies the specified condition.
    /// If no state is found that satisfies the condition, a new state instance is created.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型，必须是BaseCacheState的子类，并具有无参数构造函数 / The type of cache state, must be a subclass of BaseCacheState with a parameterless constructor</typeparam>
    /// <param name="filter">查询条件，用于限制查找的结果 / Query condition to restrict search results</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create document if it doesn't exist</param>
    /// <returns>满足条件的缓存状态，如果未找到则返回新创建的状态 / The cache state that satisfies the condition, or a new state if not found</returns>
    public async Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists = true) where TState : BaseCacheState, new()
    {
        return await FindAsync(filter, isCreateIfNotExists, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步查找满足指定条件的缓存状态。
    /// </summary>
    /// <remarks>
    /// Asynchronously finds the cache state that satisfies the specified condition.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型，必须是BaseCacheState的子类，并具有无参数构造函数 / The type of cache state, must be a subclass of BaseCacheState with a parameterless constructor</typeparam>
    /// <param name="filter">查询条件，用于限制查找的结果 / Query condition to restrict search results</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create document if it doesn't exist</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>满足条件的缓存状态，如果未找到则返回新创建的状态 / The cache state that satisfies the condition, or a new state if not found</returns>
    public async Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var state = await ExecuteReadWithRetryAsync(token => collection.AsQueryable().Where(findExpression).FirstOrDefaultAsync(token), cancellationToken, nameof(FindAsync)).ConfigureAwait(false);

        if (!isCreateIfNotExists)
        {
            return state;
        }

        var isNew = state == null;

        if (state == null)
        {
            // 如果未找到状态，则创建一个新的状态实例，并设置其ID和创建时间
            state = new TState { Id = IdGenerator.GetNextUniqueId(), CreatedTime = GetCurrentTimestamp(), };
        }

        // 调用后处理方法以加载状态的其他数据
        state.LoadFromDbPostHandler(isNew);

        return state;
    }

    /// <summary>
    /// 异步查找满足指定条件的缓存状态列表。
    /// </summary>
    /// <remarks>
    /// Asynchronously finds a list of cache states that satisfy the specified condition.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <returns>满足条件的缓存状态列表 / List of cache states that satisfy the condition</returns>
    public async Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await FindListAsync(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 异步查找满足指定条件的缓存状态列表。
    /// </summary>
    /// <remarks>
    /// Asynchronously finds a list of cache states that satisfy the specified condition.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>满足条件的缓存状态列表 / List of cache states that satisfy the condition</returns>
    public async Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var result = await ExecuteReadWithRetryAsync(token => collection.AsQueryable().Where(findExpression).ToListAsync(token), cancellationToken, nameof(FindListAsync)).ConfigureAwait(false);
        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler();
        }

        return result;
    }

    /// <summary>
    /// 根据ID列表查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query data list by ID list.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="ids">ID列表 / ID list</param>
    /// <returns>满足条件的缓存状态列表 / List of cache states that satisfy the condition</returns>
    public async Task<List<TState>> FindByIdsAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        return await FindByIdsAsync<TState>(ids, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 根据ID列表查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query data list by ID list.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="ids">ID列表 / ID list</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>满足条件的缓存状态列表 / List of cache states that satisfy the condition</returns>
    public async Task<List<TState>> FindByIdsAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var idArray = ids?.Distinct().ToArray();
        if (idArray == null || idArray.Length == 0)
        {
            return new List<TState>();
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        var defaultFilter = Builders<TState>.Filter.Where(GetDefaultFindExpression<TState>(null));
        var idFilter = Builders<TState>.Filter.In(m => m.Id, idArray);
        var result = await ExecuteReadWithRetryAsync(token => collection.Find(defaultFilter & idFilter).ToListAsync(token), cancellationToken, nameof(FindByIdsAsync)).ConfigureAwait(false);
        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler();
        }

        return result;
    }

    /// <summary>
    /// 查询分页数据，并返回总数。
    /// </summary>
    /// <remarks>
    /// Query paginated data and return total count.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="descending">是否降序 / Whether descending</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Page size</param>
    /// <returns>分页数据和总数 / Paged items and total count</returns>
    public async Task<(List<TState> Items, long Total)> FindPageAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, bool descending, int pageIndex, int pageSize) where TState : BaseCacheState, new()
    {
        return await FindPageAsync<TState>(filter, sortExpression, descending, pageIndex, pageSize, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 查询分页数据，并返回总数。
    /// </summary>
    /// <remarks>
    /// Query paginated data and return total count.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="descending">是否降序 / Whether descending</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Page size</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页数据和总数 / Paged items and total count</returns>
    public async Task<(List<TState> Items, long Total)> FindPageAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, bool descending, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = descending ? Builders<TState>.Sort.Descending(sortExpression) : Builders<TState>.Sort.Ascending(sortExpression);
        var total = await ExecuteReadWithRetryAsync(token => collection.CountDocumentsAsync(findExpression, cancellationToken: token), cancellationToken, nameof(FindPageAsync)).ConfigureAwait(false);
        var items = await ExecuteReadWithRetryAsync(token => collection.Find(findExpression).Sort(sortDefinition).Skip(pageIndex * pageSize).Limit(pageSize).ToListAsync(token), cancellationToken, nameof(FindPageAsync)).ConfigureAwait(false);
        foreach (var state in items)
        {
            state?.LoadFromDbPostHandler();
        }

        return (items, total);
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Finds the first element that matches the condition in ascending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <returns>符合条件的第一个元素 / The first element that matches the condition</returns>
    public async Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new()
    {
        return await FindSortAscendingFirstOneAsync(filter, sortExpression, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Finds the first element that matches the condition in ascending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的第一个元素 / The first element that matches the condition</returns>
    public async Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Ascending(sortExpression);
        var state = await ExecuteReadWithRetryAsync(token => collection.Find(findExpression).Sort(sortDefinition).Limit(1).FirstOrDefaultAsync(token), cancellationToken, nameof(FindSortAscendingFirstOneAsync)).ConfigureAwait(false);
        state?.LoadFromDbPostHandler();
        return state;
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Finds the first element that matches the condition in descending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <returns>符合条件的第一个元素 / The first element that matches the condition</returns>
    public async Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new()
    {
        return await FindSortDescendingFirstOneAsync(filter, sortExpression, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Finds the first element that matches the condition in descending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的第一个元素 / The first element that matches the condition</returns>
    public async Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Descending(sortExpression);
        var state = await ExecuteReadWithRetryAsync(token => collection.Find(findExpression).Sort(sortDefinition).Limit(1).FirstOrDefaultAsync(token), cancellationToken, nameof(FindSortDescendingFirstOneAsync)).ConfigureAwait(false);
        state?.LoadFromDbPostHandler();
        return state;
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Finds elements that match the condition in descending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量，默认为10 / Page size, defaults to 10</param>
    /// <returns>符合条件的元素列表 / List of elements that match the condition</returns>
    public async Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new()
    {
        return await FindSortDescendingAsync(filter, sortExpression, pageIndex, pageSize, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Finds elements that match the condition in descending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量，默认为10 / Page size, defaults to 10</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的元素列表 / List of elements that match the condition</returns>
    public async Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Descending(sortExpression);
        var result = await ExecuteReadWithRetryAsync(token => collection.Find(findExpression).Sort(sortDefinition).Skip(pageIndex * pageSize).Limit(pageSize).ToListAsync(token), cancellationToken, nameof(FindSortDescendingAsync)).ConfigureAwait(false);
        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler();
        }

        return result;
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Finds elements that match the condition in ascending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量，默认为10 / Page size, defaults to 10</param>
    /// <returns>符合条件的元素列表 / List of elements that match the condition</returns>
    public async Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new()
    {
        return await FindSortAscendingAsync(filter, sortExpression, pageIndex, pageSize, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Finds elements that match the condition in ascending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type that implements ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量，默认为10 / Page size, defaults to 10</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的元素列表 / List of elements that match the condition</returns>
    public async Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        if (pageIndex < 0)
        {
            pageIndex = 0;
        }

        if (pageSize <= 0)
        {
            pageSize = 10;
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var sortDefinition = Builders<TState>.Sort.Ascending(sortExpression);
        var result = await ExecuteReadWithRetryAsync(token => collection.Find(findExpression).Sort(sortDefinition).Skip(pageIndex * pageSize).Limit(pageSize).ToListAsync(token), cancellationToken, nameof(FindSortAscendingAsync)).ConfigureAwait(false);
        foreach (var state in result)
        {
            state?.LoadFromDbPostHandler();
        }

        return result;
    }

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Queries the count of data.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <returns>符合条件的数据数量 / The count of data that matches the condition</returns>
    public async Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await CountAsync(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Queries the count of data.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的数据数量 / The count of data that matches the condition</returns>
    public async Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var newFilter = GetDefaultFindExpression(filter);
        var count = await ExecuteReadWithRetryAsync(token => collection.CountDocumentsAsync(newFilter, cancellationToken: token), cancellationToken, nameof(CountAsync)).ConfigureAwait(false);
        return count;
    }

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Queries the count of data.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="includeDeleted">是否包含已删除数据 / Whether to include deleted data</param>
    /// <returns>符合条件的数据数量 / The count of data that matches the condition</returns>
    public async Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, bool includeDeleted) where TState : BaseCacheState, new()
    {
        return await CountAsync(filter, includeDeleted, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Queries the count of data.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="includeDeleted">是否包含已删除数据 / Whether to include deleted data</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的数据数量 / The count of data that matches the condition</returns>
    public async Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, bool includeDeleted, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        Expression<Func<TState, bool>> newFilter = includeDeleted ? filter ?? (_ => true) : GetDefaultFindExpression(filter);
        var count = await ExecuteReadWithRetryAsync(token => collection.CountDocumentsAsync(newFilter, cancellationToken: token), cancellationToken, nameof(CountAsync)).ConfigureAwait(false);
        return count;
    }

    /// <summary>
    /// 投影查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query projected data list.
    /// </remarks>
    /// <typeparam name="TState">源数据类型 / Source data type</typeparam>
    /// <typeparam name="TResult">结果数据类型 / Result data type</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="selector">投影表达式 / Projection expression</param>
    /// <returns>投影后的数据列表 / Projected data list</returns>
    public async Task<List<TResult>> FindProjectedAsync<TState, TResult>(Expression<Func<TState, bool>> filter, Expression<Func<TState, TResult>> selector) where TState : BaseCacheState, new()
    {
        return await FindProjectedAsync(filter, selector, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 投影查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query projected data list.
    /// </remarks>
    /// <typeparam name="TState">源数据类型 / Source data type</typeparam>
    /// <typeparam name="TResult">结果数据类型 / Result data type</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="selector">投影表达式 / Projection expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>投影后的数据列表 / Projected data list</returns>
    public async Task<List<TResult>> FindProjectedAsync<TState, TResult>(Expression<Func<TState, bool>> filter, Expression<Func<TState, TResult>> selector, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var findExpression = GetDefaultFindExpression(filter);
        var result = await ExecuteReadWithRetryAsync(token => collection.AsQueryable().Where(findExpression).Select(selector).ToListAsync(token), cancellationToken, nameof(FindProjectedAsync)).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// 获取默认的查询表达式。
    /// </summary>
    /// <remarks>
    /// Gets the default query expression.
    /// </remarks>
    /// <typeparam name="TState">缓存状态的类型 / The type of cache state</typeparam>
    /// <param name="filter">自定义查询表达式 / Custom query expression</param>
    /// <returns>默认的查询表达式 / Default query expression</returns>
    private static Expression<Func<TState, bool>> GetDefaultFindExpression<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        Expression<Func<TState, bool>> expression = m => m.IsDeleted == null || m.IsDeleted == false;
        if (filter != null)
        {
            expression = expression.And(filter);
        }

        return expression;
    }

    /// <summary>
    /// 判断是否存在符合条件的数据。
    /// </summary>
    /// <remarks>
    /// Determines whether any data exists that matches the condition.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <returns>如果存在符合条件的数据则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if data exists that matches the condition; otherwise <c>false</c></returns>
    public async Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        return await AnyAsync(filter, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 判断是否存在符合条件的数据。
    /// </summary>
    /// <remarks>
    /// Determines whether any data exists that matches the condition.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>如果存在符合条件的数据则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if data exists that matches the condition; otherwise <c>false</c></returns>
    public async Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        filter = GetDefaultFindExpression(filter);
        var result = await ExecuteReadWithRetryAsync(token => collection.AsQueryable().AnyAsync(filter, token), cancellationToken, nameof(AnyAsync)).ConfigureAwait(false);
        return result;
    }

    /// <summary>
    /// 根据ID判断数据是否存在。
    /// </summary>
    /// <remarks>
    /// Determines whether data exists by ID.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">数据ID / Data ID</param>
    /// <returns>如果存在符合条件的数据则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if data exists; otherwise <c>false</c></returns>
    public Task<bool> ExistsByIdAsync<TState>(long id) where TState : BaseCacheState, new()
    {
        return ExistsByIdAsync<TState>(id, CancellationToken.None);
    }

    /// <summary>
    /// 根据ID判断数据是否存在。
    /// </summary>
    /// <remarks>
    /// Determines whether data exists by ID.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">数据ID / Data ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>如果存在符合条件的数据则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if data exists; otherwise <c>false</c></returns>
    public async Task<bool> ExistsByIdAsync<TState>(long id, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var collection = _mongoDbContext.GetCollection<TState>();
        var filter = Builders<TState>.Filter.Eq(m => m.Id, id) & Builders<TState>.Filter.Where(GetDefaultFindExpression<TState>(null));
        return await ExecuteReadWithRetryAsync(token => collection.Find(filter).Limit(1).AnyAsync(token), cancellationToken, nameof(ExistsByIdAsync)).ConfigureAwait(false);
    }
}
