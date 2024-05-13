using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.Hotfix.Player.Login.Agent;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Player.Login.Handler;

[MessageMapping(typeof(ReqPlayerList))]
internal class ReqPlayerListHandler : GlobalComponentHandler<PlayerComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await Comp.OnGetPlayerList(Channel, Message as ReqPlayerList);
    }
}