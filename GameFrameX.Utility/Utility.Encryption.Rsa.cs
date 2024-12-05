using System.Security.Cryptography;
using System.Text;

namespace GameFrameX.Utility;

/// <summary>
/// 提供加密解密相关的实用函数。
/// </summary>
public static partial class Encryption
{
    /// <summary>
    /// 提供 RSA 加密解密功能。
    /// </summary>
    public sealed class Rsa
    {
        private readonly RSACryptoServiceProvider _rsa;

        /// <summary>
        /// 使用指定的 RSA 对象初始化 RSA 加密解密对象。
        /// </summary>
        /// <param name="rsa">RSA 对象。</param>
        public Rsa(RSACryptoServiceProvider rsa)
        {
            _rsa = rsa;
        }

        /// <summary>
        /// 使用指定的 XML 格式的密钥初始化 RSA 加密解密对象。
        /// </summary>
        /// <param name="key">XML 格式的密钥。</param>
        public Rsa(string key)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);
            _rsa = rsa;
        }

        /// <summary>
        /// 生成 RSA 密钥对。
        /// </summary>
        /// <returns>包含私钥和公钥的字典。</returns>
        public static Dictionary<string, string> Make()
        {
            var dic = new Dictionary<string, string>();
            var rsa = new RSACryptoServiceProvider();
            dic["privateKey"] = rsa.ToXmlString(true);
            dic["publicKey"] = rsa.ToXmlString(false);
            return dic;
        }

        /// <summary>
        /// 使用公钥加密字符串内容。
        /// </summary>
        /// <param name="publicKey">公钥。</param>
        /// <param name="content">待加密的内容。</param>
        /// <returns>加密后的内容，以 Base64 编码的字符串形式返回。</returns>
        public static string Encrypt(string publicKey, string content)
        {
            var res = Encrypt(publicKey, Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 使用当前对象的公钥加密字符串内容。
        /// </summary>
        /// <param name="content">待加密的内容。</param>
        /// <returns>加密后的内容，以 Base64 编码的字符串形式返回。</returns>
        public string Encrypt(string content)
        {
            var res = Encrypt(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 使用公钥加密字节数组内容。
        /// </summary>
        /// <param name="publicKey">公钥。</param>
        /// <param name="content">待加密的内容。</param>
        /// <returns>加密后的内容，以字节数组形式返回。</returns>
        public static byte[] Encrypt(string publicKey, byte[] content)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            var cipherBytes = rsa.Encrypt(content, false);
            return cipherBytes;
        }

        /// <summary>
        /// 使用当前对象的公钥加密字节数组内容。
        /// </summary>
        /// <param name="content">待加密的内容。</param>
        /// <returns>加密后的内容，以字节数组形式返回。</returns>
        public byte[] Encrypt(byte[] content)
        {
            var cipherBytes = _rsa.Encrypt(content, false);
            return cipherBytes;
        }

        /// <summary>
        /// 使用私钥解密 Base64 编码的字符串内容。
        /// </summary>
        /// <param name="privateKey">私钥。</param>
        /// <param name="content">待解密的内容。</param>
        /// <returns>解密后的内容，以字符串形式返回。</returns>
        public static string Decrypt(string privateKey, string content)
        {
            var res = Decrypt(privateKey, Convert.FromBase64String(content));
            return Encoding.UTF8.GetString(res);
        }

        /// <summary>
        /// 使用私钥解密字节数组内容。
        /// </summary>
        /// <param name="privateKey">私钥。</param>
        /// <param name="content">待解密的内容。</param>
        /// <returns>解密后的内容，以字节数组形式返回。</returns>
        public static byte[] Decrypt(string privateKey, byte[] content)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            var cipherBytes = rsa.Decrypt(content, false);
            return cipherBytes;
        }

        /// <summary>
        /// 使用当前对象的私钥解密 Base64 编码的字符串内容。
        /// </summary>
        /// <param name="content">待解密的内容。</param>
        /// <returns>解密后的内容，以字符串形式返回。</returns>
        public string Decrypt(string content)
        {
            var res = Decrypt(Convert.FromBase64String(content));
            return Encoding.UTF8.GetString(res);
        }

        /// <summary>
        /// 使用当前对象的私钥解密字节数组内容。
        /// </summary>
        /// <param name="content">待解密的内容。</param>
        /// <returns>解密后的内容，以字节数组形式返回。</returns>
        public byte[] Decrypt(byte[] content)
        {
            var bytes = _rsa.Decrypt(content, false);
            return bytes;
        }

        /// <summary>
        /// 使用私钥对数据进行签名。
        /// </summary>
        /// <param name="dataToSign">待签名的数据。</param>
        /// <param name="privateKey">私钥。</param>
        /// <returns>签名后的数据，以字节数组形式返回。</returns>
        public static byte[] SignData(byte[] dataToSign, string privateKey)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                return rsa.SignData(dataToSign, new SHA1CryptoServiceProvider());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 使用私钥对字符串数据进行签名。
        /// </summary>
        /// <param name="dataToSign">待签名的数据。</param>
        /// <param name="privateKey">私钥。</param>
        /// <returns>签名后的数据，以 Base64 编码的字符串形式返回。</returns>
        public static string SignData(string dataToSign, string privateKey)
        {
            var res = SignData(Encoding.UTF8.GetBytes(dataToSign), privateKey);
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 使用当前对象的私钥对数据进行签名。
        /// </summary>
        /// <param name="dataToSign">待签名的数据。</param>
        /// <returns>签名后的数据，以字节数组形式返回。</returns>
        public byte[] SignData(byte[] dataToSign)
        {
            try
            {
                return _rsa.SignData(dataToSign, new SHA1CryptoServiceProvider());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 使用当前对象的私钥对字符串数据进行签名。
        /// </summary>
        /// <param name="dataToSign">待签名的数据。</param>
        /// <returns>签名后的数据，以 Base64 编码的字符串形式返回。</returns>
        public string SignData(string dataToSign)
        {
            var res = SignData(Encoding.UTF8.GetBytes(dataToSign));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 使用公钥验证数据签名。
        /// </summary>
        /// <param name="dataToVerify">待验证的数据。</param>
        /// <param name="signedData">签名后的数据。</param>
        /// <param name="publicKey">公钥。</param>
        /// <returns>如果签名有效则返回 true，否则返回 false。</returns>
        public static bool VerifyData(byte[] dataToVerify, byte[] signedData, string publicKey)
        {
            try
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                return rsa.VerifyData(dataToVerify, new SHA1CryptoServiceProvider(), signedData);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 使用公钥验证字符串数据的签名。
        /// </summary>
        /// <param name="dataToVerify">待验证的数据。</param>
        /// <param name="signedData">签名后的数据。</param>
        /// <param name="publicKey">公钥。</param>
        /// <returns>如果签名有效则返回 true，否则返回 false。</returns>
        public static bool RsaVerifyData(string dataToVerify, string signedData, string publicKey)
        {
            return VerifyData(Encoding.UTF8.GetBytes(dataToVerify), Convert.FromBase64String(signedData), publicKey);
        }

        /// <summary>
        /// 使用当前对象的公钥验证数据签名。
        /// </summary>
        /// <param name="dataToVerify">待验证的数据。</param>
        /// <param name="signedData">签名后的数据。</param>
        /// <returns>如果签名有效则返回 true，否则返回 false。</returns>
        public bool VerifyData(byte[] dataToVerify, byte[] signedData)
        {
            try
            {
                return _rsa.VerifyData(dataToVerify, new SHA1CryptoServiceProvider(), signedData);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 使用当前对象的公钥验证字符串数据的签名。
        /// </summary>
        /// <param name="dataToVerify">待验证的数据。</param>
        /// <param name="signedData">签名后的数据。</param>
        /// <returns>如果签名有效则返回 true，否则返回 false。</returns>
        public bool VerifyData(string dataToVerify, string signedData)
        {
            try
            {
                return VerifyData(Encoding.UTF8.GetBytes(dataToVerify), Convert.FromBase64String(signedData));
            }
            catch
            {
                return false;
            }
        }
    }
}