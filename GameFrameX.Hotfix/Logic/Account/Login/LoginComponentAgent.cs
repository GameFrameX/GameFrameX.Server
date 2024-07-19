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
                    ErrCode = (int)ResultCode.Failed
                };
                await workChannel.WriteAsync(respErrorCode, reqLogin.UniId, (int)OperationStatusCode.AccountCannotBeNull);
                return;
            }


            var loginCompAgent = await ActorManager.GetComponentAgent<LoginComponentAgent>();
            var loginState = await loginCompAgent.OwnerComponent.OnLogin(reqLogin);
            if (loginState == null)
            {
                var accountId = IdGenerator.GetActorId(ActorType.Account);
                loginState = await loginCompAgent.OwnerComponent.Register(accountId, reqLogin);
            }

            //添加到session
            var session = new Session(loginState.Id, loginState.Id)
            {
                WorkChannel = workChannel,
                Sign = loginState.Id.ToString()
            };
            SessionManager.Add(session);

            RespLogin respLogin = new RespLogin
            {
                UniqueId = reqLogin.UniqueId,
                Code = loginState.State,
                CreateTime = loginState.CreateTime,
                Level = (uint)Utility.Random.GetRandom(1, 100),
                Id = loginState.Id,
                RoleName = Utility.Random.GetRandom(1, 100).ToString(),
            };
            await workChannel.WriteAsync(respLogin, reqLogin.UniId);

            //加入在线玩家
            var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
            await serverComp.AddOnlineRole(loginState.Id);

            //查询角色账号，这里设定每个服务器只能有一个角色
            /*var roleId = GetRoleIdOfPlayer(reqLogin.UserName, reqLogin.Password, reqLogin.SdkType);
            var isNewRole = roleId <= 0;
            if (isNewRole)
            {
                //没有老角色，创建新号
                roleId = IdGenerator.GetActorID(ActorType.Account);
                CreateRoleToPlayer(reqLogin.UserName, reqLogin.Password, reqLogin.SdkType, roleId);
                Log.Info("创建新号:" + roleId);
            }

            //添加到session
            var session = new Session(roleId)
            {
                Channel = channel,
                Sign = reqLogin.Device
            };
            SessionManager.Add(session);

            //登陆流程
            var loginCompAgent = await ActorMgr.GetCompAgent<LoginCompAgent>(roleId);

            var actorId = loginCompAgent.ActorId;*/
            // var roleComp = await ActorMgr.GetCompAgent<RoleCompAgent>(roleId);
            // //从登录线程-->调用Role线程 所以需要入队
            // var resLogin = await roleComp.OnLogin(reqLogin, isNewRole);
            // channel.WriteAsync(resLogin, reqLogin.UniId, StateCode.Success);
            //
            // //加入在线玩家
            // var serverComp = await ActorMgr.GetCompAgent<ServerCompAgent>();
            // await serverComp.AddOnlineRole(ActorId);
        }
    }
}