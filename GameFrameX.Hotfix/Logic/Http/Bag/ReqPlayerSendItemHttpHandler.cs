// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using GameFrameX.Apps.Common.Session;
using GameFrameX.Apps.Player.Bag.Entity;
using GameFrameX.Config;
using GameFrameX.Config.Tables;
using GameFrameX.DataBase;
using GameFrameX.Hotfix.Logic.Player.Bag;

namespace GameFrameX.Hotfix.Logic.Http.Bag;

/// <summary>
/// 请求给玩家发送道具
/// </summary>
[HttpMessageMapping(typeof(ReqPlayerSendItemHttpHandler))]
[HttpMessageRequest(typeof(ReqPlayerSendItemRequest))]
[HttpMessageResponse(typeof(ReqPlayerSendItemResponse))]
[Description("请求给玩家发送道具")]
public sealed class ReqPlayerSendItemHttpHandler : BaseHttpHandler
{
    public override async Task<string> Action(string ip, string url, HttpMessageRequestBase requestBase)
    {
        var request = (ReqPlayerSendItemRequest)requestBase;
        var playerSession = SessionManager.GetByRoleId(request.RoleId);
        Dictionary<int, long> itemDic = new Dictionary<int, long>();

        var tbItemConfig = ConfigComponent.Instance.GetConfig<TbItemConfig>();
        if (tbItemConfig != null)
        {
            foreach (var item in request.Items)
            {
                if (tbItemConfig.Get(item.Key).IsNull())
                {
                    continue;
                }

                itemDic[item.Key] = item.Value;
            }
        }
        
        if (playerSession.IsNotNull())
        {
            // 玩家在线
            var bagComponentAgent = await ActorManager.GetComponentAgent<BagComponentAgent>(request.RoleId);
            await bagComponentAgent.UpdateChanged(playerSession.WorkChannel, itemDic);
        }
        else
        {
            // 玩家不在线
            var bagState = await GameDb.FindAsync<BagState>(request.RoleId);

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
        }

        return HttpJsonResult.SuccessString(new ReqPlayerSendItemResponse { Items = itemDic, });
    }
}

public sealed class ReqPlayerSendItemRequest : HttpMessageRequestBase
{
    [Required]
    [Description("角色ID")]
    [Range(1, long.MaxValue)]
    public long RoleId { get; set; }

    [Required] [Description("道具列表")] public Dictionary<int, long> Items { get; set; } = new Dictionary<int, long>();
}

public sealed class ReqPlayerSendItemResponse : HttpMessageResponseBase
{
    [Description("成功的道具列表")] public Dictionary<int, long> Items { get; set; } = new Dictionary<int, long>();
}