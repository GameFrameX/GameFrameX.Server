using System.Text;

namespace GameFrameX.Utility;

/// <summary>
/// 哈希计算相关的实用函数。
/// </summary>
public static partial class Hash
{
    /// <summary>
    /// MurmurHash3 计算工具类
    /// </summary>
    public static class MurmurHash3
    {
        /// <summary>
        /// 使用 MurmurHash3 算法计算字符串的哈希值
        /// </summary>
        /// <param name="str">要计算哈希值的字符串</param>
        /// <param name="seed">哈希算法的种子值，默认为27</param>
        /// <returns>字符串的哈希值</returns>
        public static uint Hash(string str, uint seed = 27)
        {
            var data = Encoding.UTF8.GetBytes(str);
            return Hash(data, (uint)data.Length, seed);
        }

        /// <summary>
        /// 使用 MurmurHash3 算法计算字节数组的哈希值
        /// </summary>
        /// <param name="data">要计算哈希值的字节数组</param>
        /// <param name="length">字节数组的有效长度</param>
        /// <param name="seed">哈希算法的种子值</param>
        /// <returns>字节数组的哈希值</returns>
        public static uint Hash(byte[] data, uint length, uint seed)
        {
            var nblocks = length >> 2;

            var h1 = seed;

            const uint c1 = 0xcc9e2d51;
            const uint c2 = 0x1b873593;

            //----------
            // body

            var i = 0;

            for (var j = nblocks; j > 0; --j)
            {
                var k1l = BitConverter.ToUInt32(data, i);

                k1l *= c1;
                k1l = rotl32(k1l, 15);
                k1l *= c2;

                h1 ^= k1l;
                h1 = rotl32(h1, 13);
                h1 = h1 * 5 + 0xe6546b64;

                i += 4;
            }

            //----------
            // tail

            nblocks <<= 2;

            uint k1 = 0;

            var tailLength = length & 3;

            if (tailLength == 3)
            {
                k1 ^= (uint)data[2 + nblocks] << 16;
            }

            if (tailLength >= 2)
            {
                k1 ^= (uint)data[1 + nblocks] << 8;
            }

            if (tailLength >= 1)
            {
                k1 ^= data[nblocks];
                k1 *= c1;
                k1 = rotl32(k1, 15);
                k1 *= c2;
                h1 ^= k1;
            }

            //----------
            // finalization

            h1 ^= length;

            h1 = fmix32(h1);

            return h1;
        }

        /// <summary>
        /// 对哈希值进行最终混合操作
        /// </summary>
        /// <param name="h">要混合的哈希值</param>
        /// <returns>混合后的哈希值</returns>
        private static uint fmix32(uint h)
        {
            h ^= h >> 16;
            h *= 0x85ebca6b;
            h ^= h >> 13;
            h *= 0xc2b2ae35;
            h ^= h >> 16;

            return h;
        }

        /// <summary>
        /// 对32位整数进行循环左移操作
        /// </summary>
        /// <param name="x">要进行循环左移的整数</param>
        /// <param name="r">左移的位数</param>
        /// <returns>循环左移后的整数</returns>
        private static uint rotl32(uint x, byte r)
        {
            return (x << r) | (x >> (32 - r));
        }
    }
}