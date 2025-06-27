using System.Buffers;
using GameFrameX.Foundation.Logger;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace GameFrameX.Utility;

/// <summary>
/// 压缩解压缩辅助器。
/// </summary>
public static class CompressionHelper
{
    /// <summary>
    /// 用于压缩和解压缩操作的缓冲区大小（以字节为单位）
    /// </summary>
    private const int BufferSize = 8192;

    /// <summary>
    /// 压缩数据。使用Deflate算法将原始字节数组压缩成更小的字节数组。
    /// </summary>
    /// <param name="bytes">要压缩的原始字节数组。不能为null。</param>
    /// <returns>压缩后的字节数组。如果输入为空数组，则直接返回该空数组。如果压缩过程中发生异常，则返回原始数组。</returns>
    /// <exception cref="ArgumentNullException">当输入参数bytes为null时抛出。</exception>
    public static byte[] Compress(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
        if (bytes.Length == 0)
        {
            return bytes;
        }

        var compressor = new Deflater();
        compressor.SetLevel(Deflater.BEST_COMPRESSION);

        compressor.SetInput(bytes);
        compressor.Finish();

        using var compressorMemoryStream = new MemoryStream(bytes.Length);
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            while (!compressor.IsFinished)
            {
                var count = compressor.Deflate(buffer);
                compressorMemoryStream.Write(buffer, 0, count);
            }

            return compressorMemoryStream.ToArray();
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

        return bytes;
    }

    /// <summary>
    /// 解压数据。使用Inflate算法将压缩的字节数组还原成原始字节数组。
    /// </summary>
    /// <param name="bytes">要解压的压缩字节数组。不能为null。</param>
    /// <returns>解压后的原始字节数组。如果输入为空数组，则直接返回该空数组。如果解压过程中发生异常，则返回原始数组。</returns>
    /// <exception cref="ArgumentNullException">当输入参数bytes为null时抛出。</exception>
    /// <exception cref="InvalidDataException">当压缩数据格式无效或已损坏时抛出。</exception>
    public static byte[] Decompress(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes, nameof(bytes));
        if (bytes.Length == 0)
        {
            return bytes;
        }

        var decompressor = new Inflater();
        decompressor.SetInput(bytes, 0, bytes.Length);
        using var decompressMemoryStream = new MemoryStream(bytes.Length);
        var buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
        try
        {
            while (!decompressor.IsFinished)
            {
                var count = decompressor.Inflate(buffer);
                decompressMemoryStream.Write(buffer, 0, count);
            }

            return decompressMemoryStream.ToArray();
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, true);
        }

        return bytes;
    }
}