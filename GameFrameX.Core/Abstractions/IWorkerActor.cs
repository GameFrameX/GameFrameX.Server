namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 工作Actor接口定义
/// 用于处理异步任务队列的Actor接口，继承自IWorker接口
/// </summary>
public interface IWorkerActor : IWorker
{
    /// <summary>
    /// 判断是否需要入队
    /// 检查当前任务是否需要进入队列进行处理
    /// </summary>
    /// <returns>返回一个元组，包含两个值：needEnqueue(bool类型，表示是否需要入队)和chainId(long类型，表示调用链ID)</returns>
    (bool needEnqueue, long chainId) IsNeedEnqueue();

    /// <summary>
    /// 将无返回值的委托入队
    /// 将Action类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的无返回值委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示异步操作的Task对象</returns>
    Task Enqueue(Action work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将有返回值的委托入队
    /// 将Func<T/>类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的有返回值委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">委托返回值的类型</typeparam>
    /// <returns>表示异步操作的Task<T/>对象，其结果为委托的返回值</returns>
    Task<T> Enqueue<T>(Func<T> work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将返回Task的委托入队
    /// 将Func<Task/>类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的返回Task的异步委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <returns>表示异步操作的Task对象</returns>
    Task Enqueue(Func<Task> work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);

    /// <summary>
    /// 将返回Task<T/>的委托入队
    /// 将Func<Task/><T/>>类型的委托添加到任务队列中进行异步处理
    /// </summary>
    /// <param name="work">要执行的返回Task<T/>的异步委托方法</param>
    /// <param name="callChainId">用于跟踪调用链的唯一标识符</param>
    /// <param name="discard">当为true时，表示强制将任务入队，即使队列已满也会尝试入队</param>
    /// <param name="timeOut">任务执行的超时时间，默认为int.MaxValue</param>
    /// <param name="cancellationToken">用于取消操作的令牌</param>
    /// <typeparam name="T">异步操作返回值的类型</typeparam>
    /// <returns>表示异步操作的Task<T/>对象，其结果为委托的返回值</returns>
    Task<T> Enqueue<T>(Func<Task<T>> work, long callChainId, bool discard = false, int timeOut = int.MaxValue, CancellationToken cancellationToken = default);
}