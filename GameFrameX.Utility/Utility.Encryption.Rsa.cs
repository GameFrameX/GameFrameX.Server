using System.Security.Cryptography;
using System.Text;

namespace GameFrameX.Utility;

/// <summary>
/// 加密解密相关的实用函数。
/// </summary>
public static partial class Encryption
{
    /// <summary>
    /// RSA 加密解密
    /// </summary>
    public sealed class Rsa
    {
        private readonly RSACryptoServiceProvider _rsa = null;

        /// <summary>
        /// 构建加密解密对象
        /// </summary>
        /// <param name="rsa"></param>
        public Rsa(RSACryptoServiceProvider rsa)
        {
            _rsa = rsa;
        }

        /// <summary>
        /// 构建加密解密对象
        /// </summary>
        /// <param name="key"></param>
        public Rsa(string key)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(key);
            this._rsa = rsa;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> Make()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            RSACryptoServiceProvider dsa = new RSACryptoServiceProvider();
            dic["privateKey"] = dsa.ToXmlString(true);
            dic["publicKey"] = dsa.ToXmlString(false);
            return dic;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="publicKey">公钥</param>
        /// <param name="content">所加密的内容</param>
        /// <returns>加密后的内容</returns>
        public static string RSAEncrypt(string publicKey, string content)
        {
            byte[] res = RSAEncrypt(publicKey, Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Encrypt(string content)
        {
            byte[] res = Encrypt(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="publicKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] RSAEncrypt(string publicKey, byte[] content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);
            byte[] cipherBytes = rsa.Encrypt(content, false);
            return cipherBytes;
        }

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] Encrypt(byte[] content)
        {
            byte[] cipherBytes = _rsa.Encrypt(content, false);
            return cipherBytes;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privateKey">私钥</param>
        /// <param name="content">加密后的内容</param>
        /// <returns>解密后的内容</returns>
        public static string RSADecrypt(string privateKey, string content)
        {
            byte[] res = RSADecrypt(privateKey, Convert.FromBase64String(content));
            return Encoding.UTF8.GetString(res);
        }


        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="privateKey"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static byte[] RSADecrypt(string privateKey, byte[] content)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);
            byte[] cipherBytes = rsa.Decrypt(content, false);
            return cipherBytes;
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public string Decrypt(string content)
        {
            byte[] res = Decrypt(Convert.FromBase64String(content));
            return Encoding.UTF8.GetString(res);
        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public byte[] Decrypt(byte[] content)
        {
            byte[] bytes = _rsa.Decrypt(content, false);
            return bytes;
        }

        /// <summary>
        /// 签名
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static byte[] RSASignData(byte[] dataToSign, string privateKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(privateKey);
                return rsa.SignData(dataToSign, new SHA1CryptoServiceProvider());
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string RSASignData(string dataToSign, string privateKey)
        {
            byte[] res = RSASignData(Encoding.UTF8.GetBytes(dataToSign), privateKey);
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 签名数据
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <returns></returns>
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
        /// 签名数据
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <returns></returns>
        public string SignData(string dataToSign)
        {
            byte[] res = SignData(Encoding.UTF8.GetBytes(dataToSign));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 验证签名
        /// </summary>
        /// <param name="dataToVerify"></param>
        /// <param name="signedData"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool RSAVerifyData(byte[] dataToVerify, byte[] signedData, string publicKey)
        {
            try
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(publicKey);
                return rsa.VerifyData(dataToVerify, new SHA1CryptoServiceProvider(), signedData);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证数据是否一致
        /// </summary>
        /// <param name="dataToVerify"></param>
        /// <param name="signedData"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool RsaVerifyData(string dataToVerify, string signedData, string publicKey)
        {
            return RSAVerifyData(Encoding.UTF8.GetBytes(dataToVerify), Convert.FromBase64String(signedData),
                publicKey);
        }

        /// <summary>
        /// 验证数据是否一致
        /// </summary>
        /// <param name="dataToVerify"></param>
        /// <param name="signedData"></param>
        /// <returns></returns>
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
        /// 验证数据是否一致
        /// </summary>
        /// <param name="dataToVerify"></param>
        /// <param name="signedData"></param>
        /// <returns></returns>
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