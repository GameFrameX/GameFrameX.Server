using System.IO.Compression;

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

        using (var uncompressed = new MemoryStream(bytes))
        {
            using (var compressed = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(compressed, CompressionMode.Compress, true))
                {
                    uncompressed.CopyTo(gZipStream);
                }

                return compressed.ToArray();
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

        using (var compressed = new MemoryStream(bytes))
        {
            using (var decompressed = new MemoryStream())
            {
                using (var gZipStream = new GZipStream(compressed, CompressionMode.Decompress))
                {
                    gZipStream.CopyTo(decompressed);
                }

                return decompressed.ToArray();
            }
        }
    }
}