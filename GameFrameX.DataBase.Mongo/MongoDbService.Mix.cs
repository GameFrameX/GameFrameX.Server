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

using GameFrameX.Foundation.Utility;
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
    /// <summary>
    /// 替换选项，用于替换文档。设置
    /// <see>
    ///     <cref>IsUpsert</cref>
    /// </see>
    /// 属性为 true 可以在文档不存在时插入新文档。
    /// </summary>
    /// <remarks>
    /// Replace options for replacing documents. Setting the
    /// <see>
    ///     <cref>IsUpsert</cref>
    /// </see>
    /// property to true enables inserting a new document if it doesn't exist.
    /// </remarks>
    public static readonly ReplaceOptions ReplaceOptions = new ReplaceOptions { IsUpsert = true };

    /// <summary>
    /// 增加或更新数据（使用 Upsert 优化，单次数据库操作）。
    /// </summary>
    /// <remarks>
    /// Adds or updates data (using Upsert optimization, single database operation).
    /// </remarks>
    /// <typeparam name="TState">数据类型，必须继承自 BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">数据对象 / Data object</param>
    /// <returns>返回增加或更新后的数据对象 / The added or updated data object</returns>
    public async Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        EnsureInitialized();

        var currentTime = TimerHelper.UnixTimeMilliseconds();

        // 如果是新对象（没有创建时间），设置创建时间
        if (state.CreatedTime == 0)
        {
            state.CreatedTime = currentTime;
        }

        state.UpdateTime = currentTime;
        state.UpdateCount++;

        // 使用 ReplaceOne with Upsert - 单次数据库操作
        var filter = Builders<TState>.Filter.Eq(m => m.Id, state.Id);
        await CurrentDatabase.GetCollection<TState>(typeof(TState).Name).ReplaceOneAsync(filter, state, ReplaceOptions).ConfigureAwait(false);

        state.SaveToDbPostHandler();
        return state;
    }
}