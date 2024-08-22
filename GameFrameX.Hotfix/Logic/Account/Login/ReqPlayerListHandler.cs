using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Account.Login;

[MessageMapping(typeof(ReqPlayerList))]
internal class ReqPlayerListHandler : GlobalComponentHandler<LoginComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.OnGetPlayerList(NetWorkChannel, Message as ReqPlayerList);
    }
}