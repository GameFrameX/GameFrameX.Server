using System.Linq.Expressions;

namespace GameFrameX.DataBase.Abstractions;

/// <summary>
/// 数据库服务
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// 链接数据库
    /// </summary>
    /// <param name="url">链接地址</param>
    /// <param name="dbName">数据库名称</param>
    void Open(string url, string dbName);

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    void Close();

    /// <summary>
    /// 查询单条数据
    /// </summary>
    /// <param name="id">数据的唯一ID</param>
    /// <param name="filter">查询条件</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回符合条件的数据对象</returns>
    Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null, bool isCreateIfNotExists = true) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询单条数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回符合条件的数据对象</returns>
    Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists = true) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>1
    /// <returns>返回符合条件的数据列表</returns>
    Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <returns>符合条件的第一个元素。</returns>
    Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <returns>符合条件的第一个元素。</returns>
    Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <param name="pageIndex">页码，从0开始。</param>
    /// <param name="pageSize">每页数量，默认为10。</param>
    /// <returns>符合条件的元素列表。</returns>
    Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <param name="pageIndex">页码，从0开始。</param>
    /// <param name="pageSize">每页数量，默认为10。</param>
    /// <returns>符合条件的元素列表。</returns>
    Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据长度
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回数据的长度</returns>
    Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回删除的条数</returns>
    Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回删除的条数</returns>
    Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件批量删除数据(软删除)
    /// </summary>
    /// <param name="filter">查询条件表达式</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)
    /// </summary>
    /// <param name="ids">要删除的ID列表</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回增加成功的条数</returns>
    Task AddAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 增加或更新数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>修改的条数</returns>
    Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存多条数据
    /// </summary>
    /// <param name="states">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns></returns>
    Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据,如果数据已经存在则更新,如果不存在则插入
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回更新后的数据对象</returns>
    Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存多条数据
    /// </summary>
    /// <param name="stateList">数据列表对象</param>
    /// <returns>返回更新成功的数量</returns>
    Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询符合条件的数据是否存在
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回是否存在值,true表示存在，false表示不存在</returns>
    Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();
}