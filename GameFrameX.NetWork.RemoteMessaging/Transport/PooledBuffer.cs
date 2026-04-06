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
public sealed class PooledBuffer : IDisposable
{
    private byte[] _buffer;
    private bool _disposed;

    /// <summary>
    /// 创建池化缓冲区句柄。
    /// </summary>
    /// <param name="buffer">池化字节数组</param>
    /// <param name="length">有效数据长度</param>
    public PooledBuffer(byte[] buffer, int length)
    {
        _buffer = buffer;
        Length = length;
    }

    /// <summary>
    /// 有效数据长度。
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// 有效数据视图。
    /// </summary>
    public ReadOnlyMemory<byte> Memory => _disposed ? ReadOnlyMemory<byte>.Empty : _buffer.AsMemory(0, Length);

    /// <summary>
    /// 归还池化缓冲区。
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        ArrayPool<byte>.Shared.Return(_buffer);
        _buffer = Array.Empty<byte>();
        _disposed = true;
    }
}
