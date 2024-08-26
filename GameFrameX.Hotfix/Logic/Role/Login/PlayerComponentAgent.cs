using GameFrameX.Apps.Common.Event;
using GameFrameX.Apps.Common.Session;
using GameFrameX.Apps.Player.Player.Component;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Hotfix.Logic.Server.Server;
using GameFrameX.NetWork;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Hotfix.Logic.Role.Login
{
    public class PlayerComponentAgent : StateComponentAgent<PlayerComponent, PlayerState>
    {
        [Event(EventId.SessionRemove)]
        private class EL : EventListener<PlayerComponentAgent>
        {
            protected override Task HandleEvent(PlayerComponentAgent agent, Event evt)
            {
                return agent.OnLogout();
            }
        }

        public async Task OnLogout()
        {
            //移除在线玩家
            var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
            await serverComp.RemoveOnlineRole(ActorId);
            //下线后会被自动回收
            SetAutoRecycle(true);
            QuartzTimer.UnSchedule(ScheduleIdSet);
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
            var session = SessionManager.Get(workChannel.GameAppSession.SessionID);
            session.SetRoleId(playerState.Id);
            session.SetSign(playerState.Id.ToString());
            session.SetActorId(ActorId);

            RespPlayerLogin respPlayerLogin = new RespPlayerLogin
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
    }
}