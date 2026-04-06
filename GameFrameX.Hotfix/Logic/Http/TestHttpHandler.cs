// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


namespace GameFrameX.Hotfix.Logic.Http;
using GameFrameX.Hotfix.Logic.Player.Friend;

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
    /// 测试入口，支持普通回包与跨进程通讯示例。
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="url"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public override async Task<string> Action(string ip, string url, Dictionary<string, object> parameters)
    {
        if (TryGetMode(parameters, out var mode) && string.Equals(mode, "cross-process-friend", StringComparison.OrdinalIgnoreCase))
        {
            var friendComponentAgent = await ActorManager.GetComponentAgent<FriendComponentAgent>();
            var friendResponse = new RespFriendList();
            await friendComponentAgent.OnFriendList(null, new ReqFriendList(), friendResponse);
            var crossResponse = new HttpCrossProcessFriendResponse
            {
                Mode = mode,
                ErrorCode = friendResponse.ErrorCode,
                FriendCount = friendResponse.Friends?.Count ?? 0,
                Friends = friendResponse.Friends ?? new List<FriendInfo>(),
            };
            return HttpJsonResult.SuccessString(crossResponse);
        }

        var response = new HttpTestResponse
        {
            Message = "hello",
        };
        return HttpJsonResult.SuccessString(response);
    }

    /// <summary>
    /// 获取测试模式参数。
    /// </summary>
    /// <param name="parameters">请求参数</param>
    /// <param name="mode">模式值</param>
    /// <returns>是否获取成功</returns>
    private static bool TryGetMode(Dictionary<string, object> parameters, out string mode)
    {
        mode = string.Empty;
        if (!parameters.TryGetValue("mode", out var modeValue) || modeValue == null)
        {
            return false;
        }

        mode = modeValue.ToString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(mode);
    }
}

public sealed class HttpTestResponse : HttpMessageResponseBase
{
    [Description("返回信息")] public string Message { get; set; }
}

public sealed class HttpCrossProcessFriendResponse : HttpMessageResponseBase
{
    [Description("测试模式")] public string Mode { get; set; }
    [Description("错误码")] public int ErrorCode { get; set; }
    [Description("好友数量")] public int FriendCount { get; set; }
    [Description("好友列表")] public List<FriendInfo> Friends { get; set; }
}
