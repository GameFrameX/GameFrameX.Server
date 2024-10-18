using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Timer = System.Timers.Timer;

namespace GameFrameX.Launcher.StartUp;

public abstract class AppStartUpService : AppStartUpBase
{
    /// <summary>
    /// 链接到发现中心的客户端
    /// </summary>
    // AsyncTcpSession discoveryCenterClient;


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

    /// <summary>
    /// 连接的目标信息
    /// </summary>
    protected RespConnectServer ConnectTargetServer { get; private set; }

    protected IMessageEncoderHandler MessageEncoderHandler { get; private set; }
    protected IMessageDecoderHandler MessageDecoderHandler { get; private set; }

    protected void SetMessageHandler(IMessageEncoderHandler messageEncoderHandler, IMessageDecoderHandler messageDecoderHandler)
    {
        messageDecoderHandler.CheckNotNull(nameof(messageDecoderHandler));
        messageEncoderHandler.CheckNotNull(nameof(messageEncoderHandler));
        MessageEncoderHandler = messageEncoderHandler;
        MessageDecoderHandler = messageDecoderHandler;
    }


    private Timer ConnectTargetServerTimer { get; set; }

    /// <summary>
    /// 启动服务器
    /// </summary>
    /// <returns></returns>
    public override Task StartAsync()
    {
        if (IsRequestConnectServer)
        {
            ConnectTargetServerTimer = new Timer
            {
                Interval = 3000
            };
            ConnectTargetServerTimer.Elapsed += ConnectTargetServerTimerOnElapsed;
            ConnectTargetServerTimer.Start();
        }

        if (IsConnectDiscoveryServer)
        {
            _discoveryCenterChannelHelper?.Start();
        }

        return Task.CompletedTask;
    }


    /// <summary>
    /// 请求链接目标
    /// </summary>
    void SendConnectTargetServer()
    {
        if (!_discoveryCenterChannelHelper.IsConnected)
        {
            return;
        }

        if (ConnectTargetServer == null)
        {
            ReqConnectServer reqConnectServer = new ReqConnectServer
            {
                ServerType = GetServerType
            };

            // _discoveryCenterChannelHelper.Send(reqConnectServer);
        }
    }

    /// <summary>
    /// 链接目标重试
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void ConnectTargetServerTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        if (!IsRequestConnectServer)
        {
            return;
        }

        SendConnectTargetServer();
    }


    /// <summary>
    /// 获取到连接的目标
    /// </summary>
    protected virtual void ConnectServerHandler()
    {
    }

    /// <summary>
    /// 连接的目标下线
    /// </summary>
    protected virtual void DisconnectServerHandler()
    {
    }

    /// <summary>
    /// 收到发现中心推送的非特殊消息
    /// </summary>
    /// <param name="message"></param>
    protected virtual void DiscoveryCenterDataReceived(INetworkMessage message)
    {
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

    private void DiscoveryCenterClientOnClosed(object sender, EventArgs eventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
    }*/

    /// <summary>
    /// 终止服务器
    /// </summary>
    /// <param name="message">终止原因</param>
    /// <returns></returns>
    public override Task StopAsync(string message = "")
    {
        ConnectTargetServerTimer?.Close();
        _discoveryCenterChannelHelper?.Stop();
        _tcpService?.StopAsync();
        return base.StopAsync(message);
    }

    #region Server

    private IServer _tcpService;
    private DiscoveryCenterChannelHelper _discoveryCenterChannelHelper;

    protected async Task StartServer()
    {
        await StartTcpServer();
        _discoveryCenterChannelHelper = new DiscoveryCenterChannelHelper(Setting, MessageEncoderHandler, MessageDecoderHandler);
        GlobalSettings.IsAppRunning = true;
    }

    protected async Task StartTcpServer()
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