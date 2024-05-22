using Prometheus;

namespace GameFrameX.Monitor.Player;

public static class MetricsPlayerRegister
{
    static Counter _getPlayerListCounterOptions;

    /// <summary>
    /// 获取玩家列表
    /// </summary>
    public static Counter GetPlayerListCounterOptions
    {
        get { return _getPlayerListCounterOptions ??= Prometheus.Metrics.CreateCounter("player_get_player_list", "获取玩家列表"); }
    }

    static Counter _createCounterOptions;

    /// <summary>
    /// 玩家角色创建数量
    /// </summary>
    public static Counter CreateCounterOptions
    {
        get { return _createCounterOptions ??= Prometheus.Metrics.CreateCounter("player_create", "玩家角色创建数量"); }
    }

    static Counter _loginCounterOptions;

    /// <summary>
    /// 玩家角色登录
    /// </summary>
    public static Counter LoginCounterOptions
    {
        get { return _loginCounterOptions ??= Prometheus.Metrics.CreateCounter("player_login", "玩家角色登录"); }
    }

    static Counter _heartBeatCounterOptions;

    /// <summary>
    /// 玩家角色心跳
    /// </summary>
    public static Counter HeartBeatCounterOptions
    {
        get { return _heartBeatCounterOptions ??= Prometheus.Metrics.CreateCounter("player_heart_beat", "玩家角色心跳"); }
    }
}