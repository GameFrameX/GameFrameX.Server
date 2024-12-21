using GameFrameX.NetWork.Abstractions;
using GameFrameX.Setting;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

/// <summary>
/// 默认网络通道
/// </summary>
public class DefaultNetWorkChannel : BaseNetWorkChannel
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="session"></param>
    /// <param name="setting"></param>
    /// <param name="messageEncoder"></param>
    /// <param name="rpcSession"></param>
    /// <param name="isWebSocket"></param>
    public DefaultNetWorkChannel(IGameAppSession session, AppSetting setting, IMessageEncoderHandler messageEncoder, IRpcSession rpcSession = null, bool isWebSocket = false) : base(session, setting, messageEncoder, rpcSession, isWebSocket)
    {
    }
}