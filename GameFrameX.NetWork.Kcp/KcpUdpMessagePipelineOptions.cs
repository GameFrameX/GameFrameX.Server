// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System.Net;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// Options for integrating KCP into UDP pipeline / 用于将 KCP 集成到 UDP 管道的配置
/// </summary>
public sealed class KcpUdpMessagePipelineOptions
{
    /// <summary>
    /// Whether KCP mode is enabled / 是否启用 KCP 模式
    /// </summary>
    public bool EnableKcp { get; set; }

    /// <summary>
    /// Factory for creating KCP options / 创建 KCP 配置对象的工厂
    /// </summary>
    public Func<KcpOptions> KcpOptionsFactory { get; set; }

    /// <summary>
    /// Callback on KCP session created / KCP 会话创建时回调
    /// </summary>
    public Func<EndPoint, ValueTask> OnKcpConnected { get; set; }

    /// <summary>
    /// Callback on KCP package parsed / KCP 消息解析后回调
    /// </summary>
    public Action<IGameAppSession, IMessage> OnKcpPackage { get; set; }

    /// <summary>
    /// Callback on KCP session removed / KCP 会话移除时回调
    /// </summary>
    public Func<EndPoint, ValueTask> OnKcpDisconnected { get; set; }

    /// <summary>
    /// Creates effective KCP options / 创建生效的 KCP 配置
    /// </summary>
    public KcpOptions CreateKcpOptions()
    {
        var options = KcpOptionsFactory?.Invoke() ?? new KcpOptions();
        options.Enable = true;
        return options;
    }
}
