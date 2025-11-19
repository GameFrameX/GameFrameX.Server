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

using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP消息处理器属性，用于标记HTTP消息处理器类
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class HttpMessageMappingAttribute : Attribute
{
    /// <summary>
    /// 处理器命名前缀
    /// </summary>
    public const string HTTPprefix = "";

    /// <summary>
    /// 处理器命名后缀
    /// </summary>
    public const string HTTPsuffix = "HttpHandler";

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="classType">处理器类的类型</param>
    /// <exception cref="ArgumentNullException">当classType为null时抛出</exception>
    /// <exception cref="InvalidOperationException">当classType不是密封类或不以HTTPsuffix结尾时抛出</exception>
    public HttpMessageMappingAttribute(Type classType)
    {
        ArgumentNullException.ThrowIfNull(classType, nameof(classType));
        var className = classType.Name;
        if (!classType.IsSealed)
        {
            throw new InvalidOperationException(LocalizationService.GetString(GameFrameX.Localization.Keys.NetWorkHttp.ClassMustBeSealed, className));
        }

        // if (!className.StartsWith(HTTPprefix, StringComparison.Ordinal))
        // {
        //     throw new InvalidOperationException($"{className} 必须以{HTTPprefix}开头");
        // }

        if (!className.EndsWith(HTTPsuffix, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(LocalizationService.GetString(GameFrameX.Localization.Keys.NetWorkHttp.ClassMustEndWithSuffix, className, HTTPsuffix));
        }

        OriginalCmd = className.Substring(HTTPprefix.Length, className.Length - HTTPprefix.Length - HTTPsuffix.Length);
        StandardCmd = OriginalCmd.ConvertToSnakeCase();
    }

    /// <summary>
    /// 原始命令名称
    /// </summary>
    public string OriginalCmd { get; }

    /// <summary>
    /// 标准化后的命令名称
    /// </summary>
    public string StandardCmd { get; }
}