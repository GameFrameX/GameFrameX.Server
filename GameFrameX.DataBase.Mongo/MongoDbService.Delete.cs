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
    /// 根据条件删除单条数据(软删除)
    /// </summary>
    /// <param name="filter">查询条件表达式</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    public async Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        var state = await FindAsync(filter);
        state.DeleteTime = TimerHelper.UnixTimeMilliseconds();
        state.IsDeleted = true;
        var result = await _mongoDbContext.Update<TState>().Match(m => m.Id == state.Id).Modify(x => x.IsDeleted, state.IsDeleted).Modify(x => x.DeleteTime, state.DeleteTime).ExecuteAsync();
        return result.ModifiedCount;
    }

    /// <summary>
    /// 根据条件批量删除数据(软删除)
    /// </summary>
    /// <param name="filter">查询条件表达式</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    public async Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        var bulkUpdate = _mongoDbContext.Update<TState>();
        var list = await FindListAsync(filter);
        var deleteTime = TimerHelper.UnixTimeMilliseconds();
        foreach (var state in list)
        {
            state.DeleteTime = deleteTime;
            state.IsDeleted = true;
            bulkUpdate.MatchID(state.Id).Modify(x => x.IsDeleted, state.IsDeleted).Modify(x => x.DeleteTime, state.DeleteTime).AddToQueue();
        }

        var result = await bulkUpdate.ExecuteAsync();
        return result.ModifiedCount;
    }

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)
    /// </summary>
    /// <param name="ids">要删除的ID列表</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    public async Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        var bulkUpdate = _mongoDbContext.Update<TState>();
        var deleteTime = TimerHelper.UnixTimeMilliseconds();
        foreach (var id in ids)
        {
            bulkUpdate.MatchID(id).Modify(x => x.IsDeleted, true).Modify(x => x.DeleteTime, deleteTime).AddToQueue();
        }

        var result = await bulkUpdate.ExecuteAsync();
        return result.ModifiedCount;
    }

    /// <summary>
    /// 删除指定对象(软删除)
    /// </summary>
    /// <param name="state">要删除的对象</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    public async Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        state.DeleteTime = TimerHelper.UnixTimeMilliseconds();
        state.IsDeleted = true;
        var result = await _mongoDbContext.Update<TState>().Match(m => m.Id == state.Id).Modify(x => x.IsDeleted, state.IsDeleted).Modify(x => x.DeleteTime, state.DeleteTime).ExecuteAsync();
        return result.ModifiedCount;
    }

    #region 删除

    /*
    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="collectionName">集合名称</param>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long Delete<TState>(string collectionName, BsonDocument document)
    {
        var result = CurrentDatabase.GetCollection<TState>(collectionName).DeleteOne(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public long DeleteMany<TState>(BsonDocument document) where TState : BaseCacheState
    {
        var result = GetCollection<TState>().DeleteMany(document);
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
        var result = GetCollection(collectionName).DeleteOne(document);
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
        var result = GetCollection(collectionName).DeleteMany(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public async Task<long> DeleteAsync<TState>(BsonDocument document) where TState : BaseCacheState
    {
        var result = await GetCollection<TState>().DeleteOneAsync(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按BsonDocument条件删除
    /// </summary>
    /// <param name="document">文档</param>
    /// <returns></returns>
    public async Task<long> DeleteManyAsync<TState>(BsonDocument document) where TState : BaseCacheState
    {
        var result = await GetCollection<TState>().DeleteManyAsync(document);
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
        var result = await GetCollection(collectionName).DeleteOneAsync(document);
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
        var result = await GetCollection(collectionName).DeleteManyAsync(document);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long Delete<TState>(string json) where TState : BaseCacheState
    {
        var result = GetCollection<TState>().DeleteOne(json);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按json字符串删除
    /// </summary>
    /// <param name="json">json字符串</param>
    /// <returns></returns>
    public long DeleteMany<TState>(string json) where TState : BaseCacheState
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
    public async Task<long> DeleteManyAsync<TState>(string json) where TState : BaseCacheState
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
    public long Delete<TState>(Expression<Func<TState, bool>> predicate) where TState : BaseCacheState
    {
        var result = GetCollection<TState>().DeleteOne(predicate);
        return result.DeletedCount;
    }

    /// <summary>
    /// 按条件表达式删除
    /// </summary>
    /// <param name="predicate">条件表达式</param>
    /// <returns></returns>
    public long DeleteMany<TState>(Expression<Func<TState, bool>> predicate) where TState : BaseCacheState
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
    public async Task<long> DeleteManyAsync<TState>(Expression<Func<TState, bool>> predicate) where TState : BaseCacheState
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
    public long Delete<TState>(string collName, FilterDefinition<TState> filter) where TState : BaseCacheState
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
    public long DeleteMany<TState>(FilterDefinition<TState> filter) where TState : BaseCacheState
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
    public async Task<long> DeleteAsync<TState>(FilterDefinition<TState> filter) where TState : BaseCacheState
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
    public async Task<long> DeleteManyAsync<TState>(FilterDefinition<TState> filter) where TState : BaseCacheState
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
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">条件表达式</param>
    /// <returns>返回被删除的记录</returns>
    public TState DeleteOne<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        var result = GetCollection<TState>().FindOneAndDelete(filter);
        return result;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件表达式</param>
    /// <returns>返回被删除的记录</returns>
    public BsonDocument DeleteOne(string collName, Expression<Func<BsonDocument, bool>> filter)
    {
        var result = GetCollection(collName).FindOneAndDelete(filter);
        return result;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">条件表达式</param>
    /// <returns>返回被删除的记录</returns>
    public async Task<TState> DeleteOneAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState
    {
        var result = await GetCollection<TState>().FindOneAndDeleteAsync(filter);
        return result;
    }

    /// <summary>
    /// 删除一条记录
    /// </summary>
    /// <param name="collName">集合名称</param>
    /// <param name="filter">条件表达式</param>
    /// <returns>返回被删除的记录</returns>
    public async Task<BsonDocument> DeleteOneAsync(string collName, Expression<Func<BsonDocument, bool>> filter)
    {
        var result = await GetCollection(collName).FindOneAndDeleteAsync(filter);
        return result;
    }*/

    #endregion 删除
}