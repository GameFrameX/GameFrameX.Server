using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Bag
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