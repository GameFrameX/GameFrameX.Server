using System.IO.Compression;

namespace GameFrameX.Utility;

/// <summary>
/// 压缩解压缩辅助器。
/// </summary>
public static class CompressionHelper
{
    /// <summary>
    /// 压缩数据。
    /// </summary>
    /// <param name="bytes">要压缩的原始字节数组。</param>
    /// <returns>压缩后的字节数组。</returns>
    public static byte[] Compress(byte[] bytes)
    {
        var uncompressed = new MemoryStream(bytes);
        var compressed = new MemoryStream();
        var deflateStream = new DeflateStream(compressed, CompressionMode.Compress);
        uncompressed.CopyTo(deflateStream);
        deflateStream.Close();
        return compressed.ToArray();
    }

    /// <summary>
    /// 解压数据。
    /// </summary>
    /// <param name="bytes">要解压的压缩字节数组。</param>
    /// <returns>解压后的字节数组。</returns>
    public static byte[] Decompress(byte[] bytes)
    {
        var compressed = new MemoryStream(bytes);
        var decompressed = new MemoryStream();
        var deflateStream = new DeflateStream(compressed, CompressionMode.Decompress); // 注意：这里第一个参数同样是填写压缩的数据，但是这次是作为输入的数据
        deflateStream.CopyTo(decompressed);
        var result = decompressed.ToArray();
        return result;
    }
}