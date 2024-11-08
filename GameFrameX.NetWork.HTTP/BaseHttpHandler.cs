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
        /// 是否使用内部验证方式
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
        /// <param name="paramMap"></param>
        /// <returns></returns>
        public string CheckSign(Dictionary<string, string> paramMap)
        {
            // if (!IsCheckSign || GlobalSettings.IsDebug)
            if (!IsCheckSign)
            {
                return "";
            }

            //内部验证
            if (!paramMap.ContainsKey("token") || !paramMap.ContainsKey("timestamp"))
            {
                LogHelper.Error("http命令未包含验证参数");
                return HttpResult.Create(HttpStatusCode.Illegal, "http命令未包含验证参数");
            }

            var sign = paramMap["token"];
            var time = paramMap["timestamp"];
            long.TryParse(time, out long timeTick);
            var span = new TimeSpan(Math.Abs(DateTime.Now.Ticks - timeTick));
            if (span.TotalMinutes > 5) //5分钟内有效
            {
                LogHelper.Error("http命令已过期");
                return HttpResult.Create(HttpStatusCode.Illegal, "http命令已过期");
            }

            var str = 21001 + time;
            if (sign == GetStringSign(str))
            {
                return "";
            }
            else
            {
                return HttpResult.Create(HttpStatusCode.Illegal, "命令验证失败");
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="url"></param>
        /// <param name="paramMap"></param>
        /// <returns></returns>
        public abstract Task<string> Action(string ip, string url, Dictionary<string, string> paramMap);
    }
}