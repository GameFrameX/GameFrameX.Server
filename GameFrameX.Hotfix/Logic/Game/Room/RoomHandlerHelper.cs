using GameFrameX.Core.Session;

namespace GameFrameX.Hotfix.Logic.Game.Room;

internal static class RoomHandlerHelper
{
    public static long GetCurrentPlayerId(INetworkChannel networkChannel)
    {
        var sessionId = networkChannel.GameAppSession?.SessionId;
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return 0;
        }

        var session = PlayerSessionManager.Instance.Get(sessionId);
        return session?.PlayerId ?? 0;
    }
}
