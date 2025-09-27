using GameFrameX.StartUp;
using GameFrameX.Utility.Setting;

namespace GameFrameX.Hotfix.StartUp;

internal partial class AppStartUpHotfixGame : AppStartUpBase, IHotfixBridge
{
    protected override bool IsRegisterToDiscoveryCenter { get; set; } = true;

    public async Task<bool> OnLoadSuccess(AppSetting setting, bool reload)
    {
        if (reload)
        {
            ActorManager.ClearAgent();
            return true;
        }


        Init(setting.ServerType, setting);
        await RunServer();
        // 启动定时器
        GlobalTimer.Start();
        await ComponentRegister.ActiveGlobalComponents();
        return true;
    }

    public async Task Stop(string message = "")
    {
        await StopAsync(message);
    }
}