using GameFrameX.NetWork;

namespace GameFrameX.Launcher;

public static class GameClientSessionManager
{
    static readonly ConcurrentDictionary<string, INetChannel> Sessions = new ConcurrentDictionary<string, INetChannel>();

    public static INetChannel GetSession(string id)
    {
        Sessions.TryGetValue(id, out var session);
        return session;
    }

    public static void SetSession(string id, INetChannel session)
    {
        Sessions.TryAdd(id, session);
    }

    public static void RemoveSession(string id)
    {
        Sessions.TryRemove(id, out _);
    }
}