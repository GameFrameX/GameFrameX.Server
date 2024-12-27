using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Hotfix.Logic.Role.Login;

[MessageMapping(typeof(ReqPlayerLogin))]
internal class ReqPlayerLoginHandler : PlayerComponentHandler<PlayerComponentAgent>
{
    protected override Task InitActor()
    {
        if (ActorId <= 0 && Message is ReqPlayerLogin reqPlayerLogin)
        {
            NetWorkChannel.SetData(GlobalConst.ActorIdKey, reqPlayerLogin.Id);
            ActorId = reqPlayerLogin.Id;
        }

        return base.InitActor();
    }

    protected override async Task ActionAsync()
    {
        await ComponentAgent.OnPlayerLogin(NetWorkChannel, Message as ReqPlayerLogin);
    }
}