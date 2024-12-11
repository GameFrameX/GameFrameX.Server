namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 工作Actor接口定义
/// </summary>
public interface IWorkerActor : IWorker
{
    /// <summary>
    /// 判断是否需要入队
    /// </summary>
    /// <returns>一个元组，包含是否需要入队的布尔值和调用链ID</returns>
    (bool needEnqueue, long chainId) IsNeedEnqueue();

    /// <summary>
    /// 将无返回值的委托入队
    /// </summary>
    /// <param name="work">无返回值的委托</param>
    /// <param name="callChainId">调用链ID</param>
    /// <param name="discard">是否强制入队</param>
    /// <param name="timeOut">超时时间</param>
    /// <returns>一个表示异步操作的任务</returns>
    Task Enqueue(Action work, long callChainId, bool discard = false, int timeOut = int.MaxValue);

    /// <summary>
    /// 将有返回值的委托入队
    /// </summary>
    /// <param name="work">有返回值的委托</param>
    /// <param name="callChainId">调用链ID</param>
    /// <param name="discard">是否强制入队</param>
    /// <param name="timeOut">超时时间</param>
    /// <typeparam name="T">泛型参数</typeparam>
    /// <returns>一个表示异步操作的任务，任务结果为委托的返回值</returns>
    Task<T> Enqueue<T>(Func<T> work, long callChainId, bool discard = false, int timeOut = int.MaxValue);

    /// <summary>
    /// 将返回Task的委托入队
    /// </summary>
    /// <param name="work">返回Task的委托</param>
    /// <param name="callChainId">调用链ID</param>
    /// <param name="discard">是否强制入队</param>
    /// <param name="timeOut">超时时间</param>
    /// <returns>一个表示异步操作的任务</returns>
    Task Enqueue(Func<Task> work, long callChainId, bool discard = false, int timeOut = int.MaxValue);

    /// <summary>
    /// 将返回Task<T>的委托入队</T>
    /// </summary>
    /// <param name="work">返回Task<T>的委托</T></param>
    /// <param name="callChainId">调用链ID</param>
    /// <param name="discard">是否强制入队</param>
    /// <param name="timeOut">超时时间</param>
    /// <typeparam name="T">泛型参数</typeparam>
    /// <returns>一个表示异步操作的任务，任务结果为委托的返回值</returns>
    Task<T> Enqueue<T>(Func<Task<T>> work, long callChainId, bool discard = false, int timeOut = int.MaxValue);
}