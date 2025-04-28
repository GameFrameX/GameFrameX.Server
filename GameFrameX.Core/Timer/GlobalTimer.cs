using GameFrameX.Core.Actors;
using GameFrameX.Core.Components;
using GameFrameX.DataBase;
using GameFrameX.Utility.Extensions;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Timer;

/// <summary>
/// 全局定时器
/// </summary>
public static class GlobalTimer
{
    /// <summary>
    /// 循环任务
    /// </summary>
    private static Task _loopTask;

    /// <summary>
    /// 是否正在工作
    /// </summary>
    public static volatile bool IsWorking;

    /// <summary>
    /// 开始全局定时
    /// </summary>
    public static void Start()
    {
        LogHelper.Debug("初始化全局定时开始...");
        IsWorking = true;
        _loopTask = Task.Run(Loop);
        LogHelper.Debug("初始化全局定时完成...");
    }

    /// <summary>
    /// 循环执行的方法
    /// </summary>
    private static async Task Loop()
    {
        var nextSaveTime = NextSaveTime();
        var onceDelay = TimeSpan.FromSeconds(5);

        while (IsWorking)
        {
            LogHelper.Info($"下次定时回存时间 {nextSaveTime}");
            var currentTime = TimeHelper.UnixTimeMilliseconds();
            while (currentTime < nextSaveTime && IsWorking)
            {
                await Task.Delay(onceDelay);
                currentTime = TimeHelper.UnixTimeMilliseconds();
            }

            if (!IsWorking)
            {
                break;
            }

            var startTime = TimeHelper.UnixTimeMilliseconds();
            LogHelper.Info($"开始定时回存 时间:{startTime}");
            await StateComponent.TimerSave();
            var endTime = TimeHelper.UnixTimeMilliseconds();
            var cost = endTime - startTime;
            LogHelper.Info($"结束定时回存 时间:{endTime} 耗时: {cost}ms");
            LogHelper.Info($"开始回收空闲Actor 时间:{startTime}");
            await ActorManager.CheckIdle();
            currentTime = TimeHelper.UnixTimeMilliseconds();
            LogHelper.Info($"结束回收空闲Actor 时间:{currentTime}");
            do
            {
                nextSaveTime = NextSaveTime();
            } while (currentTime > nextSaveTime);
        }
    }

    /// <summary>
    /// 计算下次回存时间
    /// </summary>
    /// <returns>下次回存时间</returns>
    private static long NextSaveTime()
    {
        return TimeHelper.UnixTimeMilliseconds() + GlobalSettings.SaveIntervalInMilliSeconds;
    }

    /// <summary>
    /// 停止全局定时
    /// </summary>
    public static async Task Stop()
    {
        LogHelper.Info("停止全局定时开始...");
        IsWorking = false;
        await _loopTask;
        await StateComponent.SaveAll(true);
        GameDb.Close();
        LogHelper.Info("停止全局定时完成...");
    }
}