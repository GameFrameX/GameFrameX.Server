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

namespace GameFrameX.NetWork.RemoteMessaging.Abstractions;

/// <summary>
/// 连接提供器。负责 TCP 长连接的创建、复用、重连和释放。
/// </summary>
public interface IConnectionProvider : IDisposable
{
    /// <summary>
    /// 获取或创建到指定目标的可用网络流。
    /// </summary>
    /// <param name="host">目标主机</param>
    /// <param name="port">目标端口</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>可用的网络流</returns>
    Task<Stream> GetOrCreateStreamAsync(string host, int port, CancellationToken cancellationToken = default);

    /// <summary>
    /// 标记当前连接为失效，下次调用时重建。
    /// </summary>
    void Invalidate();
}
