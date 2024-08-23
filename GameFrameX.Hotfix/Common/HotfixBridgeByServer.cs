using System.Buffers;
using System.Net;
using GameFrameX.Launcher;
using GameFrameX.Launcher.Common.Session;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Connection;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using GameFrameX.SuperSocket.Server;
using GameFrameX.SuperSocket.Server.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.SuperSocket.Server.Host;
using GameFrameX.SuperSocket.WebSocket;
using GameFrameX.SuperSocket.WebSocket.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using CloseReason = GameFrameX.SuperSocket.Connection.CloseReason;

namespace GameFrameX.Hotfix.Common
{
    internal partial class HotfixBridge : IHotfixBridge
    {
        public async void RunServer(bool reload)
        {
            if (reload)
            {
                ActorManager.ClearAgent();
                return;
            }

            StartServer();
            await HttpServer.Start(Setting.HttpPort, Setting.HttpsPort, HotfixManager.GetHttpHandler);
        }

        /// <summary>
        /// 服务器。对外提供服务
        /// </summary>
        private IServer _tcpService;

        /// <summary>
        /// WS服务器
        /// </summary>
        private IHost _webSocketServer;

        private void StartServer()
        {
            messageEncoderHandler.SetCompressionHandler(new BaseMessageCompressHandler());
            messageDecoderHandler.SetDecompressionHandler(new BaseMessageDecompressHandler());

            // 启动网络服务
            StartTcpServer();
            StartWebSocketServer();
        }

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
                                                       .UseSessionHandler(OnConnected, OnDisconnected)
                                                       .ConfigureAppConfiguration((Action<HostBuilderContext, IConfigurationBuilder>)(ConfigureWebServer)).Build();
                await _webSocketServer.StartAsync();
                LogHelper.Info("启动 WebSocket 服务器完成...");
            }
        }

        /// <summary>
        /// 启动TCP
        /// </summary>
        private async void StartTcpServer()
        {
            if (Setting.InnerPort > 0)
            {
                LogHelper.Info("启动 TCP 服务器开始...");
                _tcpService = SuperSocketHostBuilder.Create<INetworkMessage, MessageObjectPipelineFilter>()
                                                    .ConfigureSuperSocket(ConfigureSuperSocket)
                                                    .UseClearIdleSession()
                                                    .UsePackageDecoder<BaseMessageDecoderHandler>()
                                                    .UseSessionHandler(OnConnected, OnDisconnected)
                                                    .UsePackageHandler(MessagePackageHandler, ClientErrorHandler)
                                                    .UseInProcSessionContainer()
                                                    .BuildAsServer();

                // 设置消息解压缩的处理器
                var baseMessageDecoderHandler = (BaseMessageDecoderHandler)_tcpService.ServiceProvider.GetService(typeof(IPackageDecoder<INetworkMessage>));
                baseMessageDecoderHandler?.SetDecompressionHandler(new BaseMessageDecompressHandler());
                await _tcpService.StartAsync();
                LogHelper.Info("启动 TCP 服务器完成...");
            }
        }

        /// <summary>
        /// 配置启动
        /// </summary>
        /// <param name="options"></param>
        protected virtual void ConfigureSuperSocket(ServerOptions options)
        {
            options.AddListener(new ListenOptions { Ip = IPAddress.Any.ToString(), Port = Setting.InnerPort });
        }

        private ValueTask<bool> ClientErrorHandler(IAppSession appSession, PackageHandlingException<INetworkMessage> arg2)
        {
            LogHelper.Error(arg2.ToString());
            return ValueTask.FromResult(true);
        }

        private ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
        {
            LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
            SessionManager.Remove(appSession.SessionID);
            return ValueTask.CompletedTask;
        }

        private ValueTask OnConnected(IAppSession appSession)
        {
            LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
            var netChannel = new DefaultNetWorkChannel(appSession, Setting, messageEncoderHandler, null, appSession is WebSocketSession);
            var session = new Session(appSession.SessionID, netChannel);
            SessionManager.Add(session);

            return ValueTask.CompletedTask;
        }

        private static readonly BaseMessageEncoderHandler messageEncoderHandler = new BaseMessageEncoderHandler();
        private static readonly BaseMessageDecoderHandler messageDecoderHandler = new BaseMessageDecoderHandler();

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

            var readOnlySequence = message.Data;
            var messageObject = messageDecoderHandler.Handler(ref readOnlySequence);
            await MessagePackageHandler(session, messageObject);
        }

        /// <summary>
        /// 处理收到的消息结果
        /// </summary>
        /// <param name="appSession"></param>
        /// <param name="messageObject"></param>
        private async ValueTask MessagePackageHandler(IAppSession appSession, INetworkMessage messageObject)
        {
            if (messageObject is MessageObject message)
            {
                if (Setting.IsDebug && Setting.IsDebugReceive)
                {
                    LogHelper.Debug($"---收到{messageObject.ToFormatMessageString()}");
                }

                var handler = HotfixManager.GetTcpHandler(message.MessageId);
                if (handler == null)
                {
                    LogHelper.Error($"找不到[{message.MessageId}][{messageObject.GetType()}]对应的handler");
                    return;
                }

                handler.Message = message;
                handler.NetWorkChannel = SessionManager.GetChannel(appSession.SessionID);
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
            if (_webSocketServer != null)
            {
                await _webSocketServer.StopAsync();
            }

            if (_tcpService != null)
            {
                await _tcpService.StopAsync();
            }
        }
    }
}