// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.DataBase;
using GameFrameX.Monitor.Account;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 账号登录
/// </summary>
[HttpMessageMapping(typeof(ReqLoginHttpHandler))]
[Description("账号登录,仅限前端调用")]
public sealed class ReqLoginHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> Action(string ip, string url, Dictionary<string, object> parameters, MessageObject messageObject)
    {
        var reqLogin = (ReqLogin)messageObject;
        var respLogin = new RespLogin();
        if (reqLogin.UserName.IsNullOrEmpty() || reqLogin.Password.IsNullOrEmpty())
        {
            respLogin.ErrorCode = (int)ResultCode.Failed;
            return null;
        }

        // MetricsAccountRegister.LoginCounterOptions.Inc();
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
        // MetricsAccountRegister.LoginCounterOptions.Inc();
        return await GameDb.FindAsync<LoginState>(m => m.UserName == reqLogin.UserName && m.Password == reqLogin.Password, false);
    }

    public async Task<LoginState> Register(long accountId, ReqLogin reqLogin)
    {
        // MetricsAccountRegister.RegisterCounterOptions.Inc();
        var loginState = new LoginState { Id = accountId, UserName = reqLogin.UserName, Password = reqLogin.Password, };
        await GameDb.AddOrUpdateAsync(loginState);
        return loginState;
    }
}