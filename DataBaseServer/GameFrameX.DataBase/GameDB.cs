using System.Linq.Expressions;
using GameFrameX.DataBase.State;

namespace GameFrameX.DataBase
{
    /// <summary>
    /// 游戏数据库类
    /// </summary>
    public static class GameDb
    {
        private static IGameDbService _dbServiceImpler;

        /// <summary>
        /// 使用指定的dbService初始化GameDb
        /// </summary>
        /// <param name="dbService">dbService的实现</param>
        public static void Init(IGameDbService dbService)
        {
            _dbServiceImpler = dbService;
        }

        /// <summary>
        /// 以指定类型获取GameDb
        /// </summary>
        /// <typeparam name="T">dbService的类型</typeparam>
        /// <returns>以指定类型返回的GameDb</returns>
        public static T As<T>() where T : IGameDbService
        {
            return (T)_dbServiceImpler;
        }

        /// <summary>
        /// 使用指定的mongoUrl和mongoDbName打开GameDb连接
        /// </summary>
        /// <param name="mongoUrl">MongoDB连接URL</param>
        /// <param name="mongoDbName">MongoDB数据库的名称</param>
        public static void Open(string mongoUrl, string mongoDbName)
        {
            _dbServiceImpler.Open(mongoUrl, mongoDbName);
        }

        /// <summary>
        /// 关闭GameDb连接
        /// </summary>
        public static void Close()
        {
            _dbServiceImpler.Close();
        }

        /// <summary>
        /// 查找与指定过滤器匹配的文档列表
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="filter">过滤器表达式</param>
        /// <returns>表示异步操作的任务。任务结果包含文档列表</returns>
        public static Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.FindListAsync<TState>(filter);
        }

        /// <summary>
        /// 计算与指定过滤器匹配的文档数量
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="filter">过滤器表达式</param>
        /// <returns>表示异步操作的任务。任务结果包含文档数量</returns>
        public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.CountAsync<TState>(filter);
        }

        /// <summary>
        /// 查找与指定过滤器匹配的文档
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="filter">过滤器表达式</param>
        /// <returns>表示异步操作的任务。任务结果包含文档</returns>
        public static Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.FindAsync<TState>(filter);
        }

        /// <summary>
        /// 以升序方式查找符合条件的第一个元素。
        /// </summary>
        /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
        /// <param name="filter">过滤表达式。</param>
        /// <param name="sortExpression">排序字段表达式。</param>
        /// <returns>符合条件的第一个元素。</returns>
        public static Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.FindSortAscendingFirstOneAsync<TState>(filter, sortExpression);
        }

        /// <summary>
        /// 以降序方式查找符合条件的第一个元素。
        /// </summary>
        /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
        /// <param name="filter">过滤表达式。</param>
        /// <param name="sortExpression">排序字段表达式。</param>
        /// <returns>符合条件的第一个元素。</returns>
        public static Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.FindSortDescendingFirstOneAsync<TState>(filter, sortExpression);
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
        public static Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, long pageIndex = 0, long pageSize = 10) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.FindSortDescendingAsync<TState>(filter, sortExpression, pageIndex, pageSize);
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
        public static Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, long pageIndex = 0, long pageSize = 10) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.FindSortAscendingAsync<TState>(filter, sortExpression, pageIndex, pageSize);
        }


        /// <summary>
        /// 加载指定id的文档
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="id">文档的id</param>
        /// <param name="defaultGetter">一个用于获取默认值的函数，如果指定的文档不存在</param>
        /// <returns>表示异步操作的任务。任务结果包含文档</returns>
        public static Task<TState> LoadState<TState>(long id, Func<TState> defaultGetter = null) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.LoadState(id, defaultGetter);
        }

        /// <summary>
        /// 异步更新指定类型的文档
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="state">要更新的文档</param>
        /// <returns>表示异步操作的任务。任务结果包含更新后的文档</returns>
        public static Task<TState> UpdateAsync<TState>(TState state) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.UpdateAsync<TState>(state);
        }

        /// <summary>
        /// 异步保存一个文档
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="state">要保存的文档</param>
        /// <returns>表示异步操作的任务</returns>
        public static Task SaveOneAsync<TState>(TState state) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.AddAsync<TState>(state);
        }

        /// <summary>
        /// 异步删除与指定过滤器匹配的文档
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="filter">过滤器表达式</param>
        /// <returns>表示异步操作的任务。任务结果包含删除的文档数量</returns>
        public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.DeleteAsync<TState>(filter);
        }

        /// <summary>
        /// 异步删除指定的文档
        /// </summary>
        /// <typeparam name="TState">文档的类型</typeparam>
        /// <param name="state">要删除的文档</param>
        /// <returns>表示异步操作的任务。任务结果包含删除的文档数量</returns>
        public static Task<long> DeleteAsync<TState>(TState state) where TState : class, ICacheState, new()
        {
            return _dbServiceImpler.DeleteAsync<TState>(state);
        }
    }
}