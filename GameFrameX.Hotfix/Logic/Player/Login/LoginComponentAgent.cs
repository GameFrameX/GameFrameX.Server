using GameFrameX.Apps;
using GameFrameX.Apps.Account.Login.Component;
using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.Core.Session;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DataBase;
using GameFrameX.Hotfix.Logic.Server;

namespace GameFrameX.Hotfix.Logic.Player.Login;

public class LoginComponentAgent : StateComponentAgent<LoginComponent, LoginState>
{
    public async Task OnLoginAsync(ReqLogin request, RespLogin response)
    {
        if (request.UserName.IsNullOrEmpty() || request.Password.IsNullOrEmpty())
        {
            response.ErrorCode = (int)OperationStatusCode.AccountCannotBeNull;
            return;
        }

        AppMetrics.AccountLogin.Add(1);
        var loginState = await GameDb.FindAsync<LoginState>(m => m.UserName == request.UserName && m.Password == request.Password, false);
        if (loginState == null)
        {
            AppMetrics.AccountRegister.Add(1);
            var accountId = ActorIdGenerator.GetUniqueId();
            loginState = new LoginState
            {
                Id = accountId,
                UserName = request.UserName,
                Password = request.Password,
            };
            await GameDb.AddOrUpdateAsync(loginState);
        }

        response.UniqueId = request.UniqueId;
        response.Code = loginState.State;
        response.CreateTime = loginState.CreatedTime;
        response.Level = loginState.Level;
        response.Id = loginState.Id;
        response.RoleName = loginState.NickName;
    }

    public async Task OnGetPlayerListAsync(ReqPlayerList request, RespPlayerList response)
    {
        response.UniqueId = request.UniqueId;
        response.PlayerList = new List<PlayerInfo>();
        if (request.Id == default)
        {
            return;
        }

        AppMetrics.PlayerListQuery.Add(1);
        var playerList = await GameDb.FindListAsync<PlayerState>(m => m.AccountId == request.Id);
        if (playerList == null)
        {
            return;
        }

        foreach (var playerState in playerList)
        {
            response.PlayerList.Add(new PlayerInfo
            {
                Id = playerState.Id,
                Name = playerState.Name,
                Level = playerState.Level,
                State = playerState.State,
                Avatar = playerState.Avatar,
            });
        }
    }

    public async Task OnPlayerCreateAsync(ReqPlayerCreate request, RespPlayerCreate response)
    {
        var playerState = new PlayerState
        {
            Id = ActorIdGenerator.GetActorId(GlobalConst.ActorTypePlayer),
            AccountId = request.Id,
            Name = request.Name,
            Level = (uint)Utility.RandomHelper.Next(1, 50),
            State = 0,
            Avatar = (uint)Utility.RandomHelper.Next(1, 50),
        };
        AppMetrics.PlayerCreate.Add(1);
        await GameDb.SaveOneAsync(playerState);

        response.UniqueId = request.UniqueId;
        response.PlayerInfo = new PlayerInfo
        {
            Id = playerState.Id,
            Name = playerState.Name,
            Level = playerState.Level,
            State = playerState.State,
            Avatar = playerState.Avatar,
        };
    }

    /// <summary>
    /// 使用角色ID登录
    /// </summary>
    /// <param name="workChannel"></param>
    /// <param name="reqLogin"></param>
    /// <param name="response"></param>
    public async Task OnPlayerLoginAsync(INetworkChannel workChannel, ReqPlayerLogin reqLogin, RespPlayerLogin response)
    {
        var playerState = await OwnerComponent.OnPlayerLoginAsync(reqLogin);
        if (playerState == null)
        {
            //角色找不到？
            return;
        }

        // 绑定玩家ID
        workChannel.SetData(GlobalConst.ActorIdKey, playerState.Id);
        var playerComponentAgent = await ActorManager.GetComponentAgentAsync<PlayerComponentAgent>(playerState.Id);
        await playerComponentAgent.OnPlayerLoginAsync(workChannel, playerState, response);
    }
}
