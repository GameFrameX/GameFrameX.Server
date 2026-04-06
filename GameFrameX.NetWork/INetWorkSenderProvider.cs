using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

/// <summary>
/// 网络发送器提供者。
/// </summary>
internal interface INetWorkSenderProvider
{
    /// <summary>
    /// 关联的会话类型。
    /// </summary>
    Type SessionType { get; }

    /// <summary>
    /// 根据会话创建发送器。
    /// </summary>
    /// <param name="session">会话</param>
    /// <returns>网络发送器</returns>
    INetWorkSender Create(IGameAppSession session);
}
