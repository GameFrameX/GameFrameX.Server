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
    Task BeforeActivation();

    /// <summary>
    /// 激活组件代理
    /// <remarks>
    /// 用于初始化并启用组件代理的功能
    /// </remarks>
    /// </summary>
    Task Active();

    /// <summary>
    /// 组件激活后的回调方法
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 在组件完成激活流程后执行，可以用于处理一些初始化后的逻辑
    /// </remarks>
    Task AfterActivation();

    /// <summary>
    /// 组件反激活前的回调方法
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 在组件开始反激活流程前执行，可以用于保存状态或清理资源
    /// </remarks>
    Task BeforeInActivation();

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
    Task AfterInActivation();

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