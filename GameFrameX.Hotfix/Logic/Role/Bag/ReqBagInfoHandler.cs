using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Bag
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