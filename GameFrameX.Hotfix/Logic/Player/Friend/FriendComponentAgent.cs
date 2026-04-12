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
using GameFrameX.Apps.Player.Friend.Entity;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.NetWork.RemoteMessaging.Contracts;
using GameFrameX.NetWork.RemoteMessaging.Unified;

namespace GameFrameX.Hotfix.Logic.Player.Friend;

public class FriendComponentAgent : StateComponentAgent<FriendComponent, FriendState>
{
    private const int RpcTimeoutMilliseconds = 5000;

    /// <summary>
    /// 获取好友列表（通过统一消息发送器调用 Social 服务）。
    /// 好友列表查询为幂等操作，允许重试。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnFriendList(INetWorkChannel netWorkChannel, ReqFriendList request, RespFriendList response)
    {
        if (string.Equals(GlobalSettings.CurrentSetting?.ServerType, GameServerConst.Social.Name, StringComparison.OrdinalIgnoreCase))
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
            var options = ServerSendOptions.Query(RpcTimeoutMilliseconds);
            var result = await UnifiedMessageSenderHolder.Sender.SendToServerAsync<RespInnerFriendList>(
                GameServerConst.Social.Name, reqInnerFriendList, options);

            if (result.IsSuccess && result.Response != null)
            {
                response.Friends = result.Response.Friends ?? new List<FriendInfo>();
                response.ErrorCode = result.Response.ErrorCode;
                return;
            }

            response.Friends = new List<FriendInfo>();
            response.ErrorCode = MapToBusinessErrorCode(result.StatusCode);
            LogHelper.Error("FriendComponentAgent.OnFriendList 统一消息发送失败, StatusCode: {statusCode}, Error: {errorMessage}, TraceId: {traceId}",
                result.StatusCode, result.ErrorMessage, result.TraceId);
        }
        catch (Exception exception)
        {
            response.Friends = new List<FriendInfo>();
            response.ErrorCode = -2;
            LogHelper.Error(exception, "FriendComponentAgent.OnFriendList 统一消息发送异常");
        }
        finally
        {
            MessageObjectPoolHelper.Return(reqInnerFriendList);
        }
    }

    /// <summary>
    /// 添加好友（通过统一消息发送器调用 Social 服务）。
    /// 添加好友为非幂等操作，不允许重试。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnAddFriend(INetWorkChannel netWorkChannel, ReqFriendByAdd request, RespFriendByAdd response)
    {
        var reqInnerAddFriend = MessageObjectPoolHelper.Get<ReqInnerFriendByAdd>();
        reqInnerAddFriend.PlayerId = request.PlayerId;
        reqInnerAddFriend.SourcePlayerId = ActorId;
        if (string.Equals(GlobalSettings.CurrentSetting?.ServerType, GameServerConst.Social.Name, StringComparison.OrdinalIgnoreCase))
        {
            var innerResponse = new RespInnerFriendByAdd();
            await OnInnerAddFriend(netWorkChannel, reqInnerAddFriend, innerResponse);
            response.Success = innerResponse.Success;
            response.ErrorCode = innerResponse.ErrorCode;
            MessageObjectPoolHelper.Return(reqInnerAddFriend);
            return;
        }

        try
        {
            var options = ServerSendOptions.Command(RpcTimeoutMilliseconds);
            var result = await UnifiedMessageSenderHolder.Sender.SendToServerAsync<RespInnerFriendByAdd>(
                GameServerConst.Social.Name, reqInnerAddFriend, options);

            if (result.IsSuccess && result.Response != null)
            {
                response.Success = result.Response.Success;
                response.ErrorCode = NormalizeBusinessErrorCode(result.Response.Success, result.Response.ErrorCode, request.PlayerId, "OnAddFriend");
                return;
            }

            response.Success = false;
            response.ErrorCode = MapToBusinessErrorCode(result.StatusCode);
            LogHelper.Error("FriendComponentAgent.OnAddFriend 统一消息发送失败, StatusCode: {statusCode}, Error: {errorMessage}, TraceId: {traceId}",
                result.StatusCode, result.ErrorMessage, result.TraceId);
        }
        catch (Exception exception)
        {
            response.Success = false;
            response.ErrorCode = -2;
            LogHelper.Error(exception, "FriendComponentAgent.OnAddFriend 统一消息发送异常");
        }
        finally
        {
            MessageObjectPoolHelper.Return(reqInnerAddFriend);
        }
    }

    /// <summary>
    /// 删除好友（通过统一消息发送器调用 Social 服务）。
    /// 删除好友为非幂等操作，不允许重试。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnDeleteFriend(INetWorkChannel netWorkChannel, ReqDeleteFriend request, RespDeleteFriend response)
    {
        var reqInnerDeleteFriend = MessageObjectPoolHelper.Get<ReqInnerFriendByDelete>();
        reqInnerDeleteFriend.PlayerId = request.PlayerId;
        reqInnerDeleteFriend.SourcePlayerId = ActorId;
        if (string.Equals(GlobalSettings.CurrentSetting?.ServerType, GameServerConst.Social.Name, StringComparison.OrdinalIgnoreCase))
        {
            var innerResponse = new RespInnerFriendByDelete();
            await OnInnerDeleteFriend(netWorkChannel, reqInnerDeleteFriend, innerResponse);
            response.Success = innerResponse.Success;
            response.ErrorCode = innerResponse.ErrorCode;
            MessageObjectPoolHelper.Return(reqInnerDeleteFriend);
            return;
        }

        try
        {
            var options = ServerSendOptions.Command(RpcTimeoutMilliseconds);
            var result = await UnifiedMessageSenderHolder.Sender.SendToServerAsync<RespInnerFriendByDelete>(
                GameServerConst.Social.Name, reqInnerDeleteFriend, options);

            if (result.IsSuccess && result.Response != null)
            {
                response.Success = result.Response.Success;
                response.ErrorCode = NormalizeBusinessErrorCode(result.Response.Success, result.Response.ErrorCode, request.PlayerId, "OnDeleteFriend");
                return;
            }

            response.Success = false;
            response.ErrorCode = MapToBusinessErrorCode(result.StatusCode);
            LogHelper.Error("FriendComponentAgent.OnDeleteFriend 统一消息发送失败, StatusCode: {statusCode}, Error: {errorMessage}, TraceId: {traceId}",
                result.StatusCode, result.ErrorMessage, result.TraceId);
        }
        catch (Exception exception)
        {
            response.Success = false;
            response.ErrorCode = -2;
            LogHelper.Error(exception, "FriendComponentAgent.OnDeleteFriend 统一消息发送异常");
        }
        finally
        {
            MessageObjectPoolHelper.Return(reqInnerDeleteFriend);
        }
    }

    /// <summary>
    /// Social 进程内的添加好友处理。
    /// 将好友关系写入数据库。
    /// </summary>
    public async Task OnInnerAddFriend(INetWorkChannel netWorkChannel, ReqInnerFriendByAdd request, RespInnerFriendByAdd response)
    {
        // 优先使用请求中的 SourcePlayerId（跨服场景），否则降级为 ActorId（本服场景）
        var ownerPlayerId = request.SourcePlayerId > 0 ? request.SourcePlayerId : ActorId;
        var targetPlayerId = request.PlayerId;

        // 参数校验
        if (ownerPlayerId <= 0 || targetPlayerId <= 0)
        {
            response.Success = false;
            response.ErrorCode = -100;
            return;
        }

        // 不能加自己为好友
        if (ownerPlayerId == targetPlayerId)
        {
            response.Success = false;
            response.ErrorCode = -101;
            return;
        }

        // 确认目标玩家存在
        var targetPlayer = await GameDb.FindAsync<PlayerState>(targetPlayerId);
        if (targetPlayer == null)
        {
            response.Success = false;
            response.ErrorCode = -102;
            return;
        }

        // 构建关系（确保 PlayerIdA < PlayerIdB 保证唯一性）
        long playerIdA = Math.Min(ownerPlayerId, targetPlayerId);
        long playerIdB = Math.Max(ownerPlayerId, targetPlayerId);

        // 检查是否已存在好友关系
        var existingRelation = await GameDb.FindAsync<FriendRelationState>(
            m => m.PlayerIdA == playerIdA && m.PlayerIdB == playerIdB && m.Status == 0, false);

        if (existingRelation != null)
        {
            response.Success = false;
            response.ErrorCode = -103; // 已是好友
            return;
        }

        // 插入新的好友关系
        var newRelation = new FriendRelationState
        {
            Id = ActorIdGenerator.GetActorId(GlobalConst.ActorTypePlayer),
            PlayerIdA = playerIdA,
            PlayerIdB = playerIdB,
            CreateTime = DateTime.UtcNow,
            CreatedBy = ownerPlayerId,
            Status = 0
        };

        await GameDb.AddOrUpdateAsync(newRelation);
        response.Success = true;
        response.ErrorCode = 0;
    }

    /// <summary>
    /// Social 进程内的好友列表处理。
    /// 从数据库查询所有有效好友关系，并填充好友详情。
    /// </summary>
    /// <param name="netWorkChannel">网络通道</param>
    /// <param name="request">请求</param>
    /// <param name="response">响应</param>
    public async Task OnInnerFriendList(INetWorkChannel netWorkChannel, ReqInnerFriendList request, RespInnerFriendList response)
    {
        var ownerPlayerId = request.PlayerId > 0 ? request.PlayerId : ActorId;

        // 查询所有包含自己的有效好友关系
        var relations = await GameDb.FindListAsync<FriendRelationState>(
            m => (m.PlayerIdA == ownerPlayerId || m.PlayerIdB == ownerPlayerId) && m.Status == 0);

        var friends = new List<FriendInfo>();
        foreach (var relation in relations)
        {
            // 找出对方玩家的ID
            long friendPlayerId = relation.PlayerIdA == ownerPlayerId ? relation.PlayerIdB : relation.PlayerIdA;

            // 查找对方玩家详情
            var friendPlayer = await GameDb.FindAsync<PlayerState>(friendPlayerId);
            if (friendPlayer != null)
            {
                friends.Add(new FriendInfo
                {
                    PlayerId = friendPlayer.Id,
                    PlayerName = friendPlayer.Name
                });
            }
        }

        response.Friends = friends;
        response.ErrorCode = 0;
    }

    /// <summary>
    /// Social 进程内的删除好友处理。
    /// 软删除好友关系（设置 Status = 1）。
    /// </summary>
    public async Task OnInnerDeleteFriend(INetWorkChannel netWorkChannel, ReqInnerFriendByDelete request, RespInnerFriendByDelete response)
    {
        // 优先使用请求中的 SourcePlayerId（跨服场景），否则降级为 ActorId（本服场景）
        var ownerPlayerId = request.SourcePlayerId > 0 ? request.SourcePlayerId : ActorId;
        var targetPlayerId = request.PlayerId;

        // 参数校验
        if (ownerPlayerId <= 0 || targetPlayerId <= 0)
        {
            response.Success = false;
            response.ErrorCode = -100;
            return;
        }

        // 构建关系键
        long playerIdA = Math.Min(ownerPlayerId, targetPlayerId);
        long playerIdB = Math.Max(ownerPlayerId, targetPlayerId);

        // 查找好友关系
        var relation = await GameDb.FindAsync<FriendRelationState>(
            m => m.PlayerIdA == playerIdA && m.PlayerIdB == playerIdB && m.Status == 0, false);

        if (relation == null)
        {
            response.Success = false;
            response.ErrorCode = -104; // 好友关系不存在
            return;
        }

        // 软删除
        relation.Status = 1;
        await GameDb.AddOrUpdateAsync(relation);

        response.Success = true;
        response.ErrorCode = 0;
    }

    /// <summary>
    /// 将技术层状态码映射为业务层错误码。
    /// </summary>
    /// <param name="statusCode">技术状态码</param>
    /// <returns>业务错误码</returns>
    private static int MapToBusinessErrorCode(RemoteStatusCode statusCode)
    {
        switch (statusCode)
        {
            case RemoteStatusCode.Success:
                return 0;
            case RemoteStatusCode.Timeout:
                return -1;
            case RemoteStatusCode.ConnectionFailed:
                return -2;
            case RemoteStatusCode.EndpointNotFound:
                return -3;
            case RemoteStatusCode.Cancelled:
                return -4;
            case RemoteStatusCode.RetryExhausted:
                return -5;
            case RemoteStatusCode.ConnectionClosed:
                return -6;
            case RemoteStatusCode.UnexpectedResponse:
                return -7;
            default:
                return -99;
        }
    }

    private static int NormalizeBusinessErrorCode(bool success, int errorCode, long requestPlayerId, string actionName)
    {
        if (success || errorCode != 0)
        {
            return errorCode;
        }

        LogHelper.Error("FriendComponentAgent.{actionName} 返回语义异常: Success=false 且 ErrorCode=0, PlayerId={playerId}",
            actionName, requestPlayerId);
        return -101;
    }
}
