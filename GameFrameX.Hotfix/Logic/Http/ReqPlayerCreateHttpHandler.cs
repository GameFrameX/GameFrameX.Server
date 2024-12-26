using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Hotfix.Common;
using GameFrameX.Monitor.Account;
using GameFrameX.Monitor.Player;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 角色创建
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerCreateHttpHandler))]
public sealed class ReqPlayerCreateHttpHandler : BaseHttpHandler
{
    public override async Task<string> Action(string ip, string url, Dictionary<string, object> paramMap)
    {
        var reqPlayerCreate = JsonHelper.Deserialize<ReqPlayerCreate>(JsonHelper.Serialize(paramMap));

        var playerState = await OnPlayerCreate(reqPlayerCreate);
        var respPlayerCreate = new RespPlayerCreate
        {
            UniqueId = reqPlayerCreate.UniqueId,
            PlayerInfo = new PlayerInfo
            {
                Id = playerState.Id,
                Name = playerState.Name,
                Level = playerState.Level,
                State = playerState.State,
                Avatar = playerState.Avatar,
            },
        };
        return HttpResult.Create(JsonHelper.Serialize(respPlayerCreate));
    }

    private async Task<PlayerState> OnPlayerCreate(ReqPlayerCreate reqPlayerCreate)
    {
        var playerState = new PlayerState
        {
            Id = ActorIdGenerator.GetActorId(GlobalConst.ActorTypePlayer),
            AccountId = reqPlayerCreate.Id,
            Name = reqPlayerCreate.Name,
            Level = (uint)Utility.Random.Next(1, 50),
            State = 0,
            Avatar = (uint)Utility.Random.Next(1, 50),
        };
        MetricsPlayerRegister.CreateCounterOptions.Inc();
        await GameDb.SaveOneAsync(playerState);
        return playerState;
    }
}