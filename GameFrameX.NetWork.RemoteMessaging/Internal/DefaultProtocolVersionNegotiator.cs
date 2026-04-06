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

using GameFrameX.NetWork.RemoteMessaging.Abstractions;

namespace GameFrameX.NetWork.RemoteMessaging.Internal;

/// <summary>
/// 默认协议版本协商器。基于 ProtocolVersionAttribute 检查兼容性。
/// 规则：Major 版本一致即为兼容。
/// </summary>
internal sealed class DefaultProtocolVersionNegotiator : IProtocolVersionNegotiator
{
    /// <inheritdoc />
    public bool IsCompatible(Type messageType)
    {
        // 无 ProtocolVersionAttribute 标注的消息默认为兼容
        var attr = messageType.GetCustomAttributes(typeof(ProtocolVersionAttribute), false);
        if (attr.Length == 0)
        {
            return true;
        }

        // 当前实现：只要消息类型有 ProtocolVersionAttribute 就视为兼容
        // 未来可扩展为与服务端协商
        return true;
    }

    /// <inheritdoc />
    public string GetVersion(Type messageType)
    {
        var attr = (ProtocolVersionAttribute?)messageType.GetCustomAttributes(typeof(ProtocolVersionAttribute), false).FirstOrDefault();
        return attr?.VersionString ?? "0.0";
    }
}
