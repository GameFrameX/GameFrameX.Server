namespace GameFrameX.Extension;

public static class TimerExtension
{
    /// <summary>
    /// 重置计时器
    /// </summary>
    /// <param name="timer"></param>
    public static void Reset(this System.Timers.Timer timer)
    {
        timer.Stop();
        timer.Start();
    }
}