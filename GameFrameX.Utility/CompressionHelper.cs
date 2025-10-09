// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Buffers;
using GameFrameX.Foundation.Logger;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace GameFrameX.Utility;

/// <summary>
/// Compression and decompression helper.
/// </summary>
/// <remarks>
/// 压缩解压缩辅助器。
/// </remarks>
public static class CompressionHelper
{
    /// <summary>
    /// Buffer size in bytes for compression and decompression operations
    /// </summary>
    /// <remarks>
    /// 用于压缩和解压缩操作的缓冲区大小（以字节为单位）
    /// </remarks>
    private const int BufferSize = 8192;

    /// <summary>
    /// Compresses data. Uses the Deflate algorithm to compress the original byte array into a smaller byte array.
    /// </summary>
    /// <remarks>
    /// 压缩数据。使用Deflate算法将原始字节数组压缩成更小的字节数组。
    /// </remarks>
    /// <param name="bytes">The original byte array to be compressed. Cannot be null. / 要压缩的原始字节数组。不能为null。</param>
    /// <returns>The compressed byte array. If the input is an empty array, returns the empty array directly. If an exception occurs during compression, returns the original array. / 压缩后的字节数组。如果输入为空数组，则直接返回该空数组。如果压缩过程中发生异常，则返回原始数组。</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input parameter bytes is null. / 当输入参数bytes为null时抛出。</exception>
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
    /// Decompresses data. Uses the Inflate algorithm to restore the compressed byte array to the original byte array.
    /// </summary>
    /// <remarks>
    /// 解压数据。使用Inflate算法将压缩的字节数组还原成原始字节数组。
    /// </remarks>
    /// <param name="bytes">The compressed byte array to be decompressed. Cannot be null. / 要解压的压缩字节数组。不能为null。</param>
    /// <returns>The decompressed original byte array. If the input is an empty array, returns the empty array directly. If an exception occurs during decompression, returns the original array. / 解压后的原始字节数组。如果输入为空数组，则直接返回该空数组。如果解压过程中发生异常，则返回原始数组。</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input parameter bytes is null. / 当输入参数bytes为null时抛出。</exception>
    /// <exception cref="InvalidDataException">Thrown when the compressed data format is invalid or corrupted. / 当压缩数据格式无效或已损坏时抛出。</exception>
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