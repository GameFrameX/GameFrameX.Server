using GameFrameX.Core.Components;
using GameFrameX.DataBase.Abstractions;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Message;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Launcher.StartUp.Social;

/// <summary>
/// 游戏服务器
/// </summary>
[StartUpTag(GlobalConst.SocialServiceName)]
internal sealed partial class AppStartUpSocial : AppStartUpBase
{
    protected override bool IsRegisterToDiscoveryCenter { get; set; } = true;

    public override async Task StartAsync()
    {
        try
        {
            var aopHandlerTypes = AssemblyHelper.GetRuntimeImplementTypeNamesInstance<IHttpAopHandler>();
            aopHandlerTypes.Sort((handlerX, handlerY) => handlerX.Priority.CompareTo(handlerY.Priority));
            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.DatabaseServiceStartBegin));
            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ActorLimitConfigBegin));
            ActorLimit.Init(ActorLimit.RuleType.None);
            LogHelper.Debug(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ActorLimitConfigEnd));
            var initResult = await GameDb.Init<MongoDbService>(new DbOptions { ConnectionString = Setting.DataBaseUrl, Name = Setting.DataBaseName, });
            if (initResult == false)
            {
                throw new InvalidOperationException(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.DatabaseServiceStartFailed));
            }

            await ComponentRegister.Init(typeof(AppsHandler).Assembly);
            HotfixManager.LoadHotfix(Setting);
            await StartServerAsync<DefaultMessageDecoderHandler, DefaultMessageEncoderHandler>(new DefaultMessageCompressHandler(), new DefaultMessageDecompressHandler(), HotfixManager.GetListHttpHandler(), HotfixManager.GetHttpHandler, aopHandlerTypes);

            await AppExitToken;
        }
        catch (Exception e)
        {
            LogHelper.Info(LocalizationService.GetString(GameFrameX.Localization.Keys.Launcher.ServerExecutionException, e));
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

    protected override void Init()
    {
        if (Setting == null)
        {
            Setting = new AppSetting
            {
                ServerType = GlobalConst.SocialServiceName,
                ServerId = GlobalConst.SocialServiceServerId,
                InnerPort = 29400,
                HttpIsDevelopment = true,
                IsDebug = true,
                IsDebugSend = true,
                IsDebugReceive = true,
                IsDebugReceiveHeartBeat = false,
                IsDebugSendHeartBeat = false,
                DataBaseUrl = "mongodb://gameframex:7bmEw5HS0otl_KNpnsGeMOq@175.178.65.215:27017/?authSource=admin",
                DataBaseName = "gameframex"
            };
        }

        base.Init();
    }
}