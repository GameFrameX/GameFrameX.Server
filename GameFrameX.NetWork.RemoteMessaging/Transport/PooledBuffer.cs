// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Buffers;

namespace GameFrameX.NetWork.RemoteMessaging.Transport;

/// <summary>
/// 池化字节缓冲区句柄。调用方在使用完成后必须释放。
/// </summary>
/// <remarks>
/// Pooled byte buffer handle. The caller must dispose after use.
/// </remarks>
public sealed class PooledBuffer : IDisposable
{
    private byte[]? _buffer;

    /// <summary>
    /// 初始化新的池化字节缓冲区句柄。
    /// </summary>
    /// <remarks>
    /// Initializes a new pooled byte buffer handle.
    /// </remarks>
    /// <param name="buffer">从共享池租用的字节数组 / The byte array rented from the shared pool</param>
    /// <param name="length">有效数据长度（字节） / The valid data length in bytes</param>
    public PooledBuffer(byte[] buffer, int length)
    {
        _buffer = buffer;
        Length = length;
    }

    /// <summary>
    /// 有效数据长度。
    /// </summary>
    /// <remarks>
    /// Returns the valid data length of the buffer.
    /// </remarks>
    public int Length { get; }

    /// <summary>
    /// 有效数据视图。
    /// </summary>
    /// <remarks>
    /// Returns the valid data view of the buffer.
    /// </remarks>
    public ReadOnlyMemory<byte> Memory => _buffer?.AsMemory(0, Length) ?? ReadOnlyMemory<byte>.Empty;

    /// <summary>
    /// 释放缓冲区。
    /// </summary>
    /// <remarks>
    /// Returns the buffer to the pool.
    /// </remarks>
    public void Dispose()
    {
        if (_buffer == null)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = null;
    }
}
