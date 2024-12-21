using System.Security.Cryptography;
using System.Text;

namespace GameFrameX.Utility;

/// <summary>
/// 哈希计算相关的实用函数。
/// </summary>
public static partial class Hash
{
    /// <summary>
    /// MD5 哈希计算工具类
    /// </summary>
    public static class Md5
    {
        private static readonly MD5 Md5Cryptography = MD5.Create();

        /// <summary>
        /// 获取字符串的 MD5 哈希值
        /// </summary>
        /// <param name="input">要计算哈希值的字符串</param>
        /// <returns>字符串的 MD5 哈希值</returns>
        public static string Hash(string input)
        {
            var data = Md5Cryptography.ComputeHash(Encoding.UTF8.GetBytes(input));
            return ToHash(data);
        }

        /// <summary>
        /// 获取流的 MD5 哈希值
        /// </summary>
        /// <param name="input">要计算哈希值的流</param>
        /// <returns>流的 MD5 哈希值</returns>
        public static string Hash(Stream input)
        {
            var data = Md5Cryptography.ComputeHash(input);
            return ToHash(data);
        }

        /// <summary>
        /// 验证输入字符串的 MD5 哈希值是否与给定的哈希值一致
        /// </summary>
        /// <param name="input">要验证的字符串</param>
        /// <param name="hash">要比较的 MD5 哈希值</param>
        /// <returns>如果哈希值一致，返回 true；否则返回 false</returns>
        public static bool IsVerify(string input, string hash)
        {
            var comparer = StringComparer.OrdinalIgnoreCase;
            return 0 == comparer.Compare(Hash(input), hash);
        }

        /// <summary>
        /// 将字节数组转换为十六进制字符串表示的哈希值
        /// </summary>
        /// <param name="data">要转换的字节数组</param>
        /// <returns>十六进制字符串表示的哈希值</returns>
        private static string ToHash(byte[] data)
        {
            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 获取指定文件路径的 MD5 哈希值
        /// </summary>
        /// <param name="filePath">文件的完整路径</param>
        /// <returns>文件的 MD5 哈希值</returns>
        /// <exception cref="FileNotFoundException">如果指定的文件不存在，则抛出此异常</exception>
        public static string HashByFilePath(string filePath)
        {
            using var file = new FileStream(filePath, FileMode.Open);
            return Hash(file);
        }

        /// <summary>
        /// 计算字节数组的 MD5 哈希值
        /// </summary>
        /// <param name="inputBytes">要计算哈希值的字节数组</param>
        /// <returns>字节数组的 MD5 哈希值</returns>
        public static string Hash(byte[] inputBytes)
        {
            var hashBytes = Md5Cryptography.ComputeHash(inputBytes);
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            return hash;
        }
    }
}