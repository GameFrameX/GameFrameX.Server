using System.Buffers;
using System.Net;
using GameFrameX.Hotfix.Launcher;
using GameFrameX.Launcher;
using GameFrameX.Launcher.Common.Session;
using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.Launcher.StartUp.Router;
using GameFrameX.NetWork;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Messages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using SuperSocket;
using SuperSocket.Connection;
using SuperSocket.Server;
using SuperSocket.Server.Abstractions;
using SuperSocket.Server.Abstractions.Session;
using SuperSocket.Server.Host;
using SuperSocket.WebSocket;
using SuperSocket.WebSocket.Server;
using CloseReason = SuperSocket.Connection.CloseReason;

namespace GameFrameX.Hotfix.Common
{
    internal class HotfixBridge : IHotfixBridge
    {
        public ServerType BridgeType => ServerType.Game;
        private BaseSetting Setting;
        AppStartUpGame appStartUpGame;
        public async Task<bool> OnLoadSuccess(BaseSetting setting, bool reload)
        {
            Setting = setting;
            if (reload)
            {
                ActorManager.ClearAgent();
                return true;
            }

            LogHelper.Info("load config data");
            await StartServer();
            await HttpServer.Start(setting.HttpPort, setting.HttpsPort, HotfixManager.GetHttpHandler);
            LogHelper.Info("启动 HTTP 服务器完成...");

            appStartUpGame = new AppStartUpGame();
            appStartUpGame.Init(setting);
            appStartUpGame.Start();
            GlobalTimer.Start();
            await ComponentRegister.ActiveGlobalComps();
            return true;
        }

        /// <summary>
        /// 服务器。对外提供服务
        /// </summary>
        private IServer tcpService;

        /// <summary>
        /// WS服务器
        /// </summary>
        private IHost webSocketServer;

        private async Task StartServer()
        {
            webSocketServer = WebSocketHostBuilder.Create()
                .UseWebSocketMessageHandler(WebSocketMessageHandler)
                .UseSessionHandler(OnConnected, OnDisconnected)
                .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
            await webSocketServer.StartAsync();
            LogHelper.Info("启动 WebSocket 服务器完成...");
            tcpService = SuperSocketHostBuilder.Create<IMessage, MessageObjectPipelineFilter>()
                .ConfigureSuperSocket(ConfigureSuperSocket)
                .UseClearIdleSession()
                .UsePackageDecoder<MessageGameDecoderHandler>()
                .UseSessionHandler(OnConnected, OnDisconnected)
                .UsePackageHandler(MessagePackageHandler, ClientErrorHandler)
                .UseInProcSessionContainer()
                .BuildAsServer();

            await tcpService.StartAsync();
            LogHelper.Info("启动 TCP 服务器完成...");
        }

        /// <summary>
        /// 配置启动
        /// </summary>
        /// <param name="options"></param>
        protected virtual void ConfigureSuperSocket(ServerOptions options)
        {
            options.AddListener(new ListenOptions { Ip = IPAddress.Any.ToString(), Port = Setting.InnerPort });
        }

        private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<IMessage> arg2)
        {
            LogHelper.Error(arg2.ToString());
            return ValueTask.FromResult(true);
        }

        private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
        {
            LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
            GameClientSessionManager.RemoveSession(appSession.SessionID); //移除
            return ValueTask.CompletedTask;
        }

        private ValueTask OnConnected(IAppSession appSession)
        {
            LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
            var netChannel = new DefaultNetWorkChannel(appSession, messageEncoderHandler, null, appSession is WebSocketSession);
            GameClientSessionManager.SetSession(appSession.SessionID, netChannel); //移除

            return ValueTask.CompletedTask;
        }

        private static readonly MessageGameEncoderHandler messageEncoderHandler = new MessageGameEncoderHandler();
        private static readonly MessageGameDecoderHandler messageDecoderHandler = new MessageGameDecoderHandler();

        /// <summary>
        /// 处理收到的WS消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        private async ValueTask WebSocketMessageHandler(WebSocketSession session, WebSocketPackage message)
        {
            if (message.OpCode != OpCode.Binary)
            {
                await session.CloseAsync(CloseReason.ProtocolError);
                return;
            }

            var bytes = message.Data;
            var buffer = bytes.ToArray();
            var messageObject = messageDecoderHandler.Handler(buffer);
            await MessagePackageHandler(session, messageObject);
        }

        /// <summary>
        /// 处理收到的消息结果
        /// </summary>
        /// <param name="appSession"></param>
        /// <param name="messageObject"></param>
        private async ValueTask MessagePackageHandler(IAppSession appSession, IMessage messageObject)
        {
            if (messageObject is MessageObject message)
            {
                var messageId = message.MessageId;
                if (Setting.IsDebug && Setting.IsDebugReceive)
                {
                    LogHelper.Debug($"---收到消息:[{messageId},{message.GetType().Name}] 消息内容:[{messageObject}]");
                }

                var handler = HotfixManager.GetTcpHandler(message.MessageId);
                if (handler == null)
                {
                    LogHelper.Error($"找不到[{message.MessageId}][{messageObject.GetType()}]对应的handler");
                    return;
                }

                handler.Message = message;
                handler.NetWorkChannel = GameClientSessionManager.GetSession(appSession.SessionID);
                await handler.Init();
                await handler.InnerAction();
            }
        }

        private void ConfigureWebServer(HostBuilderContext context, IConfigurationBuilder builder)
        {
            builder.AddInMemoryCollection(new Dictionary<string, string>()
                { { "serverOptions:name", "GameServer" }, { "serverOptions:listeners:0:ip", "Any" }, { "serverOptions:listeners:0:port", Setting.WsPort.ToString() } });
        }

        public async Task Stop()
        {
            // 断开所有连接
            await SessionManager.RemoveAll();
            // 取消所有未执行定时器
            await QuartzTimer.Stop();
            // 保证actor之前的任务都执行完毕
            await ActorManager.AllFinish();
            appStartUpGame?.Stop();
            // 关闭网络服务
            await webSocketServer.StopAsync();
            await tcpService.StopAsync();
            await HttpServer.Stop();
            // 存储所有数据
            await GlobalTimer.Stop();
            await ActorManager.RemoveAll();
        }
    }
}