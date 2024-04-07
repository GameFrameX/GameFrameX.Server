using GameFrameX.Hotfix.Account.Login.Agent;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Account.Login.Handler
{
    [MessageMapping(typeof(ReqLogin))]
    internal class ReqLoginHandler : GlobalComponentHandler<LoginComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await Comp.OnLogin(Channel, Message as ReqLogin);
        }
    }
}