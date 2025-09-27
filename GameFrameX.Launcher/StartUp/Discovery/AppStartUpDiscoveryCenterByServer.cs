using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.ServerManager;

namespace GameFrameX.Launcher.StartUp.Discovery;

/// <summary>
/// 服务发现中心服务器
/// </summary>
internal partial class AppStartUpDiscoveryCenter
{
    private readonly NamingServiceManager _namingServiceManager;

    public AppStartUpDiscoveryCenter()
    {
        _namingServiceManager = NamingServiceManager.Instance;
        _namingServiceManager.SetServerChangeCallback(OnServerAdd, OnServerRemove);
    }

    private void OnServerRemove(IServiceInfo serverInfo)
    {
        var serverList = _namingServiceManager.GetOuterNodes()
            .Where(m => m.ServerId != 0 && m.ServerId != serverInfo.ServerId)
            .ToList();
        var respServerOnlineServer = new RespServerOfflineServer
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.ServerName,
            ServerId = serverInfo.ServerId,
        };
        MessageProtoHelper.SetMessageId(respServerOnlineServer);
        foreach (var serverInfo1 in serverList)
        {
            var info = (ServiceInfo)serverInfo1;
            if (serverInfo.SessionId == info.SessionId)
            {
                // 跳过自身
                continue;
            }

            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, respServerOnlineServer);
        }
    }

    private void OnServerAdd(IServiceInfo serverInfo)
    {
        var serverList = _namingServiceManager.GetOuterNodes().Where(m => m.ServerId != serverInfo.ServerId).ToList();

        var respServerOnlineServer = new RespServerOnlineServer
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.ServerName,
            ServerId = serverInfo.ServerId,
        };
        MessageProtoHelper.SetMessageId(respServerOnlineServer);
        foreach (var serverInfo1 in serverList)
        {
            var info = (ServiceInfo)serverInfo1;
            if (serverInfo.SessionId == info.SessionId)
            {
                // 跳过自身
                continue;
            }

            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, respServerOnlineServer);
        }
    }
}