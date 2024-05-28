using GameFrameX.Core.Utility;
using GameFrameX.Setting;

namespace GameFrameX.Core.Hotfix
{
    public interface IHotfixBridge
    {
        ServerType BridgeType { get; }

        Task<bool> OnLoadSuccess(AppSetting setting, bool reload);

        Task Stop();
    }
}