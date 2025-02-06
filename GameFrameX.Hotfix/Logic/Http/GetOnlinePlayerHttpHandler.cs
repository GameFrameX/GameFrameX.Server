using GameFrameX.Apps.Common.Session;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 获取在线人数
/// http://localhost:20001/game/api/GetOnlinePlayer
/// </summary>
[HttpMessageMapping(typeof(GetOnlinePlayerHttpHandler))]
[HttpMessageRequest(typeof(GetOnlinePlayerRequest))]
[HttpMessageResponse(typeof(GetOnlinePlayerResponse))]
[Description("获取在线人数")]
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
        var res = HttpJsonResult.SuccessString(response);
        return Task.FromResult(res);
    }
}

/// <summary>
/// 获取在线人数请求
/// </summary>
public sealed class GetOnlinePlayerRequest : HttpMessageRequestBase
{
    // 空请求
}

/// <summary>
/// 获取在线人数响应
/// </summary>
public sealed class GetOnlinePlayerResponse : HttpMessageResponseBase
{
    /// <summary>
    /// 在线人数
    /// </summary>
    [Description("当前在线玩家数量")]
    public int Count { get; set; }
}