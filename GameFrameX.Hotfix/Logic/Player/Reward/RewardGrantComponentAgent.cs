using GameFrameX.Core.Session;
using GameFrameX.Apps.Player.Reward;
using GameFrameX.Apps.Player.Reward.Component;
using GameFrameX.Apps.Player.Reward.Entity;
using GameFrameX.Config;
using GameFrameX.Config.Tables;
using GameFrameX.Core.Abstractions.Attribute;
using GameFrameX.Foundation.Utility;
using GameFrameX.Hotfix.Logic.Player.Bag;

namespace GameFrameX.Hotfix.Logic.Player.Reward;

/// <summary>
/// 统一奖励发放组件代理。提供跨组件可调用的 <see cref="GrantAsync"/>，作为邮件 / 运营补偿 / 兑换码 / 活动奖励等业务发放资产的唯一入口。
/// </summary>
/// <remarks>
/// 发放幂等（B6）以 <c>RoleId:SourceType:SourceId:TraceId</c> 为键，命中账本则短路返回上次结果，不重复写背包、不重复扣附件。
/// 一期仅落地普通道具路由（写 <c>BagState</c>），其余奖励类型为预留路由位，返回 <c>Unsupported</c>，不污染普通背包。
/// </remarks>
public class RewardGrantComponentAgent : StateComponentAgent<RewardGrantComponent, RewardGrantRecordState>
{
    /// <summary>
    /// 统一奖励发放。幂等键 <c>RoleId:SourceType:SourceId:TraceId</c> 重复提交时短路返回上次结果（B6）。
    /// </summary>
    /// <param name="request">发放请求。调用方需保证 <see cref="RewardGrantRequest.SourceType"/> 非 <see cref="RewardSourceType.None"/>，<see cref="RewardGrantRequest.SourceId"/>/<see cref="RewardGrantRequest.TraceId"/> 非空。</param>
    /// <returns>逐项发放结果、整体错误码与是否全部成功。</returns>
    [Service]
    public virtual async Task<RewardGrantResult> GrantAsync(RewardGrantRequest request)
    {
        var traceKey = BuildTraceKey(request);

        // 幂等短路（B6）：命中账本直接返回上次结果，不重复发奖。
        if (State.Records.TryGetValue(traceKey, out var existed))
        {
            return new RewardGrantResult
            {
                Items = existed.Items,
                ErrorCode = existed.ErrorCode,
                AllSuccess = existed.AllSuccess,
            };
        }

        var result = await DispatchAsync(request);

        // 记录幂等账本，持久化。记录写入后不可变。
        State.Records[traceKey] = new RewardGrantRecord
        {
            TraceKey = traceKey,
            SourceType = (int)request.SourceType,
            SourceId = request.SourceId,
            TraceId = request.TraceId,
            GrantedAt = TimerHelper.UnixTimeSeconds(),
            AllSuccess = result.AllSuccess,
            ErrorCode = result.ErrorCode,
            Items = result.Items,
        };
        await OwnerComponent.WriteStateAsync();

        return result;
    }

    /// <summary>
    /// 按 <see cref="RewardType"/> 路由发放各项奖励。
    /// </summary>
    private async Task<RewardGrantResult> DispatchAsync(RewardGrantRequest request)
    {
        var items = new List<RewardGrantItemResult>(request.Rewards.Count);
        var normalItemDic = new Dictionary<int, long>();
        var bagItemResultIndex = new List<int>();

        for (var i = 0; i < request.Rewards.Count; i++)
        {
            var reward = request.Rewards[i];
            if (TryCollectNormalItem(reward, normalItemDic, out var failure))
            {
                // 暂记成功：结果延后由批量写背包回填。
                bagItemResultIndex.Add(i);
            }
            else
            {
                items.Add(failure);
            }
        }

        // 普通道具批量写背包（在线推 notify、离线只持久化）。
        if (normalItemDic.Count > 0)
        {
            var bagAgent = await ActorManager.GetComponentAgentAsync<BagComponentAgent>();
            var channel = PlayerSessionManager.Instance.GetByPlayerId(request.PlayerId)?.WorkChannel;
            var bagState = await bagAgent.UpdateChangedAsync(channel, normalItemDic);
            var bagFailed = bagState.IsNull();

            foreach (var idx in bagItemResultIndex)
            {
                var reward = request.Rewards[idx];
                items.Add(new RewardGrantItemResult
                {
                    RewardType = reward.RewardType,
                    ItemId = reward.ItemId,
                    Count = reward.Count,
                    Success = !bagFailed,
                    ErrorCode = bagFailed ? (int)OperationStatusCode.Unprocessable : (int)OperationStatusCode.Ok,
                });
            }
        }

        Aggregate(items, out var errorCode, out var allSuccess);

        return new RewardGrantResult
        {
            Items = items,
            ErrorCode = errorCode,
            AllSuccess = allSuccess,
        };
    }

    /// <summary>
    /// 校验单个奖励项并暂记普通道具待发放数量。
    /// </summary>
    /// <param name="reward">待校验的奖励项。</param>
    /// <param name="normalItemDic">普通道具累计表（ItemId → 发放总数），合法普通道具就地累加。</param>
    /// <param name="failure">校验失败时的单项失败结果；合法暂记时为 <c>null</c>。</param>
    /// <returns>合法普通道具已暂记待发放返回 <c>true</c>（结果延后由批量写背包决定）；其余返回 <c>false</c> 并经 <paramref name="failure"/> 给出失败原因。</returns>
    private static bool TryCollectNormalItem(RewardItem reward, Dictionary<int, long> normalItemDic, out RewardGrantItemResult failure)
    {
        if (reward.RewardType != RewardType.NormalItem)
        {
            // 预留路由位：隐藏道具 / 月卡 / 终生卡 / VIP 点 / 权益一期不实现。
            failure = BuildFailure(reward, OperationStatusCode.Unsupported);
            return false;
        }

        if (reward.Count <= 0)
        {
            failure = BuildFailure(reward, OperationStatusCode.InvalidReward);
            return false;
        }

        if (!ConfigComponent.Instance.GetConfig<TbItemConfig>().TryGet(reward.ItemId, out _))
        {
            failure = BuildFailure(reward, OperationStatusCode.NotFound);
            return false;
        }

        // 暂记待发放，稍后批量写背包。
        normalItemDic[reward.ItemId] = normalItemDic.TryGetValue(reward.ItemId, out var sum) ? sum + reward.Count : reward.Count;
        failure = null;
        return true;
    }

    /// <summary>
    /// 按奖励项原样字段构造单项失败结果（<c>Success=false</c> + 指定错误码）。
    /// </summary>
    private static RewardGrantItemResult BuildFailure(RewardItem reward, OperationStatusCode errorCode)
    {
        return new RewardGrantItemResult
        {
            RewardType = reward.RewardType,
            ItemId = reward.ItemId,
            Count = reward.Count,
            Success = false,
            ErrorCode = (int)errorCode,
        };
    }

    /// <summary>
    /// 构造幂等键 <c>RoleId:SourceType:SourceId:TraceId</c>（键格式为持久化契约，勿随命名统一改动）。
    /// </summary>
    private static string BuildTraceKey(RewardGrantRequest request)
    {
        return request.PlayerId + ":" + (int)request.SourceType + ":" + request.SourceId + ":" + request.TraceId;
    }

    /// <summary>
    /// 汇总逐项结果为整体错误码与是否全部成功：
    /// 全部成功 <c>Ok</c>；部分成功 <c>PartialSuccess</c>；全部失败取首个失败项错误码。
    /// </summary>
    private static void Aggregate(List<RewardGrantItemResult> items, out int errorCode, out bool allSuccess)
    {
        if (items.Count == 0)
        {
            errorCode = (int)OperationStatusCode.Ok;
            allSuccess = true;
            return;
        }

        var successCount = 0;
        var firstFailureCode = (int)OperationStatusCode.Ok;
        foreach (var item in items)
        {
            if (item.Success)
            {
                successCount++;
            }
            else if (firstFailureCode == (int)OperationStatusCode.Ok)
            {
                firstFailureCode = item.ErrorCode;
            }
        }

        if (successCount == items.Count)
        {
            errorCode = (int)OperationStatusCode.Ok;
            allSuccess = true;
        }
        else if (successCount == 0)
        {
            errorCode = firstFailureCode;
            allSuccess = false;
        }
        else
        {
            errorCode = (int)OperationStatusCode.PartialSuccess;
            allSuccess = false;
        }
    }
}
