using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 默认消息压缩器
/// </summary>
public class BaseMessageCompressHandler : IMessageCompressHandler
{
    /// <summary>
    /// 压缩处理
    /// </summary>
    /// <param name="message">消息未压缩内容</param>
    /// <returns></returns>
    public virtual byte[] Handler(byte[] message)
    {
        return Utility.Compression.Compress(message);
    }
}