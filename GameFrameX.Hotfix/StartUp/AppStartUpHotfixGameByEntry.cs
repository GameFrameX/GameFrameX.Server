using System.Reflection;
using GameFrameX.Launcher.StartUp;

namespace GameFrameX.Hotfix.StartUp
{
    internal partial class AppStartUpHotfixGame : AppStartUpService, IHotfixBridge
    {
        public async Task<bool> OnLoadSuccess(AppSetting setting, bool reload)
        {
            if (reload)
            {
                ActorManager.ClearAgent();
                return true;
            }

            
            Init(setting.ServerType, setting);
            RunServer();
            // 启动定时器
            GlobalTimer.Start();
            await ComponentRegister.ActiveGlobalComponents();
            return true;
        }

        public async Task Stop()
        {
            await StopAsync();
        }
    }
}