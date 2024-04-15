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
}