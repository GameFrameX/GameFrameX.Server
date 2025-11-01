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
/// Startup attribute for marking classes with startup configuration / 用于标记类的启动配置属性
/// </summary>
/// <remarks>
/// 此属性用于标记需要在应用程序启动时初始化的类，可以指定服务器类型和启动优先级。
/// 属性可以应用于类，支持多重应用，但不支持继承。
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
    /// Gets the startup priority, lower values have higher priority / 获取启动优先级，值越小优先级越高
    /// </summary>
    /// <value>
    /// An integer representing the startup priority. Lower values indicate higher priority.
    /// </value>
    /// <remarks>
    /// 启动优先级决定了类的初始化顺序，数值越小的类会越早被初始化。
    /// 默认优先级为1000。
    /// </remarks>
    public readonly int Priority;

    /// <summary>
    /// Gets the server type identifier / 获取服务器类型标识符
    /// </summary>
    /// <value>
    /// A string representing the server type that this startup class is associated with.
    /// </value>
    /// <remarks>
    /// 服务器类型用于标识当前启动类适用于哪种类型的服务器，
    /// 系统会根据此标识符来决定是否启动对应的类。
    /// </remarks>
    public readonly string ServerType;

    /// <summary>
    /// Initializes a new instance of the StartUpTagAttribute class / 初始化StartUpTagAttribute类的新实例
    /// </summary>
    /// <param name="serverType">The server type identifier / 服务器类型标识符</param>
    /// <param name="priority">The startup priority, default is 1000 / 启动优先级，默认为1000</param>
    /// <exception cref="ArgumentNullException">Thrown when serverType is null / 当serverType为null时抛出</exception>
    /// <exception cref="ArgumentException">Thrown when serverType is empty or whitespace / 当serverType为空或空白字符时抛出</exception>
    /// <remarks>
    /// 构造函数用于创建启动标签属性实例，需要指定服务器类型，
    /// 优先级参数是可选的，如果不指定则使用默认值1000。
    /// </remarks>
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