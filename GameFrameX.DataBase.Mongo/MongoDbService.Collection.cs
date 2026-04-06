using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Utility;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GameFrameX.DataBase.Mongo;

public sealed partial class MongoDbService
{
    /// <summary>
    /// 获取当前时间戳（毫秒）。
    /// </summary>
    /// <remarks>
    /// Gets the current timestamp in milliseconds.
    /// If <see cref="DbOptions.IsUseTimeZone"/> is <c>true</c>, returns the timestamp with time zone offset;
    /// otherwise, returns the standard UTC timestamp.
    /// </remarks>
    /// <returns>
    /// 返回当前时间的 Unix 时间戳（毫秒） / Returns the current Unix timestamp in milliseconds
    /// </returns>
    private long GetCurrentTimestamp()
    {
        if (Options.IsUseTimeZone)
        {
            return TimerHelper.UnixTimeMillisecondsWithTimeZoneOffset();
        }

        return TimerHelper.UnixTimeMilliseconds();
    }

    /// <summary>
    /// 构建数据库连接目标标识（脱敏）。
    /// </summary>
    /// <remarks>
    /// Builds a desensitized database connection target identifier.
    /// </remarks>
    /// <param name="connectionString">连接字符串 / Connection string</param>
    /// <param name="databaseName">数据库名称 / Database name</param>
    /// <returns>脱敏后的连接目标标识 / Desensitized connection target identifier</returns>
    private static string BuildConnectionTargetTag(string connectionString, string databaseName)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return $"db={databaseName ?? "unknown"}";
        }

        try
        {
            var mongoUrl = MongoUrl.Create(connectionString);
            var hostPart = mongoUrl.Servers != null && mongoUrl.Servers.Any()
                               ? string.Join(",", mongoUrl.Servers.Select(static server => server.Host))
                               : "unknown-host";
            var dbPart = string.IsNullOrWhiteSpace(mongoUrl.DatabaseName) ? databaseName : mongoUrl.DatabaseName;
            return $"host={hostPart};db={dbPart ?? "unknown"}";
        }
        catch
        {
            return $"db={databaseName ?? "unknown"}";
        }
    }

    /// <summary>
    /// 获取指定类型的MongoDB集合。
    /// </summary>
    /// <remarks>
    /// Gets the MongoDB collection of the specified type.
    /// </remarks>
    /// <typeparam name="TState">文档的类型 / The type of the document</typeparam>
    /// <param name="settings">集合的设置 / Collection settings</param>
    /// <returns>指定类型的MongoDB集合 / MongoDB collection of the specified type</returns>
    private IMongoCollection<TState> GetCollection<TState>(MongoCollectionSettings settings = null) where TState : class, ICacheState, new()
    {
        var collectionName = typeof(TState).Name;
        var collection = CurrentDatabase.GetCollection<TState>(collectionName, settings);
        CreateIndexes(collection);
        return collection;
    }

    /// <summary>
    /// 获取指定名称的MongoDB集合。
    /// </summary>
    /// <remarks>
    /// Gets the MongoDB collection with the specified name.
    /// </remarks>
    /// <param name="collectionName">集合名称 / Collection name</param>
    /// <param name="settings">集合的设置 / Collection settings</param>
    /// <returns>指定名称的MongoDB集合 / MongoDB collection with the specified name</returns>
    private IMongoCollection<BsonDocument> GetCollection(string collectionName, MongoCollectionSettings settings = null)
    {
        var collection = CurrentDatabase.GetCollection<BsonDocument>(collectionName, settings);
        return collection;
    }
}
