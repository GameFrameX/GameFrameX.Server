namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息解压器接口定义，用于解压压缩后的消息
/// </summary>
public interface IMessageDecompressHandler
{
    /// <summary>
    /// 解压处理
    /// </summary>
    /// <param name="message">消息压缩内容</param>
    /// <returns></returns>
    byte[] Handler(byte[] message);
}