using Server.Core.Utility;
using Server.Setting;
using Server.Utility;

namespace Server.Core.Hotfix
{
    public interface IHotfixBridge
    {
        ServerType BridgeType { get; }

        Task<bool> OnLoadSuccess(BaseSetting setting, bool reload);

        Task Stop();
    }
}