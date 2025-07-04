using System.Diagnostics;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Foundation.Logger;
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
    public virtual Task<bool> Init(INetworkMessage message, INetWorkChannel netWorkChannel)
    {
        _stopwatch = new Stopwatch();
        Message = message;
        NetWorkChannel = netWorkChannel;
        _isInit = true;
        return Task.FromResult(true);
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="timeout">执行超时时间，单位毫秒，默认30秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>执行任务</returns>
    public virtual async Task InnerAction(int timeout = 30000, CancellationToken cancellationToken = default)
    {
        if (_isInit == false)
        {
            throw new Exception("消息处理器未初始化,请调用先Init方法，如果已经子类实现了Init方法，请调用在子类Init中调用父类Init方法");
        }

        try
        {
            var task = InnerActionAsync();
            try
            {
                await task.WaitAsync(TimeSpan.FromMilliseconds(timeout), cancellationToken);
            }
            catch (TimeoutException timeoutException)
            {
                LogHelper.Fatal("执行超时:" + timeoutException.Message);
                //强制设状态-取消该操作
            }
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
        }
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
    protected async Task InnerActionAsync()
    {
        if (GlobalSettings.CurrentSetting.IsMonitorTimeOut)
        {
            _stopwatch.Restart();
            await ActionAsync();
            _stopwatch.Stop();
            if (_stopwatch.Elapsed.Seconds >= GlobalSettings.CurrentSetting.MonitorTimeOutSeconds)
            {
                LogHelper.Warn($"消息处理器：{GetType().Name},UniqueId：{Message.UniqueId} 执行耗时：{_stopwatch.ElapsedMilliseconds} ms");
            }

            return;
        }

        if (GlobalSettings.CurrentSetting.IsDebug)
        {
            _stopwatch.Restart();
            await ActionAsync();
            _stopwatch.Stop();
            LogHelper.Debug($"消息处理器：{GetType().Name},UniqueId：{Message.UniqueId} 执行耗时：{_stopwatch.ElapsedMilliseconds} ms");
            return;
        }


        await ActionAsync();
    }
}