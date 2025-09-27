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

using GameFrameX.DataBase.Abstractions;
using GameFrameX.Utility;
using MongoDB.Driver;
using MongoDB.Entities;

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
    /// 批量写入选项，用于批量写入文档。设置
    /// <see>
    ///     <cref>IsOrdered</cref>
    /// </see>
    /// 属性为 false 可以并行执行写入操作。
    /// </summary>
    public static readonly BulkWriteOptions BulkWriteOptions = new() { IsOrdered = false, };

    #region 插入

    /// <summary>
    /// 增加一条数据
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns>返回修改的条数</returns>
    public async Task AddAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        state.CreateTime = TimeHelper.UnixTimeMilliseconds();
        state.UpdateTime = state.CreateTime;
        await _mongoDbContext.SaveAsync(state);
    }

    /// <summary>
    /// 增加一个列表数据
    /// </summary>
    /// <param name="states"></param>
    /// <typeparam name="TState"></typeparam>
    public async Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new()
    {
        var cacheStates = states.ToList();
        foreach (var cacheState in cacheStates)
        {
            cacheState.CreateTime = TimeHelper.UnixTimeMilliseconds();
            cacheState.UpdateTime = cacheState.CreateTime;
        }

        await _mongoDbContext.SaveAsync(cacheStates);
    }


    /*
    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="state">数据</param>
    public void InsertOne<TState>(TState state) where TState : ICacheState, new()
    {
        GetCollection<TState>().InsertOne(state);
    }*/

    /*/// <summary>
    /// 插入一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="doc">文档</param>
    public void InsertOne(string collectionName, BsonDocument doc)
    {
        GetCollection(collectionName).InsertOne(doc);
    }*/

    /*
    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="t">数据</param>
    public Task InsertOneAsync<TState>(TState t) where TState : ICacheState, new()
    {
        return GetCollection<TState>().InsertOneAsync(t);
    }
    */

    /*
    /// <summary>
    /// 插入一条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="doc">文档</param>
    public Task InsertOneAsync(string collectionName, BsonDocument doc)
    {
        return GetCollection(collectionName).InsertOneAsync(doc);
    }*/

    /*/// <summary>
    /// 插入多条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">集合</param>
    public void InsertMany<TState>(IEnumerable<TState> list) where TState : ICacheState, new()
    {
        GetCollection<TState>().InsertMany(list);
    }*/

    /*
    /// <summary>
    /// 插入多条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson集合</param>
    public void InsertMany(string collectionName, IEnumerable<BsonDocument> list)
    {
        GetCollection(collectionName).InsertMany(list);
    }*/

    /*/// <summary>
    /// 插入多条数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">集合</param>
    public Task InsertManyAsync<TState>(IEnumerable<TState> list) where TState : ICacheState, new()
    {
        return GetCollection<TState>().InsertManyAsync(list);
    }*/

    /*
    /// <summary>
    /// 插入多条数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson集合</param>
    public Task InsertManyAsync(string collectionName, IEnumerable<BsonDocument> list)
    {
        return GetCollection(collectionName).InsertManyAsync(list);
    }*/

    /*
    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">数据集合</param>
    /// <returns></returns>
    public List<WriteModel<TState>> BulkInsert<TState>(IEnumerable<WriteModel<TState>> list) where TState : ICacheState, new()
    {
        var result = GetCollection<TState>().BulkWrite(list);
        return result.ProcessedRequests.ToList();
    }

    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson数据集合</param>
    /// <returns></returns>
    public List<WriteModel<BsonDocument>> BulkInsert(string collectionName, IEnumerable<WriteModel<BsonDocument>> list)
    {
        var result = GetCollection(collectionName).BulkWrite(list);
        return result.ProcessedRequests.ToList();
    }

    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="list">数据集合</param>
    /// <returns></returns>
    public async Task<List<WriteModel<TState>>> BulkInsertAsync<TState>(IEnumerable<WriteModel<TState>> list) where TState : ICacheState, new()
    {
        var result = await GetCollection<TState>().BulkWriteAsync(list);
        return result.ProcessedRequests.ToList();
    }

    /// <summary>
    /// 大批量插入数据
    /// </summary>
    /// <param name="collectionName">表名</param>
    /// <param name="list">Bson数据集合</param>
    /// <returns></returns>
    public async Task<List<WriteModel<BsonDocument>>> BulkInsertAsync(string collectionName, IEnumerable<WriteModel<BsonDocument>> list)
    {
        var result = await GetCollection(collectionName).BulkWriteAsync(list);
        return result.ProcessedRequests.ToList();
    }*/

    #endregion 插入
}