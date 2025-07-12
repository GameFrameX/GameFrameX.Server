using Timer = System.Timers.Timer;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// Timer 扩展方法类
/// </summary>
public static class TimerExtension
{
    /// <summary>
    /// 重置计时器，停止当前计时并重新开始
    /// </summary>
    /// <param name="timer">要重置的计时器实例</param>
    /// <exception cref="ArgumentNullException">当 timer 为 null 时抛出此异常</exception>
    /// <exception cref="ObjectDisposedException">当 timer 已被释放时抛出此异常</exception>
    /// <remarks>
    /// 此方法会先停止计时器，然后重新启动它，相当于重置计时器的计时周期。
    /// 计时器的配置（如间隔时间、自动重置设置等）将保持不变。
    /// </remarks>
    /// <example>
    /// <code>
    /// var timer = new Timer(1000); // 1秒间隔
    /// timer.Start();
    /// // ... 一段时间后
    /// timer.Reset(); // 重置计时器，重新开始计时
    /// </code>
    /// </example>
    public static void Reset(this Timer timer)
    {
        ArgumentNullException.ThrowIfNull(timer, nameof(timer));
        
        timer.Stop();
        timer.Start();
    }
}