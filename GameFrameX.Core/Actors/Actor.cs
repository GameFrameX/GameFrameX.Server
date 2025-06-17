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
public sealed class Actor : IActor, IDisposable
{
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
    /// 获取所有已激活的组件代理实例
    /// </summary>
    /// <remarks>
    /// 遍历组件映射字典(_componentsMap),筛选出所有处于激活状态(IsActive=true)的组件,
    /// 并获取它们对应的代理实例。这个方法通常用于需要批量处理或遍历所有活跃组件的场景。
    /// </remarks>
    /// <returns>返回包含所有已激活组件代理实例的列表</returns>
    public List<IComponentAgent> GetActiveComponentAgents()
    {
        var agents = new List<IComponentAgent>();
        foreach (var kv in _componentsMap)
        {
            if (kv.Value.IsActive)
            {
                agents.Add(kv.Value.GetAgent());
            }
        }

        return agents;
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
                        await agent.BeforeActivation();
                        await agent.Active();
                        await agent.AfterActivation();
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
                    await agent.BeforeActivation();
                    await agent.Active();
                    await agent.AfterActivation();
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

    private readonly ConcurrentDictionary<string, object> _data = new ConcurrentDictionary<string, object>();


    /// <summary>
    /// 设置Actor的数据
    /// </summary>
    /// <typeparam name="T">要存储的数据类型</typeparam>
    /// <param name="key">数据的键名</param>
    /// <param name="value">要存储的数据值</param>
    /// <remarks>
    /// 用于在Actor中存储任意类型的数据，通过键值对的方式进行管理。
    /// 如果键已存在，则会覆盖原有的值。
    /// </remarks>
    public void SetData<T>(string key, T value)
    {
        _data[key] = value;
    }

    /// <summary>
    /// 获取Actor中存储的数据
    /// </summary>
    /// <typeparam name="T">要获取的数据类型</typeparam>
    /// <param name="key">数据的键名</param>
    /// <returns>返回指定类型的数据值</returns>
    /// <remarks>
    /// 如果指定的键不存在或类型不匹配，可能会抛出异常。
    /// 使用前建议先确认数据是否存在。
    /// </remarks>
    public T GetData<T>(string key)
    {
        if (_data.TryGetValue(key, out var value))
        {
            return (T)value;
        }

        return default;
    }


    /// <summary>
    /// 移除Actor中存储的数据
    /// </summary>
    /// <param name="key">要移除的数据键名</param>
    /// <returns>如果成功移除数据返回true，如果键不存在返回false</returns>
    /// <remarks>
    /// 从Actor的数据存储中移除指定键的数据。
    /// 如果键不存在，则不会产生任何效果。
    /// </remarks>
    public bool RemoveData(string key)
    {
        return _data.TryRemove(key, out _);
    }

    /// <summary>
    /// 清除Actor中存储的所有数据
    /// </summary>
    /// <remarks>
    /// 该方法会清空Actor中所有通过SetData方法存储的数据。
    /// 清除后所有数据将无法恢复，请谨慎使用。
    /// 通常在Actor被回收或重置时调用此方法。
    /// </remarks>
    public void ClearData()
    {
        _data.Clear();
    }

    /// <summary>
    /// Actor 回收时的处理方法
    /// </summary>
    /// <remarks>
    /// 当 Actor 被系统回收时调用此方法。
    /// 用于执行必要的清理工作，如:
    /// - 释放占用的资源
    /// - 清理组件状态
    /// - 保存需要持久化的数据
    /// - 取消订阅的事件
    /// - 断开网络连接等
    /// </remarks>
    /// <returns>表示异步操作的任务</returns>
    public Task OnRecycle()
    {
        try
        {
            while (_onceRecycleCallbacks.First != null)
            {
                var first = _onceRecycleCallbacks.First;
                _onceRecycleCallbacks.RemoveFirst();
                try
                {
                    first.Value?.Invoke();
                }
                catch (Exception ex)
                {
                    // 记录回调执行异常但继续执行其他回调
                    LogHelper.Error($"Actor回收回调执行异常 actorId:{Id} actorType:{Type} 异常：\n{ex}");
                }
            }
        }
        catch (Exception ex)
        {
            // 记录整体执行异常
            LogHelper.Error($"Actor回收过程异常 actorId:{Id} actorType:{Type} 异常：\n{ex}");
        }
        finally
        {
            _onceRecycleCallbacks.Clear();
        }

        return Task.CompletedTask;
    }

    private readonly LinkedList<Action> _onceRecycleCallbacks = new LinkedList<Action>();

    /// <summary>
    /// 添加一个在Actor回收时执行一次的回调事件
    /// </summary>
    /// <param name="action">要执行的回调方法</param>
    /// <remarks>
    /// 该回调事件只会在Actor被回收时触发一次，之后会自动移除。
    /// 通常用于:
    /// - 执行一次性的清理操作
    /// - 触发状态变更通知
    /// - 记录回收日志等场景
    /// </remarks>
    public void AddOnceRecycleCallback(Action action)
    {
        _onceRecycleCallbacks.AddLast(action);
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
            await item.Value.WriteStateAsync();
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
    /// <param name="timeOut">执行执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    public void Tell(Action work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        WorkerActor.Tell(work, timeOut, cancellationToken);
    }

    /// <summary>
    /// 发送异步工作指令到Actor队列
    /// </summary>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeOut">执行执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    public void Tell(Func<Task> work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

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
    /// <param name="timeout">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsync(Action work, int timeout, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return WorkerActor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送带返回值的异步工作指令
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="work">要执行的工作内容</param>
    /// <param name="timeout">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回指定类型的异步操作结果</returns>
    public Task<T> SendAsync<T>(Func<T> work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return WorkerActor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送带锁检查的异步工作指令
    /// </summary>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeout">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="checkLock">是否检查锁,默认为true</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsync(Func<Task> work, int timeout = -1, bool checkLock = true, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return WorkerActor.SendAsync(work, timeout, checkLock, cancellationToken);
    }

    /// <summary>
    /// 发送不检查锁的异步工作指令
    /// </summary>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeout">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回表示异步操作的Task</returns>
    public Task SendAsyncWithoutCheck(Func<Task> work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return WorkerActor.SendAsync(work, timeout, false, cancellationToken);
    }

    /// <summary>
    /// 发送带返回值的异步工作指令
    /// </summary>
    /// <typeparam name="T">返回值类型</typeparam>
    /// <param name="work">要执行的异步工作内容</param>
    /// <param name="timeout">执行超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消操作的令牌</param>
    /// <returns>返回指定类型的异步操作结果</returns>
    public Task<T> SendAsync<T>(Func<Task<T>> work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return WorkerActor.SendAsync(work, timeout, cancellationToken);
    }

    #endregion

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        ClearAgent();
        ClearData();
    }
}