using GameFrameX.Foundation.Http.Normalization;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 测试
/// http://localhost:20001/game/api/test
/// </summary>
[HttpMessageMapping(typeof(TestHttpHandler))]
[HttpMessageResponse(typeof(HttpTestResponse))]
[Description("测试通讯接口。没有实际用途")]
public sealed class TestHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override Task<string> Action(string ip, string url, Dictionary<string, object> parameters)
    {
        var response = new HttpTestResponse
        {
            Message = "hello",
        };
        return Task.FromResult(HttpJsonResult.SuccessString(response));
    }
}

public sealed class HttpTestResponse : HttpMessageResponseBase
{
    [Description("返回信息")] public string Message { get; set; }
}