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

namespace GameFrameX.DataBase.Abstractions;

/// <summary>
/// 数据库服务接口。
/// </summary>
/// <remarks>
/// Database service interface.
/// </remarks>
public interface IDatabaseService
{
    /// <summary>
    /// 连接数据库。
    /// </summary>
    /// <remarks>
    /// Connect to the database.
    /// </remarks>
    /// <param name="dbOptions">数据库配置选项 / Database configuration options</param>
    /// <returns>返回数据库是否初始化成功 / Returns whether the database was initialized successfully</returns>
    Task<bool> Open(DbOptions dbOptions);

    /// <summary>
    /// 关闭数据库连接。
    /// </summary>
    /// <remarks>
    /// Close the database connection.
    /// </remarks>
    /// <returns>异步任务 / Async task</returns>
    Task Close();

    /// <summary>
    /// 查询单条数据。
    /// </summary>
    /// <remarks>
    /// Query a single record by ID.
    /// </remarks>
    /// <param name="id">数据的唯一ID / Unique ID of the data</param>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据对象 / Returns the matching data object</returns>
    Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null, bool isCreateIfNotExists = true) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询单条数据。
    /// </summary>
    /// <remarks>
    /// Query a single record by ID.
    /// </remarks>
    /// <param name="id">数据的唯一ID / Unique ID of the data</param>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据对象 / Returns the matching data object</returns>
    Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter, bool isCreateIfNotExists, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询单条数据。
    /// </summary>
    /// <remarks>
    /// Query a single record by filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据对象 / Returns the matching data object</returns>
    Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists = true) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询单条数据。
    /// </summary>
    /// <remarks>
    /// Query a single record by filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档 / Whether to create the document if it does not exist</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据对象 / Returns the matching data object</returns>
    Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query a list of data by filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据列表 / Returns the list of matching data</returns>
    Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query a list of data by filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据列表 / Returns the list of matching data</returns>
    Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID列表查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query data list by ID list.
    /// </remarks>
    /// <param name="ids">ID列表 / ID list</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据列表 / Returns the list of matching data</returns>
    Task<List<TState>> FindByIdsAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID列表查询数据列表。
    /// </summary>
    /// <remarks>
    /// Query data list by ID list.
    /// </remarks>
    /// <param name="ids">ID列表 / ID list</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回符合条件的数据列表 / Returns the list of matching data</returns>
    Task<List<TState>> FindByIdsAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询分页数据，并返回总数。
    /// </summary>
    /// <remarks>
    /// Query paginated data and return total count.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="descending">是否降序 / Whether descending</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Page size</param>
    /// <returns>分页数据和总数 / Paged items and total count</returns>
    Task<(List<TState> Items, long Total)> FindPageAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, bool descending, int pageIndex, int pageSize) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询分页数据，并返回总数。
    /// </summary>
    /// <remarks>
    /// Query paginated data and return total count.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="descending">是否降序 / Whether descending</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Page size</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>分页数据和总数 / Paged items and total count</returns>
    Task<(List<TState> Items, long Total)> FindPageAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, bool descending, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new();

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
    Task<List<TResult>> FindProjectedAsync<TState, TResult>(Expression<Func<TState, bool>> filter, Expression<Func<TState, TResult>> selector) where TState : BaseCacheState, new();

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
    Task<List<TResult>> FindProjectedAsync<TState, TResult>(Expression<Func<TState, bool>> filter, Expression<Func<TState, TResult>> selector, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in ascending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <returns>符合条件的第一个元素 / The first matching element</returns>
    Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in ascending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的第一个元素 / The first matching element</returns>
    Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in descending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <returns>符合条件的第一个元素 / The first matching element</returns>
    Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <remarks>
    /// Find the first element matching the condition in descending order.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的第一个元素 / The first matching element</returns>
    Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in descending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量，默认为10 / Number of items per page, default is 10</param>
    /// <returns>符合条件的元素列表 / List of matching elements</returns>
    Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in descending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Number of items per page</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的元素列表 / List of matching elements</returns>
    Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in ascending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量，默认为10 / Number of items per page, default is 10</param>
    /// <returns>符合条件的元素列表 / List of matching elements</returns>
    Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <remarks>
    /// Find elements matching the condition in ascending order with pagination.
    /// </remarks>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <param name="filter">过滤表达式 / Filter expression</param>
    /// <param name="sortExpression">排序字段表达式 / Sort field expression</param>
    /// <param name="pageIndex">页码，从0开始 / Page index, starting from 0</param>
    /// <param name="pageSize">每页数量 / Number of items per page</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>符合条件的元素列表 / List of matching elements</returns>
    Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex, int pageSize, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Count the number of records matching the filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回数据的数量 / Returns the count of data</returns>
    Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Count the number of records matching the filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回数据的数量 / Returns the count of data</returns>
    Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Count the number of records matching the filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="includeDeleted">是否包含已删除数据 / Whether to include deleted data</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回数据的数量 / Returns the count of data</returns>
    Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, bool includeDeleted) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据数量。
    /// </summary>
    /// <remarks>
    /// Count the number of records matching the filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="includeDeleted">是否包含已删除数据 / Whether to include deleted data</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回数据的数量 / Returns the count of data</returns>
    Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter, bool includeDeleted, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <remarks>
    /// Delete data matching the filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回删除的条数 / Returns the number of deleted records</returns>
    Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <remarks>
    /// Delete data matching the filter.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回删除的条数 / Returns the number of deleted records</returns>
    Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <remarks>
    /// Delete the specified data object.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回删除的条数 / Returns the number of deleted records</returns>
    Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据。
    /// </summary>
    /// <remarks>
    /// Delete the specified data object.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回删除的条数 / Returns the number of deleted records</returns>
    Task<long> DeleteAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回删除的记录数 / Returns the number of deleted records</returns>
    Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回删除的记录数 / Returns the number of deleted records</returns>
    Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回恢复的记录数 / Returns the number of restored records</returns>
    Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回恢复的记录数 / Returns the number of restored records</returns>
    Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by condition (soft delete).
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Returns the number of modified records</returns>
    Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by condition (soft delete).
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Returns the number of modified records</returns>
    Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by ID list (soft delete).
    /// </remarks>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Returns the number of modified records</returns>
    Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by ID list (soft delete).
    /// </remarks>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Returns the number of modified records</returns>
    Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据。
    /// </summary>
    /// <remarks>
    /// Save data to the database.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>异步任务 / Async task</returns>
    Task AddAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据。
    /// </summary>
    /// <remarks>
    /// Save data to the database.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>异步任务 / Async task</returns>
    Task AddAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 增加或更新数据。
    /// </summary>
    /// <remarks>
    /// Add or update data.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>修改后的数据对象 / The modified data object</returns>
    Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 增加或更新数据。
    /// </summary>
    /// <remarks>
    /// Add or update data.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>修改后的数据对象 / The modified data object</returns>
    Task<TState> AddOrUpdateAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 批量增加或更新数据。
    /// </summary>
    /// <remarks>
    /// Batch add or update data.
    /// </remarks>
    /// <param name="states">数据对象集合 / Collection of data objects</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回处理的记录数 / Returns the number of processed records</returns>
    Task<long> AddOrUpdateListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new();

    /// <summary>
    /// 批量增加或更新数据。
    /// </summary>
    /// <remarks>
    /// Batch add or update data.
    /// </remarks>
    /// <param name="states">数据对象集合 / Collection of data objects</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回处理的记录数 / Returns the number of processed records</returns>
    Task<long> AddOrUpdateListAsync<TState>(IEnumerable<TState> states, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 批量保存数据。
    /// </summary>
    /// <remarks>
    /// Batch save multiple data objects.
    /// </remarks>
    /// <param name="states">数据对象集合 / Collection of data objects</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>异步任务 / Async task</returns>
    Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new();

    /// <summary>
    /// 批量保存数据。
    /// </summary>
    /// <remarks>
    /// Batch save multiple data objects.
    /// </remarks>
    /// <param name="states">数据对象集合 / Collection of data objects</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>异步任务 / Async task</returns>
    Task AddListAsync<TState>(IEnumerable<TState> states, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据,如果数据已经存在则更新,如果不存在则插入。
    /// </summary>
    /// <remarks>
    /// Save data, update if exists, insert if not.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回更新后的数据对象 / Returns the updated data object</returns>
    Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据,如果数据已经存在则更新,如果不存在则插入。
    /// </summary>
    /// <remarks>
    /// Save data, update if exists, insert if not.
    /// </remarks>
    /// <param name="state">数据对象 / Data object</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回更新后的数据对象 / Returns the updated data object</returns>
    Task<TState> UpdateAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 批量更新数据。
    /// </summary>
    /// <remarks>
    /// Batch update multiple data objects.
    /// </remarks>
    /// <param name="stateList">数据列表对象 / List of data objects</param>
    /// <returns>返回更新成功的数量 / Returns the number of successfully updated records</returns>
    Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState, new();

    /// <summary>
    /// 批量更新数据。
    /// </summary>
    /// <remarks>
    /// Batch update multiple data objects.
    /// </remarks>
    /// <param name="stateList">数据列表对象 / List of data objects</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>返回更新成功的数量 / Returns the number of successfully updated records</returns>
    Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID部分更新数据。
    /// </summary>
    /// <remarks>
    /// Partially update data by ID.
    /// </remarks>
    /// <param name="id">数据ID / Data ID</param>
    /// <param name="updateFields">更新字段集合 / Update field dictionary</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回更新成功的数量 / Returns the number of successfully updated records</returns>
    Task<long> UpdatePartialAsync<TState>(long id, IReadOnlyDictionary<string, object> updateFields) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID部分更新数据。
    /// </summary>
    /// <remarks>
    /// Partially update data by ID.
    /// </remarks>
    /// <param name="id">数据ID / Data ID</param>
    /// <param name="updateFields">更新字段集合 / Update field dictionary</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回更新成功的数量 / Returns the number of successfully updated records</returns>
    Task<long> UpdatePartialAsync<TState>(long id, IReadOnlyDictionary<string, object> updateFields, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 在事务中执行操作。
    /// </summary>
    /// <remarks>
    /// Execute operation in a transaction.
    /// </remarks>
    /// <param name="action">事务内执行逻辑 / Action to execute within transaction</param>
    /// <returns>异步任务 / Async task</returns>
    Task ExecuteInTransactionAsync(Func<Task> action);

    /// <summary>
    /// 在事务中执行操作。
    /// </summary>
    /// <remarks>
    /// Execute operation in a transaction.
    /// </remarks>
    /// <param name="action">事务内执行逻辑 / Action to execute within transaction</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>异步任务 / Async task</returns>
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken);

    /// <summary>
    /// 根据ID判断数据是否存在。
    /// </summary>
    /// <remarks>
    /// Determines whether data exists by ID.
    /// </remarks>
    /// <param name="id">数据ID / Data ID</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回是否存在值,true表示存在，false表示不存在 / Returns whether data exists, true if exists, false if not</returns>
    Task<bool> ExistsByIdAsync<TState>(long id) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID判断数据是否存在。
    /// </summary>
    /// <remarks>
    /// Determines whether data exists by ID.
    /// </remarks>
    /// <param name="id">数据ID / Data ID</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回是否存在值,true表示存在，false表示不存在 / Returns whether data exists, true if exists, false if not</returns>
    Task<bool> ExistsByIdAsync<TState>(long id, CancellationToken cancellationToken) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询符合条件的数据是否存在。
    /// </summary>
    /// <remarks>
    /// Check if any data matching the condition exists.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回是否存在值,true表示存在，false表示不存在 / Returns whether data exists, true if exists, false if not</returns>
    Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询符合条件的数据是否存在。
    /// </summary>
    /// <remarks>
    /// Check if any data matching the condition exists.
    /// </remarks>
    /// <param name="filter">查询条件 / Query condition</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">实现ICacheState接口的类型 / Type implementing ICacheState interface</typeparam>
    /// <returns>返回是否存在值,true表示存在，false表示不存在 / Returns whether data exists, true if exists, false if not</returns>
    Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new();
}
