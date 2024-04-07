using GameFrameX.Hotfix.Player.Role.Bag.Agent;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Player.Role.Bag.Handler
{
    [MessageMapping(typeof(ReqBagInfo))]
    public class ReqBagInfoHandler : RoleComponentHandler<BagComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await Comp.GetBagInfo(Message as ReqBagInfo);
        }
    }
}