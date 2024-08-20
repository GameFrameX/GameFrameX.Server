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
    }
}