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

using Microsoft.AspNetCore.Http;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP 请求方法类型枚举。
/// </summary>
/// <remarks>
/// HTTP request method type enumeration.
/// </remarks>
public enum HttpMethodType
{
    /// <summary>
    /// POST 请求方法。
    /// </summary>
    /// <remarks>
    /// POST request method.
    /// </remarks>
    POST = 1,

    /// <summary>
    /// GET 请求方法。
    /// </summary>
    /// <remarks>
    /// GET request method.
    /// </remarks>
    GET = 2,

    /// <summary>
    /// PUT 请求方法。
    /// </summary>
    /// <remarks>
    /// PUT request method.
    /// </remarks>
    PUT = 3,

    /// <summary>
    /// DELETE 请求方法。
    /// </summary>
    /// <remarks>
    /// DELETE request method.
    /// </remarks>
    DELETE = 4,
}

/// <summary>
/// <see cref="HttpMethodType"/> 的扩展方法类。
/// </summary>
/// <remarks>
/// Extension methods for <see cref="HttpMethodType"/>.
/// </remarks>
public static class HttpMethodTypeExtensions
{
    /// <summary>
    /// 将 <see cref="HttpMethodType"/> 转换为标准的 HTTP 方法字符串。
    /// </summary>
    /// <remarks>
    /// Converts <see cref="HttpMethodType"/> to standard HTTP method string.
    /// </remarks>
    /// <param name="httpMethodType">要转换的 HTTP 方法类型 / HTTP method type to convert</param>
    /// <returns>标准的 HTTP 方法字符串（使用系统常量） / Standard HTTP method string (using system constants)</returns>
    public static string ToHttpMethodString(this HttpMethodType httpMethodType)
    {
        switch (httpMethodType)
        {
            case HttpMethodType.GET:
                return HttpMethods.Get;
            case HttpMethodType.POST:
                return HttpMethods.Post;
            case HttpMethodType.PUT:
                return HttpMethods.Put;
            case HttpMethodType.DELETE:
                return HttpMethods.Delete;
            default:
                return HttpMethods.Post;
        }
    }
}