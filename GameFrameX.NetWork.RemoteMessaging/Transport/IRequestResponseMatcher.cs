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

namespace GameFrameX.NetWork.RemoteMessaging.Transport;

/// <summary>
/// 请求-响应匹配器。统一管理 UniqueId 生命周期与响应匹配。
/// </summary>
public interface IRequestResponseMatcher
{
    /// <summary>
    /// 注册一个待处理请求，返回分配的唯一 ID。
    /// </summary>
    /// <param name="timeoutMs">请求超时毫秒数</param>
    /// <returns>请求唯一 ID</returns>
    int RegisterPendingRequest(int timeoutMs);

    /// <summary>
    /// 等待指定请求的响应。
    /// </summary>
    /// <param name="uniqueId">请求唯一 ID</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>响应消息对象</returns>
    Task<MessageObject> WaitResponseAsync(int uniqueId, CancellationToken cancellationToken);

    /// <summary>
    /// 收到响应时完成对应请求。
    /// </summary>
    /// <param name="uniqueId">请求唯一 ID</param>
    /// <param name="response">响应消息</param>
    /// <returns>是否成功匹配到等待中的请求</returns>
    bool TryComplete(int uniqueId, MessageObject response);

    /// <summary>
    /// 清理已超时的待处理请求。
    /// </summary>
    void CleanupExpired();
}