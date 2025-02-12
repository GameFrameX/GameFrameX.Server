// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Apps.Common.Session;

namespace GameFrameX.Hotfix.Logic.Http.Player;

/// <summary>
/// 将指定角色的玩家从当前服务断开
/// http://localhost:20001/game/api/KickOffLineByUserIdPlayer
/// </summary>
[HttpMessageMapping(typeof(KickOffLineByUserIdPlayerHttpHandler))]
[HttpMessageRequest(typeof(KickOffLineByUserIdPlayerRequest))]
[Description("将指定角色的玩家从当前服务断开")]
public sealed class KickOffLineByUserIdPlayerHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="requestBase"></param>
    /// <returns></returns>
    public override Task<string> Action(string ip, string url, HttpMessageRequestBase requestBase)
    {
        var request = (KickOffLineByUserIdPlayerRequest)requestBase;
        SessionManager.KickOffLineByUserId(request.RoleId);
        return Task.FromResult(HttpJsonResult.SuccessString());
    }
}

public sealed class KickOffLineByUserIdPlayerRequest : HttpMessageRequestBase
{
    /// <summary>
    /// 角色Id
    /// </summary>
    [Description("角色Id")]
    [Required]
    [Range(1, long.MaxValue)]
    public long RoleId { get; set; }
}