using GameFrameX.Hotfix.Logic.Game.Room;

namespace GameFrameX.Hotfix.Logic.Game.RockPaperScissors;

internal static class RockPaperScissorsHandlerHelper
{
    public static long GetCurrentPlayerId(INetworkChannel networkChannel)
    {
        return RoomHandlerHelper.GetCurrentPlayerId(networkChannel);
    }
}
