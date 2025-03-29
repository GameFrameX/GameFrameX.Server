using System.Diagnostics.Metrics;

namespace GameFrameX.Monitor.Player;

/// <summary>
/// 玩家监控帮助类
/// </summary>
public static class MetricsPlayerHelper
{
    private static Counter<ulong> _createCounterOptions;

    private static Counter<ulong> _loginCounterOptions;

    private const string ModuleName = "player.";

    /// <summary>
    /// 玩家角色创建数量总和
    /// </summary>
    public static Counter<ulong> CreateCounterOptions
    {
        get { return _createCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}create_total", "人", "玩家角色创建数量总和"); }
    }

    /// <summary>
    /// 玩家角色登录次数
    /// </summary>
    public static Counter<ulong> LoginCounterOptions
    {
        get { return _loginCounterOptions ??= MetricsHelper.Meter.CreateCounter<ulong>($"{ModuleName}login_counter", "次", "玩家角色登录次数"); }
    }

    private static Gauge<int> _onlineCounterOptions;

    /// <summary>
    /// 在线玩家数量
    /// </summary>
    public static Gauge<int> OnlineCounterOptions
    {
        get { return _onlineCounterOptions ??= MetricsHelper.Meter.CreateGauge<int>($"{ModuleName}online", "人", "在线玩家数量"); }
    }
}