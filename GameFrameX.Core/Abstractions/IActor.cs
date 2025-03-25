﻿using GameFrameX.Core.Abstractions.Agent;

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// IActor 接口定义
/// 定义了一个Actor（参与者）的基本行为和属性，用于管理游戏中的实体对象
/// </summary>
public interface IActor : IWorker
{
    /// <summary>
    /// 获取或设置 IActor 的唯一标识
    /// 用于在系统中唯一标识一个Actor实例
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// 获取是否自动回收
    /// 标识当前Actor是否可以被系统自动回收释放
    /// </summary>
    bool AutoRecycle { get; }

    /// <summary>
    /// 获取工作 Actor
    /// 返回当前Actor的工作实例，用于处理具体的业务逻辑
    /// </summary>
    IWorkerActor WorkerActor { get; }

    /// <summary>
    /// 获取订阅的哈希列表
    /// 存储当前Actor订阅的所有调度器ID
    /// </summary>
    HashSet<long> ScheduleIdSet { get; }

    /// <summary>
    /// 获取或设置 Actor 类型
    /// 用于标识Actor的具体类型，便于系统进行分类管理
    /// </summary>
    ushort Type { get; set; }

    /// <summary>
    /// 清理全部代理
    /// 清除当前Actor关联的所有组件代理实例
    /// </summary>
    void ClearAgent();

    /// <summary>
    /// 反激活所有组件
    /// 使当前Actor的所有组件进入非激活状态，通常在Actor被回收前调用
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    Task Inactive();

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// 获取指定类型的组件代理实例，如果不存在且isNew为true则创建新实例
    /// </summary>
    /// <param name="agentType">组件类型，指定要获取的组件的具体类型</param>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <returns>一个表示异步操作的任务，返回 IComponentAgent 实例</returns>
    Task<IComponentAgent> GetComponentAgent(Type agentType, bool isNew = true);

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// 泛型方法版本，用于获取指定类型的组件代理实例
    /// </summary>
    /// <typeparam name="T">组件类型，必须实现IComponentAgent接口</typeparam>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <returns>一个表示异步操作的任务，返回指定类型的 IComponentAgent 实例</returns>
    Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent;

    /// <summary>
    /// 设置自动回收标记
    /// 配置当前Actor是否允许被系统自动回收
    /// </summary>
    /// <param name="autoRecycle">是否自动回收，true表示允许自动回收，false表示禁止自动回收</param>
    void SetAutoRecycle(bool autoRecycle);

    /// <summary>
    /// Actor 跨天处理
    /// 处理游戏服务器跨天时Actor需要执行的相关逻辑
    /// </summary>
    /// <param name="serverDay">服务器运行天数，表示服务器运行的累计天数</param>
    /// <returns>一个表示异步操作的任务</returns>
    Task CrossDay(int serverDay);
}