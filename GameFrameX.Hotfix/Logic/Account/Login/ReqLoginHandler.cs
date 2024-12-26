/*using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Hotfix.Logic.Account.Login;

[MessageMapping(typeof(ReqLogin))]
internal class ReqLoginHandler : GlobalComponentHandler<LoginComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.OnLogin(NetWorkChannel, Message as ReqLogin);
    }
}*/