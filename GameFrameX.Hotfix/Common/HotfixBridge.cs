using GameFrameX.Launcher.Common.Session;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Common
{
    internal class HotfixBridge : IHotfixBridge
    {
        public ServerType BridgeType => ServerType.Game;

        public async Task<bool> OnLoadSuccess(BaseSetting setting, bool reload)
        {
            if (reload)
            {
                ActorManager.ClearAgent();
                return true;
            }

            LogHelper.Info("load config data");
            // var webSocketMessageHelper = new WebSocketMessageHelper(HotfixMgr.GetTcpHandler, HotfixMgr.GetMsgTypeById, HotfixMgr.GetMsgIdByType);
            // await WebSocketServer.Start(setting.WsPort, setting.WssPort, setting.WssCertFilePath, setting, webSocketMessageHelper, new WebSocketChannelHandler());
            LogHelper.Info("WebSocket 服务启动完成...");

            // var tcpSocketMessageHelper = new TcpSocketMessageHelper(HotfixMgr.GetTcpHandler, HotfixMgr.GetMsgTypeById, HotfixMgr.GetMsgIdByType);
            // await TcpServer.Start(appSetting.TcpPort, appSetting, tcpSocketMessageHelper, builder => { builder.UseConnectionHandler<AppTcpConnectionHandler>(); });
            LogHelper.Info("tcp 服务启动完成...");
            await HttpServer.Start(setting.HttpPort, setting.HttpsPort, HotfixManager.GetHttpHandler);
            LogHelper.Info("load config data");

            GlobalTimer.Start();
            await ComponentRegister.ActiveGlobalComps();
            return true;
        }

        public async Task Stop()
        {
            // 断开所有连接
            await SessionManager.RemoveAll();
            // 取消所有未执行定时器
            await QuartzTimer.Stop();
            // 保证actor之前的任务都执行完毕
            await ActorManager.AllFinish();
            // 关闭网络服务
            // await TcpServer.Stop();
            // await WebSocketServer.Stop();
            await HttpServer.Stop();
            // 存储所有数据
            await GlobalTimer.Stop();
            await ActorManager.RemoveAll();
        }
    }
}