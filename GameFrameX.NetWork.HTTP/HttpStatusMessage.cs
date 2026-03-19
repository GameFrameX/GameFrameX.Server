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

using GameFrameX.Utility.Setting;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP 状态消息常量和本地化消息。
/// </summary>
/// <remarks>
/// HTTP status message constants and localized messages.
/// </remarks>
public static class HttpStatusMessage
{
    /// <summary>
    /// 成功状态消息常量。
    /// </summary>
    /// <remarks>
    /// Success status message constant.
    /// </remarks>
    /// <value>成功消息字符串 / Success message string</value>
    public const string Success = "ok";

    /// <summary>
    /// 获取未定义命令的错误消息。
    /// </summary>
    /// <remarks>
    /// Gets the error message for undefined commands.
    /// </remarks>
    /// <value>未定义命令的本地化错误消息 / Localized error message for undefined commands</value>
    public static string UndefinedCommand
    {
        get { return LocalizationService.GetString(Localization.Keys.NetWorkHttp.UndefinedCommand); }
    }

    /// <summary>
    /// 获取未找到命令的错误消息。
    /// </summary>
    /// <remarks>
    /// Gets the error message for not found commands.
    /// </remarks>
    /// <value>未找到命令的本地化错误消息 / Localized error message for not found commands</value>
    public static string NotFoundCommand
    {
        get { return LocalizationService.GetString(Localization.Keys.NetWorkHttp.NotFoundCommand); }
    }

    /// <summary>
    /// 获取验证失败的错误消息。
    /// </summary>
    /// <remarks>
    /// Gets the error message for validation failures.
    /// </remarks>
    /// <value>验证失败的本地化错误消息 / Localized error message for validation failures</value>
    public static string CheckFailedCommand
    {
        get { return LocalizationService.GetString(Localization.Keys.NetWorkHttp.CheckFailedCommand); }
    }

    /// <summary>
    /// 获取服务器错误的错误消息。
    /// </summary>
    /// <remarks>
    /// Gets the error message for server errors.
    /// </remarks>
    /// <value>服务器错误的本地化错误消息 / Localized error message for server errors</value>
    public static string ServerError
    {
        get { return LocalizationService.GetString(Localization.Keys.NetWorkHttp.ServerError); }
    }

    /// <summary>
    /// 获取参数错误的错误消息。
    /// </summary>
    /// <remarks>
    /// Gets the error message for parameter errors, including missing sign and timestamp parameters.
    /// </remarks>
    /// <value>参数错误的本地化错误消息 / Localized error message for parameter errors</value>
    public static string ParamErrorMessage
    {
        get { return LocalizationService.GetString(Localization.Keys.NetWorkHttp.HttpCommandMissingValidationParameters, GlobalConst.HttpSignKey, GlobalConst.HttpTimestampKey); }
    }
}