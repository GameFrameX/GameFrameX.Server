// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Apps.Common.Event;
using GameFrameX.Apps.Common.Session;
using GameFrameX.Apps.Player.Player.Component;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Hotfix.Logic.Server.Server;

namespace GameFrameX.Hotfix.Logic.Role.Login;

public class PlayerComponentAgent : StateComponentAgent<PlayerComponent, PlayerState>
{
    public async Task OnLogout()
    {
        //移除在线玩家
        var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
        await serverComp.RemoveOnlineRole(ActorId);
        //下线后会被自动回收
        SetAutoRecycle(true);
        QuartzTimer.Remove(ScheduleIdSet);
    }

    /// <summary>
    /// 使用角色ID登录
    /// </summary>
    /// <param name="workChannel"></param>
    /// <param name="reqLogin"></param>
    public async Task OnPlayerLogin(INetWorkChannel workChannel, ReqPlayerLogin reqLogin)
    {
        var playerState = await OwnerComponent.OnPlayerLogin(reqLogin);
        if (playerState == null)
        {
            //角色找不到？
            return;
        }

        // 更新连接会话数据
        SessionManager.UpdateSession(workChannel.GameAppSession.SessionID, playerState.Id, playerState.Id.ToString());
        var respPlayerLogin = new RespPlayerLogin
        {
            UniqueId = reqLogin.UniqueId,
            Code = playerState.State,
            CreateTime = playerState.CreateTime,
            PlayerInfo = new PlayerInfo
            {
                Id = playerState.Id,
                Name = playerState.Name,
                Level = playerState.Level,
                State = playerState.State,
                Avatar = playerState.Avatar,
            },
        };
        await workChannel.WriteAsync(respPlayerLogin);

        //加入在线玩家
        var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
        await serverComp.AddOnlineRole(ActorId);
    }

    [Event(EventId.SessionRemove)]
    private class EL : EventListener<PlayerComponentAgent>
    {
        protected override Task HandleEvent(PlayerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            return agent.OnLogout();
        }
    }

    [Event(EventId.PlayerSendItem)]
    private class PlayerSendItemEventListener : EventListener<PlayerComponentAgent>
    {
        protected override Task HandleEvent(PlayerComponentAgent agent, GameEventArgs gameEventArgs)
        {
            if (agent == null)
            {
                LogHelper.Error("PlayerComponentAgent is null");
                return Task.CompletedTask;
            }

            return agent.OnLogout();
        }
    }
}