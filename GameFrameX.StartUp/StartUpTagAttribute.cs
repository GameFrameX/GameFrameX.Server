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

namespace GameFrameX.StartUp;

/// <summary>
/// 用于标记类的启动配置属性。
/// </summary>
/// <remarks>
/// Startup attribute for marking classes with startup configuration.
/// This attribute is used to mark classes that need to be initialized at application startup,
/// allowing specification of server type and startup priority.
/// The attribute can be applied to classes, supports multiple applications, but does not support inheritance.
/// </remarks>
/// <example>
/// <code>
/// [StartUpTag("GameServer", 100)]
/// public class GameServerStartUp : IAppStartUp
/// {
///     // Implementation
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class StartUpTagAttribute : Attribute
{
    /// <summary>
    /// 获取启动优先级，值越小优先级越高。
    /// </summary>
    /// <remarks>
    /// Gets the startup priority, lower values have higher priority.
    /// Startup priority determines the initialization order of classes;
    /// classes with smaller values will be initialized earlier.
    /// Default priority is 1000.
    /// </remarks>
    /// <value>表示启动优先级的整数，值越小优先级越高 / An integer representing the startup priority, lower values indicate higher priority</value>
    public int Priority { get; }

    /// <summary>
    /// 获取服务器类型标识符。
    /// </summary>
    /// <remarks>
    /// Gets the server type identifier.
    /// Server type is used to identify which type of server this startup class applies to;
    /// the system will decide whether to start the corresponding class based on this identifier.
    /// </remarks>
    /// <value>表示与此启动类关联的服务器类型的字符串 / A string representing the server type that this startup class is associated with</value>
    public string ServerType { get; }

    /// <summary>
    /// 初始化 <see cref="StartUpTagAttribute"/> 类的新实例。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the <see cref="StartUpTagAttribute"/> class.
    /// Constructor for creating a startup tag attribute instance, requires specifying the server type;
    /// the priority parameter is optional, if not specified the default value of 1000 is used.
    /// </remarks>
    /// <param name="serverType">服务器类型标识符 / The server type identifier</param>
    /// <param name="priority">启动优先级，默认为 1000 / The startup priority, default is 1000</param>
    /// <exception cref="ArgumentNullException">当 <paramref name="serverType"/> 为 null 时抛出 / Thrown when <paramref name="serverType"/> is null</exception>
    /// <exception cref="ArgumentException">当 <paramref name="serverType"/> 为空或空白字符时抛出 / Thrown when <paramref name="serverType"/> is empty or whitespace</exception>
    /// <example>
    /// <code>
    /// // 创建一个游戏服务器启动标签，优先级为100
    /// var attribute = new StartUpTagAttribute("GameServer", 100);
    ///
    /// // 创建一个登录服务器启动标签，使用默认优先级
    /// var attribute2 = new StartUpTagAttribute("LoginServer");
    /// </code>
    /// </example>
    public StartUpTagAttribute(string serverType, int priority = 1000)
    {
        ArgumentNullException.ThrowIfNull(serverType, nameof(serverType));
        ArgumentException.ThrowIfNullOrWhiteSpace(serverType, nameof(serverType));

        ServerType = serverType;
        Priority = priority;
    }
}
