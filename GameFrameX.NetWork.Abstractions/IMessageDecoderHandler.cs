namespace GameFrameX.NetWork.Abstractions;

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
    INetworkMessage Handler(byte[] data);

    /// <summary>
    /// 设置解压消息处理器
    /// </summary>
    /// <param name="decompressHandler">解压消息处理器</param>
    void SetDecompressionHandler(IMessageDecompressHandler decompressHandler = null);
}