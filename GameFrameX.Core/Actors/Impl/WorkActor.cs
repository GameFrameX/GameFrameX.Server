using System.Threading.Tasks.Dataflow;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Utility;
using Serilog;
using GameFrameX.Setting;

namespace GameFrameX.Core.Actors.Impl
{
    /// <summary>
    /// 工作Actor
    /// </summary>
    public class WorkerActor : IWorkerActor
    {
        private static readonly ILogger Log = Serilog.Log.ForContext<WorkWrapper>();

        internal      long CurrentChainId { get; set; }
        internal      long Id             { get; init; }
        private const int  TimeOut = 13000;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public WorkerActor(long id = 0)
        {
            if (id == 0)
            {
                id = IdGenerator.GetUniqueId(IdModule.WorkerActor);
            }

            Id          = id;
            ActionBlock = new ActionBlock<WorkWrapper>(InnerRun, new ExecutionDataflowBlockOptions() { MaxDegreeOfParallelism = 1 });
        }

        private static async Task InnerRun(WorkWrapper wrapper)
        {
            var task = wrapper.DoTask();
            try
            {
                await task.WaitAsync(TimeSpan.FromMilliseconds(wrapper.TimeOut));
            }
            catch (TimeoutException)
            {
                WorkerActor.Log.Fatal("wrapper执行超时:" + wrapper.GetTrace());
                //强制设状态-取消该操作
                wrapper.ForceSetResult();
            }
        }

        private ActionBlock<WorkWrapper> ActionBlock { get; init; }

        /// <summary>
        /// chainId == 0说明是新的异步环境
        /// chainId相等说明是一直await下去的（一种特殊情况是自己入自己的队）
        /// </summary>
        /// <returns></returns>
        public (bool needEnqueue, long chainId) IsNeedEnqueue()
        {
            var  chainId                             = RuntimeContext.CurrentChainId;
            bool needEnqueue                         = chainId == 0 || chainId != CurrentChainId;
            if (needEnqueue && chainId == 0) chainId = NextChainId();
            return (needEnqueue, chainId);
        }

        #region 勿调用(仅供代码生成器调用)

        /// <summary>
        /// 压入一个异步任务
        /// </summary>
        /// <param name="work">工作单元</param>
        /// <param name="callChainId">调用链ID</param>
        /// <param name="discard">是否强制</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        public Task Enqueue(Action work, long callChainId, bool discard = false, int timeOut = TimeOut)
        {
            if (!discard && GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                return default;
            var at = new ActionWrapper(work)
                     {
                         Owner       = this,
                         TimeOut     = timeOut,
                         CallChainId = callChainId
                     };
            ActionBlock.SendAsync(at);
            return at.Tcs.Task;
        }

        /// <summary>
        /// 压入一个异步任务
        /// </summary>
        /// <param name="work">工作单元</param>
        /// <param name="callChainId">调用链ID</param>
        /// <param name="discard">是否强制</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        public Task<T> Enqueue<T>(Func<T> work, long callChainId, bool discard = false, int timeOut = TimeOut)
        {
            if (!discard && GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                return default;
            var at = new FuncWrapper<T>(work)
                     {
                         Owner       = this,
                         TimeOut     = timeOut,
                         CallChainId = callChainId
                     };
            ActionBlock.SendAsync(at);
            return at.Tcs.Task;
        }

        /// <summary>
        /// 压入一个异步任务
        /// </summary>
        /// <param name="work">工作单元</param>
        /// <param name="callChainId">调用链ID</param>
        /// <param name="discard">是否强制</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns></returns>
        public Task Enqueue(Func<Task> work, long callChainId, bool discard = false, int timeOut = TimeOut)
        {
            if (!discard && GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                return default;
            var at = new ActionAsyncWrapper(work)
                     {
                         Owner       = this,
                         TimeOut     = timeOut,
                         CallChainId = callChainId
                     };
            ActionBlock.SendAsync(at);
            return at.Tcs.Task;
        }

        /// <summary>
        /// 压入一个异步任务
        /// </summary>
        /// <param name="work">工作单元</param>
        /// <param name="callChainId">调用链ID</param>
        /// <param name="discard">是否强制</param>
        /// <param name="timeOut">超时时间</param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public Task<T> Enqueue<T>(Func<Task<T>> work, long callChainId, bool discard = false, int timeOut = TimeOut)
        {
            if (!discard && GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                return default;
            var at = new FuncAsyncWrapper<T>(work)
                     {
                         Owner       = this,
                         TimeOut     = timeOut,
                         CallChainId = callChainId
                     };
            ActionBlock.SendAsync(at);
            return at.Tcs.Task;
        }

        #endregion

        #region 供框架底层调用(逻辑开发人员应尽量避免调用)

        /// <summary>
        /// 发送无返回值的工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        public void Tell(Action work, int timeOut = Actor.TimeOut)
        {
            var at = new ActionWrapper(work)
                     {
                         Owner       = this,
                         TimeOut     = timeOut,
                         CallChainId = NextChainId(),
                     };
            _ = ActionBlock.SendAsync(at);
        }

        /// <summary>
        /// 发送有返回值的工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        public void Tell(Func<Task> work, int timeOut = Actor.TimeOut)
        {
            var wrapper = new ActionAsyncWrapper(work)
                          {
                              Owner       = this,
                              TimeOut     = timeOut,
                              CallChainId = NextChainId(),
                          };
            _ = ActionBlock.SendAsync(wrapper);
        }

        /// <summary>
        /// 调用该方法禁止丢弃Task，丢弃Task请使用Tell方法
        /// </summary>
        public Task SendAsync(Action work, int timeOut = Actor.TimeOut)
        {
            (bool needEnqueue, long chainId) = IsNeedEnqueue();
            if (needEnqueue)
            {
                if (GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                    return default;

                var at = new ActionWrapper(work)
                         {
                             Owner       = this,
                             TimeOut     = timeOut,
                             CallChainId = chainId,
                         };
                ActionBlock.SendAsync(at);
                return at.Tcs.Task;
            }
            else
            {
                work();
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// 发送有返回值工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        /// <returns></returns>
        public Task<T> SendAsync<T>(Func<T> work, int timeOut = Actor.TimeOut)
        {
            (bool needEnqueue, long chainId) = IsNeedEnqueue();
            if (needEnqueue)
            {
                if (GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                    return default;

                var at = new FuncWrapper<T>(work)
                         {
                             Owner       = this,
                             TimeOut     = timeOut,
                             CallChainId = chainId,
                         };
                ActionBlock.SendAsync(at);
                return at.Tcs.Task;
            }
            else
            {
                return Task.FromResult(work());
            }
        }

        /// <summary>
        /// 发送有返回值工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        /// <returns></returns>
        public Task SendAsync(Func<Task> work, int timeOut = int.MaxValue)
        {
            return SendAsync(work, timeOut, true);
        }

        /// <summary>
        /// 发送有返回值工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        /// <param name="checkLock">是否检查锁</param>
        /// <returns></returns>
        public Task SendAsync(Func<Task> work, int timeOut = Actor.TimeOut, bool checkLock = true)
        {
            (bool needEnqueue, long chainId) = IsNeedEnqueue();
            if (needEnqueue)
            {
                if (checkLock && GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                    return default;

                var wrapper = new ActionAsyncWrapper(work)
                              {
                                  Owner       = this,
                                  TimeOut     = timeOut,
                                  CallChainId = chainId,
                              };
                ActionBlock.SendAsync(wrapper);
                return wrapper.Tcs.Task;
            }
            else
            {
                return work();
            }
        }

        /// <summary>
        /// 发送有返回值工作指令
        /// </summary>
        /// <param name="work">工作内容</param>
        /// <param name="timeOut">超时,默认为int.MaxValue</param>
        /// <returns></returns>
        public Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = Actor.TimeOut)
        {
            (bool needEnqueue, long chainId) = IsNeedEnqueue();
            if (needEnqueue)
            {
                if (GlobalSettings.IsDebug && !ActorLimit.AllowCall(Id))
                    return default;

                var wrapper = new FuncAsyncWrapper<T>(work)
                              {
                                  Owner       = this,
                                  TimeOut     = timeOut,
                                  CallChainId = chainId,
                              };
                ActionBlock.SendAsync(wrapper);
                return wrapper.Tcs.Task;
            }
            else
            {
                return work();
            }
        }

        #endregion

        private static long _chainId = DateTime.Now.Ticks;

        /// <summary>
        /// 调用链生成
        /// </summary>
        public static long NextChainId()
        {
            var id = Interlocked.Increment(ref _chainId);
            if (id == 0)
            {
                id = Interlocked.Increment(ref _chainId);
            }

            return id;
        }
    }
}