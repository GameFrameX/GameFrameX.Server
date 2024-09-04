using GameFrameX.Core.Components;
using GameFrameX.DataBase.Mongo;

namespace GameFrameX.Launcher.StartUp.Game
{
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
                LogHelper.Info($"开始启动服务器{Setting.ServerType}");
                var hotfixPath = Directory.GetCurrentDirectory() + "/hotfix";
                if (!Directory.Exists(hotfixPath))
                {
                    Directory.CreateDirectory(hotfixPath);
                }


                LogHelper.Info($"actor limit logic start...");
                ActorLimit.Init(ActorLimit.RuleType.None);
                LogHelper.Info($"actor limit logic end...");

                LogHelper.Info($"launch db service start...");
                MongoDbService mongoDbService = new MongoDbService();
                GameDb.Init(mongoDbService);
                GameDb.Open(Setting.DataBaseUrl, Setting.DataBaseName);
                LogHelper.Info($"launch db service end...");

                LogHelper.Info($"register comps start...");
                await ComponentRegister.Init(typeof(AppsHandler).Assembly);
                LogHelper.Info($"register comps end...");

                LogHelper.Info($"load hotfix module start");
                await HotfixManager.LoadHotfixModule(Setting);
                LogHelper.Info($"load hotfix module end");

                LogHelper.Info("进入游戏主循环...");
                LogHelper.Info("***进入游戏主循环***");
                GlobalSettings.LaunchTime = DateTime.Now;
                GlobalSettings.IsAppRunning = true;
                await AppExitToken;
            }
            catch (Exception e)
            {
                LogHelper.Info($"服务器执行异常，e:{e}");
                LogHelper.Fatal(e);
            }

            LogHelper.Info($"退出服务器开始");
            await HotfixManager.Stop();
            LogHelper.Info($"退出服务器成功");
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
                    HttpPort = 29001,
                    WsPort = 29110,
                    MinModuleId = 0,
                    MaxModuleId = 9999,
                    DiscoveryCenterIp = "127.0.0.1",
                    DiscoveryCenterPort = 21001,
                    IsDebug = true,
                    IsDebugSend = true,
                    IsDebugReceive = true,
                    //
                    HttpCode = "inner_httpcode",
                    DataBaseUrl = "mongodb+srv://gameframex:f9v42aU9DVeFNfAF@gameframex.8taphic.mongodb.net/?retryWrites=true&w=majority",
                    DataBaseName = "gameframex"
                };
            }

            base.Init();
        }
    }
}