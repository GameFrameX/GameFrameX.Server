using GameFrameX.Core.Utility;
using GameFrameX.Setting;

namespace GameFrameX.Core.Hotfix
{
    public interface IHotfixBridge
    {
        ServerType BridgeType { get; }

        Task<bool> OnLoadSuccess(BaseSetting setting, bool reload);

        Task Stop();
    }
}