namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// RPC会话接口
/// </summary>
public interface IRpcSession
{
    /// <summary>
    /// 异步调用,且等待返回
    /// </summary>
    /// <param name="message">调用消息对象</param>
    /// <param name="timeOutMillisecond">调用超时,单位毫秒,默认10秒</param>
    /// <returns>返回消息对象</returns>
    Task<IResponseMessage> Call(IRequestMessage message, int timeOutMillisecond = 10000);

    /// <summary>
    /// 异步发送,不等待结果
    /// </summary>
    /// <param name="message">调用消息对象</param>
    void Send(IRequestMessage message);
}