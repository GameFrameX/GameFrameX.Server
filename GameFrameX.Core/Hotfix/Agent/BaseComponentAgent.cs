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

using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Core.Components;
using GameFrameX.Core.Timer;
using GameFrameX.Core.Timer.Handler;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.Hotfix.Agent;

/// <summary>
/// 基础组件代理类，用于管理组件与Actor之间的交互
/// </summary>
/// <typeparam name="TComponent">具体的组件类型</typeparam>
public abstract partial class BaseComponentAgent<TComponent> : IComponentAgent where TComponent : BaseComponent
{
    /// <summary>
    /// 所有者的组件实例
    /// </summary>
    public TComponent OwnerComponent
    {
        get { return (TComponent)Owner; }
    }

    /// <summary>
    /// 所有者的Actor实例
    /// </summary>
    public IActor Actor
    {
        get { return Owner.Actor; }
    }

    /// <summary>
    /// 订阅的定时任务ID集合
    /// </summary>
    public HashSet<long> ScheduleIdSet
    {
        get { return Actor.ScheduleIdSet; }
    }

    /// <summary>
    /// 组件的所有者
    /// </summary>
    public IComponent Owner { get; private set; }

    /// <summary>
    /// 所有者的Actor ID
    /// </summary>
    public long ActorId
    {
        get { return Actor.Id; }
    }

    /// <summary>
    /// 所有者的类型
    /// </summary>
    public ushort OwnerType
    {
        get { return Actor.Type; }
    }

    /// <summary>
    /// 设置组件的所有者
    /// </summary>
    /// <param name="owner">所有者实例</param>
    public void SetOwner(IComponent owner)
    {
        Owner = owner;
    }

    /// <summary>
    /// 标记组件是否已执行过激活前预处理
    /// </summary>
    protected bool IsInvokedBeforeActivation { get; private set; }

    /// <summary>
    /// 组件激活前的预处理操作
    /// </summary>
    /// <returns>
    /// 返回一个 Task&lt;bool&gt; 对象：
    /// - true：表示预处理成功且是首次执行
    /// - false：表示已经执行过预处理
    /// </returns>
    public virtual Task<bool> BeforeActivation()
    {
        if (IsInvokedBeforeActivation)
        {
            return Task.FromResult(false);
        }

        IsInvokedBeforeActivation = true;

        return Task.FromResult(true);
    }

    /// <summary>
    /// 标记组件是否已执行过激活操作
    /// </summary>
    protected bool IsInvokedActivation { get; private set; }

    /// <summary>
    /// 激活组件
    /// </summary>
    /// <returns>
    /// 返回一个 Task&lt;bool&gt; 对象：
    /// - true：表示激活成功且是首次执行
    /// - false：表示组件已经被激活过
    /// </returns>
    public virtual Task<bool> Active()
    {
        if (IsInvokedActivation)
        {
            return Task.FromResult(false);
        }

        IsInvokedActivation = true;

        return Task.FromResult(true);
    }

    /// <summary>
    /// 标记组件是否已执行过激活后处理
    /// </summary>
    protected bool IsInvokedAfterActivation { get; private set; }

    /// <summary>
    /// 组件激活后的后处理操作
    /// </summary>
    /// <returns>
    /// 返回一个 Task&lt;bool&gt; 对象：
    /// - true：表示后处理成功且是首次执行
    /// - false：表示已经执行过后处理
    /// </returns>
    public virtual Task<bool> AfterActivation()
    {
        if (IsInvokedAfterActivation)
        {
            return Task.FromResult(false);
        }

        IsInvokedAfterActivation = true;

        return Task.FromResult(true);
    }

    /// <summary>
    /// 标记组件是否已执行过反激活前预处理
    /// </summary>
    protected bool IsInvokedBeforeInActivation { get; private set; }

    /// <summary>
    /// 组件反激活前的预处理操作
    /// </summary>
    /// <returns>
    /// 返回一个 Task&lt;bool&gt; 对象：
    /// - true：表示预处理成功且是首次执行
    /// - false：表示已经执行过预处理
    /// </returns>
    public virtual Task<bool> BeforeInActivation()
    {
        if (IsInvokedBeforeInActivation)
        {
            return Task.FromResult(false);
        }

        IsInvokedBeforeInActivation = true;

        return Task.FromResult(true);
    }

    /// <summary>
    /// 反激活组件
    /// </summary>
    /// <returns>
    /// 返回一个已完成的 Task 对象。
    /// 注意：此方法不检查执行状态，可以多次调用
    /// </returns>
    public virtual Task Inactive()
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// 标记组件是否已执行过反激活后处理
    /// </summary>
    protected bool IsInvokedAfterInActivation { get; private set; }

    /// <summary>
    /// 组件反激活后的后处理操作
    /// </summary>
    /// <returns>
    /// 返回一个 Task&lt;bool&gt; 对象：
    /// - true：表示后处理成功且是首次执行
    /// - false：表示已经执行过后处理
    /// </returns>
    public virtual Task<bool> AfterInActivation()
    {
        if (IsInvokedAfterInActivation)
        {
            return Task.FromResult(false);
        }

        IsInvokedAfterInActivation = true;

        return Task.FromResult(true);
    }

    /// <summary>
    /// 根据代理类型获取组件代理实例
    /// </summary>
    /// <param name="agentType">要获取的代理类型</param>
    /// <param name="isNew">是否创建新的实例，true表示创建新实例，false表示获取已存在的实例</param>
    /// <returns>返回一个异步任务，该任务完成时将返回指定类型的组件代理实例</returns>
    public Task<IComponentAgent> GetComponentAgent(Type agentType, bool isNew = true)
    {
        return Actor.GetComponentAgent(agentType, isNew);
    }

    /// <summary>
    /// 获取指定泛型类型的组件代理实例
    /// </summary>
    /// <typeparam name="T">要获取的组件代理类型，必须实现IComponentAgent接口</typeparam>
    /// <param name="isNew">是否创建新的实例，true表示创建新实例，false表示获取已存在的实例</param>
    /// <returns>返回一个异步任务，该任务完成时将返回指定泛型类型的组件代理实例</returns>
    public Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent
    {
        return Actor.GetComponentAgent<T>(isNew);
    }

    /// <summary>
    /// 发送无返回值的工作指令到Actor
    /// </summary>
    /// <param name="work">要执行的工作内容，以Action委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    public void Tell(Action work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        Actor.Tell(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 发送异步工作指令到Actor
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以 Func&lt;Task&gt; 委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    public void Tell(Func<Task> work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        Actor.Tell(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送无返回值的工作指令到Actor
    /// </summary>
    /// <param name="work">要执行的工作内容，以Action委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>表示异步操作的Task对象</returns>
    public Task SendAsync(Action work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return Actor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令到Actor
    /// </summary>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <param name="work">要执行的工作内容，以Func&lt;T&gt;委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>包含执行结果的 Task&lt;T&gt; 对象</returns>
    public Task<T> SendAsync<T>(Func<T> work, int timeout = -1, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return Actor.SendAsync(work, timeout, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令到Actor，支持锁检查
    /// </summary>
    /// <param name="work">要执行的异步工作内容，以Func&lt;Task&gt;委托形式传入</param>
    /// <param name="timeout">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="checkLock">是否检查Actor的锁状态，默认为true</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>表示异步操作的Task对象</returns>
    public Task SendAsync(Func<Task> work, int timeout = -1, bool checkLock = true, CancellationToken cancellationToken = default)
    {
        if (timeout <= 0)
        {
            timeout = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return Actor.SendAsync(work, timeout, checkLock, cancellationToken);
    }

    /// <summary>
    /// 异步发送有返回值的工作指令到Actor
    /// </summary>
    /// <typeparam name="T">返回结果的类型</typeparam>
    /// <param name="work">要执行的异步工作内容，以Func&lt;Task&lt;T&gt;&gt;委托形式传入</param>
    /// <param name="timeOut">执行超时时间，如果超过这个时间还未执行完成则会抛出异常，默认为-1,将采用配置时间ActorTimeOut</param>
    /// <param name="cancellationToken">取消令牌，用于取消正在执行的操作</param>
    /// <returns>包含执行结果的 Task&lt;T&gt; 对象</returns>
    public Task<T> SendAsync<T>(Func<Task<T>> work, int timeOut = -1, CancellationToken cancellationToken = default)
    {
        if (timeOut <= 0)
        {
            timeOut = GlobalSettings.CurrentSetting.ActorTimeOut;
        }

        return Actor.SendAsync(work, timeOut, cancellationToken);
    }

    /// <summary>
    /// 设置Actor组件是否自动回收
    /// </summary>
    /// <param name="autoRecycle">true表示启用自动回收，false表示禁用自动回收</param>
    protected void SetAutoRecycle(bool autoRecycle)
    {
        Actor.SetAutoRecycle(autoRecycle);
    }

    /// <summary>
    /// 处理Actor的跨天事件，用于执行每日重置等操作
    /// </summary>
    /// <param name="serverDay">服务器运行的天数，从启动开始计数</param>
    /// <returns>表示异步操作的Task对象</returns>
    public Task ActorCrossDay(int serverDay)
    {
        return Actor.CrossDay(serverDay);
    }
}