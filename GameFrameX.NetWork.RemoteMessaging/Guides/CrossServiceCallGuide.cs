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

// ==========================================================================================
// 跨服务调用开发指南
// ==========================================================================================
//
// 本文档定义了新增跨服务调用的标准模板和代码准入规则。
// 所有新增跨服务调用必须遵循以下规范。
//
// ## 准入规则
//
// 1. 【强制】所有跨服务调用必须通过 IRemoteMessageClient 发起，禁止在 ComponentAgent 中直接使用 TcpClient/NetworkStream。
// 2. 【强制】非幂等操作（写操作）必须设置 AllowRetry = false。
// 3. 【强制】幂等操作（读操作/查询）可设置 AllowRetry = true，但最大重试不超过 2 次。
// 4. 【推荐】使用 CallWithResultAsync 替代 CallAsync，获取结构化结果和状态码。
// 5. 【推荐】使用 RemoteMessagingBuilder 构建客户端，而非直接实例化。
// 6. 【推荐】通过 MessageObjectPoolHelper.Get/Return 管理请求对象生命周期。
//
// ## 标准模板
//
// ### 1. 幂等查询调用（允许重试）
//
// ```csharp
// public async Task OnQueryData(INetWorkChannel netWorkChannel, ReqQuery request, RespQuery response)
// {
//     var req = MessageObjectPoolHelper.Get<ReqInnerQuery>();
//     req.Param = request.Param;
//     try
//     {
//         var context = new RemoteCallContext
//         {
//             ServiceName = GlobalConst.TargetServiceName,
//             TimeoutMs = RpcTimeoutMilliseconds,
//             AllowRetry = true,  // 幂等查询允许重试
//         };
//
//         var result = await RemoteMessageClientHolder.Client.CallWithResultAsync<RespInnerQuery>(context, req);
//
//         if (result.IsSuccess && result.Response != null)
//         {
//             response.Data = result.Response.Data;
//             return;
//         }
//
//         response.Data = null;
//         response.ErrorCode = MapToBusinessErrorCode(result.StatusCode);
//         LogHelper.Error("调用失败, StatusCode: {statusCode}, TraceId: {traceId}",
//             result.StatusCode, result.TraceId);
//     }
//     catch (Exception exception)
//     {
//         response.ErrorCode = -99;
//         LogHelper.Error(exception, "调用异常");
//     }
//     finally
//     {
//         MessageObjectPoolHelper.Return(req);
//     }
// }
// ```
//
// ### 2. 非幂等写操作（禁止重试）
//
// ```csharp
// public async Task OnWriteData(INetWorkChannel netWorkChannel, ReqWrite request, RespWrite response)
// {
//     var req = MessageObjectPoolHelper.Get<ReqInnerWrite>();
//     req.Param = request.Param;
//     try
//     {
//         var context = new RemoteCallContext
//         {
//             ServiceName = GlobalConst.TargetServiceName,
//             TimeoutMs = RpcTimeoutMilliseconds,
//             AllowRetry = false,  // 非幂等操作禁止重试
//         };
//
//         var result = await RemoteMessageClientHolder.Client.CallWithResultAsync<RespInnerWrite>(context, req);
//
//         if (result.IsSuccess && result.Response != null)
//         {
//             response.Success = result.Response.Success;
//             return;
//         }
//
//         response.Success = false;
//         LogHelper.Error("调用失败, StatusCode: {statusCode}, TraceId: {traceId}",
//             result.StatusCode, result.TraceId);
//     }
//     catch (Exception exception)
//     {
//         response.Success = false;
//         LogHelper.Error(exception, "调用异常");
//     }
//     finally
//     {
//         MessageObjectPoolHelper.Return(req);
//     }
// }
// ```
//
// ## 错误码映射参考
//
// | RemoteStatusCode | 业务错误码 | 说明 |
// |---|---|---|
// | Success | 0 | 成功 |
// | Timeout | -1 | 超时 |
// | ConnectionFailed | -2 | 连接失败 |
// | EndpointNotFound | -3 | 服务发现失败 |
// | Cancelled | -4 | 取消 |
// | RetryExhausted | -5 | 重试耗尽 |
// | ConnectionClosed | -6 | 连接关闭 |
// | UnexpectedResponse | -7 | 响应异常 |
// | CircuitOpen | -10 | 熔断开启 |
// | ServiceUnavailable | -11 | 服务不可用 |
// | UnknownError | -99 | 未知错误 |

namespace GameFrameX.NetWork.RemoteMessaging.Guides;

/// <summary>
/// 跨服务调用代码准入规则标记。在 Code Review 时检查此规则的遵守情况。
/// </summary>
/// <remarks>
/// Cross-service call code admission rule marker. Checked during code review for compliance.
/// </remarks>
internal static class CrossServiceCallRules
{
    // 本文件作为代码规范的声明点，实际规则见上方注释。
    // Code Review 检查清单：
    // 1. 搜索 TcpClient/NetworkStream 的使用 → 应为 0 处（除 RemoteMessaging 内部）
    // 2. 搜索 CallAsync 的调用 → 应全部通过 IRemoteMessageClient 接口
    // 3. 搜索 AllowRetry → 写操作必须为 false
}