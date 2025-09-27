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

namespace GameFrameX.Core.Abstractions.Agent;

/// <summary>
/// 组件代理接口
/// </summary>
public interface IComponentAgent : IWorker
{
    /// <summary>
    /// 获取Actor的唯一标识
    /// <remarks>
    /// 用于唯一标识一个Actor实例的ID值
    /// </remarks>
    /// </summary>
    long ActorId { get; }

    /// <summary>
    /// 获取或设置组件的所有者
    /// <remarks>
    /// 表示当前组件所属的父级组件实例
    /// </remarks>
    /// </summary>
    IComponent Owner { get; }

    /// <summary>
    /// 获取所有者的类型
    /// <remarks>
    /// 表示所有者组件的类型标识，使用ushort类型存储
    /// </remarks>
    /// </summary>
    ushort OwnerType { get; }

    /// <summary>
    /// 设置组件的所有者
    /// </summary>
    /// <param name="owner">所有者组件实例</param>
    /// <remarks>
    /// 用于设置或更改当前组件的所有者，建立组件间的从属关系
    /// </remarks>
    void SetOwner(IComponent owner);

    /// <summary>
    /// 组件激活前的回调方法
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 在组件开始激活流程前执行，可以用于进行一些预处理操作
    /// </remarks>
    Task<bool> BeforeActivation();

    /// <summary>
    /// 激活组件代理
    /// <remarks>
    /// 用于初始化并启用组件代理的功能
    /// </remarks>
    /// </summary>
    Task<bool> Active();

    /// <summary>
    /// 组件激活后的回调方法
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 在组件完成激活流程后执行，可以用于处理一些初始化后的逻辑
    /// </remarks>
    Task<bool> AfterActivation();

    /// <summary>
    /// 组件反激活前的回调方法
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 在组件开始反激活流程前执行，可以用于保存状态或清理资源
    /// </remarks>
    Task<bool> BeforeInActivation();

    /// <summary>
    /// 反激活组件代理
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    /// <remarks>
    /// 用于停用组件代理并清理相关资源，这是一个异步操作
    /// </remarks>
    Task Inactive();

    /// <summary>
    /// 组件反激活后的回调方法
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 在组件完成反激活流程后执行，可以用于确认清理完成或执行最终操作
    /// </remarks>
    Task<bool> AfterInActivation();

    /// <summary>
    /// 根据代理类型获取代理组件
    /// </summary>
    /// <param name="agentType">代理类型</param>
    /// <param name="isNew">是否创建新实例，默认为true</param>
    /// <returns>代理组件实例</returns>
    /// <remarks>
    /// 通过Type类型参数获取或创建对应的组件代理实例
    /// </remarks>
    public Task<IComponentAgent> GetComponentAgent(Type agentType, bool isNew = true);

    /// <summary>
    /// 根据泛型代理类型获取代理组件
    /// </summary>
    /// <typeparam name="T">代理组件的类型</typeparam>
    /// <param name="isNew">是否创建新实例，默认为true</param>
    /// <returns>指定类型的代理组件实例</returns>
    /// <remarks>
    /// 泛型方法版本，用于获取或创建指定类型的组件代理实例
    /// </remarks>
    public Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent;
}