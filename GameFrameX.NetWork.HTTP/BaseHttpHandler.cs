using GameFrameX.Foundation.Hash;
using GameFrameX.Foundation.Http.Normalization;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// 基础HTTP处理器，用于处理HTTP请求的基础逻辑。
/// </summary>
public abstract class BaseHttpHandler : IHttpHandler
{
    /// <summary>
    /// 校验时间差，用于生成签名时的时间偏移量。
    /// </summary>
    protected virtual int CheckCodeTime { get; } = 38848;

    /// <summary>
    /// 头校验码，用于生成签名时的头部校验码。
    /// </summary>
    protected virtual ushort CheckCodeStart { get; } = 88;

    /// <summary>
    /// 尾校验码，用于生成签名时的尾部校验码。
    /// </summary>
    protected virtual ushort CheckCodeEnd { get; } = 66;

    /// <summary>
    /// 是否需要校验签名，默认为不需要校验。
    /// </summary>
    public virtual bool IsCheckSign
    {
        get { return false; }
    }


    /// <summary>
    /// 处理HTTP请求的异步操作，返回字符串结果。
    /// </summary>
    /// <param name="ip">客户端IP地址。</param>
    /// <param name="url">请求的URL。</param>
    /// <param name="paramMap">请求参数字典，键为参数名，值为参数值。</param>
    /// <returns>返回处理结果的字符串。</returns>
    public virtual Task<string> Action(string ip, string url, Dictionary<string, object> paramMap)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 处理HTTP请求的异步操作，返回MessageObject对象。
    /// </summary>
    /// <param name="ip">客户端IP地址。</param>
    /// <param name="url">请求的URL。</param>
    /// <param name="paramMap">请求参数字典，键为参数名，值为参数值。</param>
    /// <param name="messageObject">消息对象，包含更多信息。</param>
    /// <returns>返回处理结果的MessageObject对象。</returns>
    public virtual Task<MessageObject> Action(string ip, string url, Dictionary<string, object> paramMap, MessageObject messageObject)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 处理HTTP请求的异步操作，返回MessageObject对象。
    /// </summary>
    /// <param name="ip">客户端IP地址。</param>
    /// <param name="url">请求的URL。</param>
    /// <param name="request">请求参数对象。</param>
    /// <returns>返回处理结果的MessageObject对象。</returns>
    public virtual Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取签名字符串。
    /// </summary>
    /// <param name="str">待签名的字符串。</param>
    /// <returns>签名后的字符串。</returns>
    public string GetStringSign(string str)
    {
        // 计算MD5哈希值
        var md5 = Md5Helper.Hash(str);

        var checkCode1 = CheckCodeStart; // 头校验码
        var checkCode2 = CheckCodeEnd; // 尾校验码
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
    /// 校验签名是否有效。
    /// </summary>
    /// <param name="paramMap">请求参数字典。</param>
    /// <param name="error">错误消息，如果校验失败则返回具体的错误信息。</param>
    /// <returns>校验结果，true表示校验成功，false表示校验失败。</returns>
    public bool CheckSign(Dictionary<string, object> paramMap, out string error)
    {
        error = string.Empty;
        if (!IsCheckSign)
        {
            // 不需要校验签名
            return true;
        }

        // 内部验证
        if (!paramMap.ContainsKey(GlobalConst.HttpSignKey) || !paramMap.ContainsKey(GlobalConst.HttpTimestampKey))
        {
            error = HttpJsonResult.ValidationErrorString();
            return false;
        }

        var sign = paramMap[GlobalConst.HttpSignKey].ToString();
        var time = paramMap[GlobalConst.HttpTimestampKey].ToString();
        long.TryParse(time, out var timeTick);
        var span = TimeHelper.TimeSpanWithTimestamp(timeTick);
        // 5分钟内有效
        if (span.TotalMinutes > 5)
        {
            error = HttpJsonResult.IllegalString();
            return false;
        }

        var str = CheckCodeTime + time;
        if (sign == GetStringSign(str))
        {
            return true;
        }

        error = HttpJsonResult.ValidationErrorString();
        return false;
    }
}