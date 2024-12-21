using Prometheus;

namespace GameFrameX.Monitor.Player;

/// <summary>
/// 玩家监控帮助类
/// </summary>
public static class MetricsPlayerRegister
{
    private static Counter _getPlayerListCounterOptions;

    private static Counter _createCounterOptions;

    private static Counter _loginCounterOptions;

    private static Counter _heartBeatCounterOptions;

    /// <summary>
    /// 获取玩家列表
    /// </summary>
    public static Counter GetPlayerListCounterOptions
    {
        get { return _getPlayerListCounterOptions ??= Metrics.CreateCounter("player_get_player_list", "获取玩家列表"); }
    }

    /// <summary>
    /// 玩家角色创建数量
    /// </summary>
    public static Counter CreateCounterOptions
    {
        get { return _createCounterOptions ??= Metrics.CreateCounter("player_create", "玩家角色创建数量"); }
    }

    /// <summary>
    /// 玩家角色登录
    /// </summary>
    public static Counter LoginCounterOptions
    {
        get { return _loginCounterOptions ??= Metrics.CreateCounter("player_login", "玩家角色登录"); }
    }

    /// <summary>
    /// 玩家角色心跳
    /// </summary>
    public static Counter HeartBeatCounterOptions
    {
        get { return _heartBeatCounterOptions ??= Metrics.CreateCounter("player_heart_beat", "玩家角色心跳"); }
    }
}