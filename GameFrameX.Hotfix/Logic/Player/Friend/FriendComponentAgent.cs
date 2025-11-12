// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
// 
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
// 
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
// 
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
// 
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Apps.Player.Friend;
using GameFrameX.Hotfix.Logic.Rpc;
using GameFrameX.NetWork;
using GameFrameX.Proto.BuiltIn;

namespace GameFrameX.Hotfix.Logic.Player.Friend;

public class FriendComponentAgent : StateComponentAgent<FriendComponent, FriendState>
{
    public async Task OnFriendList(INetWorkChannel netWorkChannel, ReqFriendList request, RespFriendList response)
    {
        // 使用统一的RPC客户端调用社交服务
        var reqInnerFriendList = MessageObjectPoolHelper.Get<ReqInnerFriendList>();
        reqInnerFriendList.PlayerId = ActorId;

        try
        {
            var rpcResult = await ServiceRpcClient.Instance.CallAsync<RespInnerFriendList>(
                GlobalConst.SocialServiceName,
                reqInnerFriendList
            );

            if (rpcResult != null && rpcResult.IsSuccess && rpcResult.Message is RespInnerFriendList friendResponse)
            {
                // 将远程获取的好友列表数据转换给客户端
                response.Friends = friendResponse.Friends ?? new List<FriendInfo>();
                response.ErrorCode = friendResponse.ErrorCode;
                LogHelper.DebugConsole($"Successfully retrieved friend list for player {ActorId}, count: {response.Friends.Count}");
            }
            else
            {
                response.ErrorCode = -1;
                LogHelper.Error($"Failed to get friend list for player {ActorId}: RPC call failed");
            }
        }
        finally
        {
            // 释放消息对象回池
            MessageObjectPoolHelper.Return(reqInnerFriendList);
        }
    }

    public async Task OnAddFriend(INetWorkChannel netWorkChannel, ReqFriendByAdd request, RespFriendByAdd response)
    {
        // 使用统一的RPC客户端调用社交服务添加好友
        var reqInnerAddFriend = MessageObjectPoolHelper.Get<ReqInnerFriendByAdd>();
        reqInnerAddFriend.PlayerId = ActorId;

        try
        {
            var rpcResult = await ServiceRpcClient.Instance.CallAsync<RespInnerFriendByAdd>(
                GlobalConst.SocialServiceName,
                reqInnerAddFriend
            );

            if (rpcResult != null && rpcResult.IsSuccess && rpcResult.Message is RespInnerFriendByAdd addFriendResponse)
            {
                response.Success = addFriendResponse.Success;

                if (response.Success)
                {
                    LogHelper.DebugConsole($"Player {ActorId} successfully added friend {request.PlayerId}");
                }
                else
                {
                    LogHelper.Warning($"Player {ActorId} failed to add friend {request.PlayerId}, error code: {addFriendResponse.ErrorCode}");
                }
            }
            else
            {
                response.Success = false;
                LogHelper.Error($"RPC call failed when adding friend for player {ActorId}");
            }
        }
        finally
        {
            MessageObjectPoolHelper.Return(reqInnerAddFriend);
        }
    }

    public async Task OnInnerFriendList(INetWorkChannel netWorkChannel, ReqInnerFriendList request, RespInnerFriendList response)
    {
        // 这个方法现在主要是由Social服务器调用的，提供本地数据
        response.Friends = new List<FriendInfo>
        {
            new FriendInfo
            {
                PlayerId = 1,
                PlayerName = "Player1"
            }
        };
        await Task.CompletedTask;
    }

    }