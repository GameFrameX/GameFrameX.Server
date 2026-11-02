using GameFrameX.Apps.Common.EventData;
using GameFrameX.Core.Session;
using GameFrameX.Apps.Player.Bag.Entity;
using GameFrameX.Hotfix.Logic.Player.Bag;
using GameFrameX.Network.RemoteMessaging.Unified;
using GameFrameX.Foundation.Localization.Core;

namespace GameFrameX.Hotfix.Logic.Http.Bag;

/// <summary>
/// 请求给玩家发送道具
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerSendItemHttpHandler), typeof(ReqPlayerSendItemRequest), typeof(ReqPlayerSendItemResponse))]
[RequireHttpSignature]
[Description("请求给玩家发送道具")]
public sealed class ReqPlayerSendItemHttpHandler : BaseHttpHandler
{
    public override async Task<string> ActionAsync(HttpActionContext context)
    {
        var sendItemRequest = (ReqPlayerSendItemRequest)context.Request;
        var playerSession = PlayerSessionManager.Instance.GetByPlayerId(sendItemRequest.PlayerId);
        Dictionary<int, long> itemDic = new Dictionary<int, long>();

        var tbItemConfig = ConfigComponent.Instance.GetConfig<TbItemConfig>();
        foreach (var item in sendItemRequest.Items)
        {
            if (!tbItemConfig.TryGet(item.Key, out _))
            {
                continue;
            }

            itemDic[item.Key] = item.Value;
        }

        // 发送道具事件
        var playerSendItemEventData = new PlayerSendItemEventArgs(sendItemRequest.PlayerId, itemDic);
        EventDispatcher.Dispatch(sendItemRequest.PlayerId, playerSendItemEventData);
        if (playerSession.IsNotNull())
        {
            // 玩家在线
            var bagComponentAgent = await ActorManager.GetComponentAgentAsync<BagComponentAgent>(sendItemRequest.PlayerId);
            await bagComponentAgent.UpdateChangedAsync(playerSession.WorkChannel, itemDic);
        }
        else
        {
            // 玩家不在线
            await UpdateOfflineBagAsync(sendItemRequest.PlayerId, itemDic);
        }

        return HttpJsonResultData<string>.SuccessString(JsonHelper.Serialize(new ReqPlayerSendItemResponse { Items = itemDic, }));
    }

    // 玩家不在线：直接落库并尝试发送道具变更通知（若玩家刚上线可立即收到；仍离线则按策略丢弃）
    private static async Task UpdateOfflineBagAsync(long playerId, Dictionary<int, long> itemDic)
    {
        var bagState = await GameDb.FindAsync<BagState>(playerId);

        foreach (var item in itemDic)
        {
            if (bagState.List.TryGetValue(item.Key, out var value))
            {
                value.Count += item.Value;
                if (value.Count <= 0)
                {
                    bagState.List.Remove(item.Key);
                }
            }
            else
            {
                var bagItem = new BagItemState
                {
                    Count = item.Value,
                    ItemId = item.Key,
                };
                bagState.List[item.Key] = bagItem;
            }
        }

        await GameDb.SaveOneAsync(bagState);

        // 离线落库后尝试统一通知（若玩家刚上线可立即收到；仍离线则按策略丢弃）
        var notifyBagInfoChanged = new NotifyBagInfoChanged();
        foreach (var item in itemDic)
        {
            notifyBagInfoChanged.ItemDic[item.Key] = new NotifyBagItem
            {
                ItemId = item.Key,
                Count = bagState.List.TryGetValue(item.Key, out var bagItemState) ? bagItemState.Count : 0,
                Value = item.Value,
            };
        }

        var notifyOptions = PlayerSendOptions.Notification();
        notifyOptions.OfflineStrategy = PlayerOfflineStrategy.Discard;
        var notifyResult = await UnifiedMessageSenderHolder.Sender.SendToPlayerAsync(
            playerId,
            notifyBagInfoChanged,
            notifyOptions);
        if (!notifyResult.IsSuccess && notifyResult.Status != PlayerDeliverStatus.Offline)
        {
            // Localization: Hotfix.Bag.OfflineNotifySendFailed - ReqPlayerSendItemHttpHandler 离线通知发送失败, playerId: {0}, status: {1}, error: {2}, traceId: {3}
            LogHelper.Warning(
                LocalizationService.GetString(Localization.Keys.Hotfix.Bag.OfflineNotifySendFailed,
                    playerId,
                    notifyResult.Status,
                    notifyResult.ErrorMessage,
                    notifyResult.TraceId));
        }
    }
}
