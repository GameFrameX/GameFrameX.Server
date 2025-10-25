// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Threading.Tasks.Dataflow;
using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Utility;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;
using Serilog;

namespace GameFrameX.Core.Actors.Impl;

/// <summary>
/// 工作Actor类,用于处理异步任务队列
/// </summary>
public class WorkerActor : IWorkerActor
{
    /// <summary>
    /// 日志记录器实例
    /// </summary>
    private static readonly ILogger Log = Serilog.Log.ForContext<WorkWrapper>();

    /// <summary>
    /// 调用链ID,初始值为当前时间刻度
    /// </summary>
    private static long _chainId = DateTime.Now.Ticks;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="id">Actor的唯一标识,如果为0则自动生成</param>
    public WorkerActor(long id = 0)
    {
        if (id == 0)
        {
            id = ActorIdGenerator.GetUniqueId(GlobalConst.WorkerActorIdModuleValue);
        }

        Id = id;
        ActionBlock = new ActionBlock<WorkWrapper>(InnerRun, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 1, });
    }

    /// <summary>
    /// 当前调用链ID
    /// </summary>
    internal long CurrentChainId { get; set; }

    /// <summary>
    /// Actor的唯一标识
    /// </summary>
    internal long Id { get; init; }

    /// <summary>
    /// 任务执行块,用于串行处理异步任务
    /// </summary>
    private ActionBlock<WorkWrapper> ActionBlock { get; }

    /// <summary>
    /// 判断是否需要将任务加入队列
    /// chainId == 0说明是新的异步环境
    /// chainId相等说明是一直await下去的（一种特殊情况是自己入自己的队）
    /// </summary>
    /// <returns>返回一个元组(是否需要入队,调用链ID)</returns>
    public (bool needEnqueue, long chainId) IsNeedEnqueue()
    {
        var chainId = RuntimeContext.CurrentChainId;
        var needEnqueue = chainId == 0 || chainId != CurrentChainId;
        if (needEnqueue && chainId == 0)
        {
            chainId = NextChainId();
        }

        return (needEnqueue, chainId);
    }

    /// <summary>
    /// 内部执行方法,处理工作包装器
    /// </summary>
    /// <param name="wrapper">工作包装器</param>
    private static async Task InnerRun(WorkWrapper wrapper)
    {
        var task = wrapper.DoTask();
        try
        {
            await task.WaitAsync(TimeSpan.FromMilliseconds(wrapper.TimeOut));
        }
        catch (TimeoutException)
        {
            Log.Fatal("wrapper执行超时:" + wrapper.GetTrace());
            //强制设状态-取消该操作
            wrapper.ForceSetResult();
        }
    }

    /// <summary>
    /// 生成新的调用链ID
    /// </summary>
    /// <returns>返回新生成的调用链ID</returns>
    public static long NextChainId()
    {
        var id = Interlocked.Increment(ref _chainId);
        if (id == 0)
        {
            id = Interlocked.Increment(ref _chainId);
        }

        return id;
    }

    #region 勿调用(仅供代码生成器调用)

    /// <summary>
    /// 将一个同步Action任务压入Actor的任务队列中执行
    /// </summary>
    /// <param name="work">要执行的同步Action工作单元</param>
    /// <param name="callChainId">用于追踪任务调用链的唯一标识符</param>
    /// <param name="discard">是否忽略调试模式下的调用限制检查，true表示强制执行</param>
    /// <param name="timeOut">任务执行的超时时间(毫秒),默认为-1,将采用配置时间ActorQueueTimeOut</param>
    /// <param name="cancellationToken">用于取消任务的令牌</param>
    /// <returns>返回一个Task对象，表示异步操作的完成状态</returns>
    public Task Enqueue(Action work, long callChainId, bool discard = false, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (!discard && GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
        {
            return default;
        }

        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorQueueTimeOut;
        }

        var at = new ActionWrapper(work)
        {
            Owner = this,
            TimeOut = timeOut,
            CallChainId = callChainId,
        };
        ActionBlock.SendAsync(at, cancellationToken);
        return at.Tcs.Task;
    }

    /// <summary>
    /// 将一个有返回值的同步Func任务压入Actor的任务队列中执行
    /// </summary>
    /// <typeparam name="T">函数返回值的类型</typeparam>
    /// <param name="work">要执行的同步Func工作单元</param>
    /// <param name="callChainId">用于追踪任务调用链的唯一标识符</param>
    /// <param name="discard">是否忽略调试模式下的调用限制检查，true表示强制执行</param>
    /// <param name="timeOut">任务执行的超时时间(毫秒),默认为-1,将采用配置时间ActorQueueTimeOut</param>
    /// <param name="cancellationToken">用于取消任务的令牌</param>
    /// <returns>返回一个Task<T/>对象，表示异步操作的完成状态和结果</returns>
    public Task<T> Enqueue<T>(Func<T> work, long callChainId, bool discard = false, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (!discard && GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
        {
            return default;
        }

        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorQueueTimeOut;
        }

        var at = new FuncWrapper<T>(work)
        {
            Owner = this,
            TimeOut = timeOut,
            CallChainId = callChainId,
        };
        ActionBlock.SendAsync(at, cancellationToken);
        return at.Tcs.Task;
    }

    /// <summary>
    /// 将一个异步Task任务压入Actor的任务队列中执行
    /// </summary>
    /// <param name="work">要执行的异步Task工作单元</param>
    /// <param name="callChainId">用于追踪任务调用链的唯一标识符</param>
    /// <param name="discard">是否忽略调试模式下的调用限制检查，true表示强制执行</param>
    /// <param name="timeOut">任务执行的超时时间(毫秒),默认为-1,将采用配置时间ActorQueueTimeOut</param>
    /// <param name="cancellationToken">用于取消任务的令牌</param>
    /// <returns>返回一个Task对象，表示异步操作的完成状态</returns>
    public Task Enqueue(Func<Task> work, long callChainId, bool discard = false, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (!discard && GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
        {
            return default;
        }

        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorQueueTimeOut;
        }

        var at = new ActionAsyncWrapper(work)
        {
            Owner = this,
            TimeOut = timeOut,
            CallChainId = callChainId,
        };
        ActionBlock.SendAsync(at, cancellationToken);
        return at.Tcs.Task;
    }

    /// <summary>
    /// 将一个有返回值的异步Task任务压入Actor的任务队列中执行
    /// </summary>
    /// <typeparam name="T">异步任务返回值的类型</typeparam>
    /// <param name="work">要执行的异步Task工作单元</param>
    /// <param name="callChainId">用于追踪任务调用链的唯一标识符</param>
    /// <param name="discard">是否忽略调试模式下的调用限制检查，true表示强制执行</param>
    /// <param name="timeOut">任务执行的超时时间(毫秒),默认为-1,将采用配置时间ActorQueueTimeOut</param>
    /// <param name="cancellationToken">用于取消任务的令牌</param>
    /// <returns>返回一个Task<T/>对象，表示异步操作的完成状态和结果</returns>
    public Task<T> Enqueue<T>(Func<Task<T>> work, long callChainId, bool discard = false, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (!discard && GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
        {
            return default;
        }

        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorQueueTimeOut;
        }

        var at = new FuncAsyncWrapper<T>(work)
        {
            Owner = this,
            TimeOut = timeOut,
            CallChainId = callChainId,
        };
        ActionBlock.SendAsync(at, cancellationToken);
        return at.Tcs.Task;
    }

    #endregion

    #region 供框架底层调用(逻辑开发人员应尽量避免调用)

    /// <summary>
    /// 发送无返回值的工作指令到Actor的任务队列中
    /// </summary>
    /// <param name="work">要执行的工作内容，以Action委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    public void Tell(Action work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        var at = new ActionWrapper(work)
        {
            Owner = this,
            TimeOut = timeOut,
            CallChainId = NextChainId(),
        };
        _ = ActionBlock.SendAsync(at, cancellationToken);
    }

    /// <summary>
    /// 发送异步工作指令到Actor的任务队列中，不等待其完成
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以Func<Task/>委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    public void Tell(Func<Task> work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        var wrapper = new ActionAsyncWrapper(work)
        {
            Owner = this,
            TimeOut = timeOut,
            CallChainId = NextChainId(),
        };
        _ = ActionBlock.SendAsync(wrapper, cancellationToken);
    }

    /// <summary>
    /// 发送同步工作指令到Actor的任务队列中并等待其完成
    /// 注意：调用该方法禁止丢弃Task，如需丢弃Task请使用Tell方法
    /// </summary>
    /// <param name="work">要执行的同步工作内容，以Action委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    /// <returns>返回表示异步操作的Task对象</returns>
    public Task SendAsync(Action work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        var (needEnqueue, chainId) = IsNeedEnqueue();
        if (needEnqueue)
        {
            if (GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
            {
                return default;
            }

            if (timeOut <= 0)
            {
                timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
            }

            var at = new ActionWrapper(work)
            {
                Owner = this,
                TimeOut = timeOut,
                CallChainId = chainId,
            };
            ActionBlock.SendAsync(at, cancellationToken);
            return at.Tcs.Task;
        }

        work();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 发送有返回值的同步工作指令到Actor的任务队列中并等待其完成
    /// </summary>
    /// <typeparam name="T">返回值的类型</typeparam>
    /// <param name="work">要执行的同步工作内容，以Func<T/>委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    /// <returns>返回包含执行结果的Task<T/>对象</returns>
    public Task<T> SendAsync<T>(Func<T> work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        var (needEnqueue, chainId) = IsNeedEnqueue();
        if (needEnqueue)
        {
            if (GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
            {
                return default;
            }

            if (timeOut <= 0)
            {
                timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
            }

            var at = new FuncWrapper<T>(work)
            {
                Owner = this,
                TimeOut = timeOut,
                CallChainId = chainId,
            };
            ActionBlock.SendAsync(at, cancellationToken);
            return at.Tcs.Task;
        }

        return Task.FromResult(work());
    }

    /// <summary>
    /// 发送异步工作指令到Actor的任务队列中并等待其完成
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以Func<Task/>委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    /// <returns>返回表示异步操作的Task对象</returns>
    public Task SendAsync(Func<Task> work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return SendAsync(work, timeOut, true, cancellationToken);
    }

    /// <summary>
    /// 发送异步工作指令到Actor的任务队列中并等待其完成，支持配置是否检查锁
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以Func<Task/>委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="checkLock">是否检查Actor调用限制锁，true表示检查，false表示不检查</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    /// <returns>返回表示异步操作的Task对象</returns>
    public Task SendAsync(Func<Task> work, int timeOut = -1, bool checkLock = true, CancellationToken cancellationToken = default)
    {
        var (needEnqueue, chainId) = IsNeedEnqueue();
        if (needEnqueue)
        {
            if (checkLock && GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
            {
                return default;
            }

            if (timeOut <= 0)
            {
                timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
            }

            var wrapper = new ActionAsyncWrapper(work)
            {
                Owner = this,
                TimeOut = timeOut,
                CallChainId = chainId,
            };
            ActionBlock.SendAsync(wrapper, cancellationToken);
            return wrapper.Tcs.Task;
        }

        return work();
    }

    /// <summary>
    /// 发送有返回值的异步工作指令到Actor的任务队列中并等待其完成
    /// </summary>
    /// <typeparam name="T">返回值的类型</typeparam>
    /// <param name="work">要执行的异步工作内容，以Func<Task/><T/>>委托的形式传入</param>
    /// <param name="timeOut">任务执行的超时时间（毫秒）,默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">用于取消任务执行的令牌</param>
    /// <returns>返回包含执行结果的Task<T/>对象</returns>
    public Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        var (needEnqueue, chainId) = IsNeedEnqueue();
        if (needEnqueue)
        {
            if (GlobalSettings.CurrentSetting.IsDebug && !ActorLimit.AllowCall(Id))
            {
                return default;
            }

            if (timeOut <= 0)
            {
                timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
            }

            var wrapper = new FuncAsyncWrapper<T>(work)
            {
                Owner = this,
                TimeOut = timeOut,
                CallChainId = chainId,
            };
            ActionBlock.SendAsync(wrapper, cancellationToken);
            return wrapper.Tcs.Task;
        }

        return work();
    }

    #endregion
}