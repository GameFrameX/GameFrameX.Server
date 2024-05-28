using GameFrameX.Hotfix.Launcher;
using GameFrameX.Launcher.StartUp;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame : AppStartUpService
{
    static MessageGameDecoderHandler messageDecoderHandler = new MessageGameDecoderHandler();
    static MessageGameEncoderHandler messageEncoderHandler = new MessageGameEncoderHandler();


    public async void Start()
    {
        await EnterAsync();
        StartGatewayClient();
    }

    public override Task Stop(string message = "")
    {
        DisconnectToGateWay();
        return base.Stop(message);
    }

    public AppStartUpHotfixGame() : base(messageEncoderHandler, messageDecoderHandler)
    {
    }
}