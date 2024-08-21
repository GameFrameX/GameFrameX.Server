using GameFrameX.Apps.Account.Login.Component;
using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Hotfix.Common;
using GameFrameX.Hotfix.Logic.Server.Server;
using GameFrameX.Launcher.Common.Session;
using GameFrameX.NetWork;

namespace GameFrameX.Hotfix.Logic.Account.Login
{
    public class LoginComponentAgent : StateComponentAgent<LoginComponent, LoginState>
    {
        public async Task OnLogin(INetWorkChannel workChannel, ReqLogin reqLogin)
        {
            if (reqLogin.UserName.IsNullOrEmpty() || reqLogin.Password.IsNullOrEmpty())
            {
                RespErrorCode respErrorCode = new RespErrorCode
                {
                    ErrCode = (int)ResultCode.Failed,
                };
                await workChannel.WriteAsync(respErrorCode, (int)OperationStatusCode.AccountCannotBeNull);
                return;
            }

            var loginState = await OwnerComponent.OnLogin(reqLogin);
            if (loginState == null)
            {
                var accountId = ActorIdGenerator.GetActorId(ActorType.Account);
                loginState = await OwnerComponent.Register(accountId, reqLogin);
            }

            // 构建账号登录返回信息
            RespLogin respLogin = new RespLogin
            {
                UniqueId = reqLogin.UniqueId,
                Code = loginState.State,
                CreateTime = loginState.CreateTime,
                Level = loginState.Level,
                Id = loginState.Id,
                RoleName = loginState.NickName,
            };
            await workChannel.WriteAsync(respLogin);
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
            await workChannel.WriteAsync(respPlayerCreate);
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="workChannel"></param>
        /// <param name="reqPlayerList"></param>
        public async Task OnGetPlayerList(INetWorkChannel workChannel, ReqPlayerList reqPlayerList)
        {
            var playerList = await this.OwnerComponent.GetPlayerList(reqPlayerList);

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

            await workChannel.WriteAsync(respPlayerList);
        }

    }
}