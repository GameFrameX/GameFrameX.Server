using GameFrameX.Capability.Online.Abstractions.Contracts;
using GameFrameX.Hotfix.Logic.Game.RockPaperScissors;
using GameFrameX.Hotfix.Logic.ServerRole.Match;
using GameFrameX.Hotfix.Logic.Player.Friend;


/// <summary>
/// 测试
/// http://localhost:20001/game/api/test
/// </summary>
[HttpMessageMapping(typeof(TestHttpHandler), null, typeof(HttpTestResponse))]
[Description("测试通讯接口。没有实际用途")]
public sealed class TestHttpHandler : BaseHttpHandler
{
    /// <summary>
    /// 测试入口，支持普通回包与跨进程通讯示例。
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<string> ActionAsync(HttpActionContext context)
    {
        var paramMap = context.Parameters;
        if (TryGetMode(paramMap, out var mode) && string.Equals(mode, "cross-process-friend", StringComparison.OrdinalIgnoreCase))
        {
            // 好友查询统一经好友域桥（旧 Social Inner 转发链退役）；显式 playerId 为验证请求主体。
            return HandleFriendDomainQuery(paramMap, "cross-process-friend");
        }

        // 验证入口（与上一模式同路径）：好友域桥穿透复核（显式 playerId 为验证请求主体；
        // 作用域三元组仍由 lease 授权供给）。
        if (TryGetMode(paramMap, out var bridgeMode) && string.Equals(bridgeMode, "online-bridge-friend", StringComparison.OrdinalIgnoreCase))
        {
            return HandleFriendDomainQuery(paramMap, "online-bridge-friend");
        }

        // 验证入口：对局桥穿透复核（显式 playerId 为验证请求主体；roomId/round/gesture 定位对局与意图；
        // 作用域三元组仍由 lease 授权供给）。
        if (TryGetMode(paramMap, out var matchBridgeMode) && string.Equals(matchBridgeMode, "online-bridge-match", StringComparison.OrdinalIgnoreCase))
        {
            return HandleOnlineBridgeMatch(paramMap);
        }

        // 验证入口：匹配域桥穿透复核（显式 playerId 为验证请求主体；action 定位入队/取消/状态/入场；
        // 与 420 协议入口相同的薄适配器代码路径）。
        if (TryGetMode(paramMap, out var matchDomainMode) && string.Equals(matchDomainMode, "online-bridge-matchdomain", StringComparison.OrdinalIgnoreCase))
        {
            return HandleOnlineBridgeMatchDomain(paramMap);
        }


        var response = new HttpTestResponse
        {
            Message = "hello",
        };
        return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
    }

    /// <summary>
    /// 获取测试模式参数。
    /// </summary>
    /// <param name="parameters">请求参数</param>
    /// <param name="mode">模式值</param>
    /// <returns>是否获取成功</returns>
    private static bool TryGetMode(Dictionary<string, object> parameters, out string mode)
    {
        mode = string.Empty;
        if (!parameters.TryGetValue("mode", out var modeValue) || modeValue == null)
        {
            return false;
        }

        mode = modeValue.ToString() ?? string.Empty;
        return !string.IsNullOrWhiteSpace(mode);
    }

    /// <summary>
    /// 好友域桥穿透验证入口：以显式 <c>playerId</c> 为验证请求主体走域桥路径
    ///（与协议入口 ReqFriendList 相同的薄适配器代码路径）；playerId 缺省或非法（≤0）时显式回
    /// 会话无效错误码，不伪装成功。
    /// </summary>
    /// <param name="parameters">请求参数。</param>
    /// <param name="modeName">回显模式名。</param>
    /// <returns>序列化响应。</returns>
    private static string HandleFriendDomainQuery(Dictionary<string, object> parameters, string modeName)
    {
        long playerId = 0;
        if (parameters.TryGetValue("playerId", out var playerIdValue) && playerIdValue != null)
        {
            long.TryParse(playerIdValue.ToString(), out playerId);
        }

        // 写路径验证：action + peer 参数存在时走 660 族薄适配器代码路径（request/accept/reject/remove/relations）。
        if (parameters.TryGetValue("action", out var actionValue) && actionValue != null && !string.IsNullOrWhiteSpace(actionValue.ToString()))
        {
            return HandleFriendDomainAction(parameters, modeName, playerId, actionValue.ToString());
        }

        var bridgeResponse = new RespFriendList();
        if (playerId > 0)
        {
            FriendDomainBridge.QueryFriendListAsync(playerId, bridgeResponse).GetAwaiter().GetResult();
        }
        else
        {
            bridgeResponse.Friends = new List<FriendInfo>();
            bridgeResponse.ErrorCode = -(int)OnlineErrorCode.SessionInvalid;
        }

        var response = new HttpOnlineBridgeFriendResponse
        {
            Mode = modeName,
            PlayerId = playerId,
            BridgeEnabled = playerId > 0,
            ErrorCode = bridgeResponse.ErrorCode,
            FriendCount = bridgeResponse.Friends?.Count ?? 0,
            Friends = bridgeResponse.Friends ?? new List<FriendInfo>(),
        };
        return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
    }

    /// <summary>
    /// 好友域桥写路径验证入口：以显式 <c>playerId</c> 为验证请求主体、<c>peer</c> 为对方，
    /// <c>action</c> 路由 660 族薄适配器（SendRequest/Accept/Reject/Remove/ListRelations 同一映射代码路径）。
    /// </summary>
    /// <param name="parameters">请求参数。</param>
    /// <param name="modeName">回显模式名。</param>
    /// <param name="playerId">验证请求主体。</param>
    /// <param name="action">动作名。</param>
    /// <returns>序列化响应。</returns>
    private static string HandleFriendDomainAction(Dictionary<string, object> parameters, string modeName, long playerId, string action)
    {
        long peerId = 0;
        if (parameters.TryGetValue("peer", out var peerValue) && peerValue != null)
        {
            long.TryParse(peerValue.ToString(), out peerId);
        }

        var errorCode = playerId > 0 ? 0 : -(int)OnlineErrorCode.SessionInvalid;
        List<long> friends = null;
        List<long> incoming = null;
        List<long> outgoing = null;
        if (playerId > 0)
        {
            switch (action.ToLowerInvariant())
            {
                case "request":
                {
                    var response = new RespFriendSendRequest();
                    FriendDomainBridge.SendRequestAsync(playerId, peerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    break;
                }

                case "accept":
                {
                    var response = new RespFriendAccept();
                    FriendDomainBridge.AcceptAsync(playerId, peerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    break;
                }

                case "reject":
                {
                    var response = new RespFriendReject();
                    FriendDomainBridge.RejectAsync(playerId, peerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    break;
                }

                case "remove":
                {
                    var response = new RespFriendRemove();
                    FriendDomainBridge.RemoveAsync(playerId, peerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    break;
                }

                case "relations":
                {
                    var response = new RespFriendRelationList();
                    FriendDomainBridge.ListRelationsAsync(playerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    friends = response.Friends;
                    incoming = response.IncomingRequests;
                    outgoing = response.OutgoingRequests;
                    break;
                }

                default:
                    errorCode = -(int)OnlineErrorCode.ParameterInvalid;
                    break;
            }
        }

        var result = new HttpOnlineFriendActionResponse
        {
            Mode = modeName,
            Action = action,
            PlayerId = playerId,
            PeerId = peerId,
            ErrorCode = errorCode,
            Friends = friends,
            IncomingRequests = incoming,
            OutgoingRequests = outgoing,
        };
        return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(result));
    }

    /// <summary>
    /// 对局桥穿透验证入口：以显式 <c>playerId</c> 为验证请求主体、<c>roomId</c>/<c>round</c> 定位对局，
    /// <c>gesture</c> 提交出拳（缺省仅查询视图）——与协议入口相同的薄适配器代码路径；
    /// 桥开关未开启时显式回 <c>BridgeEnabled=false</c>，不伪装成功。
    /// </summary>
    /// <param name="parameters">请求参数。</param>
    /// <returns>序列化响应。</returns>
    private static string HandleOnlineBridgeMatch(Dictionary<string, object> parameters)
    {
        long playerId = 0;
        long roomId = 0;
        var round = 1;
        var gesture = 0;
        if (parameters.TryGetValue("playerId", out var playerIdValue) && playerIdValue != null)
        {
            long.TryParse(playerIdValue.ToString(), out playerId);
        }

        if (parameters.TryGetValue("roomId", out var roomIdValue) && roomIdValue != null)
        {
            long.TryParse(roomIdValue.ToString(), out roomId);
        }

        if (parameters.TryGetValue("round", out var roundValue) && int.TryParse(roundValue?.ToString(), out var parsedRound) && parsedRound > 0)
        {
            round = parsedRound;
        }

        if (parameters.TryGetValue("gesture", out var gestureValue) && int.TryParse(gestureValue?.ToString(), out var parsedGesture))
        {
            gesture = parsedGesture;
        }

        var bridgeEnabled = RockPaperScissorsMatchBridge.IsBridgeEnabled;
        var handled = false;
        OnlineMatchBridgeGameView view = null;
        var errorCode = 0;
        if (bridgeEnabled && playerId > 0 && roomId > 0)
        {
            if (gesture > 0)
            {
                var bridgeResponse = new RespSubmitRockPaperScissorsGesture();
                (handled, view) = RockPaperScissorsMatchBridge.TrySubmitGestureAsync(playerId, roomId, round, gesture, bridgeResponse).GetAwaiter().GetResult();
                errorCode = bridgeResponse.ErrorCode;
            }
            else
            {
                var bridgeResponse = new RespRockPaperScissorsGameInfo();
                (handled, view) = RockPaperScissorsMatchBridge.TryGetGameInfoAsync(playerId, roomId, round, bridgeResponse).GetAwaiter().GetResult();
                errorCode = bridgeResponse.ErrorCode;
            }
        }

        var response = new HttpOnlineBridgeMatchResponse
        {
            Mode = "online-bridge-match",
            PlayerId = playerId,
            RoomId = roomId,
            Round = round,
            BridgeEnabled = bridgeEnabled,
            Handled = handled,
            ErrorCode = errorCode,
            GameState = view == null ? null : view.State.ToString(),
            MatchId = view?.MatchId,
            ViewRound = view?.Round ?? 0,
            WinnerPlayerId = view?.WinnerPlayerId ?? 0,
            Players = view == null
                ? new List<HttpOnlineBridgeMatchPlayer>()
                : view.Players.Select(player => new HttpOnlineBridgeMatchPlayer
                {
                    PlayerId = player.PlayerId,
                    HasGesture = player.HasGesture,
                    Gesture = player.Gesture,
                    Wins = player.Wins,
                }).ToList(),
        };
        return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(response));
    }

    /// <summary>
    /// 匹配域桥穿透验证入口：以显式 <c>playerId</c> 为验证请求主体、<c>action</c> 定位
    /// enqueue/cancel/status/enter（enter 携 <c>matchId</c>；enqueue 携 mode/rating）——
    /// 与 420 协议入口相同的薄适配器代码路径；桥不可用时显式回错误码，不伪装成功。
    /// </summary>
    /// <param name="parameters">请求参数。</param>
    /// <returns>序列化响应。</returns>
    private static string HandleOnlineBridgeMatchDomain(Dictionary<string, object> parameters)
    {
        long playerId = 0;
        long matchId = 0;
        var mode = 1;
        var rating = 1000;
        var action = "status";
        if (parameters.TryGetValue("playerId", out var playerIdValue) && playerIdValue != null)
        {
            long.TryParse(playerIdValue.ToString(), out playerId);
        }

        if (parameters.TryGetValue("matchId", out var matchIdValue) && matchIdValue != null)
        {
            long.TryParse(matchIdValue.ToString(), out matchId);
        }

        if (parameters.TryGetValue("mode", out var modeValue) && int.TryParse(modeValue?.ToString(), out var parsedMode) && parsedMode > 0)
        {
            mode = parsedMode;
        }

        if (parameters.TryGetValue("rating", out var ratingValue) && int.TryParse(ratingValue?.ToString(), out var parsedRating) && parsedRating > 0)
        {
            rating = parsedRating;
        }

        if (parameters.TryGetValue("action", out var actionValue) && actionValue != null && !string.IsNullOrWhiteSpace(actionValue.ToString()))
        {
            action = actionValue.ToString();
        }

        var errorCode = 0;
        var queuing = false;
        var started = false;
        if (playerId > 0)
        {
            switch (action.ToLowerInvariant())
            {
                case "enqueue":
                {
                    var response = new RespMatchEnqueue();
                    MatchDomainBridge.EnqueueAsync(playerId, new ReqMatchEnqueue { Mode = (MatchMode)mode, Rating = rating }, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    matchId = response.MatchId;
                    break;
                }

                case "cancel":
                {
                    var response = new RespMatchCancel();
                    MatchDomainBridge.CancelAsync(playerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    break;
                }

                case "enter":
                {
                    var response = new RespMatchEnter();
                    MatchDomainBridge.EnterAsync(playerId, new ReqMatchEnter { MatchId = matchId }, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    matchId = response.MatchId;
                    started = response.Started;
                    break;
                }

                default:
                {
                    var response = new RespMatchStatus();
                    MatchDomainBridge.StatusAsync(playerId, response).GetAwaiter().GetResult();
                    errorCode = response.ErrorCode;
                    queuing = response.Queuing;
                    matchId = response.MatchId;
                    break;
                }
            }
        }
        else
        {
            errorCode = (int)MatchErrorCode.NotLoggedIn;
        }

        var result = new HttpOnlineBridgeMatchDomainResponse
        {
            Mode = "online-bridge-matchdomain",
            Action = action,
            PlayerId = playerId,
            MatchId = matchId,
            Queuing = queuing,
            Started = started,
            ErrorCode = errorCode,
        };
        return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(result));
    }
}
