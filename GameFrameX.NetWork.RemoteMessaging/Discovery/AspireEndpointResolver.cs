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

namespace GameFrameX.NetWork.RemoteMessaging.Discovery;

/// <summary>
/// 基于 Aspire 环境变量的服务端点解析器。扫描环境变量中 services__{ServiceName}__tcp 格式的键值。
/// </summary>
/// <remarks>
/// Aspire environment variable-based service endpoint resolver. Scans environment variables for keys in the format services__{ServiceName}__tcp.
/// </remarks>
internal sealed class AspireEndpointResolver : IServiceEndpointResolver
{
    /// <inheritdoc />
    public string ResolveTcpEndpoint(string serviceName)
    {
        var variables = Environment.GetEnvironmentVariables();
        foreach (DictionaryEntry item in variables)
        {
            if (item.Key is not string rawKey || item.Value is not string rawValue || string.IsNullOrWhiteSpace(rawValue))
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

            if (!string.Equals(segments[1], serviceName, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!string.Equals(segments[2], "tcp", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return rawValue.Trim();
        }

        return string.Empty;
    }
}