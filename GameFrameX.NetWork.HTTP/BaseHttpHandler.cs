using System.Security.Cryptography;
using System.Text;
using GameFrameX.Log;
using GameFrameX.Setting;
using GameFrameX.Utility;

namespace GameFrameX.NetWork.HTTP
{
    /// <summary>
    /// 基础HTTP处理器
    /// </summary>
    public abstract class BaseHttpHandler : IHttpHandler
    {
        /// <summary>
        /// 是否校验签名
        /// </summary>
        public virtual bool IsCheckSign
        {
            get { return false; }
        }

        /// <summary>
        /// 获取签名
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetStringSign(string str)
        {
            //取md5
            string md5 = Hash.MD5.Hash(str);

            int checkCode1 = 0; //校验码
            int checkCode2 = 0;
            foreach (var t in md5)
            {
                if (t >= 'a')
                {
                    checkCode1 += t;
                }
                else
                {
                    checkCode2 += t;
                }
            }

            md5 = checkCode1 + md5 + checkCode2;

            return md5;
        }

        /// <summary>
        /// 校验签名
        /// </summary>
        /// <param name="paramMap">参数列表</param>
        /// <param name="error">错误消息</param>
        /// <returns></returns>
        public bool CheckSign(Dictionary<string, object> paramMap, out string error)
        {
            error = string.Empty;
            // if (!IsCheckSign || GlobalSettings.IsDebug)
            if (!IsCheckSign)
            {
                // 不校验
                return true;
            }

            //内部验证
            if (!paramMap.ContainsKey("token") || !paramMap.ContainsKey("timestamp"))
            {
                LogHelper.Error("http命令未包含验证参数");
                error = HttpResult.Create(HttpStatusCode.Illegal, "http命令未包含验证参数");
                return false;
            }

            var sign = paramMap["token"].ToString();
            var time = paramMap["timestamp"].ToString();
            long.TryParse(time, out var timeTick);
            var span = new TimeSpan(Math.Abs(DateTime.Now.Ticks - timeTick));
            if (span.TotalMinutes > 5) //5分钟内有效
            {
                LogHelper.Error("http命令已过期");
                error = HttpResult.Create(HttpStatusCode.Illegal, "http命令已过期");
                return false;
            }

            var str = 21001 + time;
            if (sign == GetStringSign(str))
            {
                return true;
            }

            error = HttpResult.Create(HttpStatusCode.Illegal, "命令验证失败");
            return false;
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="url"></param>
        /// <param name="paramMap"></param>
        /// <returns></returns>
        public abstract Task<string> Action(string ip, string url, Dictionary<string, object> paramMap);
    }
}