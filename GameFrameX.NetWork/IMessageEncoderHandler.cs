using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface IMessageEncoderHandler
{
    byte[] Handler(IMessage message);

    /// <summary>
    /// 处理服务器之间的消息
    /// </summary>
    /// <param name="messageUniqueId"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    // byte[] RpcHandler(int messageUniqueId, IMessage message);
}