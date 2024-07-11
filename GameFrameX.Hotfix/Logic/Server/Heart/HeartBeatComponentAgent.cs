using GameFrameX.Apps.Server.Heart.Component;
using GameFrameX.Apps.Server.Heart.Entity;
using GameFrameX.Monitor.Player;

namespace GameFrameX.Hotfix.Logic.Server.Heart;

public class HeartBeatComponentAgent : StateComponentAgent<HeartBeatComponent, HeartBeatState>
{
    /// <summary>
    /// 心跳 定时器
    /// </summary>
    // class HeartBeatScheduleTimer : TimerHandler<HeartBeatComponentAgent>
    // {
    //     protected override Task HandleTimer(HeartBeatComponentAgent agent, Param param)
    //     {
    //         agent.Log.Debug($"心跳 时间:{TimeHelper.CurrentTimeWithFullString()}");
    //         return Task.CompletedTask;
    //     }
    // }

    // private long _heartBeatScheduleTimerId;
    public override void Active()
    {
        // _heartBeatScheduleTimerId = Schedule<HeartBeatScheduleTimer>(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));
    }

    public override Task Inactive()
    {
        // Unscheduled(_heartBeatScheduleTimerId);
        return Task.CompletedTask;
    }

    public void OnUpdateHeartBeatTime(ReqHeartBeat req)
    {
        MetricsPlayerRegister.HeartBeatCounterOptions.Inc();
    }
}