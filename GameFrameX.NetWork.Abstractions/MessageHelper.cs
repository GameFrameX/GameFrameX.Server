// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 消息处理帮助类，用于管理消息的编码和解码处理器
/// </summary>
public sealed class MessageHelper
{
    /// <summary>
    /// 消息编码处理器 - 用于将消息编码成二进制格式
    /// </summary>
    public static IMessageEncoderHandler EncoderHandler { get; private set; }

    /// <summary>
    /// 消息解码处理器 - 用于将二进制数据解码成消息对象
    /// </summary>
    public static IMessageDecoderHandler DecoderHandler { get; private set; }

    /// <summary>
    /// 设置消息解码处理器和解压缩处理器
    /// </summary>
    /// <param name="decoderHandler">消息解码处理器实例</param>
    /// <param name="decompressHandler">消息解压缩处理器实例</param>
    /// <exception cref="ArgumentNullException">当decoderHandler为null时抛出</exception>
    public static void SetMessageDecoderHandler(IMessageDecoderHandler decoderHandler, IMessageDecompressHandler decompressHandler)
    {
        ArgumentNullException.ThrowIfNull(decoderHandler, nameof(decoderHandler));
        DecoderHandler = decoderHandler;
        DecoderHandler.SetDecompressionHandler(decompressHandler);
    }

    /// <summary>
    /// 设置消息编码处理器和压缩处理器
    /// </summary>
    /// <param name="encoderHandler">消息编码处理器实例</param>
    /// <param name="compressHandler">消息压缩处理器实例</param>
    /// <exception cref="ArgumentNullException">当encoderHandler为null时抛出</exception>
    public static void SetMessageEncoderHandler(IMessageEncoderHandler encoderHandler, IMessageCompressHandler compressHandler)
    {
        ArgumentNullException.ThrowIfNull(encoderHandler, nameof(encoderHandler));
        EncoderHandler = encoderHandler;
        EncoderHandler.SetCompressionHandler(compressHandler);
    }
}