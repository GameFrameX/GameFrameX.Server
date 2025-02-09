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
    /// 增加或更新数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">数据类型</typeparam>
    /// <returns>返回增加或更新的条数</returns>
    public async Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        var resultState = await InnerFindAsync<TState>(state.Id);

        if (resultState == null)
        {
            // Add
            await AddAsync(state);
            return state;
        }

        // Update
        return await UpdateAsync(state);
    }
}