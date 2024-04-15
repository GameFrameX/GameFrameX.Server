namespace GameFrameX.NetWork;

public interface IGameSession
{
    /// <summary>
    /// 会话ID
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// 远程地址
    /// </summary>
    string RemoteEndPoint { get; }

    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="bytes"></param>
    void Send(byte[] bytes);

    /// <summary>
    /// 异步发送数据
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    Task SendAsync(byte[] bytes);
}