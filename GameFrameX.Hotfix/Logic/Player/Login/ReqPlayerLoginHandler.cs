using GameFrameX.Hotfix.Logic.Role.Login;

namespace GameFrameX.Hotfix.Logic.Player.Login;

[MessageMapping(typeof(ReqPlayerLogin))]
internal sealed class ReqPlayerLoginHandler : PlayerComponentHandler<PlayerComponentAgent>
{
    /// <summary>
    /// 玩家登录初始化
    /// 这个地方需要设置ActorID为玩家的ID,否则会报错，该代码只会执行一次。全局唯一。不能移除该代码，也请不要修改该函数ActorID
    /// </summary>
    /// <returns></returns>
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