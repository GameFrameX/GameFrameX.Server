using GameFrameX.Apps.Player.Player.Component;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Hotfix.Logic.Server.Server;
using GameFrameX.Launcher.Common.Session;
using GameFrameX.NetWork;

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
        public async Task OnLogin(INetWorkChannel workChannel, ReqPlayerLogin reqLogin)
        {
            var playerState = await OwnerComponent.OnLogin(reqLogin);
            if (playerState == null)
            {
                //角色找不到？
                return;
            }

            //添加到session
            var session = new Session(playerState.Id, playerState.Id)
            {
                WorkChannel = workChannel,
                Sign = playerState.Id.ToString()
            };
            SessionManager.Add(session);

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
                    Avatar = playerState.Avatar
                }
            };
            await workChannel.WriteAsync(respPlayerLogin, reqLogin.UniId);

            //加入在线玩家
            var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
            await serverComp.AddOnlineRole(ActorId);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="workChannel"></param>
        /// <param name="reqPlayerCreate"></param>
        public async Task OnPlayerCreate(INetWorkChannel workChannel, ReqPlayerCreate reqPlayerCreate)
        {
            var playerState = await OwnerComponent.OnPlayerCreate(reqPlayerCreate);
            RespPlayerCreate respPlayerCreate = new RespPlayerCreate
            {
                UniqueId = reqPlayerCreate.UniqueId,
                PlayerInfo = new PlayerInfo
                {
                    Id = playerState.Id,
                    Name = playerState.Name,
                    Level = playerState.Level,
                    State = playerState.State,
                    Avatar = playerState.Avatar
                }
            };
            await workChannel.WriteAsync(respPlayerCreate, reqPlayerCreate.UniId);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="workChannel"></param>
        /// <param name="reqPlayerList"></param>
        public async Task OnGetPlayerList(INetWorkChannel workChannel, ReqPlayerList reqPlayerList)
        {
            var playerList = await OwnerComponent.GetPlayerList(reqPlayerList);

            RespPlayerList respPlayerList = new RespPlayerList
            {
                UniqueId = reqPlayerList.UniqueId,
                PlayerList = new List<PlayerInfo>()
            };
            if (playerList != null)
            {
                foreach (var playerState in playerList)
                {
                    var playerInfo = new PlayerInfo
                    {
                        Id = playerState.Id,
                        Name = playerState.Name
                    };
                    playerInfo.Level = playerState.Level;
                    playerInfo.State = playerState.State;
                    playerInfo.Avatar = playerState.Avatar;
                    respPlayerList.PlayerList.Add(playerInfo);
                }
            }

            await workChannel.WriteAsync(respPlayerList, reqPlayerList.UniId);
        }
    }
}