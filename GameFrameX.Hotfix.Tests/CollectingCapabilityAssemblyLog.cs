// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规与开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 Apache License 2.0 单协议分发，
//   This project is licensed solely under the Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes or liabilities arising from secondary development based on this project
//   本组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository: https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository: https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GameFrameX.Capability.Hosting.Abstractions;
using GameFrameX.Capability.Hosting.AdminApi;
using GameFrameX.Capability.Hosting.Core;
using GameFrameX.Capability.Online.Abstractions;
using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Capability.Online.Abstractions.Idempotency;
using GameFrameX.Capability.Online.Abstractions.Scope;
using GameFrameX.Capability.Online.Operations.HotfixRollback;
using GameFrameX.Capability.Online.Social;
using Xunit;

namespace GameFrameX.Hotfix.Tests.Online;


/// <summary>
/// 测试用装配日志收集器（C294 起 <see cref="ICapabilityAssemblyLog"/> 是组合必填出口；
/// 本类在进程内收集 Info / Error 两条流，供用例断言装配日志内容）。
/// </summary>
public sealed class CollectingCapabilityAssemblyLog : ICapabilityAssemblyLog
{
    /// <summary>Info 级日志流。</summary>
    public readonly List<string> Infos = new();

    /// <summary>Error 级日志流。</summary>
    public readonly List<string> Errors = new();

    /// <summary>
    /// 将信息日志追加到 <c>Infos</c> 列表，供测试断言能力装配输出的信息条目。
    /// </summary>
    /// <remarks>
    /// Appends the informational message to the <c>Infos</c> list so tests can assert capability assembly informational output.
    /// </remarks>
    /// <param name="message">信息日志内容 / Informational log message</param>
    public void Info(string message)
    {
        Infos.Add(message);
    }

    /// <summary>
    /// 将错误日志追加到 <c>Errors</c> 列表，供测试断言能力装配输出的错误条目。
    /// </summary>
    /// <remarks>
    /// Appends the error message to the <c>Errors</c> list so tests can assert capability assembly error output.
    /// </remarks>
    /// <param name="message">错误日志内容 / Error log message</param>
    public void Error(string message)
    {
        Errors.Add(message);
    }
}

/// <summary>
/// Online 能力族测试组合挂具（C294：测试与宿主共用同一装配面
/// <see cref="CapabilityComposition.Compose"/>——族配置段 JSON、宿主 Override、过滤器与
/// 管理面适配器全部经 <see cref="CapabilityCompositionOptions"/> 注入；admin 调度器按
/// <c>AdminApiComposition.CreateAdminApiPlane</c> 同口径组装但直接暴露
/// <see cref="AdminApiDispatcher"/>，供行为用例内网调用）。
/// </summary>
