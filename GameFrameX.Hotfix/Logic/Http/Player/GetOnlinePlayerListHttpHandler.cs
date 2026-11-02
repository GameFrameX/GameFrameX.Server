using GameFrameX.Core.Session;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 获取在线玩家列表
/// http://localhost:20001/game/api/GetOnlinePlayerList
/// </summary>
[HttpMessageMapping(typeof(GetOnlinePlayerListHttpHandler), typeof(GetOnlinePlayerListRequest), typeof(GetOnlinePlayerListResponse))]
[RequireHttpSignature]
[Description("获取在线玩家列表")]
public sealed class GetOnlinePlayerListHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<string> ActionAsync(HttpActionContext context)
    {
        GetOnlinePlayerListRequest parameters = (GetOnlinePlayerListRequest)context.Request;
        var response = PlayerSessionManager.Instance.GetPageList(parameters.PageSize, parameters.PageIndex);
        var res = HttpJsonResultData<string>.SuccessString("当前在线玩家", JsonHelper.Serialize(response));
        return Task.FromResult(res);
    }
}
