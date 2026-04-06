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
/// 消息编解码器。复用当前自定义包头结构（14 字节包头 + ProtoBuf 载荷）。
/// </summary>
public interface IMessageCodec
{
    /// <summary>
    /// 将消息对象编码为二进制包。
    /// </summary>
    /// <param name="message">消息对象</param>
    /// <returns>编码后的二进制数据</returns>
    byte[] Encode(MessageObject message);

    /// <summary>
    /// 从网络流中读取并解码一条消息。
    /// </summary>
    /// <param name="stream">网络流</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>解码后的消息对象；连接关闭时返回 null</returns>
    Task<MessageObject> DecodeAsync(Stream stream, CancellationToken cancellationToken);
}
