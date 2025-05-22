using GameFrameX.Core.Abstractions;
using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Hotfix;

namespace GameFrameX.Core.Components;

/// <summary>
/// 基础组件基类
/// </summary>
public abstract class BaseComponent : IComponent, IState
{
    private readonly object _cacheAgentLock = new();
    private IComponentAgent _cacheAgent;

    /// <summary>
    /// ActorId
    /// </summary>
    internal long ActorId
    {
        get { return Actor.Id; }
    }

    /// <summary>
    /// 是否是激活状态
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// 是否准备完毕
    /// </summary>
    /// <returns>是否准备完毕</returns>
    internal virtual bool ReadyToInactive
    {
        get { return true; }
    }

    /// <summary>
    /// Actor 对象
    /// </summary>
    public IActor Actor { get; set; }

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent 数据
    /// </summary>
    /// <param name="refAssemblyType">引用程序集，如果为 null 则使用当前程序集引用</param>
    /// <returns>IComponentAgent 实例</returns>
    public IComponentAgent GetAgent(Type refAssemblyType = null)
    {
        lock (_cacheAgentLock)
        {
            if (_cacheAgent != null && !HotfixManager.DoingHotfix)
            {
                return _cacheAgent;
            }

            var agent = HotfixManager.GetAgent<IComponentAgent>(this, refAssemblyType);
            _cacheAgent = agent;
            return agent;
        }
    }

    /// <summary>
    /// 清理缓存代理
    /// </summary>
    public void ClearCacheAgent()
    {
        _cacheAgent = null;
    }

    /// <summary>
    /// 激活组件
    /// </summary>
    /// <returns>激活任务</returns>
    public virtual Task Active()
    {
        IsActive = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 反激活组件
    /// </summary>
    /// <returns>反激活任务</returns>
    public virtual async Task Inactive()
    {
        var agent = GetAgent();
        if (agent != null)
        {
            await agent.AfterInActivation();
            await agent.Inactive();
            await agent.BeforeInActivation();
        }

        IsActive = false;
    }

    /// <summary>
    /// 读取状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态读取完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步读取对象的当前状态信息
    /// </remarks>
    public abstract Task ReadStateAsync();

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态更新完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步更新对象的状态信息
    /// 在状态发生变化时应调用此方法以保持状态的同步
    /// </remarks>
    public abstract Task WriteStateAsync();
}