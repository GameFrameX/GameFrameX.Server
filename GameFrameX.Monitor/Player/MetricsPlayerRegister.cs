using Prometheus;

namespace GameFrameX.Monitor.Player;

public static class MetricsPlayerRegister
{
    static Counter _getPlayerListCounterOptions;

    public static Counter GetPlayerListCounterOptions
    {
        get { return _getPlayerListCounterOptions ??= Prometheus.Metrics.CreateCounter("player_get_player_list", "Player"); }
    }

    static Counter _createCounterOptions;

    public static Counter CreateCounterOptions
    {
        get { return _createCounterOptions ??= Prometheus.Metrics.CreateCounter("player_create", "Player"); }
    }

    static Counter _loginCounterOptions;

    public static Counter LoginCounterOptions
    {
        get { return _loginCounterOptions ??= Prometheus.Metrics.CreateCounter("player_login", "Player"); }
    }

    static Counter _heartBeatCounterOptions;

    public static Counter HeartBeatCounterOptions
    {
        get { return _heartBeatCounterOptions ??= Prometheus.Metrics.CreateCounter("player_heart_beat", "Account"); }
    }
}