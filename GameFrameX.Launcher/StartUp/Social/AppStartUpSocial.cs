// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using GameFrameX.Core.Components;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Apps.Common.Event;
using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Events;

namespace GameFrameX.Launcher.StartUp.Social;

/// <summary>
/// 游戏服务器
/// </summary>
[StartUpTag(GameServerConst.Social.Name)]
internal sealed partial class AppStartUpSocial : AppStartUpBase
{
    public override async Task StartAsync()
    {
        try
        {
            var aopHandlerTypes = AssemblyHelper.GetRuntimeImplementTypeNamesInstance<IHttpAopHandler>();
            aopHandlerTypes.Sort((handlerX, handlerY) => handlerX.Priority.CompareTo(handlerY.Priority));
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Launcher.DatabaseServiceStartBegin));
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Launcher.ActorLimitConfigBegin));
            ActorLimit.Init(ActorLimit.RuleType.None);
            LogHelper.Debug(LocalizationService.GetString(Localization.Keys.Launcher.ActorLimitConfigEnd));
            var initResult = await GameDb.Init<MongoDbService>(new DbOptions { ConnectionString = Setting.DataBaseUrl, Name = Setting.DataBaseName, IsUseTimeZone = Setting.IsUseTimeZone, });
            if (initResult == false)
            {
                throw new InvalidOperationException(LocalizationService.GetString(Localization.Keys.Launcher.DatabaseServiceStartFailed));
            }

            await ComponentRegister.Init(typeof(AppsHandler).Assembly);
            HotfixManager.LoadHotfix(Setting);
            await StartServerAsync<DefaultMessageDecoderHandler, DefaultMessageEncoderHandler>(new DefaultMessageCompressHandler(), new DefaultMessageDecompressHandler(), HotfixManager.GetListHttpHandler(), HotfixManager.GetHttpHandler, aopHandlerTypes);
            EventDispatcher.Dispatch(0, (int)EventId.ServiceOnline, new ServiceOnlineEventArgs(Setting.ServerType, Setting.ServerInstanceId, DateTime.UtcNow));

            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info(LocalizationService.GetString(Localization.Keys.Launcher.ServerExecutionException, e));
            LogHelper.Fatal(e);
        }

        await StopAsync();
    }

    protected override async ValueTask PackageHandler(IAppSession session, IMessage message)
    {
        await base.PackageHandler(session, message);
        if (message is INetworkMessagePackage messageObject)
        {
            if (Setting.IsDebug && Setting.IsDebugReceive)
            {
                // 当需要打印心跳，或当前非心跳消息时才输出日志
                // if (Setting.IsDebugReceiveHeartBeat || messageObject.Header.OperationType != (byte)MessageOperationType.HeartBeat)
                // {
                //    
                //     LogHelper.Debug($"---收到[{from} To {ServerType}]  {message.ToFormatMessageString()}");
                // }
            }

            var handler = HotfixManager.GetTcpHandler(messageObject.Header.MessageId);
            await InvokeMessageHandler(handler, messageObject.DeserializeMessageObject(), new DefaultNetWorkChannel(session, Setting));
        }
    }

    public override async Task StopAsync(string message = "")
    {
        EventDispatcher.Dispatch(0, (int)EventId.ServiceOffline, new ServiceOfflineEventArgs(Setting.ServerType, Setting.ServerInstanceId, "Stopped", DateTime.UtcNow));
        await base.StopAsync(message);
    }

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerType = GameServerConst.Social.Name,
                ServerId = GameServerConst.Social.Id,
                InnerPort = 29400,
                HttpIsDevelopment = true,
                IsDebug = true,
                IsDebugSend = true,
                IsDebugReceive = true,
                IsDebugReceiveHeartBeat = false,
                IsDebugSendHeartBeat = false,
                DataBaseUrl = "mongodb://gameframex:7bmEw5HS0otl_KNpnsGeMOq@175.178.65.215:27017/?authSource=admin",
                DataBaseName = "gameframex",
            };
        }

        base.Init();
    }
}
