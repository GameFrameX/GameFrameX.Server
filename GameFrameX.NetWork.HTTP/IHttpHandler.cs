using GameFrameX.NetWork.Messages;

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
    /// 处理HTTP请求的异步操作，返回字符串结果。
    /// </summary>
    /// <param name="ip">客户端IP地址。</param>
    /// <param name="url">请求的URL。</param>
    /// <param name="paramMap">请求参数字典，键为参数名，值为参数值。</param>
    /// <returns>返回处理结果的字符串。</returns>
    Task<string> Action(string ip, string url, Dictionary<string, object> paramMap);


    /// <summary>
    /// 处理HTTP请求的异步操作，返回MessageObject对象。
    /// </summary>
    /// <param name="ip">客户端IP地址。</param>
    /// <param name="url">请求的URL。</param>
    /// <param name="paramMap">请求参数字典，键为参数名，值为参数值。</param>
    /// <param name="messageObject">消息对象，包含更多信息。</param>
    /// <returns>返回处理结果的MessageObject对象。</returns>
    Task<MessageObject> Action(string ip, string url, Dictionary<string, object> paramMap, MessageObject messageObject);
}