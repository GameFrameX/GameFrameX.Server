using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Logger;
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
    /// 获取或设置当前使用的MongoDB数据库。
    /// </summary>
    public IMongoDatabase CurrentDatabase { get; private set; }

    private MongoDbContext _mongoDbContext;

    /// <summary>
    /// 链接数据库
    /// </summary>
    /// <param name="dbOptions">数据库配置选项</param>
    /// <returns>返回数据库是否初始化成功</returns>
    public async Task<bool> Open(DbOptions dbOptions)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(dbOptions.ConnectionString, nameof(dbOptions.ConnectionString));
            ArgumentNullException.ThrowIfNull(dbOptions.Name, nameof(dbOptions.Name));
            var settings = MongoClientSettings.FromConnectionString(dbOptions.ConnectionString);
            await DB.InitAsync(dbOptions.Name, settings);
            _mongoDbContext = new MongoDbContext();
            CurrentDatabase = DB.Database(dbOptions.Name);
            LogHelper.Info($"初始化MongoDB服务完成 Url:{dbOptions.ConnectionString} DbName:{dbOptions.Name}");
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.Fatal(exception);
            LogHelper.Error($"初始化MongoDB服务失败 Url:{dbOptions.ConnectionString} DbName:{dbOptions.Name}");
            return false;
        }
    }

    /// <summary>
    /// 关闭MongoDB连接。
    /// </summary>
    public void Close()
    {
        _mongoDbContext?.Session?.Dispose();
        // _client.Cluster.Dispose();
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