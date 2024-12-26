using GameFrameX.Apps.Common.Session;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 获取在线人数
/// http://localhost:20001/game/api/GetOnlinePlayer
/// </summary>
[HttpMessageMapping(typeof(GetOnlinePlayerHttpHandler))]
public sealed class GetOnlinePlayerHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override Task<string> Action(string ip, string url, Dictionary<string, object> parameters)
    {
        var response = new GetOnlinePlayerResponse();
        response.Count = SessionManager.Count();
        var res = HttpResult.CreateOk($"当前在线人数:{response.Count}", JsonHelper.Serialize(response));
        return Task.FromResult(res);
    }

    private class GetOnlinePlayerResponse
    {
        public int Count { get; set; }
    }
}