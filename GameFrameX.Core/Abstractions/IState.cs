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
    /// 通常用于从持久化存储（如数据库、文件等）中加载状态
    /// 在对象初始化或需要刷新状态时调用
    /// 实现此方法时应考虑异常处理和并发访问的情况
    /// </remarks>
    public Task ReadStateAsync();

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态更新完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步更新对象的状态信息
    /// 在状态发生变化时应调用此方法以保持状态的同步
    /// 负责将当前内存中的状态持久化到存储介质中
    /// 建议在以下情况调用此方法：
    /// 1. 状态数据发生重要变更时
    /// 2. 定期保存检查点时
    /// 3. 系统关闭前的状态保存
    /// 实现时需要注意：
    /// - 确保数据一致性
    /// - 处理并发写入情况
    /// - 考虑性能影响，适当使用缓存策略
    /// </remarks>
    public Task WriteStateAsync();
}