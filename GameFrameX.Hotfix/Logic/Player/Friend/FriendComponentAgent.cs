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


using GameFrameX.Apps.Player.Friend;
using GameFrameX.NetWork.RemoteMessaging;
using GameFrameX.NetWork.RemoteMessaging.Contracts;

namespace GameFrameX.Hotfix.Logic.Player.Friend;

public class FriendComponentAgent : StateComponentAgent<FriendComponent, FriendState>
{
    private const int RpcTimeoutMilliseconds = 5000;

    /// <summary>
    /// 获取好友列表（通过统一客户端 RPC 调用 Social 服务）。
    /// 好友列表查询为幂等操作，允许重试。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnFriendList(INetWorkChannel netWorkChannel, ReqFriendList request, RespFriendList response)
    {
        if (string.Equals(GlobalSettings.CurrentSetting?.ServerType, GlobalConst.SocialServiceName, StringComparison.OrdinalIgnoreCase))
        {
            var innerResponse = new RespInnerFriendList();
            await OnInnerFriendList(netWorkChannel, new ReqInnerFriendList { PlayerId = ActorId, }, innerResponse);
            response.Friends = innerResponse.Friends ?? new List<FriendInfo>();
            response.ErrorCode = innerResponse.ErrorCode;
            return;
        }

        var reqInnerFriendList = MessageObjectPoolHelper.Get<ReqInnerFriendList>();
        reqInnerFriendList.PlayerId = ActorId;
        try
        {
            var context = new RemoteCallContext
            {
                ServiceName = GlobalConst.SocialServiceName,
                TimeoutMs = RpcTimeoutMilliseconds,
                AllowRetry = true,
            };

            var result = await RemoteMessageClientHolder.Client.CallWithResultAsync<RespInnerFriendList>(
                context, reqInnerFriendList);

            if (result.IsSuccess && result.Response != null)
            {
                response.Friends = result.Response.Friends ?? new List<FriendInfo>();
                response.ErrorCode = result.Response.ErrorCode;
                return;
            }

            response.Friends = new List<FriendInfo>();
            response.ErrorCode = MapToBusinessErrorCode(result.StatusCode);
            LogHelper.Error("FriendComponentAgent.OnFriendList 跨进程调用失败, StatusCode: {statusCode}, Error: {errorMessage}, TraceId: {traceId}", result.StatusCode, result.ErrorMessage, result.TraceId);
        }
        catch (Exception exception)
        {
            response.Friends = new List<FriendInfo>();
            response.ErrorCode = -2;
            LogHelper.Error(exception, "FriendComponentAgent.OnFriendList 跨进程调用异常");
        }
        finally
        {
            MessageObjectPoolHelper.Return(reqInnerFriendList);
        }
    }

    /// <summary>
    /// 添加好友（通过统一客户端 RPC 调用 Social 服务）。
    /// 添加好友为非幂等操作，不允许重试。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnAddFriend(INetWorkChannel netWorkChannel, ReqFriendByAdd request, RespFriendByAdd response)
    {
        var reqInnerAddFriend = MessageObjectPoolHelper.Get<ReqInnerFriendByAdd>();
        reqInnerAddFriend.PlayerId = request.PlayerId;
        try
        {
            var context = new RemoteCallContext
            {
                ServiceName = GlobalConst.SocialServiceName,
                TimeoutMs = RpcTimeoutMilliseconds,
                AllowRetry = false,
            };

            var result = await RemoteMessageClientHolder.Client.CallWithResultAsync<RespInnerFriendByAdd>(
                context, reqInnerAddFriend);

            if (result.IsSuccess && result.Response != null)
            {
                response.Success = result.Response.Success;
                return;
            }

            response.Success = false;
            LogHelper.Error("FriendComponentAgent.OnAddFriend 跨进程调用失败, StatusCode: {statusCode}, Error: {errorMessage}, TraceId: {traceId}",
                result.StatusCode, result.ErrorMessage, result.TraceId);
        }
        catch (Exception exception)
        {
            response.Success = false;
            LogHelper.Error(exception, "FriendComponentAgent.OnAddFriend 跨进程调用异常");
        }
        finally
        {
            MessageObjectPoolHelper.Return(reqInnerAddFriend);
        }
    }

    /// <summary>
    /// Social 进程内的好友列表处理。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnInnerFriendList(INetWorkChannel netWorkChannel, ReqInnerFriendList request, RespInnerFriendList response)
    {
        response.Friends = new List<FriendInfo>
        {
            new FriendInfo
            {
                PlayerId = request.PlayerId <= 0 ? 1 : request.PlayerId,
                PlayerName = "SocialLocalFriend"
            }
        };
        response.ErrorCode = 0;
        await Task.CompletedTask;
    }

    /// <summary>
    /// 将技术层状态码映射为业务层错误码。
    /// </summary>
    /// <param name="statusCode">技术状态码</param>
    /// <returns>业务错误码</returns>
    private static int MapToBusinessErrorCode(RemoteStatusCode statusCode)
    {
        return statusCode switch
        {
            RemoteStatusCode.Success => 0,
            RemoteStatusCode.Timeout => -1,
            RemoteStatusCode.ConnectionFailed => -2,
            RemoteStatusCode.EndpointNotFound => -3,
            RemoteStatusCode.Cancelled => -4,
            RemoteStatusCode.RetryExhausted => -5,
            RemoteStatusCode.ConnectionClosed => -6,
            RemoteStatusCode.UnexpectedResponse => -7,
            _ => -99,
        };
    }
}
