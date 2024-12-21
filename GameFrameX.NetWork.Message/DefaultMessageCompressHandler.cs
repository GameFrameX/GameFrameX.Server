using GameFrameX.NetWork.Abstractions;
using GameFrameX.Utility;

namespace GameFrameX.NetWork.Message;

/// <summary>
/// 默认消息压缩器
/// </summary>
public sealed class DefaultMessageCompressHandler : IMessageCompressHandler
{
    /// <summary>
    /// 压缩处理
    /// </summary>
    /// <param name="message">消息未压缩内容</param>
    /// <returns></returns>
    public byte[] Handler(byte[] message)
    {
        return Compression.Compress(message);
    }
}