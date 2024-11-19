using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Core.BaseHandler;

/// <summary>
/// 基础消息处理器
/// </summary>
public abstract class BaseMessageHandler : IMessageHandler
{
    /// <summary>
    /// 网络频道
    /// </summary>
    public INetWorkChannel NetWorkChannel { get; private set; }

    /// <summary>
    /// 消息对象
    /// </summary>
    public INetworkMessage Message { get; private set; }

    private bool _isInit;

    /// <summary>
    /// 初始化
    /// 子类实现必须调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <param name="netWorkChannel">网络渠道</param>
    /// <returns></returns>
    public virtual Task Init(INetworkMessage message, INetWorkChannel netWorkChannel)
    {
        Message = message;
        NetWorkChannel = netWorkChannel;
        _isInit = true;
        return Task.CompletedTask;
    }

    /// <summary>
    /// 动作异步
    /// </summary>
    /// <returns></returns>
    protected abstract Task ActionAsync();

    /// <summary>
    /// 执行
    /// </summary>
    /// <returns></returns>
    public virtual Task InnerAction()
    {
        if (_isInit == false)
        {
            throw new Exception("消息处理器未初始化,请调用先Init方法，如果已经子类实现了Init方法，请调用在子类Init中调用父类Init方法");
        }

        return ActionAsync();
    }
}