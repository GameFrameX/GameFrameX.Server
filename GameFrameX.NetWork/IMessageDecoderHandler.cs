using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

/// <summary>
/// 消息解码器定义接口
/// </summary>
public interface IMessageDecoderHandler
{
    /// <summary>
    /// 解析消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    IMessage Handler(byte[] data);

    /// <summary>
    /// 处理服务器之间的消息
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    // IMessage? RpcHandler(byte[] data);
}