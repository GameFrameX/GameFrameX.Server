using System.Security.Cryptography;
using System.Text;
using GameFrameX.Log;

namespace GameFrameX.Utility;

/// <summary>
/// 加密解密相关的实用函数。
/// </summary>
public static partial class Encryption
{
    /// <summary>
    /// AES 加密解密
    /// </summary>
    public static class Aes
    {
        #region 加密

        #region 加密字符串

        /// <summary>
        /// 使用 AES 算法加密字符串（高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法）
        /// </summary>
        /// <param name="encryptString">待加密的明文字符串</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <returns>加密后的 Base64 编码字符串</returns>
        /// <exception cref="Exception">当明文或密钥为空时抛出异常</exception>
        public static string Encrypt(string encryptString, string encryptKey)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(encryptString), encryptKey));
        }

        #endregion

        #region 加密字节数组

        /// <summary>
        /// 使用 AES 算法加密字节数组（高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法）
        /// </summary>
        /// <param name="encryptByte">待加密的明文字节数组</param>
        /// <param name="encryptKey">加密密钥</param>
        /// <returns>加密后的字节数组</returns>
        /// <exception cref="Exception">当明文或密钥为空时抛出异常</exception>
        public static byte[] Encrypt(byte[] encryptByte, string encryptKey)
        {
            if (encryptByte.Length == 0)
            {
                throw new Exception("明文不得为空");
            }

            if (string.IsNullOrEmpty(encryptKey))
            {
                throw new Exception("密钥不得为空");
            }

            byte[] encryptedBytes = null;
            var iv = new byte[16] { 224, 131, 122, 101, 37, 254, 33, 17, 19, 28, 212, 130, 45, 65, 43, 32, };
            var salt = new byte[16] { 234, 231, 123, 100, 87, 254, 123, 17, 89, 18, 230, 13, 45, 65, 43, 32, };
            using (var aesProvider = Rijndael.Create())
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var pdb = new PasswordDeriveBytes(encryptKey, salt))
                        {
                            var transform = aesProvider.CreateEncryptor(pdb.GetBytes(32), iv);
                            using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(encryptByte, 0, encryptByte.Length);
                                cryptoStream.FlushFinalBlock();
                                encryptedBytes = memoryStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                }
            }

            return encryptedBytes;
        }

        #endregion

        #endregion

        #region 解密

        #region 解密字符串

        /// <summary>
        /// 使用 AES 算法解密字符串（高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法）
        /// </summary>
        /// <param name="decryptString">待解密的 Base64 编码字符串</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <returns>解密后的明文字符串</returns>
        /// <exception cref="Exception">当密文或密钥为空时抛出异常</exception>
        public static string Decrypt(string decryptString, string decryptKey)
        {
            return Encoding.UTF8.GetString(AesDecrypt(Convert.FromBase64String(decryptString), decryptKey));
        }

        #endregion

        #region 解密字节数组

        /// <summary>
        /// 使用 AES 算法解密字节数组（高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法）
        /// </summary>
        /// <param name="decryptByte">待解密的密文字节数组</param>
        /// <param name="decryptKey">解密密钥</param>
        /// <returns>解密后的明文字节数组</returns>
        /// <exception cref="Exception">当密文或密钥为空时抛出异常</exception>
        public static byte[] AesDecrypt(byte[] decryptByte, string decryptKey)
        {
            if (decryptByte.Length == 0)
            {
                throw new Exception("密文不得为空");
            }

            if (string.IsNullOrEmpty(decryptKey))
            {
                throw new Exception("密钥不得为空");
            }

            byte[] decryptedBytes = null;
            var iv = new byte[16] { 224, 131, 122, 101, 37, 254, 33, 17, 19, 28, 212, 130, 45, 65, 43, 32, };
            var salt = new byte[16] { 234, 231, 123, 100, 87, 254, 123, 17, 89, 18, 230, 13, 45, 65, 43, 32, };
            using (var aesProvider = Rijndael.Create())
            {
                try
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var pdb = new PasswordDeriveBytes(decryptKey, salt))
                        {
                            var transform = aesProvider.CreateDecryptor(pdb.GetBytes(32), iv);
                            using (var cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(decryptByte, 0, decryptByte.Length);
                                cryptoStream.FlushFinalBlock();
                                decryptedBytes = memoryStream.ToArray();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error(ex);
                }
            }

            return decryptedBytes;
        }

        #endregion

        #endregion
    }
}