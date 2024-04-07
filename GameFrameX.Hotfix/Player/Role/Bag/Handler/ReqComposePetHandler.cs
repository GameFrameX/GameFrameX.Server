using GameFrameX.Hotfix.Player.Role.Bag.Agent;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Player.Role.Bag.Handler
{
    [MessageMapping(typeof(ReqComposePet))]
    public class ReqComposePetHandler : RoleComponentHandler<BagComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await Comp.ComposePet(Message as ReqComposePet);
        }
    }
}