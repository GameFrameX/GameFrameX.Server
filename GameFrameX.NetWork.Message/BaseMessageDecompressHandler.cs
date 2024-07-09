using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 默认消息解压器
/// </summary>
public class BaseMessageDecompressHandler : IMessageDecompressHandler
{
    /// <summary>
    /// 解压处理
    /// </summary>
    /// <param name="message">消息压缩内容</param>
    /// <returns></returns>
    public virtual byte[] Handler(byte[] message)
    {
        return Utility.Compression.Decompress(message);
    }
}