using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.DataBase;
using GameFrameX.Hotfix.Common;
using GameFrameX.Monitor.Account;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 账号登录
/// </summary>
[HttpMessageMapping(typeof(ReqLoginHttpHandler))]
public sealed class ReqLoginHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> Action(string ip, string url, Dictionary<string, object> parameters, MessageObject messageObject)
    {
        ReqLogin reqLogin = messageObject as ReqLogin;
        var respLogin = new RespLogin();
        if (reqLogin.UserName.IsNullOrEmpty() || reqLogin.Password.IsNullOrEmpty())
        {
            respLogin.ErrorCode = (int)ResultCode.Failed;
            return null;
        }

        MetricsAccountRegister.LoginCounterOptions.Inc();
        var loginState = await OnLogin(reqLogin);

        if (loginState == null)
        {
            var accountId = ActorIdGenerator.GetUniqueId();
            loginState = await Register(accountId, reqLogin);
        }

        // 构建账号登录返回信息
        respLogin.Code = loginState.State;
        respLogin.CreateTime = loginState.CreateTime;
        respLogin.Level = loginState.Level;
        respLogin.Id = loginState.Id;
        respLogin.RoleName = loginState.NickName;
        return respLogin;
    }

    public async Task<LoginState> OnLogin(ReqLogin reqLogin)
    {
        MetricsAccountRegister.LoginCounterOptions.Inc();
        return await GameDb.FindAsync<LoginState>(m => m.UserName == reqLogin.UserName && m.Password == reqLogin.Password);
    }

    public async Task<LoginState> Register(long accountId, ReqLogin reqLogin)
    {
        MetricsAccountRegister.RegisterCounterOptions.Inc();
        var loginState = new LoginState { Id = accountId, UserName = reqLogin.UserName, Password = reqLogin.Password, };
        await GameDb.SaveOneAsync(loginState);
        return loginState;
    }
}