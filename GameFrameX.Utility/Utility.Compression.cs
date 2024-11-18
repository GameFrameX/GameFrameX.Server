using System.IO.Compression;

namespace GameFrameX.Utility
{
    /// <summary>
    /// 压缩解压缩辅助器。
    /// </summary>
    public static class Compression
    {
        /// <summary>
        /// 压缩数据。
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
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
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] bytes)
        {
            var compressed = new MemoryStream(bytes);
            var decompressed = new MemoryStream();
            var deflateStream = new DeflateStream(compressed, CompressionMode.Decompress); // 注意： 这里第一个参数同样是填写压缩的数据，但是这次是作为输入的数据
            deflateStream.CopyTo(decompressed);
            byte[] result = decompressed.ToArray();
            return result;
        }
    }
}