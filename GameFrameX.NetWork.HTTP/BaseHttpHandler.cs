// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Foundation.Hash;
using GameFrameX.Foundation.Http.Normalization;
using GameFrameX.Foundation.Utility;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// 基础 HTTP 处理器，用于处理 HTTP 请求的基础逻辑。
/// </summary>
/// <remarks>
/// Base HTTP handler for processing HTTP requests.
/// Provides signature validation and common request handling functionality.
/// </remarks>
public abstract class BaseHttpHandler : IHttpHandler
{
    /// <summary>
    /// 获取校验时间差，用于生成签名时的时间偏移量。
    /// </summary>
    /// <remarks>
    /// Gets the check code time offset used for signature generation.
    /// </remarks>
    /// <value>校验时间差值 / Check code time offset</value>
    protected virtual int CheckCodeTime { get; } = 38848;

    /// <summary>
    /// 获取头校验码，用于生成签名时的头部校验码。
    /// </summary>
    /// <remarks>
    /// Gets the header check code used for signature generation.
    /// </remarks>
    /// <value>头校验码 / Header check code</value>
    protected virtual ushort CheckCodeStart { get; } = 88;

    /// <summary>
    /// 获取尾校验码，用于生成签名时的尾部校验码。
    /// </summary>
    /// <remarks>
    /// Gets the tail check code used for signature generation.
    /// </remarks>
    /// <value>尾校验码 / Tail check code</value>
    protected virtual ushort CheckCodeEnd { get; } = 66;

    /// <summary>
    /// 获取是否需要校验签名，默认为不需要校验。
    /// </summary>
    /// <remarks>
    /// Gets whether signature validation is required, defaults to no validation.
    /// </remarks>
    /// <value>如果需要校验签名则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if signature validation is required; otherwise <c>false</c></value>
    public virtual bool IsCheckSign
    {
        get { return false; }
    }


    /// <summary>
    /// 处理 HTTP 请求的异步操作，返回字符串结果。
    /// </summary>
    /// <remarks>
    /// Asynchronously processes HTTP request and returns string result.
    /// </remarks>
    /// <param name="ip">客户端 IP 地址 / Client IP address</param>
    /// <param name="url">请求的 URL / Request URL</param>
    /// <param name="paramMap">请求参数字典，键为参数名，值为参数值 / Request parameter dictionary with parameter names as keys and values as parameter values</param>
    /// <returns>处理结果的字符串 / String result of the processing</returns>
    public virtual Task<string> Action(string ip, string url, Dictionary<string, object> paramMap)
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 处理 HTTP 请求的异步操作，返回 <see cref="MessageObject"/> 对象。
    /// </summary>
    /// <remarks>
    /// Asynchronously processes HTTP request and returns <see cref="MessageObject"/> result.
    /// </remarks>
    /// <param name="ip">客户端 IP 地址 / Client IP address</param>
    /// <param name="url">请求的 URL / Request URL</param>
    /// <param name="paramMap">请求参数字典，键为参数名，值为参数值 / Request parameter dictionary with parameter names as keys and values as parameter values</param>
    /// <param name="messageObject">消息对象，包含更多信息 / Message object containing additional information</param>
    /// <returns>处理结果的 <see cref="MessageObject"/> 对象 / <see cref="MessageObject"/> result of the processing</returns>
    public virtual Task<MessageObject> Action(string ip, string url, Dictionary<string, object> paramMap, MessageObject messageObject)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 处理 HTTP 请求的异步操作，返回字符串结果。
    /// </summary>
    /// <remarks>
    /// Asynchronously processes HTTP request and returns string result.
    /// </remarks>
    /// <param name="ip">客户端 IP 地址 / Client IP address</param>
    /// <param name="url">请求的 URL / Request URL</param>
    /// <param name="request">请求参数对象 / Request parameter object</param>
    /// <returns>处理结果的字符串 / String result of the processing</returns>
    public virtual Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取签名字符串。
    /// </summary>
    /// <remarks>
    /// Generates signature string from the input.
    /// </remarks>
    /// <param name="str">待签名的字符串 / String to be signed</param>
    /// <returns>签名后的字符串 / Signed string</returns>
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
    /// <remarks>
    /// Validates whether the signature is valid.
    /// </remarks>
    /// <param name="paramMap">请求参数字典 / Request parameter dictionary</param>
    /// <param name="error">错误消息，如果校验失败则返回具体的错误信息 / Error message, returns specific error information if validation fails</param>
    /// <returns>如果校验成功则返回 <c>true</c>；否则返回 <c>false</c> / <c>true</c> if validation succeeds; otherwise <c>false</c></returns>
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
        var span = TimerHelper.TimeSpanWithTimestampUtc(timeTick);
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