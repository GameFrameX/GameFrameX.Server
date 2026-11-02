using GameFrameX.Apps;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Network.Messages;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 获取角色列表
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerListHttpHandler))]
[Description("获取角色列表,仅限前端调用")]
public sealed class ReqPlayerListHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> ActionMessageObjectAsync(HttpActionContext context)
    {
        var messageObject = context.MessageObject;
        var reqPlayerList = (ReqPlayerList)messageObject;


        var respPlayerList = new RespPlayerList
        {
            UniqueId = reqPlayerList.UniqueId,
            PlayerList = new List<PlayerInfo>(),
        };
        if (reqPlayerList.Id == default)
        {
            return respPlayerList;
        }

        var playerList = await GetPlayerListAsync(reqPlayerList);

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

    private async Task<List<PlayerState>> GetPlayerListAsync(ReqPlayerList reqPlayerList)
    {
        AppMetrics.PlayerListQuery.Add(1);
        return await GameDb.FindListAsync<PlayerState>(m => m.AccountId == reqPlayerList.Id);
    }
}
