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
            LogHelper.Info($"The MongoDB service is initialized successfully, Url:{dbOptions.ConnectionString} DbName:{dbOptions.Name}");
            return true;
        }
        catch (Exception exception)
        {
            LogHelper.Fatal(exception);
            string message = $"MongoDB service initialization failed, Url:{dbOptions.ConnectionString} DbName:{dbOptions.Name}";
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
            LogHelper.Error(message);
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