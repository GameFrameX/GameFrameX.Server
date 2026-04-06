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

using System.Diagnostics;

namespace GameFrameX.NetWork.RemoteMessaging.Observability;

/// <summary>
/// TraceId 透传拦截器。自动在请求上下文中注入 TraceId，支持跨服务链路追踪。
/// </summary>
/// <remarks>
/// TraceId propagation interceptor. Automatically injects TraceId into the request context
/// to support cross-service distributed tracing.
/// </remarks>
internal sealed class TraceRemoteCallInterceptor : IRemoteCallInterceptor
{
    /// <summary>
    /// ActivitySource 名称常量，用于创建分布式追踪活动。
    /// </summary>
    /// <remarks>
    /// The ActivitySource name constant used for creating distributed tracing activities.
    /// </remarks>
    public const string ActivitySourceName = "GameFrameX.RemoteMessaging";
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    /// <summary>
    /// 创建分布式追踪活动并注入 TraceId。
    /// </summary>
    /// <remarks>
    /// Creates a distributed tracing activity and injects TraceId.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnBeforeCallAsync(RemoteCallContext context, MessageObject request)
    {
        var activity = ActivitySource.StartActivity("RemoteMessage.Call", ActivityKind.Client);
        if (activity != null)
        {
            activity.SetTag("rpc.system", "custom-tcp");
            activity.SetTag("rpc.service", context.ServiceName);
            activity.SetTag("rpc.message_type", request.GetType().Name);
            context.TraceActivity = activity;
            context.TraceId = activity.TraceId.ToString();
            return Task.CompletedTask;
        }

        if (string.IsNullOrWhiteSpace(context.TraceId))
        {
            context.TraceId = Guid.NewGuid().ToString("N");
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止追踪活动并记录成功状态。
    /// </summary>
    /// <remarks>
    /// Stops the tracing activity and records success status.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <param name="response">响应消息（可能为 null） / The response message (may be null)</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnAfterCallAsync(RemoteCallContext context, MessageObject request, MessageObject response, long elapsedMs)
    {
        if (context.TraceActivity != null)
        {
            context.TraceActivity.SetTag("rpc.elapsed_ms", elapsedMs);
            context.TraceActivity.SetTag("rpc.status", "ok");
            context.TraceActivity.Stop();
            context.TraceActivity.Dispose();
            context.TraceActivity = null;
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 停止追踪活动并记录错误信息。
    /// </summary>
    /// <remarks>
    /// Stops the tracing activity and records error information.
    /// </remarks>
    /// <param name="context">调用上下文 / The remote call context</param>
    /// <param name="request">请求消息 / The request message</param>
    /// <param name="exception">异常对象 / The exception that occurred</param>
    /// <param name="elapsedMs">耗时毫秒数 / The elapsed time in milliseconds</param>
    /// <returns>表示异步操作的任务 / A task representing the asynchronous operation</returns>
    public Task OnExceptionAsync(RemoteCallContext context, MessageObject request, Exception exception, long elapsedMs)
    {
        if (context.TraceActivity != null)
        {
            context.TraceActivity.SetTag("rpc.elapsed_ms", elapsedMs);
            context.TraceActivity.SetTag("rpc.status", "error");
            context.TraceActivity.SetTag("rpc.error_type", exception.GetType().Name);
            context.TraceActivity.SetTag("rpc.error_message", exception.Message);
            context.TraceActivity.Stop();
            context.TraceActivity.Dispose();
            context.TraceActivity = null;
        }

        return Task.CompletedTask;
    }
}