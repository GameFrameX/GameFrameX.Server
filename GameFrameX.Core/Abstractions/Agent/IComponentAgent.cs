﻿namespace GameFrameX.Core.Abstractions.Agent;

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
    /// 激活组件代理
    /// <remarks>
    /// 用于初始化并启用组件代理的功能
    /// </remarks>
    /// </summary>
    void Active();

    /// <summary>
    /// 反激活组件代理
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    /// <remarks>
    /// 用于停用组件代理并清理相关资源，这是一个异步操作
    /// </remarks>
    Task Inactive();

    /// <summary>
    /// 根据代理类型获取代理组件
    /// </summary>
    /// <param name="agentType">代理类型</param>
    /// <returns>代理组件</returns>
    public Task<IComponentAgent> GetComponentAgent(Type agentType);

    /// <summary>
    /// 根据泛型代理类型获取代理组件
    /// </summary>
    /// <typeparam name="T">代理组件的类型</typeparam>
    /// <returns>代理组件</returns>
    public Task<T> GetComponentAgent<T>() where T : IComponentAgent;
}