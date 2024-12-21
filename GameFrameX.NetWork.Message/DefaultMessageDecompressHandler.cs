using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 默认消息解压器
/// </summary>
public sealed class DefaultMessageDecompressHandler : IMessageDecompressHandler
{
    /// <summary>
    /// 解压处理
    /// </summary>
    /// <param name="message">消息压缩内容</param>
    /// <returns></returns>
    public byte[] Handler(byte[] message)
    {
        return Compression.Decompress(message);
    }
}