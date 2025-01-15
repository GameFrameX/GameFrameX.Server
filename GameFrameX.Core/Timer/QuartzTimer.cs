using GameFrameX.Core.Actors;
using GameFrameX.Core.Hotfix;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Core.Utility;
using GameFrameX.Utility;
using GameFrameX.Utility.Log;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace GameFrameX.Core.Timer;

/// <summary>
/// Quartz定时器
/// </summary>
public static class QuartzTimer
{
    private static readonly StatisticsTool StatisticsTool = new();

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="id"></param>
    public static void UnSchedule(long id)
    {
        if (id <= 0)
        {
            return;
        }

        _scheduler.DeleteJob(JobKey.Create(id.ToString()));
    }

    /// <summary>
    /// 取消订阅
    /// </summary>
    /// <param name="set"></param>
    public static void UnSchedule(IEnumerable<long> set)
    {
        foreach (var id in set)
        {
            UnSchedule(id);
        }
    }

    #region 热更定时器

    /// <summary>
    /// 每隔一段时间执行一次
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="delay">首次执行前的延迟时间</param>
    /// <param name="interval">每次执行之间的间隔时间</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <param name="repeatCount">循环次数，-1表示无限循环</param>
    /// <returns>生成的任务ID</returns>
    public static long Schedule<T>(long actorId, TimeSpan delay, TimeSpan interval, Param param = null, int repeatCount = -1) where T : ITimerHandler
    {
        var nextId = NextId();
        var firstTimeOffset = DateTimeOffset.Now.Add(delay);
        TriggerBuilder builder;
        if (repeatCount < 0)
        {
            builder = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x => x.WithInterval(interval).RepeatForever());
        }
        else
        {
            builder = TriggerBuilder.Create().StartAt(firstTimeOffset).WithSimpleSchedule(x => x.WithInterval(interval).WithRepeatCount(repeatCount));
        }

        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), builder.Build());
        return nextId;
    }

    /// <summary>
    /// 基于时间延迟执行一次
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="delay">延迟时间</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <returns>生成的任务ID</returns>
    public static long Delay<T>(long actorId, TimeSpan delay, Param param = null) where T : ITimerHandler
    {
        var nextId = NextId();
        var firstTimeOffset = DateTimeOffset.Now.Add(delay);
        var trigger = TriggerBuilder.Create().StartAt(firstTimeOffset).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 基于Cron表达式执行任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="cronExpression">Cron表达式</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <returns>生成的任务ID</returns>
    public static long WithCronExpression<T>(long actorId, string cronExpression, Param param = null) where T : ITimerHandler
    {
        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithCronSchedule(cronExpression).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每日特定时间执行任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="hour">小时（0-23）</param>
    /// <param name="minute">分钟（0-59）</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <returns>生成的任务ID</returns>
    public static long Daily<T>(long actorId, int hour, int minute, Param param = null) where T : ITimerHandler
    {
        if (hour < 0 || hour >= 24 || minute < 0 || minute >= 60)
        {
            throw new ArgumentOutOfRangeException($"定时器参数错误 TimerHandler:{typeof(T).FullName} {nameof(hour)}:{hour} {nameof(minute)}:{minute}");
        }

        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每周某些天特定时间执行任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="hour">小时（0-23）</param>
    /// <param name="minute">分钟（0-59）</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <param name="dayOfWeeks">指定的星期几数组</param>
    /// <returns>生成的任务ID</returns>
    public static long WithDayOfWeeks<T>(long actorId, int hour, int minute, Param param, params DayOfWeek[] dayOfWeeks) where T : ITimerHandler
    {
        if (dayOfWeeks == null || dayOfWeeks.Length <= 0)
        {
            throw new ArgumentNullException($"定时每周执行 参数为空：{nameof(dayOfWeeks)} TimerHandler:{typeof(T).FullName} actorId:{actorId} actorType:{ActorIdGenerator.GetActorType(actorId)}");
        }

        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.AtHourAndMinuteOnGivenDaysOfWeek(hour, minute, dayOfWeeks)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每周某天特定时间执行任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="dayOfWeek">指定的星期几</param>
    /// <param name="hour">小时（0-23）</param>
    /// <param name="minute">分钟（0-59）</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <returns>生成的任务ID</returns>
    public static long Weekly<T>(long actorId, DayOfWeek dayOfWeek, int hour, int minute, Param param = null) where T : ITimerHandler
    {
        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(dayOfWeek, hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), trigger);
        return nextId;
    }

    /// <summary>
    /// 每月某天特定时间执行任务
    /// </summary>
    /// <typeparam name="T">定时器处理器类型</typeparam>
    /// <param name="actorId">Actor的唯一标识</param>
    /// <param name="dayOfMonth">指定的月份中的某一天（1-31）</param>
    /// <param name="hour">小时（0-23）</param>
    /// <param name="minute">分钟（0-59）</param>
    /// <param name="param">传递给定时器处理器的参数</param>
    /// <returns>生成的任务ID</returns>
    public static long Monthly<T>(long actorId, int dayOfMonth, int hour, int minute, Param param = null) where T : ITimerHandler
    {
        if (dayOfMonth is < 0 or > 31)
        {
            throw new ArgumentException($"定时器参数错误 TimerHandler:{typeof(T).FullName} {nameof(dayOfMonth)}:{dayOfMonth} actorId:{actorId} actorType:{ActorIdGenerator.GetActorType(actorId)}");
        }

        var nextId = NextId();
        var trigger = TriggerBuilder.Create().StartNow().WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(dayOfMonth, hour, minute)).Build();
        _scheduler.ScheduleJob(GetJobDetail<T>(nextId, actorId, param), trigger);
        return nextId;
    }

    #endregion

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

    #endregion
    */

    #region 调度

    private static IScheduler _scheduler;

    /// <summary>
    /// 可防止反复初始化
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
    /// 设置日志提供程序并创建调度器实例。
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
    /// <returns>一个表示异步操作的任务。</returns>
    /// <remarks>
    /// 开始调度器，使其可以触发已安排的任务。
    /// </remarks>
    public static async Task Start()
    {
        await _scheduler.Start();
    }

    /// <summary>
    /// 停止定时任务调度器
    /// </summary>
    /// <returns>一个表示异步操作的任务。</returns>
    /// <remarks>
    /// 关闭调度器，停止所有正在运行的任务，并防止新的任务被触发。
    /// </remarks>
    public static Task Stop()
    {
        return _scheduler.Shutdown();
    }

    private static long _id = DateTime.Now.Ticks;

    private static long NextId()
    {
        return Interlocked.Increment(ref _id);
    }

    /// <summary>
    /// 定时器
    /// </summary>
    private sealed class TimerJobHelper : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var handlerType = context.JobDetail.JobDataMap.GetString(TimerKey);
            try
            {
                var param = context.JobDetail.JobDataMap.Get(ParamKey) as Param;
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
    /// </summary>
    public const string ParamKey = "param";

    /// <summary>
    /// </summary>
    private const string ActorIdKey = "actor_id";

    /// <summary>
    /// </summary>
    private const string TimerKey = "timer";

    private static IJobDetail GetJobDetail<T>(long id, long actorId, Param param) where T : ITimerHandler
    {
        var handlerType = typeof(T);
        StatisticsTool.Count(handlerType.FullName);
        if (handlerType.Assembly != HotfixManager.HotfixAssembly)
        {
            throw new Exception("定时器代码需要在热更项目里");
        }

        var job = JobBuilder.Create<TimerJobHelper>().WithIdentity(id + string.Empty).Build();
        job.JobDataMap.Add(ParamKey, param);
        job.JobDataMap.Add(ActorIdKey, actorId);
        job.JobDataMap.Add(TimerKey, handlerType.FullName);
        return job;
    }

    private static IJobDetail GetJobDetail<T>(long id, Param param) where T : NotHotfixTimerHandler
    {
        StatisticsTool.Count(typeof(T).FullName);
        var job = JobBuilder.Create<T>().WithIdentity(id + string.Empty).Build();
        job.JobDataMap.Add(ParamKey, param);
        return job;
    }

    private class ConsoleLogProvider : ILogProvider
    {
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

        public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
        {
            throw new NotImplementedException();
        }

        public IDisposable OpenNestedContext(string message)
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}