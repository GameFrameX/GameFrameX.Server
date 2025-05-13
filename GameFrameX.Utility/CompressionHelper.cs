using System.Buffers;
using ICSharpCode.SharpZipLib.GZip;

namespace GameFrameX.Utility;

/// <summary>
/// 压缩解压缩辅助器。
/// </summary>
public static class CompressionHelper
{
    /// <summary>
    /// 压缩数据。使用GZip算法将原始字节数组压缩成更小的字节数组。
    /// </summary>
    /// <param name="bytes">要压缩的原始字节数组。不能为null。</param>
    /// <returns>压缩后的字节数组。如果输入为空数组，则直接返回该空数组。</returns>
    /// <exception cref="ArgumentNullException">当输入参数bytes为null时抛出。</exception>
    public static byte[] Compress(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
        if (bytes.Length == 0)
        {
            return bytes;
        }

        using (var compressStream = new MemoryStream())
        {
            using (var gZipOutputStream = new GZipOutputStream(compressStream))
            {
                gZipOutputStream.Write(bytes, 0, bytes.Length);
                var press = compressStream.ToArray();
                return press;
            }
        }
    }

    /// <summary>
    /// 解压数据。使用GZip算法将压缩的字节数组还原成原始字节数组。
    /// </summary>
    /// <param name="bytes">要解压的压缩字节数组。不能为null。</param>
    /// <returns>解压后的原始字节数组。如果输入为空数组，则直接返回该空数组。</returns>
    /// <exception cref="ArgumentNullException">当输入参数bytes为null时抛出。</exception>
    /// <exception cref="InvalidDataException">当压缩数据格式无效或已损坏时抛出。</exception>
    public static byte[] Decompress(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
        if (bytes.Length == 0)
        {
            return bytes;
        }

        using (var compressedStream = new MemoryStream(bytes))
        {
            using (var gZipInputStream = new GZipInputStream(compressedStream))
            {
                using (var decompressedStream = new MemoryStream())
                {
                    var buffer = ArrayPool<byte>.Shared.Rent(8192);
                    try
                    {
                        int count;
                        while ((count = gZipInputStream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            decompressedStream.Write(buffer, 0, count);
                        }
                    }
                    finally
                    {
                        ArrayPool<byte>.Shared.Return(buffer);
                    }

                    var array = decompressedStream.ToArray();
                    return array;
                }
            }
        }
    }
}