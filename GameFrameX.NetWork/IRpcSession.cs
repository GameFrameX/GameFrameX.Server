using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

/// <summary>
/// RPC会话接口
/// </summary>
public interface IRpcSession
{
    /// <summary>
    /// 异步调用
    /// </summary>
    /// <param name="message"></param>
    Task<IResponseMessage> Call(IRequestMessage message);

    /// <summary>
    /// 发送
    /// </summary>
    /// <param name="message"></param>
    void Send(IRequestMessage message);
}