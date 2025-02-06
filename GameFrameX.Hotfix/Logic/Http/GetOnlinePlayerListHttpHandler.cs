using GameFrameX.Apps.Common.Session;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 获取在线玩家列表
/// http://localhost:20001/game/api/GetOnlinePlayerList
/// </summary>
[HttpMessageMapping(typeof(GetOnlinePlayerListHttpHandler))]
[HttpMessageRequest(typeof(GetOnlinePlayerListRequest))]
[HttpMessageResponse(typeof(GetOnlinePlayerListResponse))]
[Description("获取在线玩家列表")]
public sealed class GetOnlinePlayerListHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase request)
    {
        GetOnlinePlayerListRequest parameters = (GetOnlinePlayerListRequest)request;
        var response = SessionManager.GetPageList(parameters.PageSize, parameters.PageIndex);
        var res = HttpJsonResult.SuccessString("当前在线玩家", JsonHelper.Serialize(response));
        return Task.FromResult(res);
    }
}

public sealed class GetOnlinePlayerListResponse : HttpMessageResponseBase
{
    public List<Session> List { get; set; }
}

public sealed class GetOnlinePlayerListRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public int PageIndex { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required, Range(1, int.MaxValue)]
    public int PageSize { get; set; }
}