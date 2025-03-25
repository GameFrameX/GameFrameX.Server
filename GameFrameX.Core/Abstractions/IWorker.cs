namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 工作者接口定义
/// 用于处理异步和同步的工作任务，支持超时和取消操作
/// </summary>
public interface IWorker
{
    /// <summary>
    /// 发送无返回值的工作指令
    /// 同步执行指定的工作内容
    /// </summary>
    /// <param name="work">需要执行的工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒），默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    void Tell(Action work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 发送有返回值的工作指令
    /// 同步执行指定的异步工作内容
    /// </summary>
    /// <param name="work">需要执行的异步工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒），默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    void Tell(Func<Task> work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送无返回值的工作指令
    /// 异步执行指定的工作内容
    /// </summary>
    /// <param name="work">需要执行的工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒），默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示异步操作的任务对象</returns>
    Task SendAsync(Action work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// 异步执行指定的同步工作内容并返回结果
    /// </summary>
    /// <param name="work">需要执行的工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒），默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <returns>包含执行结果的异步任务对象</returns>
    Task<T> SendAsync<T>(Func<T> work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// 异步执行指定的异步工作内容
    /// </summary>
    /// <param name="work">需要执行的异步工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒），默认为int.MaxValue</param>
    /// <param name="checkLock">是否在执行前检查锁定状态</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示嵌套异步操作的任务对象</returns>
    Task SendAsync(Func<Task> work, int timeOut = int.MaxValue, bool checkLock = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// 异步发送有返回值的工作指令
    /// 异步执行指定的异步工作内容并返回结果
    /// </summary>
    /// <param name="work">需要执行的异步工作内容委托</param>
    /// <param name="timeOut">执行超时时间（毫秒），默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <returns>包含执行结果的异步任务对象</returns>
    Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);
}