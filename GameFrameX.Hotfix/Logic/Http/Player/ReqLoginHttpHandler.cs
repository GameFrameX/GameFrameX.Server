using GameFrameX.Apps;
using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.DataBase;
using GameFrameX.Network.Messages;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 账号登录
/// </summary>
[HttpMessageMapping(typeof(ReqLoginHttpHandler))]
[Description("账号登录,仅限前端调用")]
public sealed class ReqLoginHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> ActionMessageObjectAsync(HttpActionContext context)
    {
        var messageObject = context.MessageObject;
        var reqLogin = (ReqLogin)messageObject;
        var respLogin = new RespLogin();
        if (reqLogin.UserName.IsNullOrEmpty() || reqLogin.Password.IsNullOrEmpty())
        {
            respLogin.ErrorCode = (int)ResultCode.Failed;
            return null;
        }

        AppMetrics.AccountLogin.Add(1);
        var loginState = await OnLoginAsync(reqLogin);

        if (loginState == null)
        {
            var accountId = ActorIdGenerator.GetUniqueId();
            loginState = await RegisterAsync(accountId, reqLogin);
        }

        // 构建账号登录返回信息
        respLogin.Code = loginState.State;
        // CreatedTime 为非空 long（Foundation EntityBase），直接赋值
        respLogin.CreateTime = loginState.CreatedTime;

        respLogin.Level = loginState.Level;
        respLogin.Id = loginState.Id;
        respLogin.RoleName = loginState.NickName;
        return respLogin;
    }

    public async Task<LoginState> OnLoginAsync(ReqLogin reqLogin)
    {
        AppMetrics.AccountLogin.Add(1);
        return await GameDb.FindAsync<LoginState>(m => m.UserName == reqLogin.UserName && m.Password == reqLogin.Password, false);
    }

    public async Task<LoginState> RegisterAsync(long accountId, ReqLogin reqLogin)
    {
        AppMetrics.AccountRegister.Add(1);
        var loginState = new LoginState { Id = accountId, UserName = reqLogin.UserName, Password = reqLogin.Password, };
        await GameDb.AddOrUpdateAsync(loginState);
        return loginState;
    }
}
