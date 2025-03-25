using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork;

/// <summary>
/// 消息处理器
/// </summary>
public interface IMessageHandler
{
    /// <summary>
    /// 消息对象
    /// </summary>
    INetworkMessage Message { get; }

    /// <summary>
    /// 网络频道对象
    /// </summary>
    INetWorkChannel NetWorkChannel { get; }

    /// <summary>
    /// 初始化
    /// 子类实现必须调用
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <param name="netWorkChannel">网络渠道</param>
    /// <returns></returns>
    Task Init(INetworkMessage message, INetWorkChannel netWorkChannel);

    /// <summary>
    /// 内部执行
    /// </summary>
    /// <param name="timeout">执行超时时间，单位毫秒，默认30秒</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns></returns>
    Task InnerAction(int timeout = 30000, CancellationToken cancellationToken = default);
}