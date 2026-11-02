using GameFrameX.Core.Session;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 获取在线人数
/// http://localhost:20001/game/api/GetOnlinePlayer
/// </summary>
[HttpMessageMapping(typeof(GetOnlinePlayerHttpHandler), null, typeof(GetOnlinePlayerResponse))]
[RequireHttpSignature]
[Description("获取在线人数")]
public sealed class GetOnlinePlayerHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<string> ActionAsync(HttpActionContext context)
    {
        var response = new GetOnlinePlayerResponse
        {
            Count = PlayerSessionManager.Instance.Count(),
        };
        var res = HttpJsonResultData<string>.SuccessString($"当前在线人数:{response.Count}", JsonHelper.Serialize(response));
        return Task.FromResult(res);
    }
}
