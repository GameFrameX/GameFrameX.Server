// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

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
public abstract partial class BaseComponentAgent<TComponent> : IComponentAgent where TComponent : BaseComponent
{
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