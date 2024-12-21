using System.Security.Cryptography;
using System.Text;

namespace GameFrameX.Utility;

/// <summary>
/// 加密解密相关的实用函数。
/// </summary>
public static partial class Encryption
{
    /// <summary>
    /// DSA 加密解密工具类
    /// </summary>
    public sealed class Dsa
    {
        private readonly DSACryptoServiceProvider _dsa;

        /// <summary>
        /// 使用现有的 DSACryptoServiceProvider 实例初始化 Dsa 类
        /// </summary>
        /// <param name="dsa">DSACryptoServiceProvider 实例</param>
        public Dsa(DSACryptoServiceProvider dsa)
        {
            _dsa = dsa;
        }

        /// <summary>
        /// 使用 XML 格式的密钥字符串初始化 Dsa 类
        /// </summary>
        /// <param name="key">XML 格式的密钥字符串</param>
        public Dsa(string key)
        {
            var dsa = new DSACryptoServiceProvider();
            dsa.FromXmlString(key);
            _dsa = dsa;
        }

        /// <summary>
        /// 生成新的 DSA 密钥对，并以 XML 字符串形式返回
        /// </summary>
        /// <returns>包含私钥和公钥的字典</returns>
        public static Dictionary<string, string> Make()
        {
            var dic = new Dictionary<string, string>();
            var dsa = new DSACryptoServiceProvider();
            dic["privatekey"] = dsa.ToXmlString(true);
            dic["publickey"] = dsa.ToXmlString(false);
            return dic;
        }

        /// <summary>
        /// 使用私钥对数据进行签名
        /// </summary>
        /// <param name="dataToSign">要签名的数据字节数组</param>
        /// <param name="privateKey">XML 格式的私钥字符串</param>
        /// <returns>签名后的字节数组</returns>
        public static byte[] SignData(byte[] dataToSign, string privateKey)
        {
            try
            {
                var dsa = new DSACryptoServiceProvider();
                dsa.FromXmlString(privateKey);
                return dsa.SignData(dataToSign);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 使用私钥对字符串数据进行签名，并返回 Base64 编码的签名字符串
        /// </summary>
        /// <param name="dataToSign">要签名的字符串数据</param>
        /// <param name="privateKey">XML 格式的私钥字符串</param>
        /// <returns>Base64 编码的签名字符串</returns>
        public static string SignData(string dataToSign, string privateKey)
        {
            var res = SignData(Encoding.UTF8.GetBytes(dataToSign), privateKey);
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 使用实例化的 DSACryptoServiceProvider 对数据进行签名
        /// </summary>
        /// <param name="dataToSign">要签名的数据字节数组</param>
        /// <returns>签名后的字节数组</returns>
        public byte[] SignData(byte[] dataToSign)
        {
            try
            {
                return _dsa.SignData(dataToSign);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 使用实例化的 DSACryptoServiceProvider 对字符串数据进行签名，并返回 Base64 编码的签名字符串
        /// </summary>
        /// <param name="dataToSign">要签名的字符串数据</param>
        /// <returns>Base64 编码的签名字符串</returns>
        public string SignData(string dataToSign)
        {
            var res = SignData(Encoding.UTF8.GetBytes(dataToSign));
            return Convert.ToBase64String(res);
        }

        /// <summary>
        /// 使用私钥验证数据的签名
        /// </summary>
        /// <param name="dataToVerify">要验证的数据字节数组</param>
        /// <param name="signedData">签名后的字节数组</param>
        /// <param name="privateKey">XML 格式的私钥字符串</param>
        /// <returns>如果签名有效，返回 true；否则返回 false</returns>
        public static bool VerifyData(byte[] dataToVerify, byte[] signedData, string privateKey)
        {
            try
            {
                var dsa = new DSACryptoServiceProvider();
                dsa.FromXmlString(privateKey);
                return dsa.VerifyData(dataToVerify, signedData);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 使用私钥验证字符串数据的签名
        /// </summary>
        /// <param name="dataToVerify">要验证的字符串数据</param>
        /// <param name="signedData">Base64 编码的签名字符串</param>
        /// <param name="privateKey">XML 格式的私钥字符串</param>
        /// <returns>如果签名有效，返回 true；否则返回 false</returns>
        public static bool VerifyData(string dataToVerify, string signedData, string privateKey)
        {
            return VerifyData(Encoding.UTF8.GetBytes(dataToVerify), Convert.FromBase64String(signedData), privateKey);
        }

        /// <summary>
        /// 使用实例化的 DSACryptoServiceProvider 验证数据的签名
        /// </summary>
        /// <param name="dataToVerify">要验证的数据字节数组</param>
        /// <param name="signedData">签名后的字节数组</param>
        /// <returns>如果签名有效，返回 true；否则返回 false</returns>
        public bool VerifyData(byte[] dataToVerify, byte[] signedData)
        {
            try
            {
                return _dsa.VerifyData(dataToVerify, signedData);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 使用实例化的 DSACryptoServiceProvider 验证字符串数据的签名
        /// </summary>
        /// <param name="dataToVerify">要验证的字符串数据</param>
        /// <param name="signedData">Base64 编码的签名字符串</param>
        /// <returns>如果签名有效，返回 true；否则返回 false</returns>
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