// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Collections;

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 基于环境变量发现同名服务实例。
/// 支持 Aspire 变量格式：services__{serviceName}_{instanceId}__tcp__0。
/// </summary>
/// <remarks>
/// Discovers same-name service instances from environment variables.
/// Supports Aspire variable format: services__{serviceName}_{instanceId}__tcp__0.
/// </remarks>
public static class EnvironmentServiceInstanceDiscovery
{
    /// <summary>
    /// 从环境变量中发现服务实例 ID 列表。
    /// </summary>
    /// <remarks>
    /// Discovers service instance ID list from environment variables.
    /// </remarks>
    /// <param name="serviceName">逻辑服务名（不带实例后缀）/ Logical service name (without instance suffix)</param>
    /// <param name="variables">可选环境变量集合，单测可注入 / Optional environment variable set, can be injected in tests</param>
    /// <returns>实例 ID 列表（去重、升序）/ Instance ID list (distinct, ascending)</returns>
    public static IReadOnlyList<string> DiscoverInstanceIds(string serviceName, IDictionary variables = null)
    {
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return Array.Empty<string>();
        }

        variables ??= Environment.GetEnvironmentVariables();
        var prefix = serviceName + "_";
        var set = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (DictionaryEntry item in variables)
        {
            if (item.Key is not string rawKey)
            {
                continue;
            }

            var key = rawKey.Replace("__", ":", StringComparison.Ordinal).Trim();
            if (!key.StartsWith("services:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var segments = key.Split(':', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 4)
            {
                continue;
            }

            // services:{serviceToken}:tcp:{index}
            if (!string.Equals(segments[2], "tcp", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var serviceToken = segments[1];
            if (!serviceToken.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var instanceId = serviceToken[prefix.Length..].Trim();
            if (!string.IsNullOrEmpty(instanceId))
            {
                set.Add(instanceId);
            }
        }

        if (set.Count == 0)
        {
            return Array.Empty<string>();
        }

        return set.ToList();
    }
}
