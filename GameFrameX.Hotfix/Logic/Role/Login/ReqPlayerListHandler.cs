using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Login;

[MessageMapping(typeof(ReqPlayerList))]
internal class ReqPlayerListHandler : GlobalComponentHandler<PlayerComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.OnGetPlayerList(NetWorkChannel, Message as ReqPlayerList);
    }
}