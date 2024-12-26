using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Monitor.Player;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 获取角色列表
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerListHttpHandler))]
public sealed class ReqPlayerListHttpHandler : BaseHttpHandler
{
    public override async Task<string> Action(string ip, string url, Dictionary<string, object> paramMap)
    {
        ReqPlayerList reqPlayerList = JsonHelper.Deserialize<ReqPlayerList>(JsonHelper.Serialize(paramMap));

        if (reqPlayerList.Id == default)
        {
            return HttpResult.Create(HttpStatusCode.NotFound, "账号不存在");
        }

        var playerList = await GetPlayerList(reqPlayerList);

        var respPlayerList = new RespPlayerList
        {
            UniqueId = reqPlayerList.UniqueId,
            PlayerList = new List<PlayerInfo>(),
        };
        if (playerList != null)
        {
            foreach (var playerState in playerList)
            {
                var playerInfo = new PlayerInfo
                {
                    Id = playerState.Id,
                    Name = playerState.Name,
                };
                playerInfo.Level = playerState.Level;
                playerInfo.State = playerState.State;
                playerInfo.Avatar = playerState.Avatar;
                respPlayerList.PlayerList.Add(playerInfo);
            }
        }

        return HttpResult.Create(JsonHelper.Serialize(respPlayerList));
    }

    private async Task<List<PlayerState>> GetPlayerList(ReqPlayerList reqPlayerList)
    {
        MetricsPlayerRegister.GetPlayerListCounterOptions.Inc();
        return await GameDb.FindListAsync<PlayerState>(m => m.AccountId == reqPlayerList.Id);
    }
}