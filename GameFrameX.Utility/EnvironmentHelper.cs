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

// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using Microsoft.Extensions.Hosting;

namespace GameFrameX.Utility;

/// <summary>
/// 环境帮助器
/// </summary>
public static class EnvironmentHelper
{
    /// <summary>
    /// 判断是否为开发环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否为 Development
    /// </summary>
    /// <returns>如果是开发环境返回true，否则返回false</returns>
    public static bool IsDevelopment()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, Environments.Development, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 判断是否为生产环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否为 Production
    /// </summary>
    /// <returns>如果是生产环境返回true，否则返回false</returns>
    public static bool IsProduction()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, Environments.Production, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 判断是否为测试/预发布环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否为 Staging
    /// </summary>
    /// <returns>如果是测试/预发布环境返回true，否则返回false</returns>
    public static bool IsStaging()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, Environments.Staging, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 判断是否为任意自定义环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否与指定环境名称匹配
    /// </summary>
    /// <param name="environmentName">要检查的环境名称</param>
    /// <returns>如果当前环境与指定环境名称匹配返回true，否则返回false</returns>
    public static bool IsEnvironment(string environmentName)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, environmentName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 判断当前应用是否运行在Docker容器中
    /// 通过检查环境变量 DOTNET_RUNNING_IN_CONTAINER 是否存在来判断
    /// </summary>
    /// <returns>如果在Docker容器中运行返回true，否则返回false</returns>
    public static bool IsDocker()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
    }

    /// <summary>
    /// 判断当前应用是否运行在Kubernetes集群中
    /// 通过检查环境变量 KUBERNETES_SERVICE_HOST 是否存在来判断
    /// </summary>
    /// <returns>如果在Kubernetes集群中运行返回true，否则返回false</returns>
    public static bool IsKubernetes()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"));
    }

    /// <summary>
    /// 获取当前运行环境名称
    /// 优先获取 ASPNETCORE_ENVIRONMENT 环境变量，如果不存在则获取 DOTNET_ENVIRONMENT 环境变量
    /// </summary>
    /// <returns>返回当前环境名称，如果未设置环境变量则返回null</returns>
    public static string GetEnvironmentName()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        return env;
    }
}