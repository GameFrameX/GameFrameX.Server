using GameFrameX.Core.BaseHandler;

namespace GameFrameX.Hotfix.Logic.Role.Bag;

[MessageMapping(typeof(ReqBagInfo))]
public class ReqBagInfoHandler : PlayerComponentHandler<BagComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.GetBagInfo(NetWorkChannel, Message as ReqBagInfo);
    }
}