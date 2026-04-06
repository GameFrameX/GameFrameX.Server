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
/// 消息压缩算法注册表。
/// </summary>
public interface IMessageCompressionRegistry
{
    /// <summary>
    /// 注册算法实现。
    /// </summary>
    /// <param name="algorithm">算法实现</param>
    void Register(IMessageCompressionAlgorithm algorithm);

    /// <summary>
    /// 按算法 ID 获取算法实现。
    /// </summary>
    /// <param name="algorithmId">算法 ID</param>
    /// <param name="algorithm">算法实例</param>
    /// <returns>是否找到</returns>
    bool TryGet(byte algorithmId, out IMessageCompressionAlgorithm algorithm);
}
