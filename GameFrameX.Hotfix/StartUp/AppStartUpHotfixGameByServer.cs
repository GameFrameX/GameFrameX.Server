using System.Buffers;
using System.Net;
using GameFrameX.Hotfix.Launcher;
using GameFrameX.Hotfix.StartUp;
using GameFrameX.Launcher;
using GameFrameX.Launcher.Common.Session;
using GameFrameX.Launcher.PipelineFilter;
using GameFrameX.NetWork;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CloseReason = GameFrameX.SuperSocket.Connection.CloseReason;

namespace GameFrameX.Hotfix.StartUp
{
    internal partial class AppStartUpHotfixGame
    {
        private async void RunServer(bool reload)
        {
            if (reload)
            {
                ActorManager.ClearAgent();
                return;
            }

            LogHelper.Info("load config data");
            await StartServer();
            await HttpServer.Start(Setting.HttpPort, Setting.HttpsPort, HotfixManager.GetHttpHandler);
            LogHelper.Info("启动 HTTP 服务器完成...");
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
                    LogHelper.Debug($"---收到消息:[{messageId},{messageObject.UniqueId},{message.GetType().Name}] 消息内容:[{messageObject}]");
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

        public async Task StopServer()
        {
            // 关闭网络服务
            await webSocketServer.StopAsync();
            await tcpService.StopAsync();
        }
    }
}