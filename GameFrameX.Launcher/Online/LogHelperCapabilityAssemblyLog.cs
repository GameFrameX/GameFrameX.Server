// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   Any legal disputes and liabilities arising from secondary development based on this project
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool
//   CNB Repository: https://cnb.cool
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Launcher.Online;

/// <summary>
/// 装配日志的 LogHelper 桥接（<see cref="ICapabilityAssemblyLog"/> 的宿主实现，C294）：
/// 机制的装配期日志（开始 / 排除 / 逐能力汇总 / 可选缺失 / 完成 = Info，必选缺失 / 环路 / 汇总失败 = Error）
/// 统一进 <see cref="LogHelper"/>，文案由机制层本地化后传入。
/// </summary>
/// <remarks>
/// The LogHelper bridge of the assembly log (the host implementation of
/// <see cref="ICapabilityAssemblyLog"/>, C294): the mechanism's composition-time logs (start /
/// exclusion / per-capability summaries / optional absence / completion = Info, mandatory absence /
/// cycles / aggregate failure = Error) flow into <see cref="LogHelper"/> with messages already
/// localized by the mechanism layer.
/// </remarks>
public sealed class LogHelperCapabilityAssemblyLog : ICapabilityAssemblyLog
{
    /// <summary>
    /// 将能力装配的信息日志直接转发到宿主 <c>LogHelper.Info</c>。
    /// </summary>
    /// <remarks>
    /// Forwards capability assembly informational messages directly to the host <c>LogHelper.Info</c>.
    /// </remarks>
    /// <param name="message">日志消息 / Log message</param>
    public void Info(string message)
    {
        LogHelper.Info(message);
    }

    /// <summary>
    /// 将能力装配的错误日志直接转发到宿主 <c>LogHelper.Error</c>。
    /// </summary>
    /// <remarks>
    /// Forwards capability assembly error messages directly to the host <c>LogHelper.Error</c>.
    /// </remarks>
    /// <param name="message">日志消息 / Log message</param>
    public void Error(string message)
    {
        LogHelper.Error(message);
    }
}
