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
        return base.StopAsync(message);
    }

    #region Server

    private ConnectChannelHelper _discoveryCenterChannelHelper;

    protected ConnectChannelHelper DiscoveryCenterChannelHelper
    {
        get { return _discoveryCenterChannelHelper; }
    }
    // private ConnectChannelHelper connectTargetServerChannelHelper;

    protected void StartServer()
    {
        // _discoveryCenterChannelHelper = new ConnectChannelHelper(Setting, MessageEncoderHandler, MessageDecoderHandler, DiscoveryCenterMessageHandler);
        // connectTargetServerChannelHelper = new ConnectChannelHelper(Setting,MessageEncoderHandler, MessageDecoderHandler, GetServerType, Setting);
        GlobalSettings.IsAppRunning = true;
    }

    #endregion
}