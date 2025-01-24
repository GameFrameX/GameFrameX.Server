using System.Linq.Expressions;
using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.DataBase;

/// <summary>
/// 游戏数据库静态工具类,提供对数据库的基本操作封装
/// </summary>
public static class GameDb
{
    /// <summary>
    /// 数据库服务实现实例
    /// </summary>
    private static IDatabaseService _dbServiceImplementation;

    /// <summary>
    /// 初始化GameDb
    /// </summary>
    /// <typeparam name="T">数据库服务的具体实现类型,必须实现IDatabaseService接口且有无参构造函数</typeparam>
    /// <param name="mongoUrl">MongoDB连接URL,不能为null</param>
    /// <param name="mongoDbName">MongoDB数据库的名称,不能为null</param>
    /// <exception cref="ArgumentNullException">当mongoUrl或mongoDbName为null时抛出</exception>
    public static void Init<T>(string mongoUrl, string mongoDbName) where T : IDatabaseService, new()
    {
        ArgumentNullException.ThrowIfNull(mongoUrl, nameof(mongoUrl));
        ArgumentNullException.ThrowIfNull(mongoDbName, nameof(mongoDbName));
        _dbServiceImplementation = new T();
        _dbServiceImplementation.Open(mongoUrl, mongoDbName);
    }

    /// <summary>
    /// 以指定类型获取数据库服务实例
    /// </summary>
    /// <typeparam name="T">要转换的数据库服务类型,必须实现IDatabaseService接口</typeparam>
    /// <returns>转换后的数据库服务实例</returns>
    /// <exception cref="InvalidCastException">当类型转换失败时抛出</exception>
    public static T As<T>() where T : IDatabaseService
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return (T)_dbServiceImplementation;
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public static void Close()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        _dbServiceImplementation.Close();
    }

    /// <summary>
    /// 查找与指定过滤器匹配的文档列表
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>匹配的文档列表</returns>
    public static Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindListAsync(filter);
    }

    /// <summary>
    /// 计算与指定过滤器匹配的文档数量
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>匹配的文档数量</returns>
    public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.CountAsync(filter);
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null</returns>
    public static Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingFirstOneAsync(filter, sortExpression);
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null</returns>
    public static Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingFirstOneAsync(filter, sortExpression);
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <param name="pageIndex">页码,从0开始</param>
    /// <param name="pageSize">每页数量,默认为10</param>
    /// <returns>分页后的文档列表</returns>
    public static Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingAsync(filter, sortExpression, pageIndex, pageSize);
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <param name="pageIndex">页码,从0开始</param>
    /// <param name="pageSize">每页数量,默认为10</param>
    /// <returns>分页后的文档列表</returns>
    public static Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingAsync(filter, sortExpression, pageIndex, pageSize);
    }

    /// <summary>
    /// 根据ID加载指定的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数</typeparam>
    /// <param name="id">要查找的文档ID</param>
    /// <param name="filter">可选的附加过滤条件</param>
    /// <returns>找到的文档,如果不存在则返回新的空文档</returns>
    public static Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(id, filter);
    }

    /// <summary>
    /// 查找与指定过滤器匹配的单个文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>找到的第一个匹配文档,如果没有匹配项则返回null</returns>
    public static Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(filter);
    }

    /// <summary>
    /// 更新指定的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="state">要更新的文档实例</param>
    /// <returns>更新后的文档</returns>
    public static Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(state);
    }

    /// <summary>
    /// 保存单个文档到数据库
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="state">要保存的文档实例</param>
    /// <returns>保存操作的结果</returns>
    public static Task SaveOneAsync<TState>(TState state) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddAsync(state);
    }

    /// <summary>
    /// 添加或更新文档(如果存在则更新,不存在则添加)
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数</typeparam>
    /// <param name="state">要保存或更新的文档实例</param>
    /// <returns>保存或更新后的文档</returns>
    public static Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddOrUpdateAsync(state);
    }

    /// <summary>
    /// 批量保存多个文档到数据库
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="states">要保存的文档集合</param>
    /// <returns>保存操作的任务</returns>
    public static Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddListAsync(states);
    }

    /// <summary>
    /// 批量更新多个文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="stateList">要更新的文档集合</param>
    /// <returns>成功更新的文档数量</returns>
    public static Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(stateList);
    }

    /// <summary>
    /// 检查是否存在符合条件的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>如果存在匹配的文档则返回true,否则返回false</returns>
    public static Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AnyAsync(filter);
    }

    /// <summary>
    /// 删除符合条件的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选要删除文档的Lambda表达式</param>
    /// <returns>成功删除的文档数量</returns>
    public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(filter);
    }

    /// <summary>
    /// 删除指定的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="state">要删除的文档实例</param>
    /// <returns>成功删除的文档数量</returns>
    public static Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(state);
    }
}