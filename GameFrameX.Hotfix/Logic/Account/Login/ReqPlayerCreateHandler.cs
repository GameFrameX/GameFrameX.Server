using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Hotfix.Logic.Account.Login;

[MessageMapping(typeof(ReqPlayerCreate))]
internal class ReqPlayerCreateHandler : GlobalComponentHandler<LoginComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.OnPlayerCreate(NetWorkChannel, Message as ReqPlayerCreate);
    }
}