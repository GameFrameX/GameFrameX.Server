﻿// ==========================================================================================
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
using GameFrameX.DataBase.Abstractions;

namespace GameFrameX.DataBase;

/// <summary>
/// 游戏数据库静态工具类,提供对数据库的基本操作封装
/// </summary>
public static class GameDb
{
    /// <summary>
    /// 数据库服务实现实例
    /// </summary>
    private static IDatabaseService _dbServiceImplementation;

    /// <summary>
    /// 初始化GameDb
    /// </summary>
    /// <typeparam name="T">数据库服务的具体实现类型,必须实现IDatabaseService接口且有无参构造函数</typeparam>
    /// <param name="dbOptions">数据库配置选项</param>
    /// <exception cref="ArgumentNullException">当  ConnectionString 或 Name 为null时抛出</exception>
    /// <returns>返回数据库是否初始化成功</returns>
    public static async Task<bool> Init<T>(DbOptions dbOptions) where T : IDatabaseService, new()
    {
        ArgumentNullException.ThrowIfNull(dbOptions, nameof(dbOptions));
        ArgumentNullException.ThrowIfNull(dbOptions.ConnectionString, nameof(dbOptions.ConnectionString));
        ArgumentNullException.ThrowIfNull(dbOptions.Name, nameof(dbOptions.Name));
        _dbServiceImplementation = new T();
        return await _dbServiceImplementation.Open(dbOptions);
    }

    /// <summary>
    /// 以指定类型获取数据库服务实例
    /// </summary>
    /// <typeparam name="T">要转换的数据库服务类型,必须实现IDatabaseService接口</typeparam>
    /// <returns>转换后的数据库服务实例</returns>
    /// <exception cref="InvalidCastException">当类型转换失败时抛出</exception>
    public static T As<T>() where T : IDatabaseService
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return (T)_dbServiceImplementation;
    }

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    public static void Close()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        _dbServiceImplementation.Close();
    }

    /// <summary>
    /// 查找与指定过滤器匹配的文档列表
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>匹配的文档列表</returns>
    public static Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindListAsync(filter);
    }

    /// <summary>
    /// 计算与指定过滤器匹配的文档数量
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>匹配的文档数量</returns>
    public static Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.CountAsync(filter);
    }

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null</returns>
    public static Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingFirstOneAsync(filter, sortExpression);
    }

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <returns>符合条件的第一个元素,如果没有匹配项则返回null</returns>
    public static Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingFirstOneAsync(filter, sortExpression);
    }

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <param name="pageIndex">页码,从0开始</param>
    /// <param name="pageSize">每页数量,默认为10</param>
    /// <returns>分页后的文档列表</returns>
    public static Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortDescendingAsync(filter, sortExpression, pageIndex, pageSize);
    }

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="sortExpression">用于指定排序字段的Lambda表达式</param>
    /// <param name="pageIndex">页码,从0开始</param>
    /// <param name="pageSize">每页数量,默认为10</param>
    /// <returns>分页后的文档列表</returns>
    public static Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindSortAscendingAsync(filter, sortExpression, pageIndex, pageSize);
    }

    /// <summary>
    /// 根据ID加载指定的文档
    /// 当没有找到指定ID的文档时，会创建一个新的文档返回
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数</typeparam>
    /// <param name="id">要查找的文档ID</param>
    /// <param name="filter">可选的附加过滤条件</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档</param>
    /// <returns>找到的文档,如果不存在则返回新的空文档</returns>
    public static Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null, bool isCreateIfNotExists = true) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(id, filter, isCreateIfNotExists);
    }

    /// <summary>
    /// 查找与指定过滤器匹配的单个文档
    /// 当没有找到指定ID的文档时，会创建一个新的文档返回
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档</param>
    /// <returns>找到的第一个匹配文档,如果没有匹配项则返回null</returns>
    public static Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists = true) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.FindAsync(filter, isCreateIfNotExists);
    }

    /// <summary>
    /// 更新指定的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="state">要更新的文档实例</param>
    /// <returns>更新后的文档</returns>
    public static Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(state);
    }

    /// <summary>
    /// 保存单个文档到数据库
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="state">要保存的文档实例</param>
    /// <returns>保存操作的结果</returns>
    public static Task SaveOneAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddAsync(state);
    }

    /// <summary>
    /// 添加或更新文档(如果存在则更新,不存在则添加)
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState且有无参构造函数</typeparam>
    /// <param name="state">要保存或更新的文档实例</param>
    /// <returns>保存或更新后的文档</returns>
    public static Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddOrUpdateAsync(state);
    }

    /// <summary>
    /// 批量保存多个文档到数据库
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="states">要保存的文档集合</param>
    /// <returns>保存操作的任务</returns>
    public static Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AddListAsync(states);
    }

    /// <summary>
    /// 批量更新多个文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="stateList">要更新的文档集合</param>
    /// <returns>成功更新的文档数量</returns>
    public static Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.UpdateAsync(stateList);
    }

    /// <summary>
    /// 检查是否存在符合条件的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选文档的Lambda表达式</param>
    /// <returns>如果存在匹配的文档则返回true,否则返回false</returns>
    public static Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.AnyAsync(filter);
    }

    /// <summary>
    /// 删除符合条件的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="filter">用于筛选要删除文档的Lambda表达式</param>
    /// <returns>成功删除的文档数量</returns>
    public static Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(filter);
    }

    /// <summary>
    /// 删除指定的文档
    /// </summary>
    /// <typeparam name="TState">文档的类型,必须继承自BaseCacheState</typeparam>
    /// <param name="state">要删除的文档实例</param>
    /// <returns>成功删除的文档数量</returns>
    public static Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return _dbServiceImplementation.DeleteAsync(state);
    }

    /// <summary>
    /// 根据条件批量删除数据(软删除)
    /// </summary>
    /// <param name="filter">查询条件表达式</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    public static async Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return await _dbServiceImplementation.DeleteListAsync<TState>(filter);
    }

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)
    /// </summary>
    /// <param name="ids">要删除的ID列表</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    public static async Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new()
    {
        ArgumentNullException.ThrowIfNull(_dbServiceImplementation, nameof(_dbServiceImplementation));
        return await _dbServiceImplementation.DeleteListIdAsync<TState>(ids);
    }
}