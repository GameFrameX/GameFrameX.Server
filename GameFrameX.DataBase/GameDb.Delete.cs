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

using System.Linq.Expressions;
using System.Threading;
using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.DataBase;

/// <summary>
/// 游戏数据库静态工具类,提供对数据库的基本操作封装。
/// </summary>
/// <remarks>
/// Static utility class for game database operations, providing basic database operation encapsulation.
/// </remarks>
public static partial class GameDb
{
    /// <summary>
    /// 删除符合条件的文档。
    /// </summary>
    /// <remarks>
    /// Delete documents matching the condition.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选要删除文档的Lambda表达式 / Lambda expression for filtering documents to delete</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(filter);
    }

    /// <summary>
    /// 删除符合条件的文档。
    /// </summary>
    /// <remarks>
    /// Delete documents matching the condition.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="filter">用于筛选要删除文档的Lambda表达式 / Lambda expression for filtering documents to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(filter, cancellationToken);
    }

    /// <summary>
    /// 删除指定的文档。
    /// </summary>
    /// <remarks>
    /// Delete the specified document.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要删除的文档实例 / Document instance to delete</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(state);
    }

    /// <summary>
    /// 删除指定的文档。
    /// </summary>
    /// <remarks>
    /// Delete the specified document.
    /// </remarks>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState / Document type, must inherit from BaseCacheState</typeparam>
    /// <param name="state">要删除的文档实例 / Document instance to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <returns>成功删除的文档数量 / Number of successfully deleted documents</returns>
    public static Task<long> DeleteAsync<TState>(TState state, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(state, cancellationToken);
    }

    /// <summary>
    /// 根据条件批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by condition (soft delete).
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListAsync<TState>(filter);
    }

    /// <summary>
    /// 根据条件批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by condition (soft delete).
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListAsync<TState>(filter, cancellationToken);
    }

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by ID list (soft delete).
    /// </remarks>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListIdAsync<TState>(ids);
    }

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)。
    /// </summary>
    /// <remarks>
    /// Batch delete data by ID list (soft delete).
    /// </remarks>
    /// <param name="ids">要删除的ID列表 / List of IDs to delete</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回修改的记录数 / Number of modified records</returns>
    public static Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteListIdAsync<TState>(ids, cancellationToken);
    }

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回删除的记录数 / Number of deleted records</returns>
    public static Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.HardDeleteAsync<TState>(filter);
    }

    /// <summary>
    /// 根据条件物理删除数据。
    /// </summary>
    /// <remarks>
    /// Physically delete data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回删除的记录数 / Number of deleted records</returns>
    public static Task<long> HardDeleteAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.HardDeleteAsync<TState>(filter, cancellationToken);
    }

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回恢复的记录数 / Number of restored records</returns>
    public static Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.RestoreAsync<TState>(filter);
    }

    /// <summary>
    /// 根据条件恢复软删除数据。
    /// </summary>
    /// <remarks>
    /// Restore soft deleted data by condition.
    /// </remarks>
    /// <param name="filter">查询条件表达式 / Query condition expression</param>
    /// <param name="cancellationToken">取消令牌 / Cancellation token</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState / Data type, must inherit from BaseCacheState</typeparam>
    /// <returns>返回恢复的记录数 / Number of restored records</returns>
    public static Task<long> RestoreAsync<TState>(Expression<Func<TState, bool>> filter, CancellationToken cancellationToken) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.RestoreAsync<TState>(filter, cancellationToken);
    }
}
