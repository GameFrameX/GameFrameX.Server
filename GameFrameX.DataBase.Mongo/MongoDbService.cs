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
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Foundation.Utility;
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
public sealed partial class MongoDbService : IDatabaseService
{
    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库。
    /// </summary>
    /// <remarks>
    /// Gets or sets the currently used MongoDB database.
    /// </remarks>
    public IMongoDatabase CurrentDatabase { get; private set; }

    /// <summary>
    /// 获取或设置当前使用的MongoDB数据库配置选项。
    /// </summary>
    /// <remarks>
    /// Gets or sets the currently used MongoDB database configuration options.
    /// </remarks>
    public DbOptions Options{ get; private set; }

    private MongoDbContext _mongoDbContext;
    private MongoClient _mongoClient;

    /// <summary>
    /// 确保数据库服务已初始化。
    /// </summary>
    /// <remarks>
    /// Ensures that the database service has been initialized.
    /// </remarks>
    /// <exception cref="InvalidOperationException">当服务未初始化时抛出 / Thrown when the service is not initialized</exception>
    private void EnsureInitialized()
    {
        if (_mongoDbContext == null)
        {
            throw new InvalidOperationException("MongoDbService has not been initialized. Call Open() first.");
        }
    }

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
    /// 连接数据库。
    /// </summary>
    /// <remarks>
    /// Connects to the database.
    /// </remarks>
    /// <param name="dbOptions">数据库配置选项 / Database configuration options</param>
    /// <returns>返回数据库是否初始化成功 / Returns whether the database was initialized successfully</returns>
    public async Task<bool> Open(DbOptions dbOptions)
    {
        ArgumentNullException.ThrowIfNull(dbOptions, nameof(dbOptions));
        ArgumentNullException.ThrowIfNull(dbOptions.ConnectionString, nameof(dbOptions.ConnectionString));
        ArgumentNullException.ThrowIfNull(dbOptions.Name, nameof(dbOptions.Name));
        Options = dbOptions;
        try
        {
            var settings = MongoClientSettings.FromConnectionString(Options.ConnectionString);
            _mongoClient = new MongoClient(settings);
            CurrentDatabase = _mongoClient.GetDatabase(Options.Name);
            _mongoDbContext = new MongoDbContext(CurrentDatabase);

            // 测试连接
            await CurrentDatabase.RunCommandAsync((Command<BsonDocument>)"{ping:1}").ConfigureAwait(false);

            LogHelper.Info("MongoDbService.Open {dbName} {ConnectionString} {mongoDbInitializedSuccessfully}", dbOptions.Name, dbOptions.ConnectionString, LocalizationService.GetString(Localization.Keys.Database.MongoDbInitializedSuccessfully, dbOptions.ConnectionString, dbOptions.Name));
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.Fatal("MongoDbService.Open Exception {dbName} {ConnectionString} {exception}", dbOptions.Name, dbOptions.ConnectionString, exception);
            var message = LocalizationService.GetString(Localization.Keys.Database.MongoDbInitializationFailed, dbOptions.ConnectionString, dbOptions.Name);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            LogHelper.Error("MongoDbService.Open Exception {dbName} {ConnectionString} {message}", dbOptions.Name, dbOptions.ConnectionString, message);
            return false;
        }
    }

    /// <summary>
    /// 关闭MongoDB连接。
    /// </summary>
    /// <remarks>
    /// Closes the MongoDB connection.
    /// </remarks>
    public Task Close()
    {
        _mongoClient?.Dispose();
        return Task.CompletedTask;
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
