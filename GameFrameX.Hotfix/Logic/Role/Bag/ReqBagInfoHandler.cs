using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Role.Bag
{
    [MessageMapping(typeof(ReqBagInfo))]
    public class ReqBagInfoHandler : RoleComponentHandler<BagComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await ComponentAgent.GetBagInfo(Message as ReqBagInfo);
        }
    }
}