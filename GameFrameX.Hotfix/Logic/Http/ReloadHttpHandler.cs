// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws and regulations of international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
// ==========================================================================================


using GameFrameX.Network.HTTP;
using GameFrameX.Foundation.Http.Normalization;

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
    /// 按版本号触发热更模块重载。
    /// </summary>
    /// <param name="context">HTTP 请求上下文（含查询参数）。</param>
    /// <returns>JSON 结果字符串。</returns>
    public override async Task<string> ActionAsync(HttpActionContext context)
    {
        var parameters = context.Parameters;
        if (parameters.TryGetValue("version", out var version))
        {
            await HotfixManager.LoadHotfixModuleAsync(null, new HotfixLoadOptions { Version = version.ToString() });
            return HttpJsonResultData<string>.SuccessString(null);
        }

        return HttpJsonResultData<string>.ParamErrorString();
    }
}
