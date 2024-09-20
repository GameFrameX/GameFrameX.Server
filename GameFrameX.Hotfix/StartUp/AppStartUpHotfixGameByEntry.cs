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

            GameAnalyticsHelper.Init("ec33486a135ae80984dbfc66eede1e41", "379e705038d46691c5c567db947af1828f702861", Assembly.GetCallingAssembly().ImageRuntimeVersion);
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