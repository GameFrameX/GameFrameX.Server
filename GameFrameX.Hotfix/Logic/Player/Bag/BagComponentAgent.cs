using GameFrameX.Apps.Player.Bag.Component;
using GameFrameX.Apps.Player.Bag.Entity;
using GameFrameX.Config;
using GameFrameX.Config.Tables;
using GameFrameX.Network.RemoteMessaging.Unified;

namespace GameFrameX.Hotfix.Logic.Player.Bag;

public class BagComponentAgent : StateComponentAgent<BagComponent, BagState>
{
    /// <summary>
    /// 增加背包物品
    /// </summary>
    /// <param name="networkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task OnAddBagItemAsync(INetworkChannel networkChannel, ReqAddItem message, RespAddItem response)
    {
        // 校验物品是否存在
        foreach (var item in message.ItemDic)
        {
            if (ConfigComponent.Instance.GetConfig<TbItemConfig>().TryGet(item.Key, out var tbItemConfig))
            {
                continue;
            }

            response.ErrorCode = (int)OperationStatusCode.NotFound;
            return;
        }

        var result = await UpdateChangedAsync(networkChannel, message.ItemDic);
        if (result.IsNull())
        {
            response.ErrorCode = (int)OperationStatusCode.Unprocessable;
        }
    }

    /// <summary>
    /// 增加背包物品
    /// </summary>
    /// <param name="networkChannel"></param>
    /// <param name="itemDic"></param>
    /// <returns></returns>
    public async Task<BagState> UpdateChangedAsync(INetworkChannel networkChannel, Dictionary<int, long> itemDic)
    {
        //将物品添加到背包
        var bagState = OwnerComponent.State;
        var notifyBagInfoChanged = new NotifyBagInfoChanged();
        foreach (var item in itemDic)
        {
            var notifyBagItem = new NotifyBagItem() { ItemId = item.Key };
            if (bagState.List.TryGetValue(item.Key, out var value))
            {
                value.Count += item.Value;
                notifyBagItem.Count = value.Count;
                notifyBagItem.Value = item.Value;
            }
            else
            {
                bagState.List[item.Key] = new BagItemState() { Count = item.Value, ItemId = item.Key };
                notifyBagItem.Count = item.Value;
                notifyBagItem.Value = item.Value;
            }

            notifyBagInfoChanged.ItemDic[item.Key] = notifyBagItem;
        }


        var notifyResult = await UnifiedMessageSenderHolder.Sender.SendToPlayerAsync(
            ActorId,
            notifyBagInfoChanged,
            PlayerSendOptions.Notification());
        if (!notifyResult.IsSuccess)
        {
            throw new InvalidOperationException(
                $"NotifyBagInfoChanged delivery failed, PlayerId={ActorId}, Status={notifyResult.Status}, Error={notifyResult.ErrorMessage}");
        }

        await OwnerComponent.WriteStateAsync();
        return bagState;
    }

    /// <summary>
    /// 减少背包物品
    /// </summary>
    /// <param name="networkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public async Task OnRemoveBagItemAsync(INetworkChannel networkChannel, ReqRemoveItem message, RespRemoveItem response)
    {
        // 校验物品是否存在
        foreach (var item in message.ItemDic)
        {
            if (ConfigComponent.Instance.GetConfig<TbItemConfig>().TryGet(item.Key, out var tbItemConfig))
            {
                continue;
            }

            response.ErrorCode = (int)OperationStatusCode.NotFound;
            return;
        }

        //将物品添加到背包
        var bagState = OwnerComponent.State;
        if (bagState.IsNotNull())
        {
            foreach (var item in message.ItemDic)
            {
                if (bagState.List.TryGetValue((int)item.Key, out var value))
                {
                    value.Count -= item.Value;
                    if (value.Count < 0)
                    {
                        response.ErrorCode = (int)OperationStatusCode.Unprocessable;
                        return;
                    }
                }
                else
                {
                    response.ErrorCode = (int)OperationStatusCode.NotFound;
                    return;
                }
            }
        }

        await OwnerComponent.WriteStateAsync();
    }

    /// <summary>
    /// 减少背包物品
    /// </summary>
    /// <param name="networkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public async Task OnUseBagItemAsync(INetworkChannel networkChannel, ReqUseItem message, RespUseItem response)
    {
        // 校验物品是否存在
        if (!ConfigComponent.Instance.GetConfig<TbItemConfig>().TryGet(message.ItemId, out var tbItemConfig))
        {
            response.ErrorCode = (int)OperationStatusCode.NotFound;
            return;
        }

        //将物品从背包中删除
        var bagState = OwnerComponent.State;
        if (bagState.List.TryGetValue(message.ItemId, out var value))
        {
            response.ItemId = message.ItemId;

            value.Count -= message.Count;
            if (value.Count < 0)
            {
                value.Count = 0;
                bagState.List.Remove(message.ItemId); // 移除
            }

            response.Count = value.Count;
            var notifyBagInfoChanged = new NotifyBagInfoChanged
            {
                ItemDic =
                {
                    [message.ItemId] = new NotifyBagItem() { Count = value.Count, ItemId = message.ItemId, Value = message.Count },
                },
            };
            var notifyResult = await UnifiedMessageSenderHolder.Sender.SendToPlayerAsync(
                ActorId,
                notifyBagInfoChanged,
                PlayerSendOptions.Notification());
            if (!notifyResult.IsSuccess)
            {
                throw new InvalidOperationException(
                    $"NotifyBagInfoChanged delivery failed, PlayerId={ActorId}, Status={notifyResult.Status}, Error={notifyResult.ErrorMessage}");
            }
        }
        else
        {
            response.ErrorCode = (int)OperationStatusCode.NotFound;
            return;
        }

        await OwnerComponent.WriteStateAsync();
    }

    /// <summary>
    /// 异步请求背包数据
    /// </summary>
    /// <param name="networkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public async Task OnReqBagInfoAsync(INetworkChannel networkChannel, ReqBagInfo message, RespBagInfo response)
    {
        var bagState = OwnerComponent.State;
        if (bagState.IsNotNull())
        {
            response.ItemDic = bagState.List.ToDictionary(x => x.Key, x => x.Value.Count);
        }

        await Task.CompletedTask;
    }
}
