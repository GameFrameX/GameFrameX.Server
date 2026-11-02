// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and related international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please see the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations.
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any disputes or liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository: https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.Capability.Online.Operations.HotfixRollback;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Launcher.Online;

/// <summary>
/// Hotfix 回滚状态日志的 LogHelper 桥接（<see cref="IOnlineHotfixRollbackLog"/> 的宿主实现）：
/// 回滚执行链的状态变迁轨迹（链路启动 / 执行器切换 / 活跃指针重试与耗尽 / 反切 / 完成）
/// 统一进 <see cref="LogHelper"/>，Info / Warn / Error 三级直转发，文案由服务层组装后传入。
/// </summary>
/// <remarks>
/// The LogHelper bridge of the hotfix rollback trace log (the host implementation of
/// <see cref="IOnlineHotfixRollbackLog"/>): the rollback execution chain's state transitions
/// (chain start / executor switch / active pointer retries and exhaustion / revert / completion)
/// flow into <see cref="LogHelper"/> with Info / Warn / Error forwarded as-is and messages composed by the service layer.
/// </remarks>
public sealed class LogHelperHotfixRollbackLog : IOnlineHotfixRollbackLog
{
    /// <summary>
    /// 将回滚执行链的信息轨迹直接转发到宿主 <c>LogHelper.Info</c>。
    /// </summary>
    /// <remarks>
    /// Forwards rollback trace informational messages directly to the host <c>LogHelper.Info</c>.
    /// </remarks>
    /// <param name="message">日志消息 / Log message</param>
    public void Info(string message)
    {
        LogHelper.Info(message);
    }

    /// <summary>
    /// 将回滚执行链的告警轨迹（重试 / 已反切）直接转发到宿主 <c>LogHelper.Warning</c>。
    /// </summary>
    /// <remarks>
    /// Forwards rollback trace warning messages (retries / reverted) directly to the host <c>LogHelper.Warning</c>.
    /// </remarks>
    /// <param name="message">日志消息 / Log message</param>
    public void Warn(string message)
    {
        LogHelper.Warning(message);
    }

    /// <summary>
    /// 将回滚执行链的错误轨迹（重试耗尽 / 反切失败须对账）直接转发到宿主 <c>LogHelper.Error</c>。
    /// </summary>
    /// <remarks>
    /// Forwards rollback trace error messages (retry exhaustion / revert failure requiring reconciliation) directly to the host <c>LogHelper.Error</c>.
    /// </remarks>
    /// <param name="message">日志消息 / Log message</param>
    public void Error(string message)
    {
        LogHelper.Error(message);
    }
}
