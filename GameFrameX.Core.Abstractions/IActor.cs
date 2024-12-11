using GameFrameX.Core.Abstractions.Agent;

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// IActor 接口定义
/// </summary>
public interface IActor : IWorker
{
    /// <summary>
    /// 获取或设置 IActor 的唯一标识
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// 获取是否自动回收
    /// </summary>
    bool AutoRecycle { get; }

    /// <summary>
    /// 获取工作 Actor
    /// </summary>
    IWorkerActor WorkerActor { get; }

    /// <summary>
    /// 获取订阅的哈希列表
    /// </summary>
    HashSet<long> ScheduleIdSet { get; }

    /// <summary>
    /// 获取或设置 Actor 类型
    /// </summary>
    ushort Type { get; set; }

    /// <summary>
    /// 清理全部代理
    /// </summary>
    void ClearAgent();

    /// <summary>
    /// 反激活所有组件
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    Task Inactive();

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// </summary>
    /// <param name="agentType">组件类型</param>
    /// <returns>一个表示异步操作的任务，返回 IComponentAgent 实例</returns>
    Task<IComponentAgent> GetComponentAgent(Type agentType);

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>一个表示异步操作的任务，返回指定类型的 IComponentAgent 实例</returns>
    Task<T> GetComponentAgent<T>() where T : IComponentAgent;

    /// <summary>
    /// 设置自动回收标记
    /// </summary>
    /// <param name="autoRecycle">是否自动回收</param>
    void SetAutoRecycle(bool autoRecycle);

    /// <summary>
    /// Actor 跨天处理
    /// </summary>
    /// <param name="serverDay">服务器运行天数</param>
    /// <returns>一个表示异步操作的任务</returns>
    Task CrossDay(int serverDay);
}