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
using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.DataBase;

/// <summary>
/// 游戏数据库静态工具类,提供对数据库的基本操作封装。
/// </summary>
/// <remarks>
/// Static utility class for game database operations, providing basic database operation encapsulation.
/// </remarks>
public static class GameDb
{
    /// <summary>
    /// 数据库服务实现实例。
    /// </summary>
    /// <remarks>
    /// Database service implementation instance.
    /// </remarks>
    private static IDatabaseService _dbServiceImplementation;

    /// <summary>
    /// 初始化GameDb。
    /// </summary>
    /// <remarks>
    /// Initialize the GameDb instance.
    /// </remarks>
    /// <typeparam name="T">数据库服务的具体实现类型,必须实现IDatabaseService接口且有无参构造函数 / Database service implementation type, must implement IDatabaseService interface and have a parameterless constructor</typeparam>
    /// <param name="dbOptions">数据库配置选项 / Database configuration options</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="dbOptions"/> 的 ConnectionString 或 Name 为 null 时抛出 / Thrown when ConnectionString or Name of <paramref name="dbOptions"/> is null</exception>
    /// <returns>返回数据库是否初始化成功 / Returns whether the database was initialized successfully</returns>
    public static async Task<bool> Init<T>(DbOptions dbOptions) where T : IDatabaseService, new()
    {
        ArgumentNullException.ThrowIfNull(dbOptions, nameof(dbOptions));
        ArgumentNullException.ThrowIfNull(dbOptions.ConnectionString, nameof(dbOptions.ConnectionString));
        ArgumentNullException.ThrowIfNull(dbOptions.Name, nameof(dbOptions.Name));
        _dbServiceImplementation = new T();
        return await _dbServiceImplementation.Open(dbOptions);
    }

    /// <summary>
    /// 以指定类型获取数据库服务实例。
    /// </summary>
    /// <remarks>
    /// Get the database service instance as the specified type.
    /// </remarks>
    /// <typeparam name="T">要转换的数据库服务类型,必须实现IDatabaseService接口 / Database service type to convert to, must implement IDatabaseService interface</typeparam>
    /// <returns>转换后的数据库服务实例 / The converted database service instance</returns>
    /// <exception cref="InvalidCastException">当类型转换失败时抛出 / Thrown when type conversion fails</exception>
    public static T As<T>() where T : IDatabaseService
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return (T)_dbServiceImplementation;
    }

    /// <summary>
    /// 关闭数据库连接。
    /// </summary>
    /// <remarks>
    /// Close the database connection.
    /// </remarks>
    public static void Close()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        CloseAsync().GetAwaiter().GetResult();
    }

    /// <summary>
    /// 异步关闭数据库连接。
    /// </summary>
    /// <remarks>
    /// Asynchronously closes the database connection.
    /// </remarks>
    /// <returns>表示异步关闭操作的任务 / Task representing the asynchronous close operation</returns>
    public static Task CloseAsync()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.Close();
    }

    /// <summary>
    /// 查找与指定过滤器匹配的文档列表。
    /// </summary>
    /// <remarks>
    /// Find a list of documents matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <returns>匹配的文档列表 / List of matching documents</returns>
    public static Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindListAsync(filter);
    }

    /// <summary>
    /// 查找与指定过滤器匹配的文档列表。
    /// </summary>
    /// <remarks>
    /// Find a list of documents matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>匹配的文档列表 / List of matching documents</returns>
    public static Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindListAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 根据ID列表查找文档列表。
    /// </summary>
    /// <remarks>
    /// Find document list by ID list.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="ids">文档ID列表 / Document ID list</param>
    /// <returns>匹配的文档列表 / List of matching documents</returns>
    public static Task<List<TState>> FindByIdsAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindByIdsAsync<TState>(ids);
    }

    /// <summary>
    /// 根据ID列表查找文档列表。
    /// </summary>
    /// <remarks>
    /// Find document list by ID list.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="ids">文档ID列表 / Document ID list</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>匹配的文档列表 / List of matching documents</returns>
    public static Task<List<TState>> FindByIdsAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindByIdsAsync<TState>(ids, cancellationToken);
    }

    /// <summary>
    /// 查询分页数据并返回总数。
    /// </summary>
    /// <remarks>
    /// Query paginated data and return total count.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="descending">是否降序 / Whether descending</param>
    /// <param name="pageIndex">页码,从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Page size</param>
    /// <returns>分页结果和总数 / Paged result and total count</returns>
    public static Task<(List<TState> Items, long Total)> FindPageAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, bool descending, int pageIndex, int pageSize) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindPageAsync<TState>(filter, sortExpression, descending, pageIndex, pageSize);
    }

    /// <summary>
    /// 查询分页数据并返回总数。
    /// </summary>
    /// <remarks>
    /// Query paginated data and return total count.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="descending">是否降序 / Whether descending</param>
    /// <param name="pageIndex">页码,从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Page size</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页结果和总数 / Paged result and total count</returns>
    public static Task<(List<TState> Items, long Total)> FindPageAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, bool descending, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindPageAsync<TState>(filter, sortExpression, descending, pageIndex, pageSize, cancellationToken);
    }

    /// <summary>
    /// 投影查询文档列表。
    /// </summary>
    /// <remarks>
    /// Query projected document list.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <typeparam name="TResult">投影结果类型 / Projection result type</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="selector">投影表达式 / Projection expression</param>
    /// <returns>投影后的结果列表 / Projected result list</returns>
    public static Task<List<TResult>> FindProjectedAsync<TState, TResult>(Expression<Func<TState, bool>> filter, Expression<Func<TState, TResult>> selector) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindProjectedAsync<TState, TResult>(filter, selector);
    }

    /// <summary>
    /// 投影查询文档列表。
    /// </summary>
    /// <remarks>
    /// Query projected document list.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <typeparam name="TResult">投影结果类型 / Projection result type</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="selector">投影表达式 / Projection expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>投影后的结果列表 / Projected result list</returns>
    public static Task<List<TResult>> FindProjectedAsync<TState, TResult>(Expression<Func<TState, bool>> filter, Expression<Func<TState, TResult>> selector, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindProjectedAsync<TState, TResult>(filter, selector, cancellationToken);
    }

    /// <summary>
    /// 计算与指定过滤器匹配的文档数量。
    /// </summary>
    /// <remarks>
    /// Count the number of documents matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <returns>匹配的文档数量 / Number of matching documents</returns>
    public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.CountAsync(filter);
    }

    /// <summary>
    /// 计算与指定过滤器匹配的文档数量。
    /// </summary>
    /// <remarks>
    /// Count the number of documents matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>匹配的文档数量 / Number of matching documents</returns>
    public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.CountAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 计算与指定过滤器匹配的文档数量。
    /// </summary>
    /// <remarks>
    /// Count the number of documents matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="includeDeleted">是否包含软删除数据 / Whether to include soft-deleted data</param>
    /// <returns>匹配的文档数量 / Number of matching documents</returns>
    public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, bool includeDeleted) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.CountAsync(filter, includeDeleted);
    }

    /// <summary>
    /// 计算与指定过滤器匹配的文档数量。
    /// </summary>
    /// <remarks>
    /// Count the number of documents matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="includeDeleted">是否包含软删除数据 / Whether to include soft-deleted data</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>匹配的文档数量 / Number of matching documents</returns>
    public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, bool includeDeleted, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.CountAsync(filter, includeDeleted, cancellationToken);
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in ascending order.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null / The first matching element, or null if no match is found</returns>
    public static Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingFirstOneAsync(filter, sortExpression);
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in ascending order.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null / The first matching element, or null if no match is found</returns>
    public static Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingFirstOneAsync(filter, sortExpression, cancellationToken);
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in descending order.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null / The first matching element, or null if no match is found</returns>
    public static Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingFirstOneAsync(filter, sortExpression);
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in descending order.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null / The first matching element, or null if no match is found</returns>
    public static Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingFirstOneAsync(filter, sortExpression, cancellationToken);
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in descending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <param name="pageIndex">页码,从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量,默认为10 / Number of items per page, default is 10</param>
    /// <returns>分页后的文档列表 / Paginated list of documents</returns>
    public static Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingAsync(filter, sortExpression, pageIndex, pageSize);
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in descending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <param name="pageIndex">页码,从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Number of items per page</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页后的文档列表 / Paginated list of documents</returns>
    public static Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingAsync(filter, sortExpression, pageIndex, pageSize, cancellationToken);
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in ascending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <param name="pageIndex">页码,从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量,默认为10 / Number of items per page, default is 10</param>
    /// <returns>分页后的文档列表 / Paginated list of documents</returns>
    public static Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingAsync(filter, sortExpression, pageIndex, pageSize);
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in ascending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式 / Lambda expression for specifying the sort field</param>
    /// <param name="pageIndex">页码,从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Number of items per page</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页后的文档列表 / Paginated list of documents</returns>
    public static Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingAsync(filter, sortExpression, pageIndex, pageSize, cancellationToken);
    }

    /// <summary>
    /// 根据ID加载指定的文档。
    /// 当没有找到指定ID的文档时，会创建一个新的文档返回。
    /// </summary>
    /// <remarks>
    /// Load a document by the specified ID.
    /// When no document with the specified ID is found, a new document will be created and returned.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数 / Document type, must inherit from BaseCacheState and have a parameterless constructor</typeparam>
    /// <param name="id">要查找的文档ID / Document ID to find</param>
    /// <param name="filter">可选的附加过滤条件 / Optional additional filter condition</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <returns>找到的文档,如果不存在则返回新的空文档 / The found document, or a new empty document if not found</returns>
    public static Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null, bool isCreateIfNotExists = true) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(id, filter, isCreateIfNotExists);
    }

    /// <summary>
    /// 根据ID查找文档。
    /// </summary>
    /// <remarks>
    /// Find a document by ID.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">文档的唯一ID / Unique document ID</param>
    /// <param name="filter">附加过滤条件 / Additional filter condition</param>
    /// <param name="isCreateIfNotExists">当文档不存在时是否创建 / Whether to create when not found</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>找到的文档,如果未找到且不创建则返回null / Found document, or null when not found and creation is disabled</returns>
    public static Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter, bool isCreateIfNotExists, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(id, filter, isCreateIfNotExists, cancellationToken);
    }

    /// <summary>
    /// 查找与指定过滤器匹配的单个文档。
    /// 当没有找到指定ID的文档时，会创建一个新的文档返回。
    /// </summary>
    /// <remarks>
    /// Find a single document matching the specified filter.
    /// When no document is found, a new document will be created and returned.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <returns>找到的第一个匹配文档,如果没有匹配项则返回null / The first matching document, or null if no match is found</returns>
    public static Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists = true) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(filter, isCreateIfNotExists);
    }

    /// <summary>
    /// 查找与指定过滤器匹配的单个文档。
    /// </summary>
    /// <remarks>
    /// Find a single document matching the specified filter.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>找到的第一个匹配文档,如果没有匹配项则返回null / The first matching document, or null if no match is found</returns>
    public static Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(filter, isCreateIfNotExists, cancellationToken);
    }

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

    /// <summary>
    /// 根据ID检查文档是否存在。
    /// </summary>
    /// <remarks>
    /// Check whether the document exists by ID.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">文档ID / Document ID</param>
    /// <returns>如果存在匹配的文档则返回true,否则返回false / Returns true if matching document exists, otherwise false</returns>
    public static Task<bool> ExistsByIdAsync<TState>(long id) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.ExistsByIdAsync<TState>(id);
    }

    /// <summary>
    /// 根据ID检查文档是否存在。
    /// </summary>
    /// <remarks>
    /// Check whether the document exists by ID.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="id">文档ID / Document ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>如果存在匹配的文档则返回true,否则返回false / Returns true if matching document exists, otherwise false</returns>
    public static Task<bool> ExistsByIdAsync<TState>(long id, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.ExistsByIdAsync<TState>(id, cancellationToken);
    }

    /// <summary>
    /// 检查是否存在符合条件的文档。
    /// </summary>
    /// <remarks>
    /// Check if any documents matching the condition exist.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <returns>如果存在匹配的文档则返回true,否则返回false / Returns true if matching documents exist, otherwise false</returns>
    public static Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AnyAsync(filter);
    }

    /// <summary>
    /// 检查是否存在符合条件的文档。
    /// </summary>
    /// <remarks>
    /// Check if any documents matching the condition exist.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式 / Lambda expression for filtering documents</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>如果存在匹配的文档则返回true,否则返回false / Returns true if matching documents exist, otherwise false</returns>
    public static Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AnyAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 删除符合条件的文档。
    /// </summary>
    /// <remarks>
    /// Delete documents matching the condition.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选要删除文档的Lambda表达式 / Lambda expression for filtering documents to delete</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(filter);
    }

    /// <summary>
    /// 删除符合条件的文档。
    /// </summary>
    /// <remarks>
    /// Delete documents matching the condition.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选要删除文档的Lambda表达式 / Lambda expression for filtering documents to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 删除指定的文档。
    /// </summary>
    /// <remarks>
    /// Delete the specified document.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要删除的文档实例 / Document instance to delete</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(state);
    }

    /// <summary>
    /// 删除指定的文档。
    /// </summary>
    /// <remarks>
    /// Delete the specified document.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要删除的文档实例 / Document instance to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(state, cancellationToken);
    }

    /// <summary>
    /// 根据条件批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by condition (soft delete).
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListAsync<TState>(filter);
    }

    /// <summary>
    /// 根据条件批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by condition (soft delete).
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListAsync<TState>(filter, cancellationToken);
    }

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by ID list (soft delete).
    /// </remarks>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListIdAsync<TState>(ids);
    }

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by ID list (soft delete).
    /// </remarks>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListIdAsync<TState>(ids, cancellationToken);
    }

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回删除的记录数 / Number of deleted records</returns>
    public static Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.HardDeleteAsync<TState>(filter);
    }

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回删除的记录数 / Number of deleted records</returns>
    public static Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.HardDeleteAsync<TState>(filter, cancellationToken);
    }

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回恢复的记录数 / Number of restored records</returns>
    public static Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.RestoreAsync<TState>(filter);
    }

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回恢复的记录数 / Number of restored records</returns>
    public static Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.RestoreAsync<TState>(filter, cancellationToken);
    }
}
