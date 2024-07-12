namespace GameFrameX.Core.Abstractions.Timer;

/// <summary>
/// 跨小时接口
/// </summary>
public interface ICrossHour
{
    /// <summary>
    /// 在跨小时触发的方法
    /// </summary>
    /// <param name="hour">当前24小时制的小时数</param>
    /// <returns>表示异步操作的任务</returns>
    public Task OnCrossHour(int hour);
}