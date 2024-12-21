using GameFrameX.Utility;
using Newtonsoft.Json;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP 结果类，用于封装HTTP请求的响应结果
/// </summary>
public sealed class HttpResult
{
    /// <summary>
    /// 成功的HTTP结果
    /// </summary>
    public static readonly HttpResult Success = new(HttpStatusCode.Success);

    /// <summary>
    /// 未定义的命令的HTTP结果
    /// </summary>
    public static readonly HttpResult Undefined = new(HttpStatusCode.Undefined, HttpStatusMessage.UndefinedCommand);

    /// <summary>
    /// 未找到的命令的HTTP结果
    /// </summary>
    public static readonly HttpResult NotFound = new(HttpStatusCode.NotFound, HttpStatusMessage.NotFoundCommand);

    /// <summary>
    /// 验证失败的命令的HTTP结果
    /// </summary>
    public static readonly HttpResult CheckFailed = new(HttpStatusCode.CheckFailed, HttpStatusMessage.CheckFailedCommand);

    /// <summary>
    /// 参数错误的HTTP结果
    /// </summary>
    public static readonly HttpResult ParamError = new(HttpStatusCode.Illegal, HttpStatusMessage.ParamErrorMessage);

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">HTTP状态码</param>
    /// <param name="message">返回消息</param>
    /// <param name="data">数据体</param>
    private HttpResult(HttpStatusCode code = HttpStatusCode.Success, string message = "ok", object data = null)
    {
        Code = (int)code;
        Message = message;
        Data = data;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="code">HTTP状态码</param>
    /// <param name="message">返回消息</param>
    /// <param name="data">数据体</param>
    private HttpResult(int code = 0, string message = "ok", object data = null)
    {
        Code = code;
        Message = message;
        Data = data;
    }

    /// <summary>
    /// HTTP状态码
    /// </summary>
    [JsonProperty(PropertyName = "code")]
    public int Code { get; set; }

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

    /// <summary>
    /// 创建HTTP结果
    /// </summary>
    /// <param name="code">HTTP状态码</param>
    /// <param name="message">返回消息</param>
    /// <param name="data">额外数据</param>
    /// <returns>JSON字符串表示的HTTP结果</returns>
    public static string Create(HttpStatusCode code = HttpStatusCode.Success, string message = "", object data = null)
    {
        return new HttpResult(code, message, data).ToString();
    }

    /// <summary>
    /// 创建HTTP结果
    /// </summary>
    /// <param name="code">HTTP状态码</param>
    /// <param name="message">返回消息</param>
    /// <param name="data">额外数据</param>
    /// <returns>JSON字符串表示的HTTP结果</returns>
    public static string Create(int code = 0, string message = "", object data = null)
    {
        return new HttpResult(code, message, data).ToString();
    }

    /// <summary>
    /// 创建成功的HTTP结果
    /// </summary>
    /// <param name="message">返回消息</param>
    /// <param name="data">额外数据</param>
    /// <returns>JSON字符串表示的成功HTTP结果</returns>
    public static string CreateOk(string message = "", object data = null)
    {
        return new HttpResult(HttpStatusCode.Success, message, data).ToString();
    }

    /// <summary>
    /// 创建参数错误的HTTP结果
    /// </summary>
    /// <param name="errorMessage">错误消息</param>
    /// <returns>JSON字符串表示的参数错误HTTP结果</returns>
    public static string CreateErrorParam(string errorMessage = "")
    {
        return new HttpResult(HttpStatusCode.ParamErr, errorMessage).ToString();
    }

    /// <summary>
    /// 创建操作失败的HTTP结果
    /// </summary>
    /// <param name="errorMessage">错误消息</param>
    /// <returns>JSON字符串表示的操作失败HTTP结果</returns>
    public static string CreateActionFailed(string errorMessage = "")
    {
        return new HttpResult(HttpStatusCode.ActionFailed, errorMessage).ToString();
    }

    /// <summary>
    /// 将HTTP结果转换为字符串
    /// </summary>
    /// <returns>JSON字符串表示的HTTP结果</returns>
    public override string ToString()
    {
        return JsonHelper.Serialize(this);
    }

    /// <summary>
    /// 隐式转换HttpResult对象为字符串
    /// </summary>
    /// <param name="value">HttpResult对象</param>
    /// <returns>字符串表示的HttpResult对象</returns>
    public static implicit operator string(HttpResult value)
    {
        return value.ToString();
    }

    /// <summary>
    /// 创建带有数据的成功HTTP结果
    /// </summary>
    /// <param name="data">数据体</param>
    /// <returns>JSON字符串表示的成功HTTP结果</returns>
    public static string Create(object data)
    {
        return new HttpResult(HttpStatusCode.Success, HttpStatusMessage.Success, data).ToString();
    }
}