namespace GameFrameX.Core.Abstractions;

/// <summary>
/// IWorker接口定义
/// </summary>
public interface IWorker
{
    /// <summary>
    /// 发送无返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时,默认为int.MaxValue</param>
    void Tell(Action work, int timeOut = int.MaxValue);

    /// <summary>
    /// 发送有返回值的工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时,默认为int.MaxValue</param>
    void Tell(Func<Task> work, int timeOut = int.MaxValue);

    /// <summary>
    /// 发送无返回值工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时,默认为int.MaxValue</param>
    /// <returns></returns>
    Task SendAsync(Action work, int timeOut = int.MaxValue);

    /// <summary>
    /// 发送有返回值工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时,默认为int.MaxValue</param>
    /// <returns></returns>
    Task<T> SendAsync<T>(Func<T> work, int timeOut = int.MaxValue);

    /// <summary>
    /// 发送有返回值工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时,默认为int.MaxValue</param>
    /// <param name="checkLock">是否检查锁</param>
    /// <returns></returns>
    Task SendAsync(Func<Task> work, int timeOut = int.MaxValue, bool checkLock = true);

    /// <summary>
    /// 发送有返回值工作指令
    /// </summary>
    /// <param name="work">工作内容</param>
    /// <param name="timeOut">超时,默认为int.MaxValue</param>
    /// <returns></returns>
    Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = int.MaxValue);
}