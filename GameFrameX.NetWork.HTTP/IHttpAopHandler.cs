using Microsoft.AspNetCore.Http;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP 拦截处理器
/// </summary>
public interface IHttpAopHandler
{
    /// <summary>
    /// 优先级
    /// </summary>
    int Priority { get; }

    /// <summary>
    /// 执行处理
    /// </summary>
    /// <param name="context">上下文</param>
    /// <param name="ip">请求IP</param>
    /// <param name="url">请求地址</param>
    /// <param name="paramMap">参数列表</param>
    /// <returns>需要继续执行返回TRUE,否则返回FALSE</returns>
    bool Run(HttpContext context, string ip, string url, Dictionary<string, string> paramMap);
}