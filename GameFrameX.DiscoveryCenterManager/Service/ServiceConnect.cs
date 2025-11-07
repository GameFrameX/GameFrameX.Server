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

using GameFrameX.DiscoveryCenterManager.ServiceClient;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;
using Serilog;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.DiscoveryCenterManager.Service;

/// <summary>
/// Service connection manager for Discovery Center integration
/// </summary>
/// <remarks>
/// 服务连接管理器，用于与服务发现中心建立并维护连接，包含初始化、事件回调、资源释放等逻辑。
/// 提供连接状态监控，以及与 `GameFrameX.StartUp.ServiceClient.GameAppServiceClient` 的交互能力。
/// </remarks>
internal sealed class ServiceConnect : IDisposable
{
    /// <summary>
    /// Initializes a new ServiceConnect instance
    /// </summary>
    /// <param name="name">The service name / 服务名称</param>
    /// <param name="serverId">The unique server id / 服务器唯一ID</param>
    /// <param name="serverInstanceId">The unique server instance id / 服务器实例唯一ID</param>
    /// <param name="host">The target host to connect / 目标连接主机</param>
    /// <param name="port">The target port to connect / 目标连接端口</param>
    /// <remarks>
    /// 构造函数用于初始化服务连接的基本信息，并创建用于RPC会话的 `RpcSession` 实例。
    /// 在调用 <see cref="GameFrameX.DiscoveryCenterManager.Service.ServiceConnect.InitAsync"/> 前，连接不会建立。
    /// </remarks>
    public ServiceConnect(string name, long serverId, long serverInstanceId, string host, int port)
    {
        Name = name;
        ServerId = serverId;
        ServerInstanceId = serverInstanceId;
        Host = host;
        Port = port;
        RpcSession = new RpcSession();
    }


    /// <summary>
    /// Indicates whether the service connection has been initialized
    /// </summary>
    /// <value>True when initialization has occurred; otherwise false / 当初始化完成返回true，否则为false</value>
    /// <remarks>
    /// 服务连接初始化状态；外部只读，内部可写。用于避免重复初始化。
    /// </remarks>
    public bool IsInit { get; private set; }

    private readonly TaskCompletionSource _initTaskCompletionSource = new();

    /// <summary>
    /// Initializes the service connection asynchronously
    /// </summary>
    /// <returns>A task representing the asynchronous initialization operation / 表示异步初始化操作的任务</returns>
    /// <remarks>
    /// 创建并配置 `GameAppServiceClient`，注册事件回调并尝试与发现中心建立连接。
    /// 当已初始化时将跳过重复初始化并返回。连接失败时会记录错误并结束等待。
    /// </remarks>
    /// <seealso cref="GameAppServiceClient"/>
    /// <seealso cref="GameFrameX.Utility.NetHelper"/>
    public async Task InitAsync()
    {
        if (IsInit)
        {
            Log.Warning($"ServiceConnectManager Init {Name} already initialized");
            return;
        }

        IsInit = true;
        // 创建客户端事件回调对象
        var gameAppServiceConfiguration = new GameAppServiceConfiguration
        {
            OnConnected = GameAppServiceClientOnConnected,
            OnClosed = GameAppServiceClientOnClosed,
            OnMessage = GameAppServiceClientOnMessage,
            OnError = GameAppServiceClientOnError,
            MaxRetryCount = 30,
            IsEnableConnectDelay = true,
            ConnectDelay = 500,
            IsEnableHeartBeat = false,
        };


        var connectEndPoint = NetHelper.ParseEndPoint(Host, Port);
        if (connectEndPoint == default)
        {
            LogHelper.Error($"DiscoveryCenterHost: {Port} is not a valid IP address; unable to start connection to DiscoveryCenter. Please check the configuration item DiscoveryCenterHost");
            return;
        }

        var gameAppServiceClient = new GameAppServiceClient(connectEndPoint, gameAppServiceConfiguration);
        ServiceClient = gameAppServiceClient;
        await _initTaskCompletionSource.Task;
    }

    /// <summary>
    /// Handles client error events
    /// </summary>
    /// <param name="s">The client identifier / 客户端标识</param>
    /// <param name="errorEventArgs">The error event args / 错误事件参数</param>
    /// <remarks>
    /// 处理客户端错误事件，记录日志并结束初始化等待。
    /// </remarks>
    private void GameAppServiceClientOnError(string s, ErrorEventArgs errorEventArgs)
    {
        Log.Error($"ServiceConnectManager OnError {errorEventArgs.Exception}");
        _initTaskCompletionSource.TrySetResult();
    }

    /// <summary>
    /// Handles client message events
    /// </summary>
    /// <param name="s">The client identifier / 客户端标识</param>
    /// <param name="messageObject">The received message object / 接收的消息对象</param>
    /// <remarks>
    /// 处理客户端消息事件；当前实现为空，可根据业务需要扩展消息处理逻辑。
    /// </remarks>
    private void GameAppServiceClientOnMessage(string s, MessageObject messageObject)
    {
    }

    /// <summary>
    /// Handles client connected events
    /// </summary>
    /// <param name="s">The client identifier / 客户端标识</param>
    /// <remarks>
    /// 处理客户端连接成功事件，结束初始化等待以继续后续流程。
    /// </remarks>
    private void GameAppServiceClientOnConnected(string s)
    {
        _initTaskCompletionSource.TrySetResult();
    }

    /// <summary>
    /// Handles client closed events
    /// </summary>
    /// <param name="s">The client identifier / 客户端标识</param>
    /// <remarks>
    /// 处理客户端连接关闭事件，结束初始化等待，便于上层进行重试或清理。
    /// </remarks>
    private void GameAppServiceClientOnClosed(string s)
    {
        _initTaskCompletionSource.TrySetResult();
    }

    /// <summary>
    /// Gets the underlying service client instance
    /// </summary>
    /// <value>The `GameAppServiceClient` instance, may be null before initialization / `GameAppServiceClient` 实例，在初始化前可能为 null</value>
    /// <remarks>
    /// 外部只读（内部可写）；用于与发现中心进行网络交互。
    /// </remarks>
    /// <seealso cref="GameAppServiceClient"/>
    public GameAppServiceClient ServiceClient { get; private set; }
    /// <summary>
    /// Gets or sets the RPC session
    /// </summary>
    /// <value>The RPC session used for message exchange / 用于消息交互的RPC会话</value>
    /// <remarks>
    /// RPC会话对象，负责维护与服务的消息会话状态，可根据业务进行替换或复用。
    /// </remarks>
    public IRpcSession RpcSession { get; set; }
    /// <summary>
    /// Gets or sets the server id
    /// </summary>
    /// <value>The unique identifier of the server / 服务器的唯一标识</value>
    /// <remarks>
    /// 服务器ID，用于在发现中心中区分不同的服务器。
    /// </remarks>
    public long ServerId { get; set; }
    /// <summary>
    /// Gets or sets the server instance id
    /// </summary>
    /// <value>The unique identifier of the server instance / 服务器实例的唯一标识</value>
    /// <remarks>
    /// 服务器实例ID，用于区分同一服务器的不同运行实例。
    /// </remarks>
    public long ServerInstanceId { get; set; }
    /// <summary>
    /// Gets the service name
    /// </summary>
    /// <value>The service name / 服务名称</value>
    /// <remarks>
    /// 只读属性，用于标识该连接对应的服务名称。
    /// </remarks>
    public string Name { get; }
    /// <summary>
    /// Gets the target host
    /// </summary>
    /// <value>The host address / 主机地址</value>
    /// <remarks>
    /// 只读属性，表示目标连接的主机地址。
    /// </remarks>
    public string Host { get; }
    /// <summary>
    /// Gets the target port
    /// </summary>
    /// <value>The port number / 端口号</value>
    /// <remarks>
    /// 只读属性，表示目标连接的端口号。
    /// </remarks>
    public int Port { get; }

    /// <summary>
    /// Releases unmanaged resources and disposes the service client
    /// </summary>
    /// <remarks>
    /// 释放与服务客户端相关的资源，并将其置为 null。该方法可安全重复调用。
    /// </remarks>
    /// <seealso cref="System.IDisposable"/>
    public void Dispose()
    {
        ServiceClient?.Dispose();
        ServiceClient = null;
    }
}
