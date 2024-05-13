using GameFrameX.Apps.Player.Player.Component;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Hotfix.Server.Server.Agent;
using GameFrameX.Launcher.Common.Session;
using GameFrameX.NetWork;

namespace GameFrameX.Hotfix.Player.Login.Agent
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
        /// <param name="channel"></param>
        /// <param name="reqLogin"></param>
        public async Task OnLogin(INetChannel channel, ReqPlayerLogin reqLogin)
        {
            var playerState = await Comp.OnLogin(reqLogin);
            if (playerState == null)
            {
                //角色找不到？
                return;
            }

            //添加到session
            var session = new Session(playerState.Id, playerState.Id)
            {
                Channel = channel,
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
            await channel.WriteAsync(respPlayerLogin, reqLogin.UniId);

            //加入在线玩家
            var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
            await serverComp.AddOnlineRole(ActorId);
        }

        /// <summary>
        /// 创建角色
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reqPlayerCreate"></param>
        public async Task OnPlayerCreate(INetChannel channel, ReqPlayerCreate reqPlayerCreate)
        {
            var playerState = await Comp.OnPlayerCreate(reqPlayerCreate);
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
            await channel.WriteAsync(respPlayerCreate, reqPlayerCreate.UniId);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="reqPlayerList"></param>
        public async Task OnGetPlayerList(INetChannel channel, ReqPlayerList reqPlayerList)
        {
            var playerList = await this.Comp.GetPlayerList(reqPlayerList);

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

            await channel.WriteAsync(respPlayerList, reqPlayerList.UniId);
        }
    }
}