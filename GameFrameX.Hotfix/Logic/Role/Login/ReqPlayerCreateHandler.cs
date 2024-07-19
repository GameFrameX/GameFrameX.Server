using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Login;

[MessageMapping(typeof(ReqPlayerCreate))]
internal class ReqPlayerCreateHandler : GlobalComponentHandler<PlayerComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.OnPlayerCreate(NetWorkChannel, Message as ReqPlayerCreate);
    }
}