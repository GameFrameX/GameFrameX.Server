using System.Collections.Concurrent;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Timer;
using GameFrameX.Core.Actors.Impl;
using GameFrameX.Core.Components;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Actors;

/// <summary>
/// Actor类,用于管理和协调组件的生命周期、消息传递等核心功能
/// </summary>
public sealed class Actor : IActor
{
    /// <summary>
    /// 默认超时时长,使用int最大值表示无限等待
    /// </summary>
    public const int TimeOut = int.MaxValue;

    /// <summary>
    /// 组件映射字典,用于存储当前Actor下的所有组件实例
    /// </summary>
    private readonly ConcurrentDictionary<Type, BaseComponent> _componentsMap = new();

    /// <summary>
    /// Actor构造函数
    /// </summary>
    /// <param name="id">Actor的唯一标识符</param>
    /// <param name="type">Actor的类型标识</param>
    public Actor(long id, ushort type)
    {
        Id = id;
        Type = type;
        WorkerActor = new WorkerActor(id);

        if (type < GlobalConst.ActorTypeSeparator)
        {
            Tell(() => SetAutoRecycle(true));
        }
        else
        {
            Tell(() => ComponentRegister.ActiveComponents(this));
        }
    }

    /// <summary>
    /// 判断Actor是否准备好进行反激活操作
    /// </summary>
    internal bool ReadyToDeActive
    {
        get { return _componentsMap.Values.All(item => item.ReadyToInactive); }
    }

    /// <summary>
    /// Actor的唯一标识符
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 订阅的定时器任务ID集合
    /// </summary>
    public HashSet<long> ScheduleIdSet { get; } = new();

    /// <summary>
    /// Actor的类型标识,用于区分不同种类的Actor
    /// </summary>
    public ushort Type { get; set; }

    /// <summary>
    /// 工作者Actor实例,负责具体的任务执行
    /// </summary>
    public IWorkerActor WorkerActor { get; init; }

    /// <summary>
    /// 标识Actor是否启用自动回收机制
    /// </summary>
    public bool AutoRecycle { get; private set; }

    /// <summary>
    /// 设置Actor的自动回收状态
    /// </summary>
    /// <param name="autoRecycle">是否启用自动回收,true表示启用,false表示禁用</param>
    public void SetAutoRecycle(bool autoRecycle)
    {
        Tell(() => { AutoRecycle = autoRecycle; });
    }

    /// <summary>
    /// 获取指定类型的组件代理实例
    /// </summary>
    /// <typeparam name="T">组件代理类型</typeparam>
    /// <param name="isNew">当组件不存在时是否创建新实例,默认为true</param>
    /// <returns>返回指定类型的组件代理实例</returns>
    public async Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent
    {
        return (T)await GetComponentAgent(typeof(T), isNew);
    }

    /// <summary>
    /// 根据代理类型获取组件代理实例
    /// </summary>
    /// <param name="agentType">代理类型</param>
    /// <param name="isNew">当组件不存在时是否创建新实例,默认为true</param>
    /// <returns>返回指定类型的组件代理实例</returns>
    public async Task<IComponentAgent> GetComponentAgent(Type agentType, bool isNew = true)
    {
        var compType = agentType.BaseType.GetGenericArguments()[0];
        IComponentAgent agent;
        if (isNew)
        {
            var comp = _componentsMap.GetOrAdd(compType, GetOrAddFactory);
            agent = comp.GetAgent(agentType);
            if (!comp.IsActive)
            {
                async Task Worker()
                {
                    try
                    {
                        await comp.Active();
                    }
                    catch (Exception e)
                    {
                        LogHelper.Fatal(e);
                    }

                    try
                    {
                        await agent.Active();
                    }
                    catch (Exception e)
                    {
                        LogHelper.Fatal(e);
                    }
                }

                await SendAsyncWithoutCheck(Worker);
            }

            return agent;
        }

        if (!_componentsMap.TryGetValue(compType, out var component))
        {
            return default;
        }

        agent = component.GetAgent(agentType);
        if (!component.IsActive)
        {
            async Task Worker()
            {
                try
                {
                    await component.Active();
                }
                catch (Exception e)
                {
                    LogHelper.Fatal(e);
                }

                try
                {
                    await agent.Active();
                }
                catch (Exception e)
                {
                    LogHelper.Fatal(e);
                }
            }

            await SendAsyncWithoutCheck(Worker);
        }

        return agent;
    }

    /// <summary>
    /// 处理跨天逻辑,遍历所有组件并执行跨天操作
    /// </summary>
    /// <param name="openServerDay">开服天数</param>
    public async Task CrossDay(int openServerDay)
    {
        LogHelper.Debug($"actor跨天 id:{Id} type:{Type}");
        foreach (var comp in _componentsMap.Values)
        {
            var agent = comp.GetAgent();
            if (agent is ICrossDay crossDay)
            {
                // 使用try-catch缩小异常影响范围
                try
                {
                    await crossDay.OnCrossDay(openServerDay);
                }
                catch (Exception e)
                {
                    LogHelper.Error($"{agent.GetType().FullName}跨天失败 actorId:{Id} actorType:{Type} 异常：\n{e}");
                }
            }
        }
    }

    /// <summary>
    /// 反激活所有组件,使其进入非活动状态
    /// </summary>
    public async Task Inactive()
    {
        foreach (var item in _componentsMap.Values)
        {
            await item.Inactive();
        }
    }

    /// <summary>
    /// 清理所有组件的缓存代理实例
    /// </summary>
    public void ClearAgent()
    {
        foreach (var comp in _componentsMap.Values)
        {
            comp.ClearCacheAgent();
        }
    }

    /// <summary>
    /// 创建或获取指定类型的组件实例
    /// </summary>
    /// <param name="type">组件类型</param>
    /// <returns>返回基础组件实例</returns>
    private BaseComponent GetOrAddFactory(Type type)
    {
        return ComponentRegister.CreateComponent(this, type);
    }

    /// <summary>
    /// 保存所有组件的当前状态
    /// </summary>
    internal async Task SaveAllState()
    {
        foreach (var item in _componentsMap)
        {
            await item.Value.SaveState();
        }
    }

    /// <summary>
    /// 重写ToString方法,返回Actor的标识信息
    /// </summary>
    /// <returns>返回包含类型和ID的字符串表示</returns>
    public override string ToString()
    {
        return $"{base.ToString()}_{Type}_{Id}";
    }

    #region actor 入队

    /// <summary>
    /// 发送无返回值的工作指令到Actor队列
    /// </summary>
    /// <param name="work">要执行的工作内容</param>
    /// <param name="timeOut">执行超时时间,默认为TimeOut常量值</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    public void Tell(Action work, int timeOut = TimeOut, CancellationToken cancellationToken = default)
    {
        WorkerActor.Tell(work, timeOut, cancellationToken);
    }

    /// <summary>
    /// 发送异步工作指令到Actor队列
    /// </summary>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeOut">执行超时时间,默认为TimeOut常量值</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    public void Tell(Func<Task> work, int timeOut = TimeOut, CancellationToken cancellationToken = default)
    {
        WorkerActor.Tell(work, timeOut, cancellationToken);
    }

    /// <summary>
    /// 发送无返回值的异步工作指令
    /// </summary>
    /// <param name="work">要执行的工作内容</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsync(Action work)
    {
        return WorkerActor.SendAsync(work);
    }

    /// <summary>
    /// 发送带超时的异步工作指令
    /// </summary>
    /// <param name="work">要执行的工作内容</param>
    /// <param name="timeout">超时时间</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsync(Action work, int timeout, CancellationToken cancellationToken = default)
    {
        return WorkerActor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送带返回值的异步工作指令
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="work">要执行的工作内容</param>
    /// <param name="timeout">超时时间,默认为TimeOut常量值</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回指定类型的异步操作结果</returns>
    public Task<T> SendAsync<T>(Func<T> work, int timeout = TimeOut, CancellationToken cancellationToken = default)
    {
        return WorkerActor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送带锁检查的异步工作指令
    /// </summary>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeout">超时时间,默认为TimeOut常量值</param>
    /// <param name="checkLock">是否检查锁,默认为true</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsync(Func<Task> work, int timeout = TimeOut, bool checkLock = true, CancellationToken cancellationToken = default)
    {
        return WorkerActor.SendAsync(work, timeout, checkLock, cancellationToken);
    }

    /// <summary>
    /// 发送不检查锁的异步工作指令
    /// </summary>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeout">超时时间,默认为TimeOut常量值</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsyncWithoutCheck(Func<Task> work, int timeout = TimeOut, CancellationToken cancellationToken = default)
    {
        return WorkerActor.SendAsync(work, timeout, false, cancellationToken);
    }

    /// <summary>
    /// 发送带返回值的异步工作指令
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeout">超时时间,默认为TimeOut常量值</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回指定类型的异步操作结果</returns>
    public Task<T> SendAsync<T>(Func<Task<T>> work, int timeout = TimeOut, CancellationToken cancellationToken = default)
    {
        return WorkerActor.SendAsync(work, timeout, cancellationToken);
    }

    #endregion
}