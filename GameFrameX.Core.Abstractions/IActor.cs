using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Actors;

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// IActor接口定义
/// </summary>
public interface IActor : IWorker
{
    /// <summary>
    /// IActor唯一标识
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// 是否自动回收
    /// </summary>
    bool AutoRecycle { get; }

    /// <summary>
    /// 工作Actor
    /// </summary>
    IWorkerActor WorkerActor { get; }

    /// <summary>
    /// 订阅哈希列表
    /// </summary>
    HashSet<long> ScheduleIdSet { get; }

    /// <summary>
    /// Actor类型
    /// </summary>
    ActorType Type { get; set; }

    /// <summary>
    /// 清理全部代理
    /// </summary>
    void ClearAgent();

    /// <summary>
    /// 反激活所有组件
    /// </summary>
    /// <returns></returns>
    Task DeActive();

    /// <summary>
    /// 根据组件类型获取对应的IComponentAgent
    /// </summary>
    /// <param name="agentType">组件类型</param>
    /// <returns></returns>
    Task<IComponentAgent> GetComponentAgent(Type agentType);

    /// <summary>
    /// 根据组件类型获取对应的IComponentAgent
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns></returns>
    Task<T> GetComponentAgent<T>() where T : IComponentAgent;

    /// <summary>
    /// 设置自动回收标记
    /// </summary>
    /// <param name="autoRecycle">是否自动回收</param>
    void SetAutoRecycle(bool autoRecycle);

    /// <summary>
    /// Actor跨天
    /// </summary>
    /// <param name="serverDay">服务器运行天数</param>
    /// <returns></returns>
    Task CrossDay(int serverDay);
}