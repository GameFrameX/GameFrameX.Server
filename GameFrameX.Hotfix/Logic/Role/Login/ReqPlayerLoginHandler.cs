using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Login
{
    [MessageMapping(typeof(ReqPlayerLogin))]
    internal class ReqPlayerLoginHandler : GlobalComponentHandler<PlayerComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await Comp.OnLogin(NetWorkChannel, Message as ReqPlayerLogin);
        }
    }
}