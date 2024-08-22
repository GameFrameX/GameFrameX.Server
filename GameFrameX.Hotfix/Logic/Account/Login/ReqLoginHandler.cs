using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Account.Login
{
    [MessageMapping(typeof(ReqLogin))]
    internal class ReqLoginHandler : GlobalComponentHandler<LoginComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await ComponentAgent.OnLogin(NetWorkChannel, Message as ReqLogin);
        }
    }
}