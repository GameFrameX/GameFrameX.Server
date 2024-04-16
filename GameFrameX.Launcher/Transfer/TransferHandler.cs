using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;
using TouchSocket.Sockets;

namespace GameFrameX.Launcher.Transfer;

public interface ITransferHandler
{
    /// <summary>
    /// 转发消息
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="messageEncoderHandler"></param>
    /// <param name="message"></param>
    void Transfer(ITcpClientBase sender, IMessageEncoderHandler messageEncoderHandler, IMessage message);
}