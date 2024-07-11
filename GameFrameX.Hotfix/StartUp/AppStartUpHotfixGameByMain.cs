using GameFrameX.StartUp;

namespace GameFrameX.Hotfix.StartUp;

/// <summary>
/// 路由服务器.最后启动。
/// </summary>
internal partial class AppStartUpHotfixGame : AppStartUpBase
{
    /// <summary>
    /// 是否启用重连
    /// </summary>
    protected override bool IsEnableReconnection { get; } = false;

    /// <summary>
    /// 是否启用心跳
    /// </summary>
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