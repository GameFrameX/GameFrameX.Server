using GameFrameX.Hotfix.Launcher;
using GameFrameX.Launcher.StartUp;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame : AppStartUpService
{
    static             MessageGameDecoderHandler messageDecoderHandler = new MessageGameDecoderHandler();
    static             MessageGameEncoderHandler messageEncoderHandler = new MessageGameEncoderHandler();
    protected override bool                      IsConnectDiscoveryServer { get; } = false;
    protected override bool                      IsRequestConnectServer   { get; } = false;

    public override Task StartAsync()
    {
        RunServer(false);
        return base.StartAsync();
    }

    public AppStartUpHotfixGame() : base(messageEncoderHandler, messageDecoderHandler)
    {
    }
}