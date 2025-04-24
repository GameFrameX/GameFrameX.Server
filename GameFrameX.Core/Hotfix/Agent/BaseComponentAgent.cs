using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Components;
using GameFrameX.Core.Timer;
using GameFrameX.Core.Timer.Handler;

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
    public virtual Task Active()
    {
        return Task.CompletedTask;
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
    /// 根据代理类型获取组件代理实例
    /// </summary>
    /// <param name="agentType">要获取的代理类型</param>
    /// <param name="isNew">是否创建新的实例，true表示创建新实例，false表示获取已存在的实例</param>
    /// <returns>返回一个异步任务，该任务完成时将返回指定类型的组件代理实例</returns>
    public Task<IComponentAgent> GetComponentAgent(Type agentType, bool isNew = true)
    {
        return Actor.GetComponentAgent(agentType, isNew);
    }

    /// <summary>
    /// 获取指定泛型类型的组件代理实例
    /// </summary>
    /// <typeparam name="T">要获取的组件代理类型，必须实现IComponentAgent接口</typeparam>
    /// <param name="isNew">是否创建新的实例，true表示创建新实例，false表示获取已存在的实例</param>
    /// <returns>返回一个异步任务，该任务完成时将返回指定泛型类型的组件代理实例</returns>
    public Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent
    {
        return Actor.GetComponentAgent<T>(isNew);
    }

    /// <summary>
    /// 发送无返回值的工作指令到Actor
    /// </summary>
    /// <param name="work">要执行的工作内容，以Action委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    public void Tell(Action work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        Actor.Tell(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送异步工作指令到Actor
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以Func<Task>委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    public void Tell(Func<Task> work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        Actor.Tell(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送无返回值的工作指令到Actor
    /// </summary>
    /// <param name="work">要执行的工作内容，以Action委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>表示异步操作的Task对象</returns>
    public Task SendAsync(Action work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令到Actor
    /// </summary>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <param name="work">要执行的工作内容，以Func<T>委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>包含执行结果的Task<T>对象</returns>
    public Task<T> SendAsync<T>(Func<T> work, int timeout = int.MaxValue, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令到Actor，支持锁检查
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以Func<Task>委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为int.MaxValue</param>
    /// <param name="checkLock">是否检查Actor的锁状态，默认为true</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>表示异步操作的Task对象</returns>
    public Task SendAsync(Func<Task> work, int timeout = int.MaxValue, bool checkLock = true, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeout, checkLock, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令到Actor
    /// </summary>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <param name="work">要执行的异步工作内容，以Func<Task<T>>委托形式传入</param>
    /// <param name="timeOut">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为int.MaxValue</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>包含执行结果的Task<T>对象</returns>
    public Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default)
    {
        return Actor.SendAsync(work, timeOut, cancellationToken);
    }

    /// <summary>
    /// 设置Actor组件是否自动回收
    /// </summary>
    /// <param name="autoRecycle">true表示启用自动回收，false表示禁用自动回收</param>
    protected void SetAutoRecycle(bool autoRecycle)
    {
        Actor.SetAutoRecycle(autoRecycle);
    }

    /// <summary>
    /// 处理Actor的跨天事件，用于执行每日重置等操作
    /// </summary>
    /// <param name="serverDay">服务器运行的天数，从启动开始计数</param>
    /// <returns>表示异步操作的Task对象</returns>
    public Task ActorCrossDay(int serverDay)
    {
        return Actor.CrossDay(serverDay);
    }

    /// <summary>
    /// 取消订阅指定ID的定时任务，并从调度集合中移除
    /// </summary>
    /// <param name="id">要取消的定时任务ID</param>
    public void Unscheduled(long id)
    {
        QuartzTimer.Remove(id);
        ScheduleIdSet.Remove(id);
    }

    /// <summary>
    /// 暂停定时任务，不再执行
    /// </summary>
    /// <param name="id">定时任务ID</param>
    public void Pause(long id)
    {
        QuartzTimer.Pause(id);
    }

    /// <summary>
    /// 恢复暂停的定时任务，继续执行
    /// </summary>
    /// <param name="id">定时任务ID</param>
    public void Resume(long id)
    {
        QuartzTimer.Resume(id);
    }

    /// <summary>
    /// 延迟执行定时任务，在指定的时间点执行一次任务
    /// </summary>
    /// <param name="time">指定执行任务的具体时间点</param>
    /// <param name="eventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Delay<T>(DateTime time, GameEventArgs eventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Delay<T>(ActorId, time - DateTime.Now, eventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 延迟执行定时任务，将时间戳转换为DateTime后在指定时间点执行一次任务
    /// </summary>
    /// <param name="time">Unix时间戳，将被转换为DateTime类型的执行时间点</param>
    /// <param name="eventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Delay<T>(long time, GameEventArgs eventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Delay<T>(ActorId, new DateTime(time) - DateTime.Now, eventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 延迟执行定时任务，在指定的时间间隔后执行一次任务
    /// </summary>
    /// <param name="delay">从当前时间开始延迟执行的时间间隔</param>
    /// <param name="eventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Delay<T>(TimeSpan delay, GameEventArgs eventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Delay<T>(ActorId, delay, eventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 订阅定时任务，可设置延迟启动和重复执行间隔
    /// </summary>
    /// <param name="delay">首次执行前的延迟时间</param>
    /// <param name="interval">连续执行之间的时间间隔</param>
    /// <param name="gameEventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="repeatCount">任务重复执行的次数，小于0表示无限重复执行</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Schedule<T>(TimeSpan delay, TimeSpan interval, GameEventArgs gameEventArgs = null, int repeatCount = -1, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Schedule<T>(ActorId, delay, interval, gameEventArgs, repeatCount);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 创建每日定时执行的任务，在指定的时间点执行
    /// </summary>
    /// <param name="hour">每天执行的小时数（0-23）</param>
    /// <param name="minute">每天执行的分钟数（0-59）</param>
    /// <param name="gameEventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Daily<T>(int hour = 0, int minute = 0, GameEventArgs gameEventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Daily<T>(ActorId, hour, minute, gameEventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 创建每周定时执行的任务，在指定的星期几和时间点执行
    /// </summary>
    /// <param name="dayOfWeek">指定每周执行的星期几（周日到周六）</param>
    /// <param name="hour">执行的小时数（0-23）</param>
    /// <param name="minute">执行的分钟数（0-59）</param>
    /// <param name="gameEventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Weekly<T>(DayOfWeek dayOfWeek, int hour = 0, int minute = 0, GameEventArgs gameEventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Weekly<T>(ActorId, dayOfWeek, hour, minute, gameEventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 创建在每周多个指定日期执行的定时任务
    /// </summary>
    /// <param name="hour">执行的小时数（0-23）</param>
    /// <param name="minute">执行的分钟数（0-59）</param>
    /// <param name="gameEventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="dayOfWeeks">指定要执行的多个星期几，可变参数数组</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long WithDayOfWeeks<T>(int hour, int minute, GameEventArgs gameEventArgs, params DayOfWeek[] dayOfWeeks) where T : ITimerHandler
    {
        var scheduleId = QuartzTimer.WithDayOfWeeks<T>(ActorId, hour, minute, gameEventArgs, dayOfWeeks);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 创建每月定时执行的任务，在指定的日期和时间点执行
    /// </summary>
    /// <param name="dayOfMonth">指定每月执行的日期（1-31）</param>
    /// <param name="hour">执行的小时数（0-23）</param>
    /// <param name="minute">执行的分钟数（0-59）</param>
    /// <param name="gameEventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long Monthly<T>(int dayOfMonth, int hour = 0, int minute = 0, GameEventArgs gameEventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.Monthly<T>(ActorId, dayOfMonth, hour, minute, gameEventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }

    /// <summary>
    /// 使用Cron表达式创建定时任务，提供更灵活的定时任务调度
    /// </summary>
    /// <param name="cronExpression">标准的Cron表达式，用于定义复杂的执行计划</param>
    /// <param name="gameEventArgs">传递给定时任务处理器的自定义参数对象</param>
    /// <param name="unScheduleId">需要取消的已存在的定时任务ID，如果大于0则会先取消该任务</param>
    /// <typeparam name="T">实现了ITimerHandler接口的定时任务处理器类型</typeparam>
    /// <returns>新创建的定时任务ID，可用于后续取消该任务</returns>
    public long WithCronExpression<T>(string cronExpression, GameEventArgs gameEventArgs = null, long unScheduleId = 0) where T : ITimerHandler
    {
        Unscheduled(unScheduleId);
        var scheduleId = QuartzTimer.WithCronExpression<T>(ActorId, cronExpression, gameEventArgs);
        ScheduleIdSet.Add(scheduleId);
        return scheduleId;
    }
}