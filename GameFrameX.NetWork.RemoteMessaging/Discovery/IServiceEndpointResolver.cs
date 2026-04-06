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

namespace GameFrameX.NetWork.RemoteMessaging.Discovery;

/// <summary>
/// 服务端点解析器。统一从 Aspire 或环境变量中解析目标服务的 TCP 地址。
/// </summary>
public interface IServiceEndpointResolver
{
    /// <summary>
    /// 解析指定服务的 TCP 端点地址。
    /// </summary>
    /// <param name="serviceName">目标服务名</param>
    /// <returns>端点地址字符串（如 "tcp://host:port"）；未找到时返回空字符串</returns>
    string ResolveTcpEndpoint(string serviceName);
}