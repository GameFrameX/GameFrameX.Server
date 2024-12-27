using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Monitor.Player;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 获取角色列表
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerListHttpHandler))]
public sealed class ReqPlayerListHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> Action(string ip, string url, Dictionary<string, object> paramMap, MessageObject messageObject)
    {
        ReqPlayerList reqPlayerList = messageObject as ReqPlayerList;

        var respPlayerList = new RespPlayerList
        {
            UniqueId = reqPlayerList.UniqueId,
            PlayerList = new List<PlayerInfo>(),
        };
        if (reqPlayerList.Id == default)
        {
            return respPlayerList;
        }

        var playerList = await GetPlayerList(reqPlayerList);

        if (playerList != null)
        {
            foreach (var playerState in playerList)
            {
                var playerInfo = new PlayerInfo
                {
                    Id = playerState.Id,
                    Name = playerState.Name,
                    Level = playerState.Level,
                    State = playerState.State,
                    Avatar = playerState.Avatar,
                };
                respPlayerList.PlayerList.Add(playerInfo);
            }
        }

        return respPlayerList;
    }

    private async Task<List<PlayerState>> GetPlayerList(ReqPlayerList reqPlayerList)
    {
        MetricsPlayerRegister.GetPlayerListCounterOptions.Inc();
        return await GameDb.FindListAsync<PlayerState>(m => m.AccountId == reqPlayerList.Id);
    }
}