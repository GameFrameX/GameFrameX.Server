using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Utility;
using TouchSocket.Core;
using TouchSocket.Http;
using TouchSocket.Sockets;
using ResultCode = TouchSocket.Core.ResultCode;


/// <summary>
/// 路由服务器.最后启动。
/// </summary>
[StartUpTag(ServerType.Router, int.MaxValue)]
internal sealed class AppStartUpRouter : AppStartUpBase
{
    /// <summary>
    /// 链接到网关的客户端
    /// </summary>
    private TcpClient tcpClient;

    /// <summary>
    /// 服务器。对外提供服务
    /// </summary>
    private TcpService tcpService;

    /// <summary>
    /// WS服务器
    /// </summary>
    private HttpService webSocketServer;

    public override async Task EnterAsync()
    {
        try
        {
            await StartServer();
            LogHelper.Info($"启动服务器 {ServerType} 端口: {Setting.TcpPort} 结束!");
            StartClient();
            await AppExitToken;
            LogHelper.Info("全部断开...");
            await Stop();
            LogHelper.Info("Done!");
        }
        catch (Exception e)
        {
            await Stop(e.Message);
        }
    }

    /// <summary>
    /// 重连定时器
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected override void ReconnectionTimerOnElapsed(object sender, ElapsedEventArgs e)
    {
        // 重连到网关服务器
        ConnectToGateWay();
    }

    private void ConnectToGateWay()
    {
        var result = tcpClient.TryConnect();
        if (result.ResultCode != ResultCode.Success)
        {
            // 开始重连
            ReconnectionTimer.Start();
            LogHelper.Info(result.ToString());
            return;
        }

        LogHelper.Info("链接到网关服务器成功!");
        ReconnectionTimer.Stop();
        HeartBeatTimer.Start();
    }

    private void StartClient()
    {
        tcpClient = new TcpClient();
        tcpClient.Connected = ClientOnConnected; //成功连接到服务器
        tcpClient.Disconnected = ClientOnClosed; //从服务器断开连接，当连接不成功时不会触发。
        tcpClient.Received = (client, e) =>
        {
            //从服务器收到信息。但是一般byteBlock和requestInfo会根据适配器呈现不同的值。
            // var mes = Encoding.UTF8.GetString(e.ByteBlock.Buffer, 0, e.ByteBlock.Len);
            // tcpClient.Logger.Info($"客户端接收到信息：{mes}");
            return EasyTask.CompletedTask;
        };

        // var dnsEndPoint = new DnsEndPoint(Setting.CenterUrl, Setting.GrpcPort);
        IPHost ipHost = new IPHost(IPAddress.Parse(Setting.CenterUrl), Setting.GrpcPort);
        var clientConfig = new TouchSocketConfig()
            .SetRemoteIPHost(ipHost)
            .ConfigureContainer(a => a.AddConsoleLogger());
        //载入配置
        tcpClient.Setup(clientConfig);
        LogHelper.Info("开始链接到网关服务器 ...");
        ConnectToGateWay();
    }

    private Task ClientOnClosed(ITcpClientBase client, DisconnectEventArgs disconnectEventArgs)
    {
        LogHelper.Info("和网关服务器链接链接断开!");
        // 和网关服务器链接断开，开启重连
        ReconnectionTimer.Start();
        return EasyTask.CompletedTask;
    }

    private Task ClientOnConnected(ITcpClient client, ConnectedEventArgs connectedEventArgs)
    {
        LogHelper.Info("和网关服务器链接链接成功!");
        // 和网关服务器链接成功，关闭重连
        ReconnectionTimer.Stop();
        // 开启和网关服务器的心跳
        HeartBeatTimer.Start();
        return EasyTask.CompletedTask;
    }


    private async Task StartServer()
    {
        webSocketServer = new HttpService();
        var webSocketServerConfig = new TouchSocketConfig()
            .SetListenIPHosts(Setting.WsPort)
            .ConfigureContainer(a => a.AddConsoleLogger())
            .ConfigurePlugins(m =>
            {
                m.UseWebSocket();
                m.Add<RouterWebSocketMessagePlugin>().Map();
            });
        await webSocketServer.SetupAsync(webSocketServerConfig);
        await webSocketServer.StartAsync();

        tcpService = new TcpService();

        var tcpServiceConfig = new TouchSocketConfig()
            .SetListenIPHosts(Setting.TcpPort)
            .ConfigureContainer(a => a.AddConsoleLogger())
            .ConfigurePlugins(m => { m.Add<RouterSocketMessagePlugin>(); });

        await tcpService.SetupAsync(tcpServiceConfig);
        await tcpService.StartAsync(); //启动
    }

    public override async Task Stop(string message = "")
    {
        HeartBeatTimer.Close();
        ReconnectionTimer.Close();
        tcpClient.Close();
        await webSocketServer.StopAsync();
        await tcpService.StopAsync();
        await base.Stop(message);
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 1000,
                ServerType = ServerType.Router,
                TcpPort = 21000,
                WsPort = 21100,
                WssPort = 21200,
                // 网关配置
                GrpcPort = 22000,
                CenterUrl = "127.0.0.1",
            };
            if (PlatformRuntimeHelper.IsLinux)
            {
                Setting.CenterUrl = "gateway";
            }
        }

        base.Init();
    }
}