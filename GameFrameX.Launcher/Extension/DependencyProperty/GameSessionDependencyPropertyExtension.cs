using GameFrameX.NetWork;
using TouchSocket.Core;
using TouchSocket.Sockets;

namespace GameFrameX.Launcher.Extension.DependencyProperty;

public static class GameSessionDependencyPropertyExtension
{
    public static readonly DependencyProperty<IGameSession> GameSessionProperty = DependencyProperty<IGameSession>.Register("GameSession", null);

    public static ISocketClient SetGameSession(this ISocketClient config, IGameSession value)
    {
        config.SetValue(GameSessionProperty, value);
        return config;
    }

    public static IGameSession GetGameSession(this ISocketClient config)
    {
        return config.GetValue(GameSessionProperty);
    }
}