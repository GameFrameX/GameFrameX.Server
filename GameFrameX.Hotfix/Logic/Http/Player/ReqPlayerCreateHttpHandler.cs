// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Monitor.Player;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 角色创建
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerCreateHttpHandler))]
[Description("角色创建,仅限前端调用")]
public sealed class ReqPlayerCreateHttpHandler : BaseHttpHandler
{
    public override async Task<MessageObject> Action(string ip, string url, Dictionary<string, object> paramMap, MessageObject messageObject)
    {
        var reqPlayerCreate = messageObject as ReqPlayerCreate;

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
        return respPlayerCreate;
    }

    private async Task<PlayerState> OnPlayerCreate(ReqPlayerCreate reqPlayerCreate)
    {
        var playerState = new PlayerState
        {
            Id = ActorIdGenerator.GetActorId(GlobalConst.ActorTypePlayer),
            AccountId = reqPlayerCreate.Id,
            Name = reqPlayerCreate.Name,
            Level = (uint)RandomHelper.Next(1, 50),
            State = 0,
            Avatar = (uint)RandomHelper.Next(1, 50),
        };
        // MetricsPlayerRegister.CreateCounterOptions.Inc();
        await GameDb.AddOrUpdateAsync(playerState);
        return playerState;
    }
}