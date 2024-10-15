using GameFrameX.Apps.Common.Session;
using GameFrameX.Launcher.StartUp.Discovery;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.ServerManager;
using GameFrameX.SuperSocket.Primitives;
using GameFrameX.SuperSocket.ProtoBase;
using Microsoft.Extensions.DependencyInjection;


/// <summary>
/// 网关服务器
/// </summary>
[StartUpTag(ServerType.Gateway)]
internal sealed partial class AppStartUpGateway : AppStartUpService
{
    /// <summary>
    /// 是否请求其他服务信息
    /// </summary>
    protected override bool IsRequestConnectServer { get; } = false;

    public override async Task StartAsync()
    {
        try
        {
            await StartServer();
            // _namingServiceManager.OnServerAdd = OnServerAdd;
            // _namingServiceManager.OnServerRemove = OnServerRemove;
            // _namingServiceManager.AddSelf(Setting);
            await base.StartAsync();
            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Fatal(e);
            AppExitSource.TrySetException(e);
            await StopAsync(e.Message);
        }
    }


    #region Server

    protected override ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs disconnectEventArgs)
    {
        LogHelper.Info("有客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        // if (_namingServiceManager.TrySessionRemove(appSession.SessionID))
        {
            LogHelper.Info("有游戏业务客户端网络断开连接成功！。断开信息：" + appSession.SessionID + "  " + disconnectEventArgs.Reason);
        }

        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有客户端网络连接成功！。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        var netChannel = new DefaultNetWorkChannel(appSession, Setting, MessageEncoderHandler, null);
        var session = new Session(appSession.SessionID, netChannel);
        SessionManager.Add(session);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask PackageHandler(IAppSession session, INetworkMessage message)
    {
        if (message is MessageObject messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive && !MessageProtoHelper.IsHeartbeat(message.GetType()))
            {
                LogHelper.Debug($"---收到[{ServerType}] {messageObject.ToMessageString()}");
            }

            if (message is ReqHeartBeat reqActorHeartBeat)
            {
                var respActorHeartBeat = new NotifyHeartBeat()
                {
                    UniqueId = reqActorHeartBeat.UniqueId,
                    Timestamp = TimeHelper.UnixTimeSeconds()
                };
                SendMessage(session, respActorHeartBeat);
                return ValueTask.CompletedTask;
            }

            if (message is NotifyHeartBeat discoveryCenterRespActorHeartBeat)
            {
                // 发现中心的心跳返回
                return ValueTask.CompletedTask;
            }

            if (message is ReqRegisterGameServer reqRegisterGameServer)
            {
                GameServiceInfo gameServiceInfo = new GameServiceInfo(reqRegisterGameServer.ServerType,
                                                                      session,
                                                                      session.SessionID,
                                                                      reqRegisterGameServer.ServerName,
                                                                      reqRegisterGameServer.ServerID,
                                                                      reqRegisterGameServer.MinModuleMessageID,
                                                                      reqRegisterGameServer.MaxModuleMessageID
                );

                // _namingServiceManager.Add(gameServiceInfo);
                return ValueTask.CompletedTask;
            }

            var mainId = MessageIdUtility.GetMainId(messageObject.MessageId);
            // var serviceInfos = _namingServiceManager.GetNodesByType(ServerType.Game);
            // foreach (var serviceInfo in serviceInfos)
            // {
            //     if (serviceInfo is GameServiceInfo gameServiceInfo)
            //     {
            //         if (mainId >= gameServiceInfo.MinModuleMessageId && mainId <= gameServiceInfo.MaxModuleMessageId)
            //         {
            //             SendMessage((IAppSession)gameServiceInfo.Session, messageObject);
            //             break;
            //         }
            //     }
            // }
        }

        return ValueTask.CompletedTask;
    }

    #endregion

    private async void SendMessage(IAppSession session, INetworkMessage message)
    {
        if (session == null || session.Connection.IsClosed)
        {
            LogHelper.Error("目标链接已断开，取消发送");
            return;
        }

        if (message is MessageObject messageObject)
        {
            var result = MessageEncoderHandler.Handler(message);
            if (Setting.IsDebug && Setting.IsDebugSend && !MessageProtoHelper.IsHeartbeat(message.GetType()))
            {
                LogHelper.Debug($"---发送[{ServerType}] {messageObject.ToMessageString()}");
            }

            await session.SendAsync(result);
        }
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 22000,
                ServerType = ServerType.Gateway,
                InnerIp = "127.0.0.1",
                InnerPort = 22001,
                OuterIp = "127.0.0.1",
                OuterPort = 22001,
                APMPort = 22090,
                DiscoveryCenterPort = 21001,
                DiscoveryCenterIp = "127.0.0.1",
            };
            if (PlatformRuntimeHelper.IsLinux)
            {
                Setting.DiscoveryCenterIp = "discovery";
                Setting.InnerIp = "gateway";
            }
        }

        base.Init();
    }


    // private NamingServiceManager _namingServiceManager;
    //
    // public AppStartUpGateway()
    // {
    //     _namingServiceManager = new NamingServiceManager();
    // }
}