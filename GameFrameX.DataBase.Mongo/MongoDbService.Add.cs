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
using GameFrameX.Foundation.Utility;
using MongoDB.Driver;
using System.Threading;

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
    /// <summary>
    /// 批量写入选项，用于批量写入文档。设置
    /// <see>
    ///     <cref>IsOrdered</cref>
    /// </see>
    /// 属性为 false 可以并行执行写入操作。
    /// </summary>
    /// <remarks>
    /// Bulk write options for batch document writing. Setting the
    /// <see>
    ///     <cref>IsOrdered</cref>
    /// </see>
    /// property to false enables parallel write operations.
    /// </remarks>
    public static readonly BulkWriteOptions BulkWriteOptions = new() { IsOrdered = false, };

    #region 插入

    /// <summary>
    /// 增加一条数据。
    /// </summary>
    /// <remarks>
    /// Adds a single data record.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要添加的数据对象 / Data object to add</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public async Task AddAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        await AddAsync(state, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 增加一条数据。
    /// </summary>
    /// <remarks>
    /// Adds a single data record.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要添加的数据对象 / Data object to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public async Task AddAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var currentTime = GetCurrentTimestamp();
        state.CreatedTime = currentTime;
        state.UpdateTime = currentTime;
        var collection = _mongoDbContext.GetCollection<TState>();
        await ExecuteWriteWithRetryAsync(token => collection.InsertOneAsync(state, cancellationToken: token), cancellationToken, nameof(AddAsync), false).ConfigureAwait(false);
    }

    /// <summary>
    /// 增加一个列表数据。
    /// </summary>
    /// <remarks>
    /// Adds a list of data records.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="states">要添加的数据列表 / List of data objects to add</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public async Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new()
    {
        await AddListAsync(states, CancellationToken.None).ConfigureAwait(false);
    }

    /// <summary>
    /// 增加一个列表数据。
    /// </summary>
    /// <remarks>
    /// Adds a list of data records.
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="states">要添加的数据列表 / List of data objects to add</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public async Task AddListAsync<TState>(IEnumerable<TState> states, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        cancellationToken.ThrowIfCancellationRequested();
        EnsureInitialized();
        var cacheStates = states?.ToList();
        if (cacheStates == null || cacheStates.Count == 0)
        {
            return;
        }

        var currentTime = GetCurrentTimestamp();
        foreach (var cacheState in cacheStates)
        {
            cacheState.CreatedTime = currentTime;
            cacheState.UpdateTime = currentTime;
        }

        var collection = _mongoDbContext.GetCollection<TState>();
        await ExecuteWriteWithRetryAsync(token => collection.InsertManyAsync(cacheStates, cancellationToken: token), cancellationToken, nameof(AddListAsync), false).ConfigureAwait(false);
    }

    #endregion 插入
}
