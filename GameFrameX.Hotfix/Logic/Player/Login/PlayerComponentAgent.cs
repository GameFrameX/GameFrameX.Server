using GameFrameX.Core.Session;
using GameFrameX.Apps.Common.EventData;
using GameFrameX.Foundation.Localization.Core;
using GameFrameX.Apps.Player.Player.Component;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Hotfix.Logic.Game.Room;
using GameFrameX.Hotfix.Logic.Player.Attribute;
using GameFrameX.Hotfix.Logic.Player.Mail;
using GameFrameX.Core.Events;

namespace GameFrameX.Hotfix.Logic.Player.Login;

public class PlayerComponentAgent : StateComponentAgent<PlayerComponent, PlayerState>
{
    public async Task OnLogoutAsync()
    {
        EventDispatcher.Dispatch(ActorId, new OnRoleOfflineEventArgs());
        //下线后会被自动回收
        SetAutoRecycle(true);
        await QuartzTimer.RemoveAsync(ScheduleIdSet);
    }

    /// <summary>
    /// 使用角色ID登录
    /// </summary>
    /// <param name="workChannel"></param>
    /// <param name="playerState"></param>
    /// <param name="reqLoginUniqueId"></param>
    /// <param name="response"></param>
    public async Task OnPlayerLoginAsync(INetworkChannel workChannel, PlayerState playerState, RespPlayerLogin response)
    {
        // 更新连接会话数据
        await PlayerSessionManager.Instance.UpdateSessionAsync(workChannel.GameAppSession.SessionId, playerState.Id, playerState.Id.ToString(), NotifyDuplicateLoginAsync);
        response.Code = playerState.State;
        response.CreateTime = playerState.CreatedTime;
        response.PlayerInfo = new PlayerInfo
        {
            Id = playerState.Id,
            Name = playerState.Name,
            Level = playerState.Level,
            State = playerState.State,
            Avatar = playerState.Avatar,
        };

        // 断线前凭据交付：登录成功即随响应下发会话恢复凭据（桥不可用 / 签发失败为 null，登录主流程不受影响）。
        response.SessionToken = await Logic.ServerRole.SessionResume.SessionResumeDomainBridge.IssueLoginCredentialAsync(playerState.Id, workChannel.GameAppSession?.SessionId);

        var attributeComponentAgent = await ActorManager.GetComponentAgentAsync<PlayerAttributeComponentAgent>(playerState.Id);
        await attributeComponentAgent.InitializeDefaultsSilentAsync();

        EventDispatcher.Dispatch(ActorId, new OnRoleOnlineEventArgs());

        // 房间断线重连标记（房间系统）
        var roomComp = await ActorManager.GetComponentAgentAsync<RoomComponentAgent>();
        await roomComp.MarkPlayerReconnectedAsync(ActorId);

        // 推送属性快照 + 邮件懒同步（邮件系统）
        await workChannel.WriteAsync(attributeComponentAgent.BuildSyncSnapshot());
        var mailAgent = await ActorManager.GetComponentAgentAsync<MailComponentAgent>(playerState.Id);
        await mailAgent.SyncAsync();
    }

    /// <summary>
    /// 顶号通知器：向被顶掉的旧会话发送"账号已在其他设备登录"提示（RespPrompt Type=5，本地化文案）。
    /// 顶号编排（通知 → 清数据 → 断开 → 移除）封装在 PlayerSessionManager 内，通知器须在 Close 前内联 await，
    /// RespPrompt 属游戏协议（Proto 在 Core 之上），故由本调用方注入构造逻辑。
    /// </summary>
    /// <param name="oldSession">被顶掉的旧会话</param>
    private static Task NotifyDuplicateLoginAsync(IPlayerSession oldSession)
    {
        var msg = new RespPrompt
        {
            Type = 5,
            Content = LocalizationService.GetString(Localization.Keys.Apps.SessionManager.AccountAlreadyLoggedIn),
        };
        return oldSession.WriteAsync(msg);
    }
}
