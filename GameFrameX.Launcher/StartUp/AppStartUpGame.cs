// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.DataBase.Abstractions;
using GameFrameX.Foundation.Utility;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Launcher.StartUp;

/// <summary>
/// 游戏服务器
/// </summary>
[StartUpTag(GlobalConst.GameServiceName)]
internal sealed class AppStartUpGame : AppStartUpBase
{
    protected override bool IsRegisterToDiscoveryCenter { get; set; } = false;

    public override async Task StartAsync()
    {
        string exitMessage = null;
        try
        {
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ServerStartBegin, Setting.ServerType));
            var hotfixPath = Directory.GetCurrentDirectory() + "/hotfix";
            if (!Directory.Exists(hotfixPath))
            {
                Directory.CreateDirectory(hotfixPath);
            }

            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ActorLimitConfigBegin));
            ActorLimit.Init(ActorLimit.RuleType.None);
            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ActorLimitConfigEnd));

            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.DatabaseServiceStartBegin));
            var initResult = await GameDb.Init<MongoDbService>(new DbOptions { ConnectionString = Setting.DataBaseUrl, Name = Setting.DataBaseName, });
            if (initResult == false)
            {
                throw new InvalidOperationException(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.DatabaseServiceStartFailed));
            }

            LogHelper.DebugConsole(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.DatabaseServiceStartEnd));

            LogHelper.DebugConsole(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ComponentRegisterBegin));
            await ComponentRegister.Init(typeof(AppsHandler).Assembly);
            LogHelper.DebugConsole(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ComponentRegisterEnd));

            LogHelper.DebugConsole(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.HotfixModuleLoadBegin));
            await HotfixManager.LoadHotfixModule(Setting);
            LogHelper.DebugConsole(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.HotfixModuleLoadEnd));

            LogHelper.DebugConsole(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.EnterMainLoop));
            GlobalSettings.LaunchTime = TimerHelper.GetUtcNow();
            GlobalSettings.IsAppRunning = true;
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ServerStartEnd, Setting.ServerType));
            exitMessage = await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ServerExecutionException, e));
            LogHelper.Fatal(e);
        }

        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ServerExitBegin));
        await HotfixManager.Stop(exitMessage);
        LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ServerExitSuccess));
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerId = GlobalConst.GameServiceServerId,
                ServerType = GlobalConst.GameServiceName,
                InnerPort = 29100,
                MetricsPort = 29090,
                HttpPort = 28080,
                WsPort = 29110,
                MinModuleId = 10,
                HttpIsDevelopment = true,
                MaxModuleId = 9999,
                DiscoveryCenterHost = "127.0.0.1",
                TagName = "GameFrameX",
                DiscoveryCenterPort = 21001,
                DataBaseUrl = "mongodb+srv://gameframex:f9v42aU9DVeFNfAF@gameframex.8taphic.mongodb.net/?retryWrites=true&w=majority",
                DataBaseName = "gameframex",
            };
        }

        base.Init();
    }
}