using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.ChannelBase;
using GameFrameX.NetWork.Message;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpService : AppStartUpBase
{
    /// <summary>
    /// 从发现中心请求的目标服务器类型
    /// </summary>
    protected virtual ServerType GetServerType { get; } = ServerType.None;

    /// <summary>
    /// 是否请求其他服务信息
    /// </summary>
    protected virtual bool IsRequestConnectServer { get; } = true;

    /// <summary>
    /// 是否连接到发现中心
    /// </summary>
    protected virtual bool IsConnectDiscoveryServer { get; } = true;

    protected IMessageEncoderHandler MessageEncoderHandler { get; private set; }
    protected IMessageDecoderHandler MessageDecoderHandler { get; private set; }

    protected void SetMessageHandler(IMessageEncoderHandler messageEncoderHandler, IMessageDecoderHandler messageDecoderHandler)
    {
        messageDecoderHandler.CheckNotNull(nameof(messageDecoderHandler));
        messageEncoderHandler.CheckNotNull(nameof(messageEncoderHandler));
        MessageEncoderHandler = messageEncoderHandler;
        MessageDecoderHandler = messageDecoderHandler;
    }

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public override Task StartAsync()
    {
        if (IsRequestConnectServer)
        {
            // connectTargetServerChannelHelper.Start();
        }

        if (IsConnectDiscoveryServer)
        {
            _discoveryCenterChannelHelper?.Start(Setting.DiscoveryCenterIp, Setting.DiscoveryCenterPort);
        }

        return Task.CompletedTask;
    }

    /*
    private void DiscoveryCenterClientOnDataReceived(object o, DataEventArgs dataEventArgs)
    {
        var messageData = dataEventArgs.Data.ReadBytes(dataEventArgs.Offset, dataEventArgs.Length);
        var message = MessageDecoderHandler.Handler(messageData);
        if (message == null)
        {
            LogHelper.Error("数据解析失败！");
            return;
        }

        if (Setting.IsDebug && Setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
        {
            LogHelper.Debug(message.ToMessageString());
        }

        if (message is IResponseMessage actorResponseMessage)
        {
            RpcSession.Reply(actorResponseMessage);
        }

        if (message is RespConnectServer respConnectServer && ConnectTargetServer == null)
        {
            ConnectTargetServer = respConnectServer;
            ConnectTargetServerTimer?.Stop();
            ConnectServerHandler();
            return;
        }

        if (message is RespServerOfflineServer respServerOfflineServer)
        {
            if (respServerOfflineServer.ServerType == ConnectTargetServer?.ServerType && respServerOfflineServer.ServerID == ConnectTargetServer?.ServerID)
            {
                ConnectTargetServer = null;
                ConnectTargetServerTimer?.Start();
                DisconnectServerHandler();
            }

            return;
        }


        DiscoveryCenterDataReceived(message);
    }
*/

    /// <summary>
    /// 终止服务器
    /// </summary>
    /// <param name="message">终止原因</param>
    /// <returns></returns>
    public override Task StopAsync(string message = "")
    {
        // connectTargetServerChannelHelper?.Stop();
        _discoveryCenterChannelHelper?.Stop();
        StopWebSocketServer();
        _tcpService?.StopAsync();
        return base.StopAsync(message);
    }

    #region Server

    private IServer _tcpService;
    private ConnectChannelHelper _discoveryCenterChannelHelper;
    protected ConnectChannelHelper DiscoveryCenterChannelHelper
    {
        get { return _discoveryCenterChannelHelper; }
    }
    // private ConnectChannelHelper connectTargetServerChannelHelper;

    protected async void StartServer()
    {
        await StartTcpServer();
        StartWebSocketServer();
        // _discoveryCenterChannelHelper = new ConnectChannelHelper(Setting, MessageEncoderHandler, MessageDecoderHandler, DiscoveryCenterMessageHandler);
        // connectTargetServerChannelHelper = new ConnectChannelHelper(Setting,MessageEncoderHandler, MessageDecoderHandler, GetServerType, Setting);
        GlobalSettings.IsAppRunning = true;
    }

    private void DiscoveryCenterMessageHandler(IMessage message)
    {
        if (message is InnerNetworkMessage innerNetworkMessage)
        {
            switch (innerNetworkMessage.Header.OperationType)
            {
                case MessageOperationType.None:
                    break;
                case MessageOperationType.HeartBeat:
                    return;
                case MessageOperationType.Cache:
                    break;
                case MessageOperationType.Database:
                    break;
                case MessageOperationType.Game:
                    break;
                case MessageOperationType.GameManager:
                    break;
                case MessageOperationType.Forbid:
                    break;
                case MessageOperationType.Reboot:
                    break;
                case MessageOperationType.Reconnect:
                    break;
                case MessageOperationType.Reload:
                    break;
                case MessageOperationType.Exit:
                    break;
                case MessageOperationType.Kick:
                    break;
                case MessageOperationType.Notify:
                    break;
                case MessageOperationType.Forward:
                    break;
                case MessageOperationType.Register:
                    break;
                case MessageOperationType.RequestConnectServer:
                    break;
            }
        }

        LogHelper.Info(message.ToFormatMessageString());
    }

    /// <summary>
    /// 启动TCP服务器
    /// </summary>
    private async Task StartTcpServer()
    {
        if (Setting.InnerPort > 0)
        {
            LogHelper.InfoConsole($"启动服务器{ServerType} 开始! address: {Setting.InnerIp}  port: {Setting.InnerPort}");
            var hostBuilder = SuperSocketHostBuilder
                              .Create<IMessage, MessageObjectPipelineFilter>()
                              .ConfigureSuperSocket(ConfigureSuperSocket)
                              .UseClearIdleSession()
                              .UsePackageDecoder<DefaultMessageDecoderHandler>()
                              .UsePackageEncoder<DefaultMessageEncoderHandler>()
                              .UseSessionHandler(OnConnected, OnDisconnected)
                              .UsePackageHandler(PackageHandler, PackageErrorHandler)
                              .UseInProcSessionContainer()
                ;

            hostBuilder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog(Serilog.Log.Logger, true);
            });
            _tcpService = hostBuilder.BuildAsServer();
            var messageEncoderHandler = (DefaultMessageEncoderHandler)_tcpService.ServiceProvider.GetService<IPackageEncoder<IMessage>>();
            var messageDecoderHandler = (DefaultMessageDecoderHandler)_tcpService.ServiceProvider.GetService<IPackageDecoder<IMessage>>();

            SetMessageHandler(messageEncoderHandler, messageDecoderHandler);

            await _tcpService.StartAsync();

            LogHelper.InfoConsole($"启动服务器 {ServerType} 端口: {Setting.InnerPort} 结束!");
        }
        else
        {
            LogHelper.Error("启动服务器失败，内网端口不能小于0,检查端口值是否正确");
        }
    }

    #region WebSocket

    /// <summary>
    /// WS服务器
    /// </summary>
    private IHost _webSocketServer;

    /// <summary>
    /// 启动WebSocket
    /// </summary>
    private async void StartWebSocketServer()
    {
        if (Setting.WsPort > 0)
        {
            LogHelper.Info("启动 WebSocket 服务器开始...");
            _webSocketServer = WebSocketHostBuilder.Create()
                                                   .UseWebSocketMessageHandler(WebSocketMessageHandler)
                                                   .UseSessionHandler(OnConnected, OnDisconnected).ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
            await _webSocketServer.StartAsync();
            LogHelper.Info("启动 WebSocket 服务器完成...");
        }
    }

    protected async void StopWebSocketServer()
    {
        // 关闭WS网络服务
        if (_webSocketServer != null)
        {
            await _webSocketServer.StopAsync();
        }
    }

    private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
    {
        builder.AddInMemoryCollection(new Dictionary<string, string>()
                                          { { "serverOptions:name", "TestServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
    }

    /// <summary>
    /// 处理收到的WS消息
    /// </summary>
    /// <param name="session"></param>
    /// <param name="messagePackage"></param>
    private async ValueTask WebSocketMessageHandler(WebSocketSession session, WebSocketPackage messagePackage)
    {
        if (messagePackage.OpCode != OpCode.Binary)
        {
            await session.CloseAsync(CloseReason.ProtocolError);
            return;
        }

        var readOnlySequence = messagePackage.Data;
        var message = MessageDecoderHandler.Handler(ref readOnlySequence);
        await PackageHandler(session, message);
    }

    #endregion

    protected virtual ValueTask<bool> PackageErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> exception)
    {
        return ValueTask.FromResult(true);
    }

    protected virtual ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        return ValueTask.CompletedTask;
    }

    protected virtual ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (Setting.IsDebug && Setting.IsDebugReceive)
        {
            LogHelper.Debug($"---收到外部发给[{ServerType}]的消息  {message.ToFormatMessageString()}");
        }

        return ValueTask.CompletedTask;
    }

    #endregion
}