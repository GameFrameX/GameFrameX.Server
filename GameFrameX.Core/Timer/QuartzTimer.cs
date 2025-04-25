using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Core.Utility;
using GameFrameX.Foundation.Logger;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace GameFrameX.Core.Timer;

/// <summary>
/// Quartz定时器
/// 提供基于Quartz的定时任务调度功能,支持热更新和非热更新两种模式
/// </summary>
public static class QuartzTimer
{
    /// <summary>
    /// 统计工具实例,用于记录定时器的使用情况
    /// </summary>
    private static readonly StatisticsTool StatisticsTool = new();

    /// <summary>
    /// 取消订阅定时任务
    /// </summary>
    /// <param name="id">要取消的定时任务ID</param>
    [Obsolete("请使用Remove(long id)")]
    public static void UnSchedule(long id)
    {
        if (id <= 0)
        {
            return;
        }

        _scheduler.DeleteJob(JobKey.Create(id.ToString()));
    }

    /// <summary>
    /// 批量取消订阅定时任务
    /// </summary>
    /// <param name="set">要取消的定时任务ID集合</param>
    [Obsolete("请使用Remove(IEnumerable<long> set)")]
    public static void UnSchedule(IEnumerable<long> set)
    {
        foreach (var id in set)
        {
            UnSchedule(id);
        }
    }

    /// <summary>
    /// 删除指定的定时任务
    /// </summary>
    /// <param name="id">要删除的定时任务ID</param>
    public static void Remove(long id)
    {
        if (id <= 0)
        {
            return;
        }

        _scheduler.DeleteJob(JobKey.Create(id.ToString()));
    }

    /// <summary>
    /// 批量删除定时任务
    /// </summary>
    /// <param name="set">要删除的定时任务ID集合</param>
    public static void Remove(IEnumerable<long> set)
    {
        foreach (var id in set)
        {
            Remove(id);
        }
    }

    /// <summary>
    /// 暂停指定的定时任务
    /// </summary>
    /// <param name="id">要暂停的定时任务ID</param>
    public static void Pause(long id)
    {
        if (id <= 0)
        {
            return;
        }

        _scheduler.PauseJob(JobKey.Create(id.ToString()));
    }

    /// <summary>
    /// 批量暂停定时任务
    /// </summary>
    /// <param name="set">要暂停的定时任务ID集合</param>
    public static void Pause(IEnumerable<long> set)
    {
        foreach (var id in set)
        {
            Pause(id);
        }
    }

    /// <summary>
    /// 恢复指定的定时任务
    /// </summary>
    /// <param name="id">要恢复的定时任务ID</param>
    public static void Resume(long id)
    {
        if (id <= 0)
        {
            return;
        }

        _scheduler.ResumeJob(JobKey.Create(id.ToString()));
    }

    /// <summary>
    /// 批量恢复定时任务
    /// </summary>
    /// <param name="set">要恢复的定时任务ID集合</param>
    public static void Resume(IEnumerable<long> set)
    {
        foreach (var id in set)
        {
            Resume(id);
        }
    }

    #region 热更定时器

    /// <summary>
    /// 每隔一段时间执行一次的定时任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="delay">首次执行前的延迟时间</param>
    /// <param name="interval">每次执行之间的间隔时间</param>
    /// <param name="eventArgs">传递给定时器处理器的自定义参数</param>
    /// <param name="repeatCount">循环次数,设置为-1表示无限循环执行</param>
    /// <param name="isMissFire">是否允许错过执行</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    public static long Schedule<T>(long actorId, TimeSpan delay, TimeSpan interval, GameEventArgs eventArgs = null, int repeatCount = -1, bool isMissFire = true) where T : ITimerHandler
    {
        var nextId = NextId();
        var firstTimeOffset = DateTimeOffset.Now.Add(delay);
        TriggerBuilder builder;
        if (repeatCount < 0)
        {
            builder = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x =>
            {
                var scheduleBuilder = x.WithInterval(interval).RepeatForever();
                if (isMissFire)
                {
                    scheduleBuilder.WithMisfireHandlingInstructionIgnoreMisfires();
                }
            });
        }
        else
        {
            builder = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x =>
            {
                var scheduleBuilder = x.WithInterval(interval).WithRepeatCount(repeatCount);
                if (isMissFire)
                {
                    scheduleBuilder.WithMisfireHandlingInstructionIgnoreMisfires();
                }
            });
        }

        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, eventArgs), builder.Build());
        return nextId;
    }

    /// <summary>
    /// 延迟执行一次的定时任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="delay">延迟执行的时间</param>
    /// <param name="eventArgs">传递给定时器处理器的自定义参数</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    public static long Delay<T>(long actorId, TimeSpan delay, GameEventArgs eventArgs = null) where T : ITimerHandler
    {
        var nextId = NextId();
        var firstTimeOffset = DateTimeOffset.Now.Add(delay);
        var trigger = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x => x.WithMisfireHandlingInstructionNextWithRemainingCount()).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, eventArgs), trigger);
        return nextId;
    }

    /// <summary>
    /// 基于Cron表达式的定时任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="cronExpression">标准的Cron表达式,用于配置复杂的执行时间规则</param>
    /// <param name="eventArgs">传递给定时器处理器的自定义参数</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    public static long WithCronExpression<T>(long actorId, string cronExpression, GameEventArgs eventArgs = null) where T : ITimerHandler
    {
        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(cronExpression).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, eventArgs), trigger);
        return nextId;
    }

    /// <summary>
    /// 每日定时执行的任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="hour">指定执行的小时(0-23)</param>
    /// <param name="minute">指定执行的分钟(0-59)</param>
    /// <param name="eventArgs">传递给定时器处理器的自定义参数</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    /// <exception cref="ArgumentOutOfRangeException">当hour或minute参数超出有效范围时抛出</exception>
    public static long Daily<T>(long actorId, int hour, int minute, GameEventArgs eventArgs = null) where T : ITimerHandler
    {
        if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
        {
            throw new ArgumentOutOfRangeException($"定时器参数错误 TimerHandler:{typeof(T).FullName} {nameof(hour)}:{hour} {nameof(minute)}:{minute}");
        }

        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, eventArgs), trigger);
        return nextId;
    }

    /// <summary>
    /// 在每周指定的多个日期执行的定时任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="hour">指定执行的小时(0-23)</param>
    /// <param name="minute">指定执行的分钟(0-59)</param>
    /// <param name="gameEventArgs">传递给定时器处理器的自定义参数</param>
    /// <param name="dayOfWeeks">指定要执行的星期几,可以指定多个</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    /// <exception cref="ArgumentNullException">当dayOfWeeks参数为空或长度为0时抛出</exception>
    public static long WithDayOfWeeks<T>(long actorId, int hour, int minute, GameEventArgs gameEventArgs, params DayOfWeek[] dayOfWeeks) where T : ITimerHandler
    {
        if (dayOfWeeks == null || dayOfWeeks.Length <= 0)
        {
            throw new ArgumentNullException($"定时每周执行 参数为空：{nameof(dayOfWeeks)} TimerHandler:{typeof(T).FullName} actorId:{actorId} actorType:{ActorIdGenerator.GetActorType(actorId)}");
        }

        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(hour, minute, dayOfWeeks)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, gameEventArgs), trigger);
        return nextId;
    }

    /// <summary>
    /// 每周固定某天执行的定时任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="dayOfWeek">指定执行的星期几</param>
    /// <param name="hour">指定执行的小时(0-23)</param>
    /// <param name="minute">指定执行的分钟(0-59)</param>
    /// <param name="gameEventArgs">传递给定时器处理器的自定义参数</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    public static long Weekly<T>(long actorId, DayOfWeek dayOfWeek, int hour, int minute, GameEventArgs gameEventArgs = null) where T : ITimerHandler
    {
        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(dayOfWeek, hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, gameEventArgs), trigger);
        return nextId;
    }

    /// <summary>
    /// 每月固定某天执行的定时任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型,必须实现ITimerHandler接口</typeparam>
    /// <param name="actorId">Actor的唯一标识,用于定位执行任务的Actor</param>
    /// <param name="dayOfMonth">指定执行的日期(1-31)</param>
    /// <param name="hour">指定执行的小时(0-23)</param>
    /// <param name="minute">指定执行的分钟(0-59)</param>
    /// <param name="gameEventArgs">传递给定时器处理器的自定义参数</param>
    /// <returns>生成的定时任务ID,可用于后续管理该任务</returns>
    /// <exception cref="ArgumentException">当dayOfMonth参数超出有效范围时抛出</exception>
    public static long Monthly<T>(long actorId, int dayOfMonth, int hour, int minute, GameEventArgs gameEventArgs = null) where T : ITimerHandler
    {
        if (dayOfMonth is < 0 or > 31)
        {
            throw new ArgumentException($"定时器参数错误 TimerHandler:{typeof(T).FullName} {nameof(dayOfMonth)}:{dayOfMonth} actorId:{actorId} actorType:{ActorIdGenerator.GetActorType(actorId)}");
        }

        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(dayOfMonth, hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, gameEventArgs), trigger);
        return nextId;
    }

    #endregion 热更定时器

    /*

    #region 非热更定时器

    /// <summary>
    /// 每隔一段时间执行一次
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delay"></param>
    /// <param name="interval"></param>
    /// <param name="param"></param>
    /// <param name="repeatCount"> -1 表示永远 </param>
    /// <returns></returns>
    public static long Schedule<T>(TimeSpan delay, TimeSpan interval, Param param = null, int repeatCount = -1) where T : NotHotfixTimerHandler
    {
        var            nextId          = NextId();
        var            firstTimeOffset = DateTimeOffset.Now.Add(delay);
        TriggerBuilder builder;
        if (repeatCount < 0)
        {
            builder = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x => x.WithInterval(interval).RepeatForever());
        }
        else
        {
            builder = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x => x.WithInterval(interval).WithRepeatCount(repeatCount));
        }

        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), builder.Build());
        return nextId;
    }

    /// <summary>
    /// 基于时间delay
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="delay"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static long Delay<T>(TimeSpan delay, Param param = null) where T : NotHotfixTimerHandler
    {
        var nextId          = NextId();
        var firstTimeOffset = DateTimeOffset.Now.Add(delay);
        var trigger         = TriggerBuilder.Create().StartAt(firstTimeOffset).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 基于cron表达式
    /// </summary>
    /// <param name="cronExpression"></param>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static long WithCronExpression<T>(string cronExpression, Param param = null) where T : NotHotfixTimerHandler
    {
        var nextId  = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(cronExpression).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每日
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static long Daily<T>(int hour, int minute, Param param = null) where T : NotHotfixTimerHandler
    {
        if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
        {
            throw new ArgumentOutOfRangeException($"定时器参数错误 TimerHandler:{typeof(T).FullName} {nameof(hour)}:{hour} {nameof(minute)}:{minute}");
        }

        var nextId  = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每周某些天
    /// </summary>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="param"></param>
    /// <param name="dayOfWeeks"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static long WithDayOfWeeks<T>(int hour, int minute, Param param, params DayOfWeek[] dayOfWeeks) where T : NotHotfixTimerHandler
    {
        if (dayOfWeeks == null || dayOfWeeks.Length <= 0)
        {
            throw new ArgumentNullException($"定时每周执行 参数为空：{nameof(dayOfWeeks)} TimerHandler:{typeof(T).FullName}");
        }

        var nextId  = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(hour, minute, dayOfWeeks)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每周某天
    /// </summary>
    /// <param name="dayOfWeek"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static long Weekly<T>(DayOfWeek dayOfWeek, int hour, int minute, Param param = null) where T : NotHotfixTimerHandler
    {
        var nextId  = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(dayOfWeek, hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每月某天
    /// </summary>
    /// <param name="dayOfMonth"></param>
    /// <param name="hour"></param>
    /// <param name="minute"></param>
    /// <param name="param"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static long Monthly<T>(int dayOfMonth, int hour, int minute, Param param = null) where T : NotHotfixTimerHandler
    {
        if (dayOfMonth is < 0 or > 31)
        {
            throw new ArgumentException($"定时器参数错误 TimerHandler:{typeof(T).FullName} {nameof(dayOfMonth)}:{dayOfMonth}");
        }

        var nextId  = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(dayOfMonth, hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, param), trigger);
        return nextId;
    }

    #endregion 非热更定时器

    */

    #region 调度

    /// <summary>
    /// Quartz调度器实例
    /// </summary>
    private static IScheduler _scheduler;

    /// <summary>
    /// 静态构造函数
    /// 用于初始化调度器,确保只初始化一次
    /// </summary>
    static QuartzTimer()
    {
        Init().Wait();
        Start().Wait();
    }

    /// <summary>
    /// 初始化定时任务调度器
    /// </summary>
    /// <remarks>
    /// 设置日志提供程序并创建调度器实例
    /// </remarks>
    private static async Task Init()
    {
        LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
        var factory = new StdSchedulerFactory();
        _scheduler = await factory.GetScheduler();
    }

    /// <summary>
    /// 启动定时任务调度器
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 启动调度器,使其开始处理已安排的任务
    /// </remarks>
    public static async Task Start()
    {
        await _scheduler.Start();
    }

    /// <summary>
    /// 停止定时任务调度器
    /// </summary>
    /// <returns>表示异步操作的任务</returns>
    /// <remarks>
    /// 关闭调度器,停止所有正在运行的任务,并阻止新任务的触发
    /// </remarks>
    public static Task Stop()
    {
        return _scheduler.Shutdown();
    }

    /// <summary>
    /// 任务ID生成器的初始值,基于当前时间的Ticks
    /// </summary>
    private static long _id = DateTime.Now.Ticks;

    /// <summary>
    /// 生成下一个任务ID
    /// </summary>
    /// <returns>新的任务ID</returns>
    private static long NextId()
    {
        return Interlocked.Increment(ref _id);
    }

    /// <summary>
    /// 定时器任务执行助手类
    /// 负责执行定时任务并处理异常情况
    /// </summary>
    private sealed class TimerJobHelper : IJob
    {
        /// <summary>
        /// 执行定时任务
        /// </summary>
        /// <param name="context">任务执行上下文</param>
        /// <returns>表示异步操作的任务</returns>
        public async Task Execute(IJobExecutionContext context)
        {
            var handlerType = context.JobDetail.JobDataMap.GetString(TimerKey);
            try
            {
                var param = context.JobDetail.JobDataMap.Get(ParamKey) as GameEventArgs;
                var handler = HotfixManager.GetInstance<ITimerHandler>(handlerType);
                if (handler != null)
                {
                    var actorId = context.JobDetail.JobDataMap.GetLong(ActorIdKey);
                    var agentType = handler.GetType().BaseType.GenericTypeArguments[0];
                    var comp = await ActorManager.GetComponentAgent(actorId, agentType);
                    comp.Tell(() => handler.InnerHandleTimer(comp, param));
                }
                else
                {
                    LogHelper.Error($"错误的ITimer类型，回调失败 type:{handlerType}");
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
            }
        }
    }

    /// <summary>
    /// 任务参数的键名
    /// </summary>
    public const string ParamKey = "param";

    /// <summary>
    /// Actor ID的键名
    /// </summary>
    private const string ActorIdKey = "actor_id";

    /// <summary>
    /// 定时器类型的键名
    /// </summary>
    private const string TimerKey = "timer";

    /// <summary>
    /// 创建热更新定时任务的作业详情
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="id">任务ID</param>
    /// <param name="actorId">Actor ID</param>
    /// <param name="eventArgs">任务参数</param>
    /// <returns>作业详情对象</returns>
    /// <exception cref="Exception">当定时器不在热更新程序集中时抛出</exception>
    private static IJobDetail GetJobDetail<T>(long id, long actorId, GameEventArgs eventArgs) where T : ITimerHandler
    {
        var handlerType = typeof(T);
        StatisticsTool.Count(handlerType.FullName);
        if (handlerType.Assembly != HotfixManager.HotfixAssembly)
        {
            throw new Exception("定时器代码需要在热更项目里");
        }

        var job = JobBuilder.Create<TimerJobHelper>().WithIdentity(id + string.Empty).Build();
        job.JobDataMap.Add(ParamKey, eventArgs);
        job.JobDataMap.Add(ActorIdKey, actorId);
        job.JobDataMap.Add(TimerKey, handlerType.FullName);
        return job;
    }

    /// <summary>
    /// 创建非热更新定时任务的作业详情
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="id">任务ID</param>
    /// <param name="gameEventArgs">任务参数</param>
    /// <returns>作业详情对象</returns>
    private static IJobDetail GetJobDetail<T>(long id, GameEventArgs gameEventArgs) where T : NotHotfixTimerHandler
    {
        StatisticsTool.Count(typeof(T).FullName);
        var job = JobBuilder.Create<T>().WithIdentity(id + string.Empty).Build();
        job.JobDataMap.Add(ParamKey, gameEventArgs);
        return job;
    }

    /// <summary>
    /// 控制台日志提供程序
    /// 用于处理Quartz的日志输出
    /// </summary>
    private class ConsoleLogProvider : ILogProvider
    {
        /// <summary>
        /// 获取日志记录器
        /// </summary>
        /// <param name="name">日志记录器名称</param>
        /// <returns>日志记录委托</returns>
        public Logger GetLogger(string name)
        {
            return (level, func, exception, parameters) =>
            {
                if (func != null)
                {
                    if (level < LogLevel.Warn)
                    {
                        // if (level == LogLevel.Debug)
                        // {
                        //     LogHelper.Debug(func(), parameters);
                        // }
                        // else
                        // {
                        //     LogHelper.Info(func(), parameters);
                        // }
                    }
                    else if (level == LogLevel.Warn)
                    {
                        LogHelper.Warn(func(), parameters);
                    }
                    else
                    {
                        LogHelper.Error(func(), parameters);
                    }
                }

                return true;
            };
        }

        /// <summary>
        /// 打开映射上下文
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="destructure">是否解构</param>
        /// <returns>可释放的对象</returns>
        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 打开嵌套上下文
        /// </summary>
        /// <param name="message">消息</param>
        /// <returns>可释放的对象</returns>
        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }

    #endregion 调度
}