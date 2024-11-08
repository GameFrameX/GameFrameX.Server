namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP处理器
/// </summary>
public interface IHttpHandler
{
    /// <summary>
    /// 是否校验签名
    /// </summary>
    bool IsCheckSign { get; }

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="ip">来源ID</param>
    /// <param name="url">请求URL</param>
    /// <param name="paramMap">参数字典</param>
    /// <returns></returns>
    Task<string> Action(string ip, string url, Dictionary<string, object> paramMap);
}