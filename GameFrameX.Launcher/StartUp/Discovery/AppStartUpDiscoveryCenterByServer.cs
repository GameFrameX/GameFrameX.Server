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

using GameFrameX.DiscoveryCenterManager.Player;
using GameFrameX.DiscoveryCenterManager.Server;
using GameFrameX.Foundation.Json;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.BuiltIn;
using Mapster;

namespace GameFrameX.Launcher.StartUp.Discovery;

/// <summary>
/// 服务发现中心服务器
/// </summary>
internal partial class AppStartUpDiscoveryCenter
{
    private readonly NamingServiceManager _namingServiceManager;
    private readonly NamingPlayerManager _namingPlayerManager;

    public AppStartUpDiscoveryCenter()
    {
        _namingServiceManager = NamingServiceManager.Instance;
        _namingServiceManager.SetServerChangeCallback(OnServerAdd, OnServerRemove);
        _namingPlayerManager = NamingPlayerManager.Instance;
        _namingPlayerManager.SetChangeCallback(OnPlayerAdd, OnPlayerRemove);
    }

    private void OnPlayerRemove(IPlayerInfo playerInfo)
    {
        LogHelper.DebugConsole("玩家下线回调，通知其他服务器:" + JsonHelper.Serialize(playerInfo));

        var serverList = _namingServiceManager.GetOuterNodes();
        serverList = serverList.Where(m => m.ServerId == 8000).ToList();

        var notifyPlayerOffLine = playerInfo.Adapt<NotifyPlayerOffLine>();
        MessageProtoHelper.SetMessageId(notifyPlayerOffLine);
        foreach (var serviceInfo in serverList)
        {
            var info = (ServiceInfo)serviceInfo;
            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, notifyPlayerOffLine);
        }
    }

    private void OnPlayerAdd(IPlayerInfo playerInfo)
    {
        LogHelper.DebugConsole("玩家上线回调，通知其他服务器:" + JsonHelper.Serialize(playerInfo));
        var serverList = _namingServiceManager.GetOuterNodes();
        serverList = serverList.Where(m => m.ServerId == GlobalConst.SocialServiceServerId).ToList();

        var notifyPlayerOnLine = playerInfo.Adapt<NotifyPlayerOnLine>();
        MessageProtoHelper.SetMessageId(notifyPlayerOnLine);
        foreach (var serviceInfo in serverList)
        {
            var info = (ServiceInfo)serviceInfo;
            var appSession = (IAppSession)info.Session;
            SendMessage(appSession, notifyPlayerOnLine);
        }
    }

    private void OnServerRemove(IServiceInfo serverInfo)
    {
        LogHelper.DebugConsole("服务下线回调，通知其他服务器:" + serverInfo);
        var serverList = _namingServiceManager.GetOuterNodes();
        serverList = serverList.Where(m => m.ServerId != 0 && m.ServerInstanceId != serverInfo.ServerInstanceId)
                               .ToList();

        var respServerOnlineServer = new NotifyServiceOffLine
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.Name,
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
        LogHelper.DebugConsole("服务上线回调，通知其他服务器:" + serverInfo);
        var serverList = _namingServiceManager.GetOuterNodes();
        serverList = serverList.Where(m => m.ServerInstanceId != serverInfo.ServerInstanceId).ToList();

        var respServerOnlineServer = new NotifyServiceOnLine
        {
            ServerType = serverInfo.Type,
            ServerName = serverInfo.Name,
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