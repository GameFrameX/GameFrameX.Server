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

using GameFrameX.NetWork.Messages;

namespace GameFrameX.StartUp.ServiceClient;

/// <summary>
/// Game application service configuration class
/// </summary>
/// <remarks>
/// 游戏应用服务配置类
/// </remarks>
public sealed class GameAppServiceConfiguration
{
    /// <summary>
    /// Gets or sets whether heartbeat is enabled
    /// </summary>
    /// <remarks>
    /// 获取或设置是否启用心跳包，默认启用
    /// </remarks>
    /// <value>
    /// Returns true if heartbeat is enabled, otherwise false. Default is true
    /// </value>
    public bool IsEnableHeartBeat { get; init; } = true;

    /// <summary>
    /// Gets or sets the heartbeat interval in milliseconds
    /// </summary>
    /// <remarks>
    /// 获取或设置心跳间隔（毫秒），默认 5000 毫秒
    /// </remarks>
    /// <value>
    /// Returns the heartbeat interval in milliseconds. Default is 5000
    /// </value>
    public int HeartBeatInterval { get; init; } = 5000;

    /// <summary>
    /// Gets or sets whether connection delay is enabled
    /// </summary>
    /// <remarks>
    /// 获取或设置是否启用连接延迟，默认启用
    /// </remarks>
    /// <value>
    /// Returns true if connection delay is enabled, otherwise false. Default is true
    /// </value>
    public bool IsEnableConnectDelay { get; init; } = true;

    /// <summary>
    /// Gets or sets the connection delay in milliseconds
    /// </summary>
    /// <remarks>
    /// 获取或设置连接延迟（毫秒），默认 5000 毫秒
    /// </remarks>
    /// <value>
    /// Returns the connection delay in milliseconds. Default is 5000
    /// </value>
    public int ConnectDelay { get; init; } = 5000;

    /// <summary>
    /// Gets or sets the retry delay in milliseconds
    /// </summary>
    /// <remarks>
    /// 获取或设置重试延迟（毫秒），默认 5000 毫秒
    /// </remarks>
    /// <value>
    /// Returns the retry delay in milliseconds. Default is 5000
    /// </value>
    public int RetryDelay { get; init; } = 5000;

    /// <summary>
    /// Gets or sets the maximum retry count
    /// </summary>
    /// <remarks>
    /// 获取或设置最大重试次数，默认 -1 表示无限重试
    /// </remarks>
    /// <value>
    /// Returns the maximum retry count. Default is -1 which means infinite retries
    /// </value>
    public int MaxRetryCount { get; init; } = -1;

    /// <summary>
    /// Gets or sets the callback action when connection is established
    /// </summary>
    /// <remarks>
    /// 获取或设置连接成功时的回调
    /// </remarks>
    /// <value>
    /// Returns the action to be invoked when connection is successfully established
    /// </value>
    public Action<string> OnConnected { get; init; }

    /// <summary>
    /// Gets or sets the callback action when connection is closed
    /// </summary>
    /// <remarks>
    /// 获取或设置连接关闭时的回调
    /// </remarks>
    /// <value>
    /// Returns the action to be invoked when connection is closed
    /// </value>
    public Action<string> OnClosed { get; init; }

    /// <summary>
    /// Gets or sets the callback action when an error occurs
    /// </summary>
    /// <remarks>
    /// 获取或设置发生错误时的回调
    /// </remarks>
    /// <value>
    /// Returns the action to be invoked when an error occurs, with error event arguments as parameter
    /// </value>
    public Action<string, SuperSocket.ClientEngine.ErrorEventArgs> OnError { get; init; }

    /// <summary>
    /// Gets or sets the callback action when a message is received
    /// </summary>
    /// <remarks>
    /// 获取或设置接收到消息时的回调
    /// </remarks>
    /// <value>
    /// Returns the action to be invoked when a message is received, with MessageObject as parameter
    /// </value>
    public Action<string, MessageObject> OnMessage { get; init; }

    /// <summary>
    /// Gets or sets the heartbeat message generation callback
    /// </summary>
    /// <remarks>
    /// 获取或设置心跳包生成回调，返回待发送的心跳消息
    /// </remarks>
    /// <value>
    /// Returns the function that generates heartbeat messages to be sent
    /// </value>
    public Func<string, MessageObject> OnHeartBeat { get; init; }
}