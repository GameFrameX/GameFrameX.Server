using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Components;
using GameFrameX.Core.Timer;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Utility;

namespace GameFrameX.Core.Hotfix.Agent;

/// <summary>
/// 基础组件代理类，用于管理组件与Actor之间的交互
/// </summary>
/// <typeparam name="TComponent">具体的组件类型</typeparam>
public abstract class BaseComponentAgent<TComponent> : IComponentAgent where TComponent : BaseComponent
{
    /// <summary>
    /// 所有者的组件实例
    /// </summary>
    public TComponent OwnerComponent
    {
        get { return (TComponent)Owner; }
    }

    /// <summary>
    /// 所有者的Actor实例
    /// </summary>
    public IActor Actor
    {
        get { return Owner.Actor; }
    }

    /// <summary>
    /// 订阅的定时任务ID集合
    /// </summary>
    public HashSet<long> ScheduleIdSet
    {
        get { return Actor.ScheduleIdSet; }
    }

    /// <summary>
    /// 组件的所有者
    /// </summary>
    public IComponent Owner { get; private set; }

    /// <summary>
    /// 所有者的Actor ID
    /// </summary>
    public long ActorId
    {
        get { return Actor.Id; }
    }

    /// <summary>
    /// 所有者的类型
    /// </summary>
    public ushort OwnerType
    {
        get { return Actor.Type; }
    }

    /// <summary>
    /// 设置组件的所有者
    /// </summary>
    /// <param name="owner">所有者实例</param>
    public void SetOwner(IComponent owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// 激活组件
    /// </summary>
    public virtual void Active()
    {
    }

    /// <summary>
    /// 反激活组件
    /// </summary>
    /// <returns>一个已完成的任务</returns>
    public virtual Task Inactive()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据代理类型获取组件
    /// </summary>
    /// <param name="agentType">代理类型</param>
    /// <returns>一个任务，返回组件代理实例</returns>
    public Task<IComponentAgent> GetComponentAgent(Type agentType)
    {
        return Actor.GetComponentAgent(agentType);
    }

    /// <summary>
    /// 根据代理类型获取组件代理
    /// </summary>
    /// <typeparam name="T">代理类型</typeparam>
    /// <returns>一个任务，返回指定类型的组件代理实例</returns>
    public Task<T> GetComponentAgent<T>() where T : IComponentAgent
    {
        return Actor.GetComponentAgent<T>();
    }

    /// <summary>
    /// 发送无返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeout">超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌 </param>
    public void Tell(Action work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        Actor.Tell(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送有返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeout">超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌 </param>
    public void Tell(Func<Task> work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        Actor.Tell(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送无返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeout">超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌 </param>
    /// <returns>一个任务</returns>
    public Task SendAsync(Action work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeout">超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌 </param>
    /// <returns>一个任务，返回工作结果</returns>
    public Task<T> SendAsync<T>(Func<T> work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeout">超时时间，默认为int.MaxValue</param>
    /// <param name="checkLock">是否检查锁</param>
    /// <param name="cancellationToken"></param>
    /// <returns>一个任务</returns>
    public Task SendAsync(Func<Task> work, int timeout = int.MaxValue, bool checkLock = true, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeout, checkLock, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken"></param>
    /// <returns>一个任务，返回工作结果</returns>
    public Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeOut, cancellationToken);
    }

    /// <summary>
    /// 设置组件是否自动回收
    /// </summary>
    /// <param name="autoRecycle">是否自动回收</param>
    protected void SetAutoRecycle(bool autoRecycle)
    {
        Actor.SetAutoRecycle(autoRecycle);
    }

    /// <summary>
    /// 处理Actor跨天事件
    /// </summary>
    /// <param name="serverDay">服务器运行天数</param>
    /// <returns>一个任务</returns>
    public Task ActorCrossDay(int serverDay)
    {
        return Actor.CrossDay(serverDay);
    }

    /// <summary>
    /// 取消订阅定时任务
    /// </summary>
    /// <param name="id">定时任务ID</param>
    public void Unscheduled(long id)
    {
        QuartzTimer.Remove(id);
        ScheduleIdSet.Remove(id);
    }

    /// <summary>
    /// 延迟执行定时任务
    /// </summary>
    /// <param name="time">延迟的具体时间</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅的ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Delay<T>(DateTime time, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Delay<T>(ActorId, time - DateTime.Now, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 延迟执行定时任务
    /// </summary>
    /// <param name="time">延迟的时间</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅的ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Delay<T>(long time, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Delay<T>(ActorId, new DateTime(time) - DateTime.Now, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 延迟执行定时任务
    /// </summary>
    /// <param name="delay">延迟多久的时间差</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅的ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Delay<T>(TimeSpan delay, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Delay<T>(ActorId, delay, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 订阅定时任务
    /// </summary>
    /// <param name="delay">延迟时间差</param>
    /// <param name="interval">间隔时间差</param>
    /// <param name="param">参数</param>
    /// <param name="repeatCount">调用次数，如果小于0，则一直循环</param>
    /// <param name="unScheduleId">取消订阅的ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Schedule<T>(TimeSpan delay, TimeSpan interval, Param param = null, int repeatCount = -1, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Schedule<T>(ActorId, delay, interval, param, repeatCount);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 基于每天的定时任务
    /// </summary>
    /// <param name="hour">小时</param>
    /// <param name="minute">分钟</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅的ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Daily<T>(int hour = 0, int minute = 0, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Daily<T>(ActorId, hour, minute, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 基于每周某天的定时任务
    /// </summary>
    /// <param name="dayOfWeek">具体是哪天</param>
    /// <param name="hour">小时</param>
    /// <param name="minute">分钟</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅的ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Weekly<T>(DayOfWeek dayOfWeek, int hour = 0, int minute = 0, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Weekly<T>(ActorId, dayOfWeek, hour, minute, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 基于每周某天的定时任务
    /// </summary>
    /// <param name="hour">小时</param>
    /// <param name="minute">分钟</param>
    /// <param name="param">参数</param>
    /// <param name="dayOfWeeks">某天的参数列表</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long WithDayOfWeeks<T>(int hour, int minute, Param param, params DayOfWeek[] dayOfWeeks) where T : ITimerHandler
    {
        var scheduleId = QuartzTimer.WithDayOfWeeks<T>(ActorId, hour, minute, param, dayOfWeeks);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 基于每月某天的定时任务
    /// </summary>
    /// <param name="dayOfMonth">具体哪天</param>
    /// <param name="hour">小时</param>
    /// <param name="minute">分钟</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long Monthly<T>(int dayOfMonth, int hour = 0, int minute = 0, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Monthly<T>(ActorId, dayOfMonth, hour, minute, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 基于Cron表达式的定时任务
    /// </summary>
    /// <param name="cronExpression">cron表达式</param>
    /// <param name="param">参数</param>
    /// <param name="unScheduleId">取消订阅ID</param>
    /// <typeparam name="T">定时任务处理器类型</typeparam>
    /// <returns>定时任务ID</returns>
    public long WithCronExpression<T>(string cronExpression, Param param = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.WithCronExpression<T>(ActorId, cronExpression, param);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }
}