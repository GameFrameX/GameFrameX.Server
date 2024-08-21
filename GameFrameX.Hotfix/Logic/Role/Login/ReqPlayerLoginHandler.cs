using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Login
{
    [MessageMapping(typeof(ReqPlayerLogin))]
    internal class ReqPlayerLoginHandler : GlobalComponentHandler<PlayerComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await ComponentAgent.OnPlayerLogin(NetWorkChannel, Message as ReqPlayerLogin);
        }
    }
}