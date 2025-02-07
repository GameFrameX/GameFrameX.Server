using GameFrameX.Foundation.Http.Normalization;
using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 热更新
/// http://localhost:20001/game/api/Reload?version=1.0.0
/// </summary>
[HttpMessageMapping(typeof(ReloadHttpHandler))]
[Description("热更新")]
public sealed class ReloadHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task<string> Action(string ip, string url, Dictionary<string, object> parameters)
    {
        if (parameters.TryGetValue("version", out var version))
        {
            await HotfixManager.LoadHotfixModule(null, version.ToString());
            return Task.FromResult(HttpJsonResult.SuccessString(null)).Result;
        }

        var result = HttpJsonResult.ParamErrorString();

        return Task.FromResult(result).Result;
    }
}