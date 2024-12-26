using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.DataBase;
using GameFrameX.Hotfix.Common;
using GameFrameX.Monitor.Account;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 账号登录
/// </summary>
[HttpMessageMapping(typeof(ReqLoginHttpHandler))]
public sealed class ReqLoginHttpHandler : BaseHttpHandler
{
    public override async Task<string> Action(string ip, string url, Dictionary<string, object> paramMap)
    {
        ReqLogin reqLogin = JsonHelper.Deserialize<ReqLogin>(JsonHelper.Serialize(paramMap));

        if (reqLogin.UserName.IsNullOrEmpty() || reqLogin.Password.IsNullOrEmpty())
        {
            return HttpResult.Create((int)ResultCode.Failed);
        }

        MetricsAccountRegister.LoginCounterOptions.Inc();
        var loginState = await OnLogin(reqLogin);

        if (loginState == null)
        {
            var accountId = ActorIdGenerator.GetUniqueId();
            loginState = await Register(accountId, reqLogin);
        }

        // 构建账号登录返回信息
        var respLogin = new RespLogin
        {
            Code = loginState.State,
            CreateTime = loginState.CreateTime,
            Level = loginState.Level,
            Id = loginState.Id,
            RoleName = loginState.NickName,
        };
        return HttpResult.Create(JsonHelper.Serialize(respLogin));
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