namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 状态接口
/// </summary>
public interface IState
{
    /// <summary>
    /// 读取状态
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    public Task ReadStateAsync();
}