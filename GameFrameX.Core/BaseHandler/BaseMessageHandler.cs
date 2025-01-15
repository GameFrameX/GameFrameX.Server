using System.Diagnostics;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility.Log;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 基础消息处理器
/// </summary>
public abstract class BaseMessageHandler : IMessageHandler
{
    private bool _isInit;

    /// <summary>
    /// 监控器
    /// </summary>
    private Stopwatch _stopwatch;

    /// <summary>
    /// 网络频道
    /// </summary>
    public INetWorkChannel NetWorkChannel { get; private set; }

    /// <summary>
    /// 消息对象
    /// </summary>
    public INetworkMessage Message { get; private set; }

    /// <summary>
    /// 初始化
    /// 子类实现必须调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <param name="netWorkChannel">网络渠道</param>
    /// <returns>初始化任务</returns>
    public virtual Task Init(INetworkMessage message, INetWorkChannel netWorkChannel)
    {
        _stopwatch = new Stopwatch();
        Message = message;
        NetWorkChannel = netWorkChannel;
        _isInit = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <returns>执行任务</returns>
    public virtual Task InnerAction()
    {
        if (_isInit == false)
        {
            throw new Exception("消息处理器未初始化,请调用先Init方法，如果已经子类实现了Init方法，请调用在子类Init中调用父类Init方法");
        }

        return InnerActionAsync();
    }

    /// <summary>
    /// 动作异步
    /// </summary>
    /// <returns>动作执行任务</returns>
    protected abstract Task ActionAsync();

    /// <summary>
    /// 内部动作异步
    /// 记录执行时间并调用 <see cref="ActionAsync" />
    /// </summary>
    /// <returns>动作执行任务</returns>
    protected Task InnerActionAsync()
    {
        if (GlobalSettings.IsDebug)
        {
            _stopwatch.Restart();
            var result = ActionAsync();
            _stopwatch.Stop();
            LogHelper.Debug($"消息处理器：{GetType().Name},UniqueId：{Message.UniqueId} 执行耗时：{_stopwatch.ElapsedMilliseconds} ms");
            return result;
        }

        return ActionAsync();
    }
}