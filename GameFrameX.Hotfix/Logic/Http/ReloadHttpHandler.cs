using GameFrameX.NetWork.HTTP;

namespace GameFrameX.Hotfix.Logic.Http;

/// <summary>
/// 热更新
/// http://localhost:20001/game/api/Reload?version=1.0.0
/// </summary>
[HttpMessageMapping(typeof(ReloadHttpHandler))]
public sealed class ReloadHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 
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
            return Task.FromResult(HttpResult.CreateOk()).Result;
        }

        var result = HttpResult.CreateErrorParam($"参数错误 {nameof(version)}");

        return Task.FromResult(result).Result;
    }
}