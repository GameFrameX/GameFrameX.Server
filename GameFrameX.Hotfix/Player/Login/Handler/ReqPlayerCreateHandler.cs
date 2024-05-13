using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.Hotfix.Player.Login.Agent;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Player.Login.Handler;

[MessageMapping(typeof(ReqPlayerCreate))]
internal class ReqPlayerCreateHandler : GlobalComponentHandler<PlayerComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await Comp.OnPlayerCreate(Channel, Message as ReqPlayerCreate);
    }
}