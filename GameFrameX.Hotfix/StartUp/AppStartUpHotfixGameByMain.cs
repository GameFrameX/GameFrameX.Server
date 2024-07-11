using GameFrameX.Hotfix.Launcher;
using GameFrameX.Launcher.StartUp;
using GameFrameX.StartUp;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame : AppStartUpBase
{
    static MessageGameDecoderHandler messageDecoderHandler = new MessageGameDecoderHandler();
    static MessageGameEncoderHandler messageEncoderHandler = new MessageGameEncoderHandler();

    protected override bool IsEnableReconnection { get; } = false;

    protected override bool IsEnableHeartBeat { get; } = false;


    public override Task StartAsync()
    {
        return Task.CompletedTask;
    }

    /*public override Task Stop(string message = "")
    {
        DisconnectToGateWay();
        return base.Stop(message);
    }*/
}