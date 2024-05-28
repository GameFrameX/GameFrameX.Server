using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface IMessageDecoderHandler
{
    /// <summary>
    /// 处理消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    IMessage? Handler(byte[] data);

    /// <summary>
    /// 处理服务器之间的消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    // IMessage? RpcHandler(byte[] data);
}