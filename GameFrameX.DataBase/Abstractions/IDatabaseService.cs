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

namespace GameFrameX.DataBase.Abstractions;

/// <summary>
/// 数据库服务
/// </summary>
public interface IDatabaseService
{
    /// <summary>
    /// 链接数据库
    /// </summary>
    /// <param name="dbOptions">数据库配置选项</param>
    /// <returns>返回数据库是否初始化成功</returns>
    Task<bool> Open(DbOptions dbOptions);

    /// <summary>
    /// 关闭数据库连接
    /// </summary>
    void Close();

    /// <summary>
    /// 查询单条数据
    /// </summary>
    /// <param name="id">数据的唯一ID</param>
    /// <param name="filter">查询条件</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回符合条件的数据对象</returns>
    Task<TState> FindAsync<TState>(long id, Expression<Func<TState, bool>> filter = null, bool isCreateIfNotExists = true) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询单条数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <param name="isCreateIfNotExists">是否创建不存在的文档</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回符合条件的数据对象</returns>
    Task<TState> FindAsync<TState>(Expression<Func<TState, bool>> filter, bool isCreateIfNotExists = true) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>1
    /// <returns>返回符合条件的数据列表</returns>
    Task<List<TState>> FindListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的第一个元素。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <returns>符合条件的第一个元素。</returns>
    Task<TState> FindSortAscendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的第一个元素。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <returns>符合条件的第一个元素。</returns>
    Task<TState> FindSortDescendingFirstOneAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression) where TState : BaseCacheState, new();

    /// <summary>
    /// 以降序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <param name="pageIndex">页码，从0开始。</param>
    /// <param name="pageSize">每页数量，默认为10。</param>
    /// <returns>符合条件的元素列表。</returns>
    Task<List<TState>> FindSortDescendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new();

    /// <summary>
    /// 以升序方式查找符合条件的元素并进行分页。
    /// </summary>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <param name="filter">过滤表达式。</param>
    /// <param name="sortExpression">排序字段表达式。</param>
    /// <param name="pageIndex">页码，从0开始。</param>
    /// <param name="pageSize">每页数量，默认为10。</param>
    /// <returns>符合条件的元素列表。</returns>
    Task<List<TState>> FindSortAscendingAsync<TState>(Expression<Func<TState, bool>> filter, Expression<Func<TState, object>> sortExpression, int pageIndex = 0, int pageSize = 10) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询数据长度
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回数据的长度</returns>
    Task<long> CountAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回删除的条数</returns>
    Task<long> DeleteAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回删除的条数</returns>
    Task<long> DeleteAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据条件批量删除数据(软删除)
    /// </summary>
    /// <param name="filter">查询条件表达式</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    Task<long> DeleteListAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();

    /// <summary>
    /// 根据ID列表批量删除数据(软删除)
    /// </summary>
    /// <param name="ids">要删除的ID列表</param>
    /// <typeparam name="TState">数据类型,必须继承自BaseCacheState</typeparam>
    /// <returns>返回修改的记录数</returns>
    Task<long> DeleteListIdAsync<TState>(IEnumerable<long> ids) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回增加成功的条数</returns>
    Task AddAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 增加或更新数据
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>修改的条数</returns>
    Task<TState> AddOrUpdateAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存多条数据
    /// </summary>
    /// <param name="states">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns></returns>
    Task AddListAsync<TState>(IEnumerable<TState> states) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存数据,如果数据已经存在则更新,如果不存在则插入
    /// </summary>
    /// <param name="state">数据对象</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回更新后的数据对象</returns>
    Task<TState> UpdateAsync<TState>(TState state) where TState : BaseCacheState, new();

    /// <summary>
    /// 保存多条数据
    /// </summary>
    /// <param name="stateList">数据列表对象</param>
    /// <returns>返回更新成功的数量</returns>
    Task<long> UpdateAsync<TState>(IEnumerable<TState> stateList) where TState : BaseCacheState, new();

    /// <summary>
    /// 查询符合条件的数据是否存在
    /// </summary>
    /// <param name="filter">查询条件</param>
    /// <typeparam name="TState">实现ICacheState接口的类型。</typeparam>
    /// <returns>返回是否存在值,true表示存在，false表示不存在</returns>
    Task<bool> AnyAsync<TState>(Expression<Func<TState, bool>> filter) where TState : BaseCacheState, new();
}