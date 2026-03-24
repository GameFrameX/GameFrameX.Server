// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using System.Reflection;
using GameFrameX.NetWork.HTTP;
using GameFrameX.StartUp;
using Xunit;

namespace GameFrameX.Tests.StartUp;

/// <summary>
/// HTTP 路由注册去重测试。
/// </summary>
public class AppStartUpByHttpServerRouteTests
{
    /// <summary>
    /// 验证同一HTTP方法下仅大小写不同的路径会被识别为重复路由。
    /// </summary>
    [Fact]
    public void TryAddRouteKey_SameMethodDifferentCasePath_ShouldTreatAsDuplicate()
    {
        var registeredRouteKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var firstAddResult = InvokeTryAddRouteKey(HttpMethodType.POST, "/game/api/Test", registeredRouteKeys);
        var secondAddResult = InvokeTryAddRouteKey(HttpMethodType.POST, "/game/api/test", registeredRouteKeys);

        Assert.True(firstAddResult);
        Assert.False(secondAddResult);
        Assert.Single(registeredRouteKeys);
    }

    /// <summary>
    /// 验证不同HTTP方法允许注册相同路径。
    /// </summary>
    [Fact]
    public void TryAddRouteKey_DifferentMethodSamePath_ShouldAllowBoth()
    {
        var registeredRouteKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var postAddResult = InvokeTryAddRouteKey(HttpMethodType.POST, "/game/api/test", registeredRouteKeys);
        var getAddResult = InvokeTryAddRouteKey(HttpMethodType.GET, "/game/api/test", registeredRouteKeys);

        Assert.True(postAddResult);
        Assert.True(getAddResult);
        Assert.Equal(2, registeredRouteKeys.Count);
    }

    /// <summary>
    /// 通过反射调用 AppStartUpBase 的路由键去重方法。
    /// </summary>
    /// <param name="httpMethod">HTTP方法</param>
    /// <param name="apiPath">API路径</param>
    /// <param name="registeredRouteKeys">已注册路由集合</param>
    /// <returns>返回去重结果</returns>
    private static bool InvokeTryAddRouteKey(HttpMethodType httpMethod, string apiPath, ISet<string> registeredRouteKeys)
    {
        var method = typeof(AppStartUpBase).GetMethod("TryAddRouteKey", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        var result = method.Invoke(null, new object[] { httpMethod, apiPath, registeredRouteKeys });
        Assert.NotNull(result);
        return (bool)result;
    }
}
