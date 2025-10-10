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



using Microsoft.Extensions.Hosting;

namespace GameFrameX.Utility;

/// <summary>
/// Environment helper utility class.
/// </summary>
/// <remarks>
/// 环境帮助器
/// </remarks>
public static class EnvironmentHelper
{
    /// <summary>
    /// Determines whether the current environment is a development environment.
    /// Checks if the ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT environment variable is set to Development.
    /// </summary>
    /// <remarks>
    /// 判断是否为开发环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否为 Development
    /// </remarks>
    /// <returns>True if the current environment is development, otherwise false. / 如果是开发环境返回true，否则返回false</returns>
    public static bool IsDevelopment()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, Environments.Development, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current environment is a production environment.
    /// Checks if the ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT environment variable is set to Production.
    /// </summary>
    /// <remarks>
    /// 判断是否为生产环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否为 Production
    /// </remarks>
    /// <returns>True if the current environment is production, otherwise false. / 如果是生产环境返回true，否则返回false</returns>
    public static bool IsProduction()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, Environments.Production, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current environment is a staging/testing environment.
    /// Checks if the ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT environment variable is set to Staging.
    /// </summary>
    /// <remarks>
    /// 判断是否为测试/预发布环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否为 Staging
    /// </remarks>
    /// <returns>True if the current environment is staging, otherwise false. / 如果是测试/预发布环境返回true，否则返回false</returns>
    public static bool IsStaging()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, Environments.Staging, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current environment matches the specified custom environment name.
    /// Checks if the ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT environment variable matches the specified environment name.
    /// </summary>
    /// <remarks>
    /// 判断是否为任意自定义环境
    /// 通过检查环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 的值是否与指定环境名称匹配
    /// </remarks>
    /// <param name="environmentName">The environment name to check. / 要检查的环境名称</param>
    /// <returns>True if the current environment matches the specified environment name, otherwise false. / 如果当前环境与指定环境名称匹配返回true，否则返回false</returns>
    public static bool IsEnvironment(string environmentName)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

        return string.Equals(env, environmentName, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Determines whether the current application is running in a Docker container.
    /// Checks if the DOTNET_RUNNING_IN_CONTAINER environment variable exists.
    /// </summary>
    /// <remarks>
    /// 判断当前应用是否运行在Docker容器中
    /// 通过检查环境变量 DOTNET_RUNNING_IN_CONTAINER 是否存在来判断
    /// </remarks>
    /// <returns>True if running in a Docker container, otherwise false. / 如果在Docker容器中运行返回true，否则返回false</returns>
    public static bool IsDocker()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
    }

    /// <summary>
    /// Determines whether the current application is running in a Kubernetes cluster.
    /// Checks if the KUBERNETES_SERVICE_HOST environment variable exists.
    /// </summary>
    /// <remarks>
    /// 判断当前应用是否运行在Kubernetes集群中
    /// 通过检查环境变量 KUBERNETES_SERVICE_HOST 是否存在来判断
    /// </remarks>
    /// <returns>True if running in a Kubernetes cluster, otherwise false. / 如果在Kubernetes集群中运行返回true，否则返回false</returns>
    public static bool IsKubernetes()
    {
        return !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST"));
    }

    /// <summary>
    /// Gets the current environment name from environment variables.
    /// Returns the value of ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT, or "Production" if neither is set.
    /// </summary>
    /// <remarks>
    /// 获取当前环境名称
    /// 从环境变量 ASPNETCORE_ENVIRONMENT 或 DOTNET_ENVIRONMENT 中获取，如果都未设置则返回 "Production"
    /// </remarks>
    /// <returns>The current environment name. / 当前环境名称</returns>
    public static string GetEnvironmentName()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                  ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
        return env;
    }
}