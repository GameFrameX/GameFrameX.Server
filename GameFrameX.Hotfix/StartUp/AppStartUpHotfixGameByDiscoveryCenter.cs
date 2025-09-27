using GameFrameX.Proto.BuiltIn;

namespace GameFrameX.Hotfix.StartUp;

internal partial class AppStartUpHotfixGame
{
    protected override void GameAppClientOnConnected()
    {
        base.GameAppClientOnConnected();
        // 注册服务器到发现中心
        var reqRegisterServer = new ReqRegisterServer
        {
            ServerId = Setting.ServerId,
            ServerName = Setting.ServerName,
            ServerType = Setting.ServerType,
            ServerInstanceId = Setting.ServerInstanceId,
            InnerIp = Setting.InnerIp,
            InnerPort = Setting.InnerPort,
            OuterIp = Setting.OuterIp,
            OuterPort = Setting.OuterPort,
        };
        Send(reqRegisterServer);
    }
}