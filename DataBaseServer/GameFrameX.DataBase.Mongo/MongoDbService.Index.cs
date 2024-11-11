using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Utility;
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
    sealed class MongoIndexModel
    {
        public MongoIndexModel(bool unique, string name)
        {
            Unique = unique;
            Name = name;
        }

        public string Name { get; set; }
        public bool Unique { get; set; }
        public List<BsonDocument> Keys { get; set; }
    }

    readonly ConcurrentDictionary<string, List<MongoIndexModel>> _indexCache = new ConcurrentDictionary<string, List<MongoIndexModel>>();

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
            var indexAttribute = property.GetCustomAttribute<MongoIndexAttribute>();
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