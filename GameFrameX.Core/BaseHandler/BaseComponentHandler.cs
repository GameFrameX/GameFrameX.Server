using GameFrameX.Core.Abstractions.Agent;
using GameFrameX.Core.Actors;
using GameFrameX.Core.Utility;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using Serilog;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 基础组件处理器基类
/// </summary>
public abstract class BaseComponentHandler : BaseMessageHandler
{
    /// <summary>
    /// 组件代理ID
    /// </summary>
    protected long ActorId { get; set; }

    /// <summary>
    /// 组件代理类型
    /// </summary>
    protected abstract Type ComponentAgentType { get; }

    /// <summary>
    /// 缓存组件代理对象
    /// </summary>
    public IComponentAgent CacheComponent { get; protected set; }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <returns>初始化任务</returns>
    protected abstract Task<bool> InitActor();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="message">网络消息</param>
    /// <param name="netWorkChannel">网络通道</param>
    /// <returns>返回是否初始化成功,true:成功,false:失败</returns>
    public override async Task<bool> Init(INetworkMessage message, INetWorkChannel netWorkChannel)
    {
        var initSuccess = await base.Init(message, netWorkChannel);
        if (!initSuccess)
        {
            return false;
        }

        initSuccess = await InitActor();
        if (!initSuccess)
        {
            return false;
        }

        if (CacheComponent == null)
        {
            if (ActorId == default)
            {
                LogHelper.Fatal($"ActorId is 0, can not get component，{message.GetType().FullName}, close channel");
                NetWorkChannel.Close();
                return false;
            }

            try
            {
                CacheComponent = await ActorManager.GetComponentAgent(ActorId, ComponentAgentType);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "get component failed, close channel");
                NetWorkChannel.Close();
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 内部执行
    /// </summary>
    /// <param name="timeout">执行超时时间，单位毫秒，默认30秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>内部执行任务</returns>
    public override Task InnerAction(int timeout = 30000, CancellationToken cancellationToken = default)
    {
        if (CacheComponent == null)
        {
            LogHelper.Fatal("CacheComponent is null, can not get component, close channel");
            NetWorkChannel.Close();
            return Task.CompletedTask;
        }

        CacheComponent.Tell(InnerActionAsync, timeout, cancellationToken);
        return Task.CompletedTask;
    }

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// </summary>
    /// <typeparam name="TOtherAgent">组件代理类型</typeparam>
    /// <returns>组件代理任务</returns>
    protected Task<TOtherAgent> GetComponentAgent<TOtherAgent>() where TOtherAgent : IComponentAgent
    {
        if (CacheComponent == null)
        {
            LogHelper.Fatal("CacheComponent is null, can not get component, close channel");
            NetWorkChannel.Close();
            return default;
        }

        return CacheComponent.GetComponentAgent<TOtherAgent>();
    }
}