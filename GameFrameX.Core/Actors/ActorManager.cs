using System.Collections.Concurrent;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Actors.Impl;
using GameFrameX.Core.Components;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Timer;
using GameFrameX.Core.Utility;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Actors;

/// <summary>
/// Actor管理器
/// </summary>
public static class ActorManager
{
    private const int WorkerCount = 10;

    private const int OnceSaveCount = 1000;

    private const int CrossDayGlobalWaitSeconds = 60;
    private const int CrossDayNotRoleWaitSeconds = 120;
    private static readonly ConcurrentDictionary<long, Actor> ActorMap = new();

    private static readonly ConcurrentDictionary<long, DateTime> ActiveTimeDic = new ConcurrentDictionary<long, DateTime>();

    private static readonly List<WorkerActor> WorkerActors = new();

    static ActorManager()
    {
        for (var i = 0; i < WorkerCount; i++)
        {
            WorkerActors.Add(new WorkerActor());
        }
    }

    /// <summary>
    /// 根据ActorId获取对应的IComponentAgent对象
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <typeparam name="T">组件代理类型</typeparam>
    /// <returns>组件代理任务</returns>
    public static async Task<T> GetComponentAgent<T>(long actorId, bool isNew = true) where T : IComponentAgent
    {
        Actor actor;
        if (isNew)
        {
            actor = await GetOrNew(actorId);
            return await actor.GetComponentAgent<T>();
        }

        actor = Get(actorId);
        if (actor != null)
        {
            return await actor.GetComponentAgent<T>();
        }

        return await Task.FromResult<T>(default);
    }

    /// <summary>
    /// 是否存在指定的Actor
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <returns>是否存在</returns>
    public static bool HasActor(long actorId)
    {
        return ActorMap.ContainsKey(actorId);
    }

    /// <summary>
    /// 根据ActorId获取对应的Actor
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <returns>Actor对象</returns>
    internal static Actor GetActor(long actorId)
    {
        ActorMap.TryGetValue(actorId, out var actor);
        return actor;
    }

    /// <summary>
    /// 根据ActorId和组件类型获取对应的IComponentAgent数据
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <param name="agentType">组件类型</param>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <returns>组件代理任务</returns>
    internal static async Task<IComponentAgent> GetComponentAgent(long actorId, Type agentType, bool isNew = true)
    {
        Actor actor;
        if (isNew)
        {
            actor = await GetOrNew(actorId);
            return await actor.GetComponentAgent(agentType);
        }

        actor = Get(actorId);
        if (actor != null)
        {
            return await actor.GetComponentAgent(agentType);
        }

        return await Task.FromResult<IComponentAgent>(default);
    }

    /// <summary>
    /// 根据ActorId获取对应Actor中所有激活状态的组件代理对象
    /// </summary>
    /// <param name="actorId">要查询的ActorId</param>
    /// <returns>该Actor下所有处于激活状态的组件代理对象列表,如果Actor不存在则返回空列表</returns>
    /// <remarks>
    /// 该方法会返回指定Actor中所有已经被激活的组件代理对象。
    /// 如果指定的ActorId不存在,将返回一个空列表。
    /// 组件的激活状态由Actor内部维护。
    /// </remarks>
    public static List<IComponentAgent> GetActiveComponentAgents(long actorId)
    {
        var result = new List<IComponentAgent>();
        var actor = GetActor(actorId);
        if (actor.IsNull())
        {
            return result;
        }

        return actor.GetActiveComponentAgents();
    }

    /// <summary>
    /// 根据组件类型获取对应的IComponentAgent数据
    /// </summary>
    /// <typeparam name="T">组件代理类型</typeparam>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <returns>组件代理任务</returns>
    public static Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent
    {
        var compType = HotfixManager.GetComponentType(typeof(T));
        var actorType = ComponentRegister.GetActorType(compType);
        var actorId = ActorIdGenerator.GetActorId(actorType);
        return GetComponentAgent<T>(actorId, isNew);
    }

    /// <summary>
    /// 根据actorId获取对应的actor实例，不存在则新生成一个Actor对象
    /// </summary>
    /// <param name="actorId">actorId</param>
    /// <returns>Actor对象任务</returns>
    internal static async Task<Actor> GetOrNew(long actorId)
    {
        var actorType = ActorIdGenerator.GetActorType(actorId);
        if (actorType < GlobalConst.ActorTypeSeparator)
        {
            var now = TimeHelper.GetUtcNow();
            if (ActiveTimeDic.TryGetValue(actorId, out var activeTime)
                && (now - activeTime).TotalMinutes < 10
                && ActorMap.TryGetValue(actorId, out var actor))
            {
                ActiveTimeDic[actorId] = now;
                return actor;
            }

            return await GetLifeActor(actorId).SendAsync(() =>
            {
                ActiveTimeDic[actorId] = now;
                return ActorMap.GetOrAdd(actorId, k => new Actor(k, ActorIdGenerator.GetActorType(k)));
            });
        }

        return ActorMap.GetOrAdd(actorId, k => new Actor(k, ActorIdGenerator.GetActorType(k)));
    }

    /// <summary>
    /// 根据actorId获取对应的actor实例，不存在则返回空
    /// </summary>
    /// <param name="actorId">actorId</param>
    /// <returns>Actor对象任务</returns>
    private static Actor Get(long actorId)
    {
        var actorType = ActorIdGenerator.GetActorType(actorId);
        Actor valueActor;
        if (actorType < GlobalConst.ActorTypeSeparator)
        {
            var now = TimeHelper.GetUtcNow();
            if (ActiveTimeDic.TryGetValue(actorId, out var activeTime)
                && (now - activeTime).TotalMinutes < 10
                && ActorMap.TryGetValue(actorId, out var actor))
            {
                ActiveTimeDic[actorId] = now;
                return actor;
            }

            ActorMap.TryGetValue(actorId, out valueActor);
            return valueActor;
        }

        ActorMap.TryGetValue(actorId, out valueActor);
        return valueActor;
    }

    /// <summary>
    /// 全部完成
    /// </summary>
    /// <returns>任务集合</returns>
    public static Task AllFinish()
    {
        var tasks = new List<Task>();
        foreach (var actor in ActorMap.Values)
        {
            tasks.Add(actor.SendAsync(() => true));
        }

        return Task.WhenAll(tasks);
    }

    /// <summary>
    /// 根据ActorId 获取玩家
    /// </summary>
    /// <param name="actorId">ActorId</param>
    /// <returns>WorkerActor对象</returns>
    private static WorkerActor GetLifeActor(long actorId)
    {
        return WorkerActors[(int)(actorId % WorkerCount)];
    }

    /// <summary>
    /// 检查并回收空闲的Actor
    /// </summary>
    /// <returns>任务</returns>
    public static Task CheckIdle()
    {
        foreach (var actor in ActorMap.Values)
        {
            if (!actor.AutoRecycle)
            {
                continue;
            }

            async Task Func()
            {
                if (actor.AutoRecycle && (TimeHelper.GetUtcNow() - ActiveTimeDic[actor.Id]).TotalMinutes > GlobalSettings.CurrentSetting.ActorRecycleTime)
                {
                    async Task<bool> Work()
                    {
                        if (ActiveTimeDic.TryGetValue(actor.Id, out var activeTime) && (TimeHelper.GetUtcNow() - ActiveTimeDic[actor.Id]).TotalMinutes > GlobalSettings.CurrentSetting.ActorRecycleTime)
                        {
                            // 防止定时回存失败时State被直接移除
                            if (actor.ReadyToDeActive)
                            {
                                await actor.Inactive();
                                await actor.OnRecycle();
                                ActorMap.TryRemove(actor.Id, out var _);
                                LogHelper.Debug($"actor回收 id:{actor.Id} type:{actor.Type}");
                            }
                            else
                            {
                                // 不能存就久一点再判断
                                ActiveTimeDic[actor.Id] = TimeHelper.GetUtcNow();
                            }
                        }

                        return true;
                    }

                    await GetLifeActor(actor.Id).SendAsync(Work);
                }
            }

            actor.Tell(Func);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 保存所有数据
    /// </summary>
    /// <returns>任务</returns>
    public static async Task SaveAll()
    {
        try
        {
            var begin = DateTime.Now;
            var taskList = new List<Task>();
            foreach (var actor in ActorMap.Values)
            {
                async void Save()
                {
                    await actor.SaveAllState();
                }

                taskList.Add(actor.SendAsync(Save));
            }

            await Task.WhenAll(taskList);
            LogHelper.Info($"save all state, use: {(DateTime.Now - begin).TotalMilliseconds}ms");
        }
        catch (Exception e)
        {
            LogHelper.Error($"save all state error \n{e}");
            throw;
        }
    }

    /// <summary>
    /// 定时回存所有数据
    /// </summary>
    /// <returns>任务</returns>
    public static async Task TimerSave()
    {
        try
        {
            var count = 0;
            var taskList = new List<Task>();
            foreach (var actor in ActorMap.Values)
            {
                // 如果定时回存的过程中关服了，直接终止定时回存，因为关服时会调用SaveAll以保证数据回存
                if (!GlobalTimer.IsWorking)
                {
                    return;
                }

                if (count < OnceSaveCount)
                {
                    async void Work()
                    {
                        await actor.SaveAllState();
                    }

                    taskList.Add(actor.SendAsync(Work));
                    count++;
                }
                else
                {
                    await Task.WhenAll(taskList);
                    await Task.Delay(1000);
                    taskList = new List<Task>();
                    count = 0;
                }
            }
        }
        catch (Exception e)
        {
            LogHelper.Info("timer save state error");
            LogHelper.Error(e.ToString());
        }
    }

    /// <summary>
    /// 角色跨天
    /// </summary>
    /// <param name="openServerDay">开服天数</param>
    /// <returns>任务</returns>
    public static Task RoleCrossDay(int openServerDay)
    {
        foreach (var actor in ActorMap.Values)
        {
            if (actor.Type < GlobalConst.ActorTypeSeparator)
            {
                actor.Tell(() => actor.CrossDay(openServerDay));
            }
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 跨天
    /// </summary>
    /// <param name="openServerDay">开服天数</param>
    /// <param name="driverActorType">驱动Actor类型</param>
    /// <returns>任务</returns>
    public static async Task CrossDay(int openServerDay, ushort driverActorType)
    {
        // 驱动actor优先执行跨天
        var id = ActorIdGenerator.GetActorId(driverActorType);
        var driverActor = ActorMap[id];
        await driverActor.CrossDay(openServerDay);

        var begin = DateTime.Now;
        var a = 0;
        var b = 0;
        foreach (var actor in ActorMap.Values)
        {
            if (actor.Type > GlobalConst.ActorTypeSeparator && actor.Type != driverActorType)
            {
                b++;

                async Task Work()
                {
                    LogHelper.Info($"全局Actor：{actor.Type}执行跨天");
                    await actor.CrossDay(openServerDay);
                    Interlocked.Increment(ref a);
                }

                actor.Tell(Work);
            }
        }

        while (a < b)
        {
            if ((DateTime.Now - begin).TotalSeconds > CrossDayGlobalWaitSeconds)
            {
                LogHelper.Warning($"全局comp跨天耗时过久，不阻止其他comp跨天，当前已过{CrossDayGlobalWaitSeconds}秒");
                break;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }

        var globalCost = (DateTime.Now - begin).TotalMilliseconds;
        LogHelper.Info($"全局comp跨天完成 耗时：{globalCost:f4}ms");
        a = 0;
        b = 0;
        foreach (var actor in ActorMap.Values)
        {
            if (actor.Type > GlobalConst.ActorTypeSeparator)
            {
                b++;

                async Task Work()
                {
                    await actor.CrossDay(openServerDay);
                    Interlocked.Increment(ref a);
                }

                actor.Tell(Work);
            }
        }

        while (a < b)
        {
            if ((DateTime.Now - begin).TotalSeconds > CrossDayNotRoleWaitSeconds)
            {
                LogHelper.Warning($"非玩家comp跨天耗时过久，不阻止玩家comp跨天，当前已过{CrossDayNotRoleWaitSeconds}秒");
                break;
            }

            await Task.Delay(TimeSpan.FromMilliseconds(10));
        }

        var otherCost = (DateTime.Now - begin).TotalMilliseconds - globalCost;
        LogHelper.Info($"非玩家comp跨天完成 耗时：{otherCost:f4}ms");
    }

    /// <summary>
    /// 删除所有actor
    /// </summary>
    /// <returns>任务</returns>
    public static async Task RemoveAll()
    {
        var tasks = new List<Task>();
        foreach (var actor in ActorMap.Values)
        {
            tasks.Add(actor.Inactive());
        }

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 删除actor
    /// </summary>
    /// <param name="actorId">actorId</param>
    /// <returns>任务</returns>
    public static Task Remove(long actorId)
    {
        if (ActorMap.Remove(actorId, out var actor))
        {
            actor.Tell(actor.Inactive);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 遍历所有actor
    /// </summary>
    /// <param name="action">遍历回调</param>
    public static void ActorForEach(Action<IActor> action)
    {
        foreach (var actor in ActorMap.Values)
        {
            try
            {
                action.Invoke(actor);
            }
            catch (Exception e)
            {
                LogHelper.Error(e);
            }
        }
    }

    /// <summary>
    /// 遍历所有actor
    /// </summary>
    /// <param name="func">遍历actor回调</param>
    /// <typeparam name="T">组件代理类型</typeparam>
    public static void ActorForEach<T>(Func<T, Task> func) where T : IComponentAgent
    {
        var agentType = typeof(T);
        var compType = HotfixManager.GetComponentType(agentType);
        var actorType = ComponentRegister.GetActorType(compType);
        foreach (var actor in ActorMap.Values)
        {
            if (actor.Type == actorType)
            {
                async Task Work()
                {
                    var comp = await actor.GetComponentAgent<T>();
                    await func(comp);
                }

                actor.Tell(Work);
            }
        }
    }

    /// <summary>
    /// 遍历所有actor
    /// </summary>
    /// <param name="action">遍历actor回调</param>
    /// <typeparam name="T">组件代理类型</typeparam>
    public static void ActorForEach<T>(Action<T> action) where T : IComponentAgent
    {
        var agentType = typeof(T);
        var compType = HotfixManager.GetComponentType(agentType);
        var actorType = ComponentRegister.GetActorType(compType);
        foreach (var actor in ActorMap.Values)
        {
            if (actor.Type == actorType)
            {
                async Task Work()
                {
                    var comp = await actor.GetComponentAgent<T>();
                    action(comp);
                }

                actor.Tell(Work);
            }
        }
    }

    /// <summary>
    /// 清除所有agent
    /// </summary>
    public static void ClearAgent()
    {
        foreach (var actor in ActorMap.Values)
        {
            actor.Tell(actor.ClearAgent);
        }
    }
}