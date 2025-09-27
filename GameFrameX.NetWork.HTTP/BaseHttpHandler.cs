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