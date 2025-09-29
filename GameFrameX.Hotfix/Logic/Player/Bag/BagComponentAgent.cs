// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Apps.Player.Bag.Component;
using GameFrameX.Apps.Player.Bag.Entity;
using GameFrameX.Config;
using GameFrameX.Config.Tables;
using GameFrameX.Foundation.Extensions;

namespace GameFrameX.Hotfix.Logic.Player.Bag;

public class BagComponentAgent : StateComponentAgent<BagComponent, BagState>
{
    /// <summary>
    /// 增加背包物品
    /// </summary>
    /// <param name="netWorkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    /// <exception cref="NotImplementedException"></exception>
    public async Task OnAddBagItem(INetWorkChannel netWorkChannel, ReqAddItem message, RespAddItem response)
    {
        // 校验物品是否存在
        foreach (var item in message.ItemDic)
        {
            var hasItem = ConfigComponent.Instance.GetConfig<TbItemConfig>().Get((int)item.Key);
            if (hasItem.IsNull())
            {
                response.ErrorCode = (int)OperationStatusCode.NotFound;
                return;
            }
        }

        await UpdateChanged(netWorkChannel, message.ItemDic);
    }

    /// <summary>
    /// 增加背包物品
    /// </summary>
    /// <param name="netWorkChannel"></param>
    /// <param name="itemDic"></param>
    /// <returns></returns>
    public async Task UpdateChanged(INetWorkChannel netWorkChannel, Dictionary<int, long> itemDic)
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


        await netWorkChannel.WriteAsync(notifyBagInfoChanged);
        await OwnerComponent.WriteStateAsync();
    }

    /// <summary>
    /// 减少背包物品
    /// </summary>
    /// <param name="netWorkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public async Task OnRemoveBagItem(INetWorkChannel netWorkChannel, ReqRemoveItem message, RespRemoveItem response)
    {
        // 校验物品是否存在
        foreach (var item in message.ItemDic)
        {
            var hasItem = ConfigComponent.Instance.GetConfig<TbItemConfig>().Get((int)item.Key);
            if (hasItem.IsNull())
            {
                response.ErrorCode = (int)OperationStatusCode.NotFound;
                return;
            }
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
    /// <param name="netWorkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public async Task OnUseBagItem(INetWorkChannel netWorkChannel, ReqUseItem message, RespUseItem response)
    {
        // 校验物品是否存在
        var hasItem = ConfigComponent.Instance.GetConfig<TbItemConfig>().Get(message.ItemId);
        if (hasItem.IsNull())
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
            await netWorkChannel.WriteAsync(notifyBagInfoChanged);
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
    /// <param name="netWorkChannel"></param>
    /// <param name="message"></param>
    /// <param name="response"></param>
    public void OnReqBagInfoAsync(INetWorkChannel netWorkChannel, ReqBagInfo message, RespBagInfo response)
    {
        var bagState = OwnerComponent.State;
        if (bagState.IsNotNull())
        {
            response.ItemDic = bagState.List.ToDictionary(x => x.Key, x => x.Value.Count);
        }
    }
}