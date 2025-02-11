﻿using GameFrameX.StartUp;

namespace GameFrameX.Hotfix.StartUp;

internal partial class AppStartUpHotfixGame : AppStartUpBase, IHotfixBridge
{
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

    public async Task Stop()
    {
        await StopAsync();
    }
}