using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.NetWork.HTTP
{
    /// <summary>
    /// HTTP结果
    /// </summary>
    public sealed class HttpResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly HttpResult Success = new HttpResult(HttpStatusCode.Success, "ok");

        /// <summary>
        /// 未定义的命令
        /// </summary>
        public static readonly HttpResult Undefine = new HttpResult(HttpStatusCode.Undefine, "undefine command");


        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="retMsg"></param>
        /// <param name="extraMap"></param>
        /// <returns></returns>
        public static string Create(HttpStatusCode statusCode = HttpStatusCode.Success, string retMsg = "", object extraMap = null)
        {
            return new HttpResult(statusCode, retMsg, extraMap).ToString();
        }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="retMsg"></param>
        /// <param name="extraMap"></param>
        /// <returns></returns>
        public static string CreateOk(string retMsg = "", object extraMap = null)
        {
            return new HttpResult(HttpStatusCode.Success, retMsg, extraMap).ToString();
        }

        /// <summary>
        /// 参数错误
        /// </summary>
        /// <param name="retMsg"></param>
        /// <returns></returns>
        public static string CreateErrorParam(string retMsg = "")
        {
            return new HttpResult(HttpStatusCode.ParamErr, retMsg).ToString();
        }

        /// <summary>
        /// 操作失败
        /// </summary>
        /// <param name="retMsg"></param>
        /// <returns></returns>
        public static string CreateActionFailed(string retMsg = "")
        {
            return new HttpResult(HttpStatusCode.ActionFailed, retMsg).ToString();
        }

        /// <summary>
        /// 消息码
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public HttpStatusCode Code { get; set; }

        /// <summary>
        /// 消息描述
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        /// <summary>
        /// 数据体
        /// </summary>
        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }

        private HttpResult(HttpStatusCode retCode = HttpStatusCode.Success, string retMessage = "ok", object data = null)
        {
            Code    = retCode;
            Message = retMessage;
            Data    = data;
        }


        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonHelper.Serialize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static implicit operator string(HttpResult value)
        {
            return value.ToString();
        }

        /// <summary>
        /// 创建成功数据
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Create(object data)
        {
            return new HttpResult(HttpStatusCode.Success, HttpStatusMessage.Success, data).ToString();
        }
    }
}