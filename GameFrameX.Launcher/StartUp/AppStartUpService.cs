using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.ChannelBase;

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

        GlobalSettings.IsAppRunning = true;
        return Task.CompletedTask;
    }

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

    protected async void StartServer()
    {
        _discoveryCenterChannelHelper = new ConnectChannelHelper(Setting, MessageEncoderHandler, MessageDecoderHandler, DiscoveryCenterMessageHandler);
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

    #endregion
}