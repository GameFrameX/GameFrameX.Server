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


namespace GameFrameX.NetWork.Kcp;

/// <summary>
/// KCP configuration options / KCP 配置选项
/// </summary>
public sealed class KcpOptions
{
    /// <summary>
    /// Enable KCP server / 是否启用 KCP 服务器
    /// </summary>
    public bool Enable { get; set; } = false;

    /// <summary>
    /// Enable no-delay mode / 是否启用无延迟模式
    /// </summary>
    public bool NoDelay { get; set; } = true;

    /// <summary>
    /// Internal update interval in milliseconds / 内部更新间隔（毫秒）
    /// </summary>
    public int Interval { get; set; } = 10;

    /// <summary>
    /// Fast resend count (0=disable, 2=recommended) / 快速重传次数（0=禁用，2=推荐值）
    /// </summary>
    public int Resend { get; set; } = 2;

    /// <summary>
    /// Enable flow control / 是否启用流控
    /// </summary>
    public bool EnableFlowControl { get; set; } = false;

    /// <summary>
    /// Send window size / 发送窗口大小
    /// </summary>
    public int SendWindow { get; set; } = 128;

    /// <summary>
    /// Receive window size / 接收窗口大小
    /// </summary>
    public int ReceiveWindow { get; set; } = 128;

    /// <summary>
    /// Maximum transmission unit / 最大传输单元
    /// </summary>
    public int Mtu { get; set; } = 1400;

    /// <summary>
    /// Connection timeout in milliseconds / 连接超时时间（毫秒）
    /// </summary>
    public int ConnectionTimeout { get; set; } = 60000;

    /// <summary>
    /// Keep alive interval in milliseconds / 心跳间隔（毫秒）
    /// </summary>
    public int KeepAliveInterval { get; set; } = 10000;

    /// <summary>
    /// Update period in milliseconds / 更新周期（毫秒）
    /// </summary>
    public int UpdatePeriod { get; set; } = 5;

    /// <summary>
    /// Session timeout in seconds / 会话超时时间（秒）
    /// </summary>
    public int SessionTimeout { get; set; } = 120;
}