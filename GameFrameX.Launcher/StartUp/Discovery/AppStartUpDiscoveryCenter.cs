// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.DiscoveryCenterManager.Player;
using GameFrameX.DiscoveryCenterManager.Server;
using GameFrameX.Foundation.Extensions;

namespace GameFrameX.Launcher.StartUp.Discovery;

/// <summary>
/// 服务发现中心服务器
/// </summary>
[StartUpTag(GlobalConst.DiscoveryCenterName)]
internal partial class AppStartUpDiscoveryCenter : AppStartUpBase
{
    public override async Task StartAsync()
    {
        try
        {
            _namingServiceManager.AddSelf(Setting);
            var aopHandlerTypes = AssemblyHelper.GetRuntimeImplementTypeNamesInstance<IHttpAopHandler>();
            aopHandlerTypes.Sort((handlerX, handlerY) => handlerX.Priority.CompareTo(handlerY.Priority));
            await ComponentRegister.Init(typeof(AppsHandler).Assembly);
            HotfixManager.LoadHotfix(Setting);
            await StartServerAsync<DefaultMessageDecoderHandler, DefaultMessageEncoderHandler>(new DefaultMessageCompressHandler(), new DefaultMessageDecompressHandler(), HotfixManager.GetListHttpHandler(), HotfixManager.GetHttpHandler, aopHandlerTypes);

            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info($"服务器{ServerType}执行异常，e:{e}");
            LogHelper.Fatal(e);
        }

        await StopAsync();
    }

    /// <summary>
    /// 发送消息给注册的服务
    /// </summary>
    /// <param name="session">连接会话对象</param>
    /// <param name="message">消息对象</param>
    private async void SendMessage(IAppSession session, INetworkMessage message)
    {
        if (session == null)
        {
            return;
        }

        if (session.Connection.IsClosed)
        {
            return;
        }

        MessageProtoHelper.SetMessageId(message);
        var messageObjectHeader = new InnerMessageObjectHeader
        {
            ServerId = Setting.ServerId,
        };
        var innerNetworkMessage = InnerNetworkMessage.Create(message, messageObjectHeader);
        var buffer = MessageHelper.EncoderHandler.Handler(message);
        if (Setting.IsDebug && Setting.IsDebugSend)
        {
            if (Setting.IsDebugSendHeartBeat || messageObjectHeader.OperationType != (byte)MessageOperationType.HeartBeat)
            {
                var serverInfo = _namingServiceManager.GetNodeBySessionId(session.SessionID);
                var toServerType = serverInfo != null ? serverInfo.Type.ToString() : ServerType.ToString();
                LogHelper.Debug($"---发送[{ServerType} To {toServerType}]  {innerNetworkMessage.ToFormatMessageString()}");
            }
        }

        await session.SendAsync(buffer);
    }

    protected override ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        if (message is IInnerNetworkMessage messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                // 当需要打印心跳，或当前非心跳消息时才输出日志
                if (Setting.IsDebugReceiveHeartBeat || messageObject.Header.OperationType != (byte)MessageOperationType.HeartBeat)
                {
                    var serverInfo = _namingServiceManager.GetNodeBySessionId(session.SessionID);
                    var from = serverInfo != null ? serverInfo.Type.ToString() : ServerType.ToString();
                    LogHelper.Debug($"---收到[{from} To {ServerType}]  {message.ToFormatMessageString()}");
                }
            }

            switch (messageObject.Header.OperationType)
            {
                case (byte)MessageOperationType.HeartBeat:
                {
                    // 心跳响应
                    var reqHeartBeat = messageObject.DeserializeMessageObject();
                    var response = new NotifyActorHeartBeat
                    {
                        UniqueId = reqHeartBeat.UniqueId,
                        Timestamp = TimeHelper.UnixTimeMilliseconds(),
                    };
                    SendMessage(session, response);
                    return ValueTask.CompletedTask;
                }
                case (byte)MessageOperationType.Game:
                {
                    var reqConnectServer = (ReqConnectServer)messageObject.DeserializeMessageObject();
                    var serverList = _namingServiceManager.GetNodesByType(reqConnectServer.ServerType);
                    if (reqConnectServer.ServerId > 0)
                    {
                        serverList = serverList.Where(m => m.ServerId == reqConnectServer.ServerId).ToList();
                    }

                    if (serverList.Count > 0)
                    {
                        var serverInfo = (ServiceInfo)serverList.Random();

                        var respConnectServer = new RespConnectServer
                        {
                            UniqueId = reqConnectServer.UniqueId,
                            ServerType = serverInfo.Type,
                            ServerName = serverInfo.Name,
                            ServerId = serverInfo.ServerId,
                            TargetHost = serverInfo.ExternalHost,
                            TargetPort = serverInfo.ExternalPort,
                        };
                        SendMessage(session, respConnectServer);
                    }
                }
                    break;

                case (byte)MessageOperationType.NotifyPlayerOnLine:
                {
                    var reqRegisterPlayer = (NotifyPlayerOnLine)messageObject.DeserializeMessageObject();
                    // 注册玩家
                    NamingPlayerManager.Instance.Add(reqRegisterPlayer.PlayerId, reqRegisterPlayer.ServerId, reqRegisterPlayer.ServerInstanceId);
                    LogHelper.Info($"注册玩家成功：{reqRegisterPlayer.PlayerId}  {reqRegisterPlayer}");
                    return ValueTask.CompletedTask;
                }
                case (byte)MessageOperationType.NotifyPlayerOffLine:
                {
                    var reqRegisterPlayer = (NotifyPlayerOffLine)messageObject.DeserializeMessageObject();
                    // 注销玩家
                    NamingPlayerManager.Instance.TryRemove(reqRegisterPlayer.PlayerId, out var playerInfo);
                    LogHelper.Info($"注销玩家成功：{reqRegisterPlayer.PlayerId}  {reqRegisterPlayer}");
                    return ValueTask.CompletedTask;
                }
                case (byte)MessageOperationType.NotifyServiceOnLine:
                {
                    var reqRegisterServer = (ReqServiceRegister)messageObject.DeserializeMessageObject();
                    // 注册服务
                    var serviceInfo = new ServiceInfo(reqRegisterServer.ServerType, session, session.SessionID, reqRegisterServer.ServerName, reqRegisterServer.ServerId, reqRegisterServer.ServerInstanceId, reqRegisterServer.InnerHost, reqRegisterServer.InnerPort, reqRegisterServer.OuterHost, reqRegisterServer.OuterPort);
                    _namingServiceManager.Add(serviceInfo);
                    LogHelper.Info($"注册服务成功：{reqRegisterServer.ServerType}  {reqRegisterServer.ServerName}  {reqRegisterServer}");
                    return ValueTask.CompletedTask;
                }
                case (byte)MessageOperationType.ConnectService:
                {
                    var reqConnectServer = (ReqConnectServer)messageObject.DeserializeMessageObject();
                    var serverList = _namingServiceManager.GetNodesByType(reqConnectServer.ServerType);
                    if (reqConnectServer.ServerId > 0)
                    {
                        serverList = serverList.Where(m => m.ServerId == reqConnectServer.ServerId).ToList();
                    }

                    var respConnectServer = new RespConnectServer
                    {
                        UniqueId = messageObject.Header.UniqueId,
                    };
                    if (serverList.Count > 0)
                    {
                        var serverInfo = (ServiceInfo)serverList.Random();
                        respConnectServer.ServerType = serverInfo.Type;
                        respConnectServer.ServerName = serverInfo.Name;
                        respConnectServer.ServerId = serverInfo.ServerId;
                        respConnectServer.TargetHost = serverInfo.ExternalHost;
                        respConnectServer.TargetPort = serverInfo.ExternalPort;
                    }

                    SendMessage(session, respConnectServer);
                }
                    break;
            }
        }

        return ValueTask.CompletedTask;
    }


    protected override ValueTask OnConnected(IAppSession appSession)
    {
        LogHelper.Info("有外部服务连接到中心服务器成功" + "。链接信息：SessionID:" + appSession.SessionID + " RemoteEndPoint:" + appSession.RemoteEndPoint);
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnDisconnected(IAppSession appSession, CloseEventArgs args)
    {
        LogHelper.Info("有外部服务从中心服务器断开。链接信息：断开原因:" + args.Reason);
        _namingServiceManager.TryRemoveBySessionId(appSession.SessionID);
        return ValueTask.CompletedTask;
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 21000,
                ServerType = GlobalConst.DiscoveryCenterName,
                InnerPort = 21001,
                HttpPort = 21011,
                MetricsPort = 21090,
                IsDebug = true,
                IsDebugReceive = true,
                IsDebugSend = true,
            };
        }

        base.Init();
    }
}