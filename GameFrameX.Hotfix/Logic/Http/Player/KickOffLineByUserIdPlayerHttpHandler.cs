using GameFrameX.Core.Session;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 将指定角色的玩家从当前服务断开
/// http://localhost:20001/game/api/KickOffLineByUserIdPlayer
/// </summary>
[HttpMessageMapping(typeof(KickOffLineByUserIdPlayerHttpHandler), typeof(KickOffLineByUserIdPlayerRequest))]
[RequireHttpSignature]
[Description("将指定角色的玩家从当前服务断开")]
public sealed class KickOffLineByUserIdPlayerHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<string> ActionAsync(HttpActionContext context)
    {
        var kickOffRequest = (KickOffLineByUserIdPlayerRequest)context.Request;
        PlayerSessionManager.Instance.KickOffLineByPlayerId(kickOffRequest.PlayerId);
        return Task.FromResult(HttpJsonResultData<string>.SuccessString());
    }
}
