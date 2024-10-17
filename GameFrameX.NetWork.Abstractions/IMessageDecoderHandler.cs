using System.Buffers;

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息解码器定义接口
/// </summary>
public interface IMessageDecoderHandler
{
    /// <summary>
    /// 消息头长度
    /// </summary>
    int MessageHeaderLength { get; }

    /// <summary>
    /// 解析消息
    /// </summary>
    /// <param name="data">消息数据</param>
    /// <returns>消息结果对象</returns>
    IMessage Handler(byte[] data);

    /// <summary>
    /// 解析消息
    /// </summary>
    /// <param name="sequence">消息数据</param>
    /// <returns>消息结果对象</returns>
    IMessage Handler(ref ReadOnlySequence<byte> sequence);

    /// <summary>
    /// 设置解压消息处理器
    /// </summary>
    /// <param name="decompressHandler">解压消息处理器</param>
    void SetDecompressionHandler(IMessageDecompressHandler decompressHandler = null);
}