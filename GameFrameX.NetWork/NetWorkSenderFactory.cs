using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

/// <summary>
/// 网络发送器工厂。
/// </summary>
internal static class NetWorkSenderFactory
{
    private static readonly IReadOnlyDictionary<Type, INetWorkSenderProvider> SenderProviders = BuildProviders();

    public static INetWorkSender Create(IGameAppSession session)
    {
        ArgumentNullException.ThrowIfNull(session, nameof(session));
        var sessionType = session.GetType();
        return ResolveProvider(sessionType).Create(session);
    }

    private static IReadOnlyDictionary<Type, INetWorkSenderProvider> BuildProviders()
    {
        var providers = new INetWorkSenderProvider[]
        {
            new WebSocketNetWorkSenderProvider(),
            new SessionNetWorkSenderProvider(),
        };
        return providers.ToDictionary(p => p.SessionType, p => p);
    }

    private static INetWorkSenderProvider ResolveProvider(Type sessionType)
    {
        return SenderProviders
                   .Where(pair => pair.Key == sessionType || pair.Key.IsAssignableFrom(sessionType))
                   .OrderBy(pair => pair.Key == sessionType ? 0 : 1)
                   .Select(pair => pair.Value)
                   .First();
    }
}
