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
/// <remarks>
/// MongoDB service connection class that implements the
/// <see>
///     <cref>IDatabaseService</cref>
/// </see>
/// interface.
/// </remarks>
public sealed partial class MongoDbService
{
    private readonly ConcurrentDictionary<string, List<MongoIndexModel>> _indexCache = new();

    /// <summary>
    /// 检查待创建的索引与已创建的索引是否一致。
    /// </summary>
    /// <remarks>
    /// Checks whether the indexes to be created are consistent with the existing indexes.
    /// </remarks>
    /// <typeparam name="T">文档类型 / Document type</typeparam>
    /// <param name="toBeCreatedIndexes">待创建的索引列表 / List of indexes to be created</param>
    /// <param name="createdIndexes">已创建的索引列表 / List of existing indexes</param>
    /// <returns>如果索引一致则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if indexes are consistent; otherwise <c>false</c></returns>
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

            var uniqueAttribute = indexInfo.Options.Unique ?? false;
            var createdIndexUnique = correspondingCreatedIndex.Contains("unique") && correspondingCreatedIndex["unique"].AsBoolean;
            if (uniqueAttribute != createdIndexUnique)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 为指定集合创建索引。
    /// </summary>
    /// <remarks>
    /// Creates indexes for the specified collection.
    /// </remarks>
    /// <typeparam name="T">文档类型 / Document type</typeparam>
    /// <param name="collection">MongoDB集合 / MongoDB collection</param>
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

    /// <summary>
    /// MongoDB索引模型。
    /// </summary>
    /// <remarks>
    /// MongoDB index model.
    /// </remarks>
    private sealed class MongoIndexModel
    {
        /// <summary>
        /// 初始化 MongoIndexModel 的新实例。
        /// </summary>
        /// <remarks>
        /// Initializes a new instance of MongoIndexModel.
        /// </remarks>
        /// <param name="unique">是否为唯一索引 / Whether the index is unique</param>
        /// <param name="name">索引名称 / Index name</param>
        internal MongoIndexModel(bool unique, string name)
        {
            Unique = unique;
            Name = name;
        }

        /// <summary>
        /// 获取或设置索引名称。
        /// </summary>
        /// <remarks>
        /// Gets or sets the index name.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置是否为唯一索引。
        /// </summary>
        /// <remarks>
        /// Gets or sets whether the index is unique.
        /// </remarks>
        public bool Unique { get; set; }
    }

    #region 索引

    /// <summary>
    /// 创建索引。
    /// </summary>
    /// <remarks>
    /// Creates an index.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="index">索引键 / Index key</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称，如果索引已存在则返回空字符串 / The created index name, or empty string if index already exists</returns>
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
    /// 异步创建索引。
    /// </summary>
    /// <remarks>
    /// Asynchronously creates an index.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="index">索引键 / Index key</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称，如果索引已存在则返回空字符串 / The created index name, or empty string if index already exists</returns>
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
    /// 更新索引。
    /// </summary>
    /// <remarks>
    /// Updates an index.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="index">索引键 / Index key</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称 / The created index name</returns>
    public string UpdateIndex(string collectionName, string index, bool asc = true)
    {
        var mgr = GetCollection(collectionName).Indexes;
        return mgr.CreateOne(new CreateIndexModel<BsonDocument>(asc ? Builders<BsonDocument>.IndexKeys.Ascending(doc => doc[index]) : Builders<BsonDocument>.IndexKeys.Descending(doc => doc[index])));
    }

    /// <summary>
    /// 异步更新索引。
    /// </summary>
    /// <remarks>
    /// Asynchronously updates an index.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="index">索引键 / Index key</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称 / The created index name</returns>
    public async Task<string> UpdateIndexAsync(string collectionName, string index, bool asc = true)
    {
        var mgr = GetCollection(collectionName).Indexes;
        return await mgr.CreateOneAsync(new CreateIndexModel<BsonDocument>(asc ? Builders<BsonDocument>.IndexKeys.Ascending(doc => doc[index]) : Builders<BsonDocument>.IndexKeys.Descending(doc => doc[index])));
    }

    /// <summary>
    /// 删除索引。
    /// </summary>
    /// <remarks>
    /// Drops an index.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="index">索引名称 / Index name</param>
    public void DropIndex(string collectionName, string index)
    {
        GetCollection(collectionName).Indexes.DropOne(index);
    }

    /// <summary>
    /// 异步删除索引。
    /// </summary>
    /// <remarks>
    /// Asynchronously drops an index.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="index">索引名称 / Index name</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task DropIndexAsync(string collectionName, string index)
    {
        return GetCollection(collectionName).Indexes.DropOneAsync(index);
    }

    /// <summary>
    /// 创建泛型索引。
    /// </summary>
    /// <remarks>
    /// Creates a generic index.
    /// </remarks>
    /// <typeparam name="TState">文档类型，必须实现 ICacheState 接口 / Document type, must implement ICacheState interface</typeparam>
    /// <param name="index">索引名称 / Index name</param>
    /// <param name="key">索引键表达式 / Index key expression</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称，如果索引已存在则返回空字符串 / The created index name, or empty string if index already exists</returns>
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
    /// 异步创建泛型索引。
    /// </summary>
    /// <remarks>
    /// Asynchronously creates a generic index.
    /// </remarks>
    /// <typeparam name="TState">文档类型，必须实现 ICacheState 接口 / Document type, must implement ICacheState interface</typeparam>
    /// <param name="index">索引名称 / Index name</param>
    /// <param name="key">索引键表达式 / Index key expression</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称，如果索引已存在则返回空字符串 / The created index name, or empty string if index already exists</returns>
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
    /// 更新泛型索引。
    /// </summary>
    /// <remarks>
    /// Updates a generic index.
    /// </remarks>
    /// <typeparam name="TState">文档类型，必须实现 ICacheState 接口 / Document type, must implement ICacheState interface</typeparam>
    /// <param name="key">索引键表达式 / Index key expression</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称 / The created index name</returns>
    public string UpdateIndex<TState>(Expression<Func<TState, object>> key, bool asc = true) where TState : class, ICacheState, new()
    {
        var mgr = GetCollection<TState>().Indexes;
        return mgr.CreateOne(new CreateIndexModel<TState>(asc ? Builders<TState>.IndexKeys.Ascending(key) : Builders<TState>.IndexKeys.Descending(key)));
    }

    /// <summary>
    /// 异步更新泛型索引。
    /// </summary>
    /// <remarks>
    /// Asynchronously updates a generic index.
    /// </remarks>
    /// <typeparam name="TState">文档类型，必须实现 ICacheState 接口 / Document type, must implement ICacheState interface</typeparam>
    /// <param name="key">索引键表达式 / Index key expression</param>
    /// <param name="asc">是否升序，默认为 true / Whether ascending, defaults to true</param>
    /// <returns>创建的索引名称 / The created index name</returns>
    public async Task<string> UpdateIndexAsync<TState>(Expression<Func<TState, object>> key, bool asc = true) where TState : class, ICacheState, new()
    {
        var mgr = GetCollection<TState>().Indexes;
        return await mgr.CreateOneAsync(new CreateIndexModel<TState>(asc ? Builders<TState>.IndexKeys.Ascending(key) : Builders<TState>.IndexKeys.Descending(key)));
    }

    #endregion 索引
}