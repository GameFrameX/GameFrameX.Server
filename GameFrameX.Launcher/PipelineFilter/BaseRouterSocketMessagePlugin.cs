/*using GameFrameX.Launcher.Extension.DependencyProperty;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace GameFrameX.Launcher.PipelineFilter;

public class BaseRouterSocketMessagePlugin : PluginBase, ITcpConnectedPlugin<ISocketClient>, ITcpDisconnectedPlugin<ISocketClient>
{
    static readonly MessageRouterEncoderHandler messageEncoderHandler = new MessageRouterEncoderHandler();
    private readonly MessageRouterDecoderHandler messageRouterDecoderHandler = new MessageRouterDecoderHandler();

    protected async Task<bool> MessageHandler(string socketClientIp, byte[] buffer)
    {
        var message = (MessageObject)messageRouterDecoderHandler.Handler(buffer);
        if (message != null)
        {
            LogHelper.Debug($"---收到消息ID:[{message}] ==>消息类型:{message.GetType()} 消息内容:{message}");
            var handler = HotfixMgr.GetTcpHandler(message.MessageId);
            if (handler == null)
            {
                LogHelper.Error($"找不到[{message.MessageId}][{message.GetType()}]对应的handler");
                return false;
            }

            handler.Message = message;
            handler.Channel = GameClientSessionManager.GetSession(socketClientIp); // (INetChannel)socketClient.GetPropertyValue(nameof(INetChannel));
            await handler.Init();
            await handler.InnerAction();
            RespHeartBeat resp = new RespHeartBeat
            {
                Timestamp = TimeHelper.UnixTimeSeconds()
            };
            await handler.Channel.WriteAsync(resp);
            return true;
        }

        return false;
    }

    public Task OnTcpConnected(ISocketClient socketClient, ConnectedEventArgs e)
    {
        LogHelper.Info("有外部客户端网络连接成功！。链接信息：SessionID:" + socketClient.Id + " RemoteEndPoint:" + socketClient.Id);
        var gameSession = new GameSession(socketClient.IP, socketClient);
        socketClient.SetGameSession(gameSession);
        var netChannel = new DefaultNetChannel(gameSession, messageEncoderHandler);
        GameClientSessionManager.SetSession(socketClient.IP, netChannel); //移除
        return Task.CompletedTask;
    }

    public Task OnTcpDisconnected(ISocketClient socketClient, DisconnectEventArgs e)
    {
        LogHelper.Info("有外部客户端网络断开连接成功！。断开信息：" + socketClient.Id + "  " + e.Message);
        GameClientSessionManager.RemoveSession(socketClient.IP); //移除
        return Task.CompletedTask;
    }
}*/