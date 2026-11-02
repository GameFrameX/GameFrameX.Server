using GameFrameX.Apps;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Network.Messages;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 角色创建
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerCreateHttpHandler))]
[Description("角色创建,仅限前端调用")]
public sealed class ReqPlayerCreateHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> ActionMessageObjectAsync(HttpActionContext context)
    {
        var messageObject = context.MessageObject;
        var reqPlayerCreate = messageObject as ReqPlayerCreate;

        var playerState = await OnPlayerCreateAsync(reqPlayerCreate);
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
        return respPlayerCreate;
    }

    private async Task<PlayerState> OnPlayerCreateAsync(ReqPlayerCreate reqPlayerCreate)
    {
        var playerState = new PlayerState
        {
            Id = ActorIdGenerator.GetActorId(GlobalConst.ActorTypePlayer),
            AccountId = reqPlayerCreate.Id,
            Name = reqPlayerCreate.Name,
            Level = (uint)Utility.RandomHelper.Next(1, 50),
            State = 0,
            Avatar = (uint)Utility.RandomHelper.Next(1, 50),
        };
        AppMetrics.PlayerCreate.Add(1);
        await GameDb.SaveOneAsync(playerState);
        return playerState;
    }
}
