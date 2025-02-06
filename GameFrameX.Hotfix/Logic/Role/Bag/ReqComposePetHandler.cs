using GameFrameX.Core.BaseHandler;

namespace GameFrameX.Hotfix.Logic.Role.Bag;

[MessageMapping(typeof(ReqComposePet))]
public class ReqComposePetHandler : PlayerComponentHandler<BagComponentAgent>
{
    protected override async Task ActionAsync()
    {
        await ComponentAgent.ComposePet(Message as ReqComposePet);
    }
}