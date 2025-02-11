namespace GameFrameX.Launcher.StartUp;

/// <summary>
/// 游戏服务器
/// </summary>
[StartUpTag(ServerType.Game)]
internal sealed class AppStartUpGame : AppStartUpBase
{
    public override async Task StartAsync()
    {
        try
        {
            LogHelper.InfoConsole($"开始启动服务器{Setting.ServerType}");
            var hotfixPath = Directory.GetCurrentDirectory() + "/hotfix";
            if (!Directory.Exists(hotfixPath))
            {
                Directory.CreateDirectory(hotfixPath);
            }

            LogHelper.InfoConsole("开始配置Actor限制逻辑...");
            ActorLimit.Init(ActorLimit.RuleType.None);
            LogHelper.InfoConsole("配置Actor限制逻辑结束...");

            LogHelper.InfoConsole("开始启动数据库服务...");
            GameDb.Init<MongoDbService>(Setting.DataBaseUrl, Setting.DataBaseName);
            LogHelper.InfoConsole("启动数据库服务 结束...");

            LogHelper.InfoConsole("注册组件开始...");
            await ComponentRegister.Init(typeof(AppsHandler).Assembly);
            LogHelper.InfoConsole("注册组件结束...");

            LogHelper.InfoConsole("开始加载热更新模块...");
            await HotfixManager.LoadHotfixModule(Setting);
            LogHelper.InfoConsole("加载热更新模块结束...");

            LogHelper.InfoConsole("进入游戏主循环...");
            GlobalSettings.LaunchTime = DateTime.Now;
            GlobalSettings.IsAppRunning = true;
            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.InfoConsole($"服务器执行异常，e:{e}");
            LogHelper.Fatal(e);
        }

        LogHelper.InfoConsole("退出服务器开始");
        await HotfixManager.Stop();
        LogHelper.InfoConsole("退出服务器成功");
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = 9000,
                ServerType = ServerType.Game,
                InnerPort = 29100,
                APMPort = 29090,
                HttpPort = 28080,
                WsPort = 29110,
                MinModuleId = 10,
                MaxModuleId = 9999,
                DiscoveryCenterIp = "127.0.0.1",
                DiscoveryCenterPort = 21001,
                DataBaseUrl = "mongodb+srv://gameframex:f9v42aU9DVeFNfAF@gameframex.8taphic.mongodb.net/?retryWrites=true&w=majority",
                DataBaseName = "gameframex",
            };
        }

        base.Init();
    }
}