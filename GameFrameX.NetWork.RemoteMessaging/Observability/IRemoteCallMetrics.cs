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

namespace GameFrameX.NetWork.RemoteMessaging.Observability;

/// <summary>
/// 远程调用指标采集器。采集 QPS、成功率、超时率、重试率、耗时分布等指标。
/// 通过 System.Diagnostics.Metrics 对接 OpenTelemetry。
/// </summary>
public interface IRemoteCallMetrics
{
    /// <summary>
    /// 记录一次成功的调用。
    /// </summary>
    /// <param name="serviceName">目标服务名</param>
    /// <param name="messageType">消息类型名</param>
    /// <param name="elapsedMs">耗时毫秒数</param>
    /// <param name="retryCount">重试次数</param>
    void RecordSuccess(string serviceName, string messageType, long elapsedMs, int retryCount = 0);

    /// <summary>
    /// 记录一次失败的调用。
    /// </summary>
    /// <param name="serviceName">目标服务名</param>
    /// <param name="messageType">消息类型名</param>
    /// <param name="statusCode">状态码</param>
    /// <param name="elapsedMs">耗时毫秒数</param>
    void RecordFailure(string serviceName, string messageType, RemoteStatusCode statusCode, long elapsedMs);

    /// <summary>
    /// 记录一次重试。
    /// </summary>
    /// <param name="serviceName">目标服务名</param>
    /// <param name="messageType">消息类型名</param>
    /// <param name="attemptCount">第几次重试</param>
    void RecordRetry(string serviceName, string messageType, int attemptCount);
}
