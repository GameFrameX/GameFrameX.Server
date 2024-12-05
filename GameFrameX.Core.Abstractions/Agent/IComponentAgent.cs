namespace GameFrameX.Core.Abstractions.Agent;

/// <summary>
/// 组件代理接口
/// </summary>
public interface IComponentAgent : IWorker
{
    /// <summary>
    /// 获取Actor的唯一标识
    /// </summary>
    long ActorId { get; }

    /// <summary>
    /// 获取或设置组件的所有者
    /// </summary>
    IComponent Owner { get; set; }

    /// <summary>
    /// 激活组件代理
    /// </summary>
    void Active();

    /// <summary>
    /// 反激活组件代理
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    Task Inactive();

    /// <summary>
    /// 获取所有者的类型
    /// </summary>
    ActorType OwnerType { get; }

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
