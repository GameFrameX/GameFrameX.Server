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

using GameFrameX.Foundation.Utility;

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
    /// 保存数据
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public async Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        var isChanged = state.IsModify();
        if (isChanged)
        {
            state.UpdateTime = TimerHelper.UnixTimeMilliseconds();
            state.UpdateCount++;
            var result = await _mongoDbContext.Update<TState>().MatchID(state.Id).ModifyExcept(m => new { m.CreateId, m.CreateTime, m.Id, m.IsDeleted, m.DeleteTime, }, state).ExecuteAsync();
            if (result.IsAcknowledged)
            {
                state.SaveToDbPostHandler();
            }
        }

        return state;
    }

    /// <summary>
    /// 保存多条数据
    /// </summary>
    /// <param name="stateList">数据列表对象</param>
    /// <returns>返回更新成功的数量</returns>
    public async Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState, new()
    {
        long resultCount = 0;
        var bulkUpdate = _mongoDbContext.Update<TState>();
        var cacheStates = stateList as TState[] ?? stateList.ToArray();
        foreach (var state in cacheStates)
        {
            var isChanged = state.IsModify();
            if (isChanged)
            {
                state.UpdateTime = TimerHelper.UnixTimeMilliseconds();
                state.UpdateCount++;
                bulkUpdate.MatchID(state.Id).ModifyExcept(m => new { m.CreateId, m.CreateTime, m.Id, m.IsDeleted, m.DeleteTime, }, state).AddToQueue();
                resultCount++;
            }
        }

        var result = await bulkUpdate.ExecuteAsync();
        if (result.IsAcknowledged)
        {
            foreach (var state in cacheStates)
            {
                state.SaveToDbPostHandler();
            }
        }

        return resultCount;
    }

    /*/// <summary>
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    public async Task<long> UpdateCountAsync<TState>(TState state) where TState : BaseCacheState
    {
        var isChanged = state.IsModify();
        if (isChanged)
        {
            state.UpdateTime = TimeHelper.UnixTimeMilliseconds();
            state.UpdateCount++;
            var result = await _mongoDbContext.Update<TState>().ModifyWith(state).ExecuteAsync();
            if (result.IsAcknowledged)
            {
                state.SaveToDbPostHandler();
                return result.ModifiedCount;
            }
        }

        return 0;
    }*/

    #region 更新

    /*
    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public UpdateResult UpdateOne<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert = true) where TState : BaseCacheState
    {
        return GetCollection<TState>().UpdateOne(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public UpdateResult UpdateOne(string collectionName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert)
    {
        return GetCollection(collectionName).UpdateOne(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public async Task<UpdateResult> UpdateOneAsync<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert) where TState : BaseCacheState
    {
        return await GetCollection<TState>().UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新的数据</param>
    /// <param name="upsert">如果它不存在是否插入文档</param>
    /// <returns></returns>
    public async Task<UpdateResult> UpdateOneAsync(string collectionName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert)
    {
        return await GetCollection(collectionName).UpdateOneAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public long UpdateMany<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert = false) where TState : BaseCacheState
    {
        var result = GetCollection<TState>().UpdateMany(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public long UpdateMany(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert = false)
    {
        var result = GetCollection(collName).UpdateMany(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改多个文档
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public async Task<long> UpdateManyAsync<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update, bool upsert = false) where TState : BaseCacheState
    {
        var result = await GetCollection<TState>().UpdateManyAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }

    /// <summary>
    /// 修改多个文档
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">修改条件</param>
    /// <param name="update">修改结果</param>
    /// <param name="upsert">是否插入新文档（filter条件满足就更新，否则插入新文档）</param>
    /// <returns></returns>
    public async Task<long> UpdateManyAsync(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update, bool upsert = false)
    {
        var result = await GetCollection(collName).UpdateManyAsync(filter, update, new UpdateOptions
        {
            IsUpsert = upsert,
        });
        return result.ModifiedCount;
    }
    */

    /*
    /// <summary>
    /// 修改文档
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的数据</param>
    /// <returns></returns>
    public TState UpdateOne<TState>(string collName, Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update) where TState : BaseCacheState
    {
        var result = GetCollection<TState>().FindOneAndUpdate(filter, update);
        return result;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的Bson数据</param>
    /// <returns></returns>
    public BsonDocument UpdateOne(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update)
    {
        var result = GetCollection(collName).FindOneAndUpdate(filter, update);
        return result;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的数据</param>
    /// <returns></returns>
    public async Task<TState> UpdateOneAsync<TState>(Expression<Func<TState, bool>> filter, UpdateDefinition<TState> update) where TState : BaseCacheState
    {
        var result = await GetCollection<TState>().FindOneAndUpdateAsync(filter, update);
        return result;
    }

    /// <summary>
    /// 修改文档
    /// </summary>
    /// <param name="collName">表名</param>
    /// <param name="filter">条件</param>
    /// <param name="update">更新后的Bson数据</param>
    /// <returns></returns>
    public async Task<BsonDocument> UpdateOneAsync(string collName, Expression<Func<BsonDocument, bool>> filter, UpdateDefinition<BsonDocument> update)
    {
        var result = await GetCollection(collName).FindOneAndUpdateAsync(filter, update);
        return result;
    }*/

    #endregion 更新
}