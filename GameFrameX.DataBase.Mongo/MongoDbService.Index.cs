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

using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Orm.Attribute;
using MongoDB.Bson;
using MongoDB.Driver;

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
    private readonly ConcurrentDictionary<string, List<MongoIndexModel>> _indexCache = new();

    private static bool AreIndexesConsistent<T>(List<CreateIndexModel<T>> toBeCreatedIndexes, List<BsonDocument> createdIndexes)
    {
        var toBeCreatedIndexNames = toBeCreatedIndexes.Select(i => i.Options.Name).ToList();
        var createdIndexNames = createdIndexes.Select(i => i["name"].AsString).ToList();
        if (toBeCreatedIndexNames.Count != createdIndexNames.Count)
        {
            return false;
        }


        foreach (var indexInfo in toBeCreatedIndexes)
        {
            var correspondingCreatedIndex = createdIndexes.FirstOrDefault(i => i["name"].AsString == indexInfo.Options.Name);
            if (correspondingCreatedIndex == null)
            {
                return false;
            }

            // var createdIndexKeys = ((BsonDocument)correspondingCreatedIndex["key"]);
            // if (indexInfo.Keys != createdIndexKeys)
            // {
            //     return false;
            // }
            //
            // foreach (var key in indexInfo.Keys)
            // {
            //     if (!createdIndexKeys.Contains(key))
            //     {
            //         return false;
            //     }
            // }

            var uniqueAttribute = indexInfo.Options.Unique;
            var createdIndexUnique = correspondingCreatedIndex["unique"].AsBoolean;
            if (uniqueAttribute != createdIndexUnique)
            {
                return false;
            }
        }

        return true;
    }

    private void CreateIndexes<T>(IMongoCollection<T> collection)
    {
        var entityType = typeof(T);
        if (_indexCache.TryGetValue(entityType.Name, out var list))
        {
            return;
        }

        list = new List<MongoIndexModel>();
        _indexCache.TryAdd(entityType.Name, list);
        var properties = entityType.GetProperties();
        // 索引列表
        var result = collection.Indexes.List().ToList();

        var indexModels = new List<CreateIndexModel<T>>();
        foreach (var property in properties)
        {
            var indexAttribute = property.GetCustomAttribute<EntityIndexAttribute>();
            if (indexAttribute != null)
            {
                var indexKeys = indexAttribute.IsAscending ? Builders<T>.IndexKeys.Ascending(property.Name) : Builders<T>.IndexKeys.Descending(property.Name);

                var indexModel = new CreateIndexModel<T>(indexKeys, new CreateIndexOptions
                {
                    Unique = indexAttribute.Unique,
                    Name = indexAttribute.Name,
                });
                indexModels.Add(indexModel);
                var mongoIndexModel = new MongoIndexModel(indexAttribute.Unique, indexAttribute.Name);
                list.Add(mongoIndexModel);
            }
        }

        if (indexModels.Count > 0 && !AreIndexesConsistent(indexModels, result))
        {
            collection.Indexes.CreateMany(indexModels);
        }
    }

    private sealed class MongoIndexModel
    {
        internal MongoIndexModel(bool unique, string name)
        {
            Unique = unique;
            Name = name;
        }

        public string Name { get; set; }
        public bool Unique { get; set; }
    }

    #region 索引

    /// <summary>
    /// 创建索引
    /// </summary>
    /// <param name="collectionName">集合名</param>
    /// <param name="index">索引键</param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public string CreateIndex(string collectionName, string index, bool asc = true)
    {
        var mgr = GetCollection(collectionName).Indexes;
        var list = mgr.List();
        while (list.MoveNext())
        {
            if (!list.Current.Any(doc => doc["name"].AsString.StartsWith(index)))
            {
                return mgr.CreateOne(new CreateIndexModel<BsonDocument>(asc ? Builders<BsonDocument>.IndexKeys.Ascending(doc => doc[index]) : Builders<BsonDocument>.IndexKeys.Descending(doc => doc[index])));
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 创建索引
    /// </summary>
    /// <param name="collectionName">集合名</param>
    /// <param name="index">索引键</param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public async Task<string> CreateIndexAsync(string collectionName, string index, bool asc = true)
    {
        var mgr = GetCollection(collectionName).Indexes;
        var list = await mgr.ListAsync();
        while (await list.MoveNextAsync())
        {
            if (!list.Current.Any(doc => doc["name"].AsString.StartsWith(index)))
            {
                return await mgr.CreateOneAsync(new CreateIndexModel<BsonDocument>(asc ? Builders<BsonDocument>.IndexKeys.Ascending(doc => doc[index]) : Builders<BsonDocument>.IndexKeys.Descending(doc => doc[index])));
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 更新索引
    /// </summary>
    /// <param name="collectionName">集合名</param>
    /// <param name="index">索引键</param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public string UpdateIndex(string collectionName, string index, bool asc = true)
    {
        var mgr = GetCollection(collectionName).Indexes;
        return mgr.CreateOne(new CreateIndexModel<BsonDocument>(asc ? Builders<BsonDocument>.IndexKeys.Ascending(doc => doc[index]) : Builders<BsonDocument>.IndexKeys.Descending(doc => doc[index])));
    }

    /// <summary>
    /// 更新索引
    /// </summary>
    /// <param name="collectionName">集合名</param>
    /// <param name="index">索引键</param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public async Task<string> UpdateIndexAsync(string collectionName, string index, bool asc = true)
    {
        var mgr = GetCollection(collectionName).Indexes;
        return await mgr.CreateOneAsync(new CreateIndexModel<BsonDocument>(asc ? Builders<BsonDocument>.IndexKeys.Ascending(doc => doc[index]) : Builders<BsonDocument>.IndexKeys.Descending(doc => doc[index])));
    }

    /// <summary>
    /// 删除索引
    /// </summary>
    /// <param name="collectionName">集合名</param>
    /// <param name="index">索引键</param>
    /// <returns></returns>
    public void DropIndex(string collectionName, string index)
    {
        GetCollection(collectionName).Indexes.DropOne(index);
    }

    /// <summary>
    /// 删除索引
    /// </summary>
    /// <param name="collectionName">集合名</param>
    /// <param name="index">索引键</param>
    /// <returns></returns>
    public Task DropIndexAsync(string collectionName, string index)
    {
        return GetCollection(collectionName).Indexes.DropOneAsync(index);
    }

    /// <summary>
    /// 创建索引
    /// </summary>
    /// <param name="index">索引键</param>
    /// <param name="key"></param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public string CreateIndex<TState>(string index, Expression<Func<TState, object>> key, bool asc = true) where TState : class, ICacheState, new()
    {
        var mgr = GetCollection<TState>().Indexes;
        var list = mgr.List();
        while (list.MoveNext())
        {
            if (!list.Current.Any(doc => doc["name"].AsString.StartsWith(index)))
            {
                return mgr.CreateOne(new CreateIndexModel<TState>(asc ? Builders<TState>.IndexKeys.Ascending(key) : Builders<TState>.IndexKeys.Descending(key)));
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 创建索引
    /// </summary>
    /// <param name="index">索引键</param>
    /// <param name="key"></param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public async Task<string> CreateIndexAsync<TState>(string index, Expression<Func<TState, object>> key, bool asc = true) where TState : class, ICacheState, new()
    {
        var mgr = GetCollection<TState>().Indexes;
        var list = await mgr.ListAsync();
        while (await list.MoveNextAsync())
        {
            if (!list.Current.Any(doc => doc["name"].AsString.StartsWith(index)))
            {
                return await mgr.CreateOneAsync(new CreateIndexModel<TState>(asc ? Builders<TState>.IndexKeys.Ascending(key) : Builders<TState>.IndexKeys.Descending(key)));
            }
        }

        return string.Empty;
    }

    /// <summary>
    /// 更新索引
    /// </summary>
    /// <param name="key"></param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public string UpdateIndex<TState>(Expression<Func<TState, object>> key, bool asc = true) where TState : class, ICacheState, new()
    {
        var mgr = GetCollection<TState>().Indexes;
        return mgr.CreateOne(new CreateIndexModel<TState>(asc ? Builders<TState>.IndexKeys.Ascending(key) : Builders<TState>.IndexKeys.Descending(key)));
    }

    /// <summary>
    /// 更新索引
    /// </summary>
    /// <param name="key"></param>
    /// <param name="asc"></param>
    /// <returns></returns>
    public async Task<string> UpdateIndexAsync<TState>(Expression<Func<TState, object>> key, bool asc = true) where TState : class, ICacheState, new()
    {
        var mgr = GetCollection<TState>().Indexes;
        return await mgr.CreateOneAsync(new CreateIndexModel<TState>(asc ? Builders<TState>.IndexKeys.Ascending(key) : Builders<TState>.IndexKeys.Descending(key)));
    }

    #endregion 索引
}