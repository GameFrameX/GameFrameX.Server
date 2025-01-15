using Timer = System.Timers.Timer;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// </summary>
public static class TimerExtension
{
    /// <summary>
    /// 重置计时器
    /// </summary>
    /// <param name="timer"></param>
    public static void Reset(this Timer timer)
    {
        timer.Stop();
        timer.Start();
    }
}