using GameFrameX.Apps.Common.Session;
using GameFrameX.Foundation.Http.Normalization;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 将指定角色的玩家从当前服务断开
/// http://localhost:20001/game/api/KickOffLineByUserIdPlayer
/// </summary>
[HttpMessageMapping(typeof(KickOffLineByUserIdPlayerHttpHandler))]
[Description("将指定角色的玩家从当前服务断开")]
public sealed class KickOffLineByUserIdPlayerHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override Task<string> Action(string ip, string url, Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("roleId", out var roleId) && !string.IsNullOrEmpty(roleId.ToString()))
        {
            SessionManager.KickOffLineByUserId(Convert.ToInt64(roleId));

            return Task.FromResult(HttpJsonResult.SuccessString());
        }

        var res = HttpJsonResult.ParamErrorString();
        return Task.FromResult(res);
    }
}