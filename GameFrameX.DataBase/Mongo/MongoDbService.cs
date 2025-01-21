using GameFrameX.DataBase.Abstractions;
using GameFrameX.Utility.Log;
using MongoDB.Bson;
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
public sealed partial class MongoDbService : IDatabaseService
{
    /// <summary>
    /// 获取或设置MongoDB客户端。
    /// </summary>
    public MongoClient Client { get; private set; }

    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库。
    /// </summary>
    public IMongoDatabase CurrentDatabase { get; private set; }

    public MongoDbContext _mongoDbContext { get; private set; }

    /// <summary>
    /// 打开MongoDB连接并指定URL和数据库名称。
    /// </summary>
    /// <param name="url">MongoDB连接URL。</param>
    /// <param name="dbName">要使用的数据库名称。</param>
    public async void Open(string url, string dbName)
    {
        try
        {
            var settings = MongoClientSettings.FromConnectionString(url);
            await DB.InitAsync(dbName, settings);
            _mongoDbContext = new MongoDbContext();
            CurrentDatabase = DB.Database(dbName);
            LogHelper.Info($"初始化MongoDB服务完成 Url:{url} DbName:{dbName}");
        }
        catch (Exception)
        {
            LogHelper.Error($"初始化MongoDB服务失败 Url:{url} DbName:{dbName}");
            throw;
        }
    }

    /// <summary>
    /// 关闭MongoDB连接。
    /// </summary>
    public void Close()
    {
        Client.Cluster.Dispose();
    }

    /// <summary>
    /// 获取指定类型的MongoDB集合。
    /// </summary>
    /// <typeparam name="TState">文档的类型。</typeparam>
    /// <param name="settings">集合的设置。</param>
    /// <returns>指定类型的MongoDB集合。</returns>
    private IMongoCollection<TState> GetCollection<TState>(MongoCollectionSettings settings = null) where TState : class, ICacheState, new()
    {
        var collectionName = typeof(TState).Name;
        var collection = CurrentDatabase.GetCollection<TState>(collectionName, settings);
        CreateIndexes(collection);
        return collection;
    }

    /// <summary>
    /// 获取指定类型的MongoDB集合。
    /// </summary>
    /// <param name="collectionName">集合名称。</param>
    /// <param name="settings">集合的设置。</param>
    /// <returns>指定类型的MongoDB集合。</returns>
    private IMongoCollection<BsonDocument> GetCollection(string collectionName, MongoCollectionSettings settings = null)
    {
        var collection = CurrentDatabase.GetCollection<BsonDocument>(collectionName, settings);
        return collection;
    }
}