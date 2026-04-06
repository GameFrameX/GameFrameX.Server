// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


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
