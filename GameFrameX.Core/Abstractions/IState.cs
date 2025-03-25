namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 状态接口
/// 用于管理和维护对象的状态信息
/// </summary>
public interface IState
{
    /// <summary>
    /// 读取状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态读取完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步读取对象的当前状态信息
    /// </remarks>
    public Task ReadStateAsync();

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态更新完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步更新对象的状态信息
    /// 在状态发生变化时应调用此方法以保持状态的同步
    /// </remarks>
    public Task WriteStateAsync();
}